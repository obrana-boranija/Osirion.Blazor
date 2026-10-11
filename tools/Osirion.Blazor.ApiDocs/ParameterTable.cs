using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.ApiDocs;

/// <summary>
/// Builds the Markdown parameter table of a component from its <c>[Parameter]</c> properties and XML documentation.
/// </summary>
internal sealed partial class ParameterTable
{
    private readonly Dictionary<string, string> _summaries = new(StringComparer.Ordinal);
    private readonly HashSet<Assembly> _loadedDocs = [];

    /// <summary>One row of the table.</summary>
    internal sealed record Row(string Name, string Type, string Default, string Description);

    /// <summary>Gets the rows for <paramref name="componentType"/>: its own parameters in declaration order, then
    /// inherited ones. Parameters obsolete as errors and the catch-all attribute dictionary are left out.</summary>
    public IReadOnlyList<Row> GetRows(Type componentType)
    {
        ArgumentNullException.ThrowIfNull(componentType);
        var instance = TryCreate(componentType);

        return componentType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is { CaptureUnmatchedValues: false })
            .Where(property => property.GetCustomAttribute<ObsoleteAttribute>() is not { IsError: true })
            .Select(property => new
            {
                Property = property,
                Depth = Depth(componentType, property.DeclaringType!),
            })
            .OrderBy(entry => entry.Depth)
            .ThenBy(entry => entry.Property.MetadataToken)
            .Select(entry => new Row(
                entry.Property.Name,
                TypeName(entry.Property.PropertyType, entry.Property),
                DefaultValue(entry.Property, instance),
                Describe(entry.Property)))
            .ToList();
    }

    /// <summary>Formats <paramref name="rows"/> as a Markdown table.</summary>
    public static string ToMarkdown(IEnumerable<Row> rows)
    {
        var builder = new StringBuilder();
        builder.AppendLine("| Parameter | Type | Default | Description |");
        builder.AppendLine("| --- | --- | --- | --- |");
        foreach (var row in rows)
        {
            builder.Append("| `").Append(row.Name).Append("` | `").Append(Cell(row.Type)).Append("` | ")
                .Append(row.Default.Length == 0 ? "-" : "`" + Cell(row.Default) + "`").Append(" | ")
                .Append(Cell(row.Description)).AppendLine(" |");
        }

        return builder.ToString();
    }

    private static int Depth(Type type, Type declaring)
    {
        var depth = 0;
        for (var current = type; current is not null && current != declaring; current = current.BaseType)
        {
            depth++;
        }

        return depth;
    }

    // Defaults come from the property initializers, which run in the constructor. Components that take services in
    // their constructor get nulls; when a constructor refuses them, the defaults stay blank.
    private static object? TryCreate(Type type)
    {
        if (type.IsAbstract || type.ContainsGenericParameters)
        {
            return null;
        }

        try
        {
            var constructor = type.GetConstructors().OrderBy(c => c.GetParameters().Length).FirstOrDefault();
            if (constructor is null)
            {
                return null;
            }

            var arguments = constructor.GetParameters()
                .Select(parameter => parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null)
                .ToArray();
            return constructor.Invoke(arguments);
        }
        catch (TargetInvocationException)
        {
            return null;
        }
    }

    private static string DefaultValue(PropertyInfo property, object? instance)
    {
        if (instance is null || property.GetMethod is null)
        {
            return string.Empty;
        }

        object? value;
        try
        {
            value = property.GetValue(instance);
        }
        catch (TargetInvocationException)
        {
            return string.Empty;
        }

        return value switch
        {
            null => "null",
            bool flag => flag ? "true" : "false",
            string text => text.Length == 0 ? "\"\"" : "\"" + text + "\"",
            Enum member => member.GetType().Name + "." + member,
            IFormattable number => number.ToString(null, CultureInfo.InvariantCulture),
            Delegate => string.Empty,
            IEnumerable sequence when !sequence.Cast<object>().Any() => "[]",
            _ => string.Empty,
        };
    }

    private static string TypeName(Type type, PropertyInfo? property = null)
    {
        var nullable = property is not null && new NullabilityInfoContext().Create(property).ReadState == NullabilityState.Nullable;
        var name = TypeName(type);
        return nullable && !name.EndsWith('?') ? name + "?" : name;
    }

    private static string TypeName(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlying)
        {
            return TypeName(underlying) + "?";
        }

        if (type.IsArray)
        {
            return TypeName(type.GetElementType()!) + "[]";
        }

        var alias = type.FullName switch
        {
            "System.String" => "string",
            "System.Boolean" => "bool",
            "System.Int32" => "int",
            "System.Int64" => "long",
            "System.Double" => "double",
            "System.Decimal" => "decimal",
            "System.Object" => "object",
            _ => null,
        };
        if (alias is not null)
        {
            return alias;
        }

        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var baseName = type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)];
        return baseName + "<" + string.Join(", ", type.GetGenericArguments().Select(TypeName)) + ">";
    }

    private string Describe(PropertyInfo property)
    {
        var assembly = property.DeclaringType!.Assembly;
        LoadDocs(assembly);
        var key = "P:" + property.DeclaringType!.FullName!.Replace('+', '.') + "." + property.Name;
        return _summaries.TryGetValue(key, out var summary) ? summary : string.Empty;
    }

    private void LoadDocs(Assembly assembly)
    {
        if (!_loadedDocs.Add(assembly))
        {
            return;
        }

        var path = Path.ChangeExtension(assembly.Location, ".xml");
        if (!File.Exists(path))
        {
            return;
        }

        foreach (var member in XDocument.Load(path).Descendants("member"))
        {
            var name = (string?)member.Attribute("name");
            var summary = member.Element("summary");
            if (name is not null && summary is not null)
            {
                _summaries[name] = Flatten(summary);
            }
        }
    }

    // XML doc markup to plain Markdown text: cref and langword references become code, tags become their text.
    internal static string Flatten(XElement element)
    {
        var builder = new StringBuilder();
        foreach (var node in element.Nodes())
        {
            switch (node)
            {
                case XText text:
                    builder.Append(text.Value);
                    break;
                case XElement { Name.LocalName: "see" or "seealso" } see:
                    var reference = (string?)see.Attribute("langword")
                        ?? ShortName((string?)see.Attribute("cref") ?? (string?)see.Attribute("href") ?? see.Value);
                    builder.Append('`').Append(reference).Append('`');
                    break;
                case XElement { Name.LocalName: "paramref" or "typeparamref" } parameter:
                    builder.Append('`').Append((string?)parameter.Attribute("name")).Append('`');
                    break;
                case XElement { Name.LocalName: "c" } code:
                    builder.Append('`').Append(code.Value).Append('`');
                    break;
                case XElement other:
                    builder.Append(Flatten(other));
                    break;
            }
        }

        return Whitespace().Replace(builder.ToString(), " ").Trim();
    }

    private static string ShortName(string reference)
    {
        var name = reference.Contains(':', StringComparison.Ordinal) ? reference[(reference.IndexOf(':', StringComparison.Ordinal) + 1)..] : reference;
        var parenthesis = name.IndexOf('(', StringComparison.Ordinal);
        if (parenthesis >= 0)
        {
            name = name[..parenthesis];
        }

        // A member keeps its type name (ThemeMode.Dark), a type is shown alone (ThemeMode).
        var parts = name.Split('.');
        var isMember = reference.Length > 1 && reference[1] == ':' && reference[0] is 'P' or 'M' or 'F' or 'E';
        return isMember && parts.Length >= 2 ? parts[^2] + "." + parts[^1] : parts[^1];
    }

    private static string Cell(string text) => text.Replace("|", "\\|", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal);

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
