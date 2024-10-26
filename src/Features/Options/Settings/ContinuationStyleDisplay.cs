using Nikse.SubtitleEdit.Core.Enums;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class ContinuationStyleDisplay
{
    public string Name { get; set; }
    public ContinuationStyle Style { get; set; }

    public ContinuationStyleDisplay(string name, ContinuationStyle style)
    {
        Name = name;
        Style = style;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<ContinuationStyleDisplay> GetDialogStyles()
    {
        var l = Se.Language.Settings;
        return new List<ContinuationStyleDisplay>
        {
            new (l.ContinuationStyleNone, ContinuationStyle.None),
            new (l.ContinuationStyleNoneTrailingDots, ContinuationStyle.NoneTrailingDots),
            new (l.ContinuationStyleNoneLeadingTrailingDots, ContinuationStyle.NoneLeadingTrailingDots),
            new (l.ContinuationStyleNoneTrailingEllipsis, ContinuationStyle.NoneTrailingEllipsis),
            new (l.ContinuationStyleNoneLeadingTrailingEllipsis, ContinuationStyle.NoneLeadingTrailingEllipsis),
            new (l.ContinuationStyleOnlyTrailingDots, ContinuationStyle.OnlyTrailingDots),
            new (l.ContinuationStyleLeadingTrailingDots, ContinuationStyle.LeadingTrailingDots),
            new (l.ContinuationStyleOnlyTrailingEllipsis, ContinuationStyle.OnlyTrailingEllipsis),
            new (l.ContinuationStyleLeadingTrailingEllipsis, ContinuationStyle.LeadingTrailingEllipsis),
            new (l.ContinuationStyleLeadingTrailingDash, ContinuationStyle.LeadingTrailingDash),
            new (l.ContinuationStyleLeadingTrailingDashDots, ContinuationStyle.LeadingTrailingDashDots),
            new (l.ContinuationStyleCustom, ContinuationStyle.Custom),
        };
    }
}
