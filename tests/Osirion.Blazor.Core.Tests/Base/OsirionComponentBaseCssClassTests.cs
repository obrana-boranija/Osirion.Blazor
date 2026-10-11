using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Base;

/// <summary>
/// The CSS classes <see cref="OsirionComponentBase"/> hands to components are a contract with the framework adapters
/// and with host styles: each framework must get its own classes, and an unknown framework value falls back to the
/// neutral Osirion classes.
/// </summary>
public class OsirionComponentBaseCssClassTests : OsirionComponentTestBase
{
    private static readonly CssFramework UndefinedFramework = (CssFramework)99;

    public static TheoryData<CssFramework, string, string> ExpectedClasses => new()
    {
        // Bootstrap uses Bootstrap's own utility and component classes.
        { CssFramework.Bootstrap, "Button", "btn btn-primary" },
        { CssFramework.Bootstrap, "PrimaryButton", "btn btn-primary" },
        { CssFramework.Bootstrap, "SecondaryButton", "btn btn-secondary" },
        { CssFramework.Bootstrap, "OutlinedPrimaryButton", "btn btn-outline-primary" },
        { CssFramework.Bootstrap, "OutlinedSecondaryButton", "btn btn-outline-secondary" },
        { CssFramework.Bootstrap, "TextButton", "btn btn-link" },
        { CssFramework.Bootstrap, "SuccessButton", "btn btn-success" },
        { CssFramework.Bootstrap, "DangerButton", "btn btn-danger" },
        { CssFramework.Bootstrap, "WarningButton", "btn btn-warning" },
        { CssFramework.Bootstrap, "InfoButton", "btn btn-info" },
        { CssFramework.Bootstrap, "SmallButton", "btn-sm" },
        { CssFramework.Bootstrap, "LargeButton", "btn-lg" },
        { CssFramework.Bootstrap, "DisabledButton", "disabled" },
        { CssFramework.Bootstrap, "BlockButton", "w-100" },
        { CssFramework.Bootstrap, "IconButton", "btn btn-icon" },
        { CssFramework.Bootstrap, "RoundedButton", "rounded-pill" },
        { CssFramework.Bootstrap, "Input", "form-control" },
        { CssFramework.Bootstrap, "Select", "form-select" },
        { CssFramework.Bootstrap, "Checkbox", "form-check-input" },
        { CssFramework.Bootstrap, "Radio", "form-check-input" },
        { CssFramework.Bootstrap, "Label", "form-label" },
        { CssFramework.Bootstrap, "Card", "card" },
        { CssFramework.Bootstrap, "CardHeader", "card-header" },
        { CssFramework.Bootstrap, "CardBody", "card-body" },
        { CssFramework.Bootstrap, "Badge", "badge" },
        { CssFramework.Bootstrap, "Alert", "alert" },
        { CssFramework.Bootstrap, "Table", "table" },
        { CssFramework.Bootstrap, "Modal", "modal" },
        { CssFramework.Bootstrap, "Dropdown", "dropdown" },
        { CssFramework.Bootstrap, "Tooltip", "tooltip" },
        { CssFramework.Bootstrap, "Progress", "progress" },
        { CssFramework.Bootstrap, "Spinner", "spinner-border" },
        { CssFramework.Bootstrap, "Tabs", "nav nav-tabs" },
        { CssFramework.Bootstrap, "Accordion", "accordion" },
        { CssFramework.Bootstrap, "TextArea", "form-control osirion-contact-textarea" },

        { CssFramework.FluentUI, "Button", "osirion-fluent-button" },
        { CssFramework.FluentUI, "OutlinedPrimaryButton", "osirion-button osirion-button-outlined osirion-button-outlined-primary" },
        { CssFramework.FluentUI, "Checkbox", "osirion-checkbox osirion-fluent-checkbox" },
        { CssFramework.FluentUI, "TextArea", "osirion-textarea osirion-fluent-textarea osirion-contact-textarea" },

        { CssFramework.MudBlazor, "Button", "mud-button mud-button-filled" },
        { CssFramework.MudBlazor, "DangerButton", "mud-button mud-button-filled mud-button-filled-error" },
        { CssFramework.MudBlazor, "Card", "mud-paper mud-elevation-1" },
        { CssFramework.MudBlazor, "Accordion", "mud-expansion-panels" },

        { CssFramework.Radzen, "Button", "rz-button rz-button-primary" },
        { CssFramework.Radzen, "OutlinedPrimaryButton", "rz-button rz-button-primary rz-button-light" },
        { CssFramework.Radzen, "Table", "rz-data-grid" },
        { CssFramework.Radzen, "DisabledButton", "rz-state-disabled" },

        { CssFramework.None, "Button", "osirion-search-button" },
        { CssFramework.None, "Card", "osirion-card" },
        { CssFramework.None, "TextArea", "osirion-textarea osirion-contact-textarea" },
    };

    public static TheoryData<CssFramework, string> FrameworkClassPrefixes => new()
    {
        { CssFramework.FluentUI, "osirion-" },
        { CssFramework.MudBlazor, "mud-" },
        { CssFramework.Radzen, "rz-" },
        { CssFramework.None, "osirion-" },
    };

    [Theory]
    [MemberData(nameof(ExpectedClasses))]
    public void SingleElementClass_MatchesTheFrameworkContract(CssFramework framework, string element, string expected)
    {
        var probe = RenderProbe(framework);

        probe.SingleElementClasses()[element].ShouldBe(expected);
    }

    // Every token belongs to the framework, except the neutral Osirion classes the MudBlazor adapter styles
    // (inputs, selects, text areas) and the shared contact text area hook.
    [Theory]
    [MemberData(nameof(FrameworkClassPrefixes))]
    public void SingleElementClasses_UseTheFrameworksOwnClassNames(CssFramework framework, string prefix)
    {
        var probe = RenderProbe(framework);

        foreach (var (element, classes) in probe.SingleElementClasses())
        {
            foreach (var token in classes.Split(' '))
            {
                (token.StartsWith(prefix, StringComparison.Ordinal) || token.StartsWith("osirion-", StringComparison.Ordinal))
                    .ShouldBeTrue($"{framework} {element} class '{token}' should start with '{prefix}'.");
            }
        }
    }

    [Fact]
    public void SingleElementClasses_UndefinedFramework_FallBackToTheNeutralClasses()
    {
        // One render per test context, so the framework is switched on the options the probe reads from.
        var options = new ThemingOptions { Framework = CssFramework.None };
        Services.AddSingleton(Options.Create(options));
        SetRendererInfo(new RendererInfo("Static", false));
        var probe = Render<CssClassProbe>().Instance;
        var neutral = probe.SingleElementClasses();

        options.Framework = UndefinedFramework;
        var undefined = probe.SingleElementClasses();

        undefined.ShouldBe(neutral);
    }

    [Theory]
    [InlineData(ButtonVariant.Primary, "btn btn-primary")]
    [InlineData(ButtonVariant.Secondary, "btn btn-secondary")]
    [InlineData(ButtonVariant.Success, "btn btn-success")]
    [InlineData(ButtonVariant.Danger, "btn btn-danger")]
    [InlineData(ButtonVariant.Warning, "btn btn-warning")]
    [InlineData(ButtonVariant.Info, "btn btn-info")]
    [InlineData(ButtonVariant.Text, "btn btn-link")]
    [InlineData(ButtonVariant.OutlinedPrimary, "btn btn-outline-primary")]
    [InlineData(ButtonVariant.OutlinedSecondary, "btn btn-outline-secondary")]
    [InlineData((ButtonVariant)99, "btn btn-primary")]
    public void ButtonClass_Variant_MapsToTheVariantsButtonClass(ButtonVariant variant, string expected)
    {
        RenderProbe(CssFramework.Bootstrap).ButtonClass(variant).ShouldBe(expected);
    }

    [Theory]
    [InlineData(ButtonSize.Small, "btn-sm")]
    [InlineData(ButtonSize.Large, "btn-lg")]
    [InlineData(ButtonSize.Normal, "")]
    [InlineData((ButtonSize)99, "")]
    public void ButtonSizeClass_Size_MapsToTheSizeClass(ButtonSize size, string expected)
    {
        RenderProbe(CssFramework.Bootstrap).ButtonSizeClass(size).ShouldBe(expected);
    }

    [Theory]
    [InlineData(ButtonShape.Rounded, "rounded-pill")]
    [InlineData(ButtonShape.Square, "btn btn-icon")]
    [InlineData(ButtonShape.Default, "")]
    [InlineData((ButtonShape)99, "")]
    public void ButtonShapeClass_Shape_MapsToTheShapeClass(ButtonShape shape, string expected)
    {
        RenderProbe(CssFramework.Bootstrap).ButtonShapeClass(shape).ShouldBe(expected);
    }

    [Fact]
    public void BuildButtonClass_Defaults_IsThePrimaryButtonOnly()
    {
        RenderProbe(CssFramework.Bootstrap).BuildButton().ShouldBe("btn btn-primary");
    }

    [Fact]
    public void BuildButtonClass_AllOptions_CombinesThemInOrder()
    {
        var classes = RenderProbe(CssFramework.Bootstrap).BuildButton(
            ButtonVariant.Danger, ButtonSize.Large, ButtonShape.Rounded, isBlock: true, isDisabled: true, additionalClasses: "my-button");

        classes.ShouldBe("btn btn-danger btn-lg rounded-pill w-100 disabled my-button");
    }

    [Fact]
    public void BuildButtonClass_WhitespaceAdditionalClasses_AreIgnored()
    {
        RenderProbe(CssFramework.Bootstrap).BuildButton(additionalClasses: "  ").ShouldBe("btn btn-primary");
    }

    public static TheoryData<CssFramework, Severity, string> ExpectedAlertClasses => new()
    {
        { CssFramework.Bootstrap, Severity.Success, "alert alert-success" },
        { CssFramework.Bootstrap, Severity.Error, "alert alert-danger" },
        { CssFramework.Bootstrap, Severity.Warning, "alert alert-warning" },
        { CssFramework.Bootstrap, Severity.Info, "alert alert-info" },
        { CssFramework.FluentUI, Severity.Success, "osirion-alert osirion-alert-success" },
        { CssFramework.FluentUI, Severity.Error, "osirion-alert osirion-alert-error" },
        { CssFramework.FluentUI, Severity.Warning, "osirion-alert osirion-alert-warning" },
        { CssFramework.FluentUI, Severity.Info, "osirion-alert osirion-alert-info" },
        { CssFramework.MudBlazor, Severity.Success, "mud-alert mud-alert-text-success" },
        { CssFramework.MudBlazor, Severity.Error, "mud-alert mud-alert-text-error" },
        { CssFramework.MudBlazor, Severity.Warning, "mud-alert mud-alert-text-warning" },
        { CssFramework.MudBlazor, Severity.Info, "mud-alert mud-alert-text-info" },
        { CssFramework.Radzen, Severity.Success, "rz-message rz-message-success" },
        { CssFramework.Radzen, Severity.Error, "rz-message rz-message-error" },
        { CssFramework.Radzen, Severity.Warning, "rz-message rz-message-warning" },
        { CssFramework.Radzen, Severity.Info, "rz-message rz-message-info" },
        { CssFramework.None, Severity.Success, "osirion-alert osirion-alert-success" },
        { CssFramework.None, Severity.Error, "osirion-alert osirion-alert-error" },
        { CssFramework.None, Severity.Warning, "osirion-alert osirion-alert-warning" },
        { CssFramework.None, Severity.Info, "osirion-alert osirion-alert-info" },
        // Out-of-range values fall back to the info alert and to the neutral classes.
        { CssFramework.Bootstrap, (Severity)99, "alert alert-info" },
        { CssFramework.MudBlazor, (Severity)99, "mud-alert mud-alert-text-info" },
        { UndefinedFramework, Severity.Error, "osirion-alert osirion-alert-error" },
        { UndefinedFramework, (Severity)99, "osirion-alert osirion-alert-info" },
    };

    [Theory]
    [MemberData(nameof(ExpectedAlertClasses))]
    public void AlertClass_Severity_CombinesTheBaseAndSeverityClasses(CssFramework framework, Severity severity, string expected)
    {
        RenderProbe(framework).AlertClass(severity).ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, "base")]
    [InlineData(" ", "base")]
    [InlineData("extra", "base extra")]
    public void CombineCssClasses_AppendsTheClassParameterWhenSet(string? userClass, string expected)
    {
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        SetRendererInfo(new RendererInfo("Static", false));

        var probe = Render<CssClassProbe>(parameters => parameters.Add(p => p.Class, userClass)).Instance;

        probe.Combine("base").ShouldBe(expected);
    }

    private CssClassProbe RenderProbe(CssFramework framework)
    {
        Services.AddSingleton(Options.Create(new ThemingOptions { Framework = framework }));
        SetRendererInfo(new RendererInfo("Static", false));
        return Render<CssClassProbe>().Instance;
    }

    private sealed class CssClassProbe : OsirionComponentBase
    {
        public IReadOnlyDictionary<string, string> SingleElementClasses() => new Dictionary<string, string>
        {
            ["Button"] = GetButtonClass(),
            ["PrimaryButton"] = GetPrimaryButtonClass(),
            ["SecondaryButton"] = GetSecondaryButtonClass(),
            ["OutlinedPrimaryButton"] = GetOutlinedPrimaryButtonClass(),
            ["OutlinedSecondaryButton"] = GetOutlinedSecondaryButtonClass(),
            ["TextButton"] = GetTextButtonClass(),
            ["SuccessButton"] = GetSuccessButtonClass(),
            ["DangerButton"] = GetDangerButtonClass(),
            ["WarningButton"] = GetWarningButtonClass(),
            ["InfoButton"] = GetInfoButtonClass(),
            ["SmallButton"] = GetSmallButtonClass(),
            ["LargeButton"] = GetLargeButtonClass(),
            ["DisabledButton"] = GetDisabledButtonClass(),
            ["BlockButton"] = GetBlockButtonClass(),
            ["IconButton"] = GetIconButtonClass(),
            ["RoundedButton"] = GetRoundedButtonClass(),
            ["Input"] = GetInputClass(),
            ["Select"] = GetSelectClass(),
            ["Checkbox"] = GetCheckboxClass(),
            ["Radio"] = GetRadioClass(),
            ["Label"] = GetLabelClass(),
            ["Card"] = GetCardClass(),
            ["CardHeader"] = GetCardHeaderClass(),
            ["CardBody"] = GetCardBodyClass(),
            ["Badge"] = GetBadgeClass(),
            ["Alert"] = GetAlertClass(),
            ["Table"] = GetTableClass(),
            ["Modal"] = GetModalClass(),
            ["Dropdown"] = GetDropdownClass(),
            ["Tooltip"] = GetTooltipClass(),
            ["Progress"] = GetProgressClass(),
            ["Spinner"] = GetSpinnerClass(),
            ["Tabs"] = GetTabsClass(),
            ["Accordion"] = GetAccordionClass(),
            ["TextArea"] = GetTextAreaClass(),
        };

        public string ButtonClass(ButtonVariant variant) => GetButtonClass(variant);

        public string ButtonSizeClass(ButtonSize size) => GetButtonSizeClass(size);

        public string ButtonShapeClass(ButtonShape shape) => GetButtonShapeClass(shape);

        public string BuildButton(
            ButtonVariant variant = ButtonVariant.Primary,
            ButtonSize size = ButtonSize.Normal,
            ButtonShape shape = ButtonShape.Default,
            bool isBlock = false,
            bool isDisabled = false,
            string? additionalClasses = null) =>
            BuildButtonClass(variant, size, shape, isBlock, isDisabled, additionalClasses);

        public string AlertClass(Severity severity) => GetAlertClass(severity);

        public string Combine(string baseClass) => CombineCssClasses(baseClass);
    }
}
