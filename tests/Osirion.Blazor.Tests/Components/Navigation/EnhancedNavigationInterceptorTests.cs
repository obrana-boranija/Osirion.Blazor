using Osirion.Blazor.Components.Navigation;

namespace Osirion.Blazor.Tests.Components.Navigation;

public class EnhancedNavigationInterceptorTests
{
    [Theory]
    [InlineData(ScrollBehavior.Auto, "auto")]
    [InlineData(ScrollBehavior.Smooth, "smooth")]
    [InlineData(ScrollBehavior.Instant, "instant")]
    public void BehaviorString_ShouldReturnCorrectString(ScrollBehavior behavior, string expected)
    {
        // Arrange
        var component = new EnhancedNavigationInterceptor { Behavior = behavior };

        // Act
        var behaviorString = component.GetType()
            .GetProperty("BehaviorString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(component) as string;

        // Assert
        Assert.Equal(expected, behaviorString);
    }

    [Fact]
    public void GetScript_ShouldReturnCorrectScript()
    {
        // Arrange
        var component = new EnhancedNavigationInterceptor
        {
            Behavior = ScrollBehavior.Smooth
        };

        // Act
        var script = component.GetType()
            .GetMethod("GetScript", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(component, null) as string;

        // Assert
        Assert.NotNull(script);
        Assert.Contains("behavior: 'smooth'", script);
        Assert.Contains("Blazor.addEventListener('enhancedload'", script);
    }
}
