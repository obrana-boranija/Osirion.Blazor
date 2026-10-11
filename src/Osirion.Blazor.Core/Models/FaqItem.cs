namespace Osirion.Blazor.Core.Models;

/// <summary>
/// A question and its answer for <see cref="Osirion.Blazor.Components.OsirionFaq"/>.
/// </summary>
/// <param name="Question">The question, shown as the item's title and written as the Question's name.</param>
/// <param name="Answer">The answer as plain text, shown under the question and written as the accepted answer.</param>
public sealed record FaqItem(string Question, string Answer);
