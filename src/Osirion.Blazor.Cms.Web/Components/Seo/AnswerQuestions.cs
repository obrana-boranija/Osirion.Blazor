namespace Osirion.Blazor.Cms.Web.Components.Seo;

/// <summary>
/// Finds the question a page answers for answer engine optimization: a title that already is a question, or an
/// implicit question derived from a title pattern such as "Guide to X" ("How to X?"), per language.
/// </summary>
internal static class AnswerQuestions
{
    private static readonly string[] EnglishQuestionWords =
        ["how", "what", "why", "when", "where", "who", "which", "can", "does", "is", "are", "should"];

    private static readonly Dictionary<string, string[]> QuestionWords = CreateQuestionWords();

    private static readonly Dictionary<string, TitlePattern[]> ImplicitQuestionPatterns = CreateImplicitQuestionPatterns();

    /// <summary>
    /// Returns whether <paramref name="title"/> starts with a question word of the language; unknown languages use English.
    /// </summary>
    internal static bool IsQuestion(string title, string? languageCode)
    {
        var words = QuestionWords.GetValueOrDefault(Normalize(languageCode), EnglishQuestionWords);
        return words.Any(word => StartsWithWord(title, word));
    }

    // The word must end at the end of the title, a space or punctuation, so "However" is not read as "How".
    private static bool StartsWithWord(string title, string word) =>
        title.StartsWith(word, StringComparison.OrdinalIgnoreCase)
        && (title.Length == word.Length
            || char.IsWhiteSpace(title[word.Length])
            || char.IsPunctuation(title[word.Length]));

    /// <summary>
    /// Derives a question from a title pattern of the language, or returns <see langword="null"/> when no pattern
    /// matches or the language has none.
    /// </summary>
    internal static string? GenerateImplicitQuestion(string title, string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(title)
            || !ImplicitQuestionPatterns.TryGetValue(Normalize(languageCode), out var patterns))
        {
            return null;
        }

        return patterns.FirstOrDefault(pattern => pattern.Matches(title))?.ToQuestion(title);
    }

    private static string Normalize(string? languageCode) => languageCode?.ToLowerInvariant() ?? string.Empty;

    private static Dictionary<string, string[]> CreateQuestionWords()
    {
        var words = new Dictionary<string, string[]>(StringComparer.Ordinal);
        Add(words, EnglishQuestionWords, "en", "en-us", "en-gb");
        Add(words, ["kako", "šta", "zašto", "kada", "gde", "ko", "koji", "da li", "može", "treba"], "sr", "sr-latn", "sr-latn-rs");
        Add(words, ["како", "шта", "зашто", "када", "где", "ко", "који", "да ли", "може", "треба"], "sr-cyrl", "sr-cyrl-rs");
        Add(words, ["wie", "was", "warum", "wann", "wo", "wer", "welche", "kann", "ist", "sind", "soll"], "de", "de-de", "de-at", "de-ch");
        Add(words, ["comment", "quoi", "pourquoi", "quand", "où", "qui", "quel", "quelle", "peut", "est", "sont"], "fr", "fr-fr", "fr-ca");
        Add(words, ["cómo", "qué", "por qué", "cuándo", "dónde", "quién", "cuál", "puede", "es", "son", "debe"], "es", "es-es", "es-mx");
        Add(words, ["come", "cosa", "perché", "quando", "dove", "chi", "quale", "può", "è", "sono", "deve"], "it", "it-it");
        Add(words, ["como", "o que", "por que", "quando", "onde", "quem", "qual", "pode", "é", "são", "deve"], "pt", "pt-pt", "pt-br");
        Add(words, ["hoe", "wat", "waarom", "wanneer", "waar", "wie", "welke", "kan", "is", "zijn", "moet"], "nl", "nl-nl", "nl-be");
        Add(words, ["как", "что", "почему", "когда", "где", "кто", "который", "может", "является"], "ru", "ru-ru");
        return words;
    }

    private static Dictionary<string, TitlePattern[]> CreateImplicitQuestionPatterns()
    {
        var patterns = new Dictionary<string, TitlePattern[]>(StringComparer.Ordinal);

        // The skip lengths are the prefix length, plus one where the following space is skipped as well.
        Add(patterns,
            [TitlePattern.Prefix("guide to", 9, "How to"), TitlePattern.Contained("tutorial", "How to"), TitlePattern.Contained("introduction to", "What is")],
            "en", "en-us", "en-gb");
        Add(patterns,
            [TitlePattern.Prefix("vodič za", 8, "Kako"), TitlePattern.Contained("tutorijal", "Kako"), TitlePattern.Prefix("uvod u", 6, "Šta je")],
            "sr", "sr-latn", "sr-latn-rs");
        Add(patterns,
            [TitlePattern.Prefix("водич за", 8, "Како"), TitlePattern.Contained("туторијал", "Како"), TitlePattern.Prefix("увод у", 6, "Шта је")],
            "sr-cyrl", "sr-cyrl-rs");
        Add(patterns,
            [TitlePattern.Prefix("anleitung zu", 12, "Wie"), TitlePattern.Contained("tutorial", "Wie"), TitlePattern.Prefix("einführung in", 13, "Was ist")],
            "de", "de-de", "de-at", "de-ch");
        Add(patterns,
            [TitlePattern.Prefix("guide de", 8, "Comment"), TitlePattern.Contained("tutoriel", "Comment"), TitlePattern.Prefix("introduction à", 14, "Qu'est-ce que")],
            "fr", "fr-fr", "fr-ca");
        Add(patterns,
            [TitlePattern.Prefix("guía de", 8, "Cómo"), TitlePattern.Contained("tutorial", "Cómo"), TitlePattern.Prefix("introducción a", 14, "Qué es")],
            "es", "es-es", "es-mx");
        return patterns;
    }

    private static void Add<TValue>(Dictionary<string, TValue> map, TValue value, params string[] languageCodes)
    {
        foreach (var languageCode in languageCodes)
        {
            map.Add(languageCode, value);
        }
    }

    /// <summary>
    /// A title pattern: a keyword at the start of the title (the rest after <see cref="SkipLength"/> characters
    /// becomes the subject) or anywhere in it (the title without the keyword becomes the subject).
    /// </summary>
    private sealed record TitlePattern(string Keyword, int? SkipLength, string QuestionPrefix)
    {
        public static TitlePattern Prefix(string keyword, int skipLength, string questionPrefix) =>
            new(keyword, skipLength, questionPrefix);

        public static TitlePattern Contained(string keyword, string questionPrefix) =>
            new(keyword, null, questionPrefix);

        // A prefix pattern needs a subject after the keyword: a bare "Guide to" has no question to ask.
        public bool Matches(string title) => SkipLength is not { } skip
            ? title.Contains(Keyword, StringComparison.OrdinalIgnoreCase)
            : title.StartsWith(Keyword, StringComparison.OrdinalIgnoreCase)
                && title.Length > skip
                && !string.IsNullOrWhiteSpace(title[skip..]);

        public string ToQuestion(string title)
        {
            var subject = SkipLength is { } skip
                ? title[skip..]
                : title.Replace(Keyword, string.Empty, StringComparison.OrdinalIgnoreCase);
            return $"{QuestionPrefix} {subject.Trim()}?";
        }
    }
}
