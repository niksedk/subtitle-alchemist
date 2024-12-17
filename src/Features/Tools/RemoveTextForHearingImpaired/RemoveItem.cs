using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;
public class RemoveItem
{
    public bool Apply { get; set; }
    public int Index { get; set; }
    public string Before { get; set; }
    public string After { get; set; }
    public Paragraph Paragraph { get; set; }

    public RemoveItem(bool apply, int index, string before, string after, Paragraph paragraph)
    {
        Apply = apply;
        Index = index;
        Before = before;
        After = after;
        Paragraph = paragraph;
    }
}
