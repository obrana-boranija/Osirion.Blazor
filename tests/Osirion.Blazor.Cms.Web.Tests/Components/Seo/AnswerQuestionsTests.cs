using Osirion.Blazor.Cms.Web.Components.Seo;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components.Seo;

public class AnswerQuestionsTests
{
    [Theory]
    [InlineData("How it works", null, true)]
    [InlineData("How it works", "xx", true)]
    [InlineData("Wie es geht", "DE-AT", true)]
    [InlineData("Wie es geht", "en", false)]
    [InlineData("Plain title", "en", false)]
    public void IsQuestion_UsesTheLanguageQuestionWordsOrEnglish(string title, string? languageCode, bool expected)
    {
        AnswerQuestions.IsQuestion(title, languageCode).ShouldBe(expected);
    }

    [Theory]
    [InlineData("", "en")]
    [InlineData("  ", "en")]
    [InlineData("Guide to testing", null)]
    [InlineData("Guide to testing", "it")]
    [InlineData("Plain title", "en")]
    public void GenerateImplicitQuestion_NoTitleLanguageOrPattern_ReturnsNull(string title, string? languageCode)
    {
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBeNull();
    }

    [Theory]
    [InlineData("GUIDE TO Testing", "En-Gb", "How to Testing?")]
    [InlineData("A Tutorial", "en", "How to A?")]
    [InlineData("Introduction to tutorial writing", "en", "How to Introduction to  writing?")]
    public void GenerateImplicitQuestion_FirstMatchingPatternWins(string title, string languageCode, string expected)
    {
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBe(expected);
    }
}
