using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;
public class RemoveItem //: IEqualityComparer<RemoveItem>
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

    public bool Equals(RemoveItem? x, RemoveItem? y)
    {
        if (x is RemoveItem item1 && y is RemoveItem item2)
        {
            return item1.Index == item2.Index &&
                   item1.Before == item2.Before &&
                   item1.After == item2.After;
        }

        return false;
    }

    public int GetHashCode([DisallowNull] RemoveItem item)
    {
        return item.Index.GetHashCode() + item.Before.GetHashCode() + item.After.GetHashCode();
    }
}
