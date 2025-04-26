namespace Osirion.Blazor.Cms.Models;

public class TextSelection
{
    public string Text { get; set; } = string.Empty;
    public int Start { get; set; }
    public int End { get; set; }
}