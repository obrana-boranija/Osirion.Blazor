using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Components;

/// <summary>
/// Pricing plans side by side: name, price, description, features and a call to action, with the recommended plan
/// set apart. The buttons take the classes of the configured CSS framework. It renders without script.
/// </summary>
public partial class OsirionPricingTable
{
    /// <summary>
    /// The plans, in the order they are shown. Required; with no plans nothing is rendered.
    /// </summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<PricingPlan> Plans { get; set; } = [];

    private string GetPricingTableClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-pricing-table" : $"osirion-pricing-table {Class}";

    private static string GetPlanClass(PricingPlan plan) =>
        plan.Highlighted ? "osirion-pricing-plan osirion-pricing-plan-highlighted" : "osirion-pricing-plan";
}
