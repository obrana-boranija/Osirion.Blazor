using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("Test_String", "test-string")]
        [InlineData("Multiple   Spaces", "multiple-spaces")]
        [InlineData("Special@#Characters!", "specialcharacters")]
        [InlineData("", "/untitled")]
        [InlineData(null, "/untitled")]
        public void ToUrlSlug_WithVariousInputs_ReturnsExpectedSlug(string input, string expected)
        {
            // Act
            var result = input.ToUrlSlug();

            // Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void ToUrlSlug_WithSuffixLength_AddsRandomSuffix()
        {
            // Arrange
            var input = "Test";
            uint suffixLength = 5;

            // Act
            var result = input.ToUrlSlug(suffixLength);

            // Assert
            result.Length.ShouldBeGreaterThan(input.Length);
            result.ShouldStartWith("test");
            result.Length.ShouldBe(input.Length + (int)suffixLength);
        }
    }
}