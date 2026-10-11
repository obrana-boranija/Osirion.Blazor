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
    [InlineData("en-gb", "What is it")]
    [InlineData("sr", "Kako radi")]
    [InlineData("sr-cyrl", "Како ради")]
    [InlineData("de-ch", "Wie geht das")]
    [InlineData("fr", "Comment faire")]
    [InlineData("es", "Cómo hacer")]
    [InlineData("it", "Come fare")]
    [InlineData("pt-br", "Como fazer")]
    [InlineData("pt-pt", "o que")]
    [InlineData("nl", "Hoe werkt")]
    [InlineData("ru", "Как это")]
    [InlineData("ja", "How to")]
    public void IsQuestion_TitleStartsWithAQuestionWordOfItsLanguage_ReturnsTrue(string languageCode, string title)
    {
        AnswerQuestions.IsQuestion(title, languageCode).ShouldBeTrue();
    }

    [Theory]
    [InlineData("How", "en")]
    [InlineData("How? A primer", "en")]
    [InlineData("What's new in Blazor", "en")]
    [InlineData("Is\tit worth it", "en")]
    [InlineData("Da li je spremno", "sr")]
    public void IsQuestion_QuestionWordEndsAtABoundary_ReturnsTrue(string title, string languageCode)
    {
        AnswerQuestions.IsQuestion(title, languageCode).ShouldBeTrue();
    }

    [Theory]
    [InlineData("However we ship", "en")]
    [InlineData("Isolation levels", "en")]
    [InlineData("Cancel culture", "en")]
    [InlineData("Whoever wins", "en")]
    [InlineData("Wochenende in Berlin", "de")]
    [InlineData("Estrategias de prueba", "es")]
    public void IsQuestion_QuestionWordIsOnlyThePrefixOfALongerWord_ReturnsFalse(string title, string languageCode)
    {
        AnswerQuestions.IsQuestion(title, languageCode).ShouldBeFalse();
    }

    [Theory]
    [InlineData("sr", "Obican naslov")]
    [InlineData("sr-cyrl", "Обичан")]
    [InlineData("de", "Normal")]
    [InlineData("fr", "Normal")]
    [InlineData("es", "Normal")]
    [InlineData("it-it", "Guida")]
    [InlineData("nl-be", "Gids")]
    [InlineData("ru-ru", "Гид")]
    [InlineData("", "Guide to x")]
    public void IsQuestion_TitleWithoutQuestionWordOrPattern_IsNoQuestionAndDerivesNone(string languageCode, string title)
    {
        AnswerQuestions.IsQuestion(title, languageCode).ShouldBeFalse();
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBeNull();
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
    [InlineData("EN-US", "Guide to x", "How to x?")]
    [InlineData("sr-latn-rs", "Uvod u Blazor", "Šta je Blazor?")]
    [InlineData("sr-cyrl", "Увод у Блазор", "Шта је Блазор?")]
    [InlineData("de-at", "Einführung in Blazor", "Was ist Blazor?")]
    [InlineData("fr-ca", "Introduction à Blazor", "Qu'est-ce que Blazor?")]
    [InlineData("es-mx", "Introducción a Blazor", "Qué es Blazor?")]
    public void GenerateImplicitQuestion_LanguagePattern_DerivesTheQuestion(string languageCode, string title, string expected)
    {
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBe(expected);
    }

    // Known defect, pinned as current behavior: the "how" patterns put the question word in front of a noun and the contained patterns leave a double space or the words before the keyword, so the question is not grammatical. Update this expectation when it is fixed.
    [Theory]
    [InlineData("en", "Guide to testing Blazor", "How to testing Blazor?")]
    [InlineData("en", "Blazor tutorial basics", "How to Blazor  basics?")]
    [InlineData("en", "An introduction to Blazor", "What is An  Blazor?")]
    [InlineData("sr", "Vodič za testiranje", "Kako testiranje?")]
    [InlineData("sr-latn", "Osnovni tutorijal", "Kako Osnovni?")]
    [InlineData("sr-cyrl", "Водич за тестирање", "Како тестирање?")]
    [InlineData("sr-cyrl-rs", "Основни туторијал", "Како Основни?")]
    [InlineData("de", "Anleitung zu Blazor", "Wie Blazor?")]
    [InlineData("de-de", "Blazor Tutorial", "Wie Blazor?")]
    [InlineData("fr", "Guide de Blazor", "Comment Blazor?")]
    [InlineData("fr-fr", "Tutoriel Blazor", "Comment Blazor?")]
    [InlineData("es", "Guía de Blazor", "Cómo Blazor?")]
    [InlineData("es-es", "Tutorial de Blazor", "Cómo de Blazor?")]
    public void GenerateImplicitQuestion_HowOrContainedPattern_DerivesAnUngrammaticalQuestion(string languageCode, string title, string expected)
    {
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBe(expected);
    }

    // Known defect, pinned as current behavior: the derived questions are not grammatical ("How to A?", a double space after removing "tutorial"). Update this expectation when it is fixed.
    [Theory]
    [InlineData("GUIDE TO Testing", "En-Gb", "How to Testing?")]
    [InlineData("A Tutorial", "en", "How to A?")]
    [InlineData("Introduction to tutorial writing", "en", "How to Introduction to  writing?")]
    public void GenerateImplicitQuestion_SeveralPatternsCouldMatch_TheFirstOneWins(string title, string languageCode, string expected)
    {
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBe(expected);
    }

    // Threw ArgumentOutOfRangeException before the fix, so the page failed to render with generated FAQ on.
    [Theory]
    [InlineData("Guide to", "en")]
    [InlineData("Guide to ", "en")]
    [InlineData("Guía de", "es")]
    public void GenerateImplicitQuestion_PrefixWithoutSubject_ReturnsNull(string title, string languageCode)
    {
        AnswerQuestions.GenerateImplicitQuestion(title, languageCode).ShouldBeNull();
    }
}
