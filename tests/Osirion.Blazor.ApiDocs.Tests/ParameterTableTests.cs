using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace Osirion.Blazor.ApiDocs.Tests;

public class ParameterTableTests
{
    [Fact]
    public void GetRows_DerivedComponent_ListsOwnParametersBeforeInheritedOnes()
    {
        var rows = new ParameterTable().GetRows(typeof(SampleComponent));

        rows.Select(row => row.Name).ShouldBe(["Title", "Count", "Kind", "Items", "OnClick", "Class"]);
    }

    [Fact]
    public void GetRows_ObsoleteErrorAndCatchAllParameters_AreLeftOut()
    {
        var names = new ParameterTable().GetRows(typeof(SampleComponent)).Select(row => row.Name).ToList();

        names.ShouldNotContain("Legacy");
        names.ShouldNotContain("Attributes");
        names.ShouldNotContain("NotAParameter");
    }

    [Fact]
    public void GetRows_Defaults_ComeFromThePropertyInitializers()
    {
        var rows = new ParameterTable().GetRows(typeof(SampleComponent)).ToDictionary(row => row.Name);

        rows["Title"].Default.ShouldBe("\"Hello\"");
        rows["Count"].Default.ShouldBe("3");
        rows["Kind"].Default.ShouldBe("SampleKind.Second");
        rows["Items"].Default.ShouldBe("[]");
        rows["OnClick"].Default.ShouldBeEmpty();
        rows["Class"].Default.ShouldBe("null");
    }

    [Fact]
    public void GetRows_Types_UseCSharpNamesAndNullability()
    {
        var rows = new ParameterTable().GetRows(typeof(SampleComponent)).ToDictionary(row => row.Name);

        rows["Title"].Type.ShouldBe("string");
        rows["Count"].Type.ShouldBe("int");
        rows["Items"].Type.ShouldBe("IReadOnlyList<string>");
        rows["OnClick"].Type.ShouldBe("EventCallback<int>");
        rows["Class"].Type.ShouldBe("string?");
    }

    [Fact]
    public void GetRows_Description_ComesFromTheXmlSummary()
    {
        var rows = new ParameterTable().GetRows(typeof(SampleComponent)).ToDictionary(row => row.Name);

        rows["Title"].Description.ShouldBe("The heading, shown when `true` is not `Count`.");
        rows["Class"].Description.ShouldBe("Extra CSS classes.");
    }

    [Fact]
    public void GetRows_ConstructorThatThrows_LeavesDefaultsBlank()
    {
        var rows = new ParameterTable().GetRows(typeof(ThrowingComponent));

        rows.ShouldHaveSingleItem().Default.ShouldBeEmpty();
    }

    [Fact]
    public void ToMarkdown_PipesInCells_AreEscaped()
    {
        var markdown = ParameterTable.ToMarkdown([new ParameterTable.Row("Mode", "string", "\"a|b\"", "Either a | b.")]);

        markdown.ShouldContain("| `Mode` | `string` | `\"a\\|b\"` | Either a \\| b. |");
    }

    [Fact]
    public void Flatten_DocMarkup_BecomesInlineCode()
    {
        var summary = XElement.Parse(
            "<summary>Uses <see cref=\"T:Osirion.Blazor.Components.ThemeMode\"/> or <see cref=\"F:Osirion.Blazor.Components.ThemeMode.Dark\"/>,\n"
            + "    <c>x</c>, <paramref name=\"value\"/> and <see langword=\"null\"/>.</summary>");

        ParameterTable.Flatten(summary).ShouldBe("Uses `ThemeMode` or `ThemeMode.Dark`, `x`, `value` and `null`.");
    }
}

public enum SampleKind
{
    First,
    Second,
}

public class SampleBase : ComponentBase
{
    /// <summary>Extra CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
}

public sealed class SampleComponent : SampleBase
{
    /// <summary>The heading, shown when <see langword="true"/> is not <see cref="P:Count"/>.</summary>
    [Parameter]
    public string Title { get; set; } = "Hello";

    [Parameter]
    public int Count { get; set; } = 3;

    [Parameter]
    public SampleKind Kind { get; set; } = SampleKind.Second;

    [Parameter]
    public IReadOnlyList<string> Items { get; set; } = [];

    [Parameter]
    public EventCallback<int> OnClick { get; set; }

    [Parameter]
    [Obsolete("Use Title.", true)]
    public string? Legacy { get; set; }

    public string? NotAParameter { get; set; }
}

public sealed class ThrowingComponent : ComponentBase
{
    public ThrowingComponent(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
    }

    [Parameter]
    public bool Enabled { get; set; } = true;
}
