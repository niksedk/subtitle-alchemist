using Nikse.SubtitleEdit.Core.Enums;

namespace SubtitleAlchemist.Logic.Config.Language;

public class LanguageSettings
{
    public string DialogStyle { get; set; }

    public string DialogStyleDashSecondLineWithoutSpace { get; set; }
    public string DialogStyleDashSecondLineWithSpace { get; set; }
    public string DialogStyleDashBothLinesWithSpace { get; set; }
    public string DialogStyleDashBothLinesWithoutSpace { get; set; }

    public string ContinuationStyle { get; set; }
    public string ContinuationStyleNone { get; set; }
    public string ContinuationStyleNoneTrailingDots { get; set; }
    public string ContinuationStyleNoneLeadingTrailingDots { get; set; }
    public string ContinuationStyleNoneTrailingEllipsis { get; set; }
    public string ContinuationStyleNoneLeadingTrailingEllipsis { get; set; }
    public string ContinuationStyleOnlyTrailingDots { get; set; }
    public string ContinuationStyleLeadingTrailingDots { get; set; }
    public string ContinuationStyleOnlyTrailingEllipsis { get; set; }
    public string ContinuationStyleLeadingTrailingEllipsis { get; set; }
    public string ContinuationStyleLeadingTrailingDash { get; set; }
    public string ContinuationStyleLeadingTrailingDashDots { get; set; }
    public string ContinuationStyleCustom { get; set; }


    public LanguageSettings()
    {
        DialogStyle = "Dialog style";
        DialogStyleDashBothLinesWithSpace = "Dash both lines with space";
        DialogStyleDashBothLinesWithoutSpace = "Dash both lines without space";
        DialogStyleDashSecondLineWithSpace = "Dash second line with space";
        DialogStyleDashSecondLineWithoutSpace = "Dash second line without space";

        ContinuationStyle = "Continuation style";
        ContinuationStyleNone = "None";
        ContinuationStyleNoneTrailingDots = "None, dots for pauses (trailing only)";
        ContinuationStyleNoneLeadingTrailingDots = "None, dots for pauses";
        ContinuationStyleNoneTrailingEllipsis = "None, ellipsis for pauses (trailing only)";
        ContinuationStyleNoneLeadingTrailingEllipsis = "None, ellipsis for pauses";
        ContinuationStyleOnlyTrailingDots = "Dots (trailing only)";
        ContinuationStyleLeadingTrailingDots = "Dots";
        ContinuationStyleOnlyTrailingEllipsis = "Ellipsis (trailing only)";
        ContinuationStyleLeadingTrailingEllipsis = "Ellipsis";
        ContinuationStyleLeadingTrailingDash = "Dash";
        ContinuationStyleLeadingTrailingDashDots = "Dash, but dots for pauses";
        ContinuationStyleCustom = "Custom";
    }

    public string GetContinuationStyleName(ContinuationStyle continuationStyle)
    {
        return continuationStyle switch
        {
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.NoneTrailingDots => ContinuationStyleNoneTrailingDots,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.NoneLeadingTrailingDots => ContinuationStyleNoneLeadingTrailingDots,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.NoneTrailingEllipsis => ContinuationStyleNoneTrailingEllipsis,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.NoneLeadingTrailingEllipsis => ContinuationStyleNoneLeadingTrailingEllipsis,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.OnlyTrailingDots => ContinuationStyleOnlyTrailingDots,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.LeadingTrailingDots => ContinuationStyleLeadingTrailingDots,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.OnlyTrailingEllipsis => ContinuationStyleOnlyTrailingEllipsis,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.LeadingTrailingEllipsis => ContinuationStyleLeadingTrailingEllipsis,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.LeadingTrailingDash => ContinuationStyleLeadingTrailingDash,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.LeadingTrailingDashDots => ContinuationStyleLeadingTrailingDashDots,
            Nikse.SubtitleEdit.Core.Enums.ContinuationStyle.Custom => ContinuationStyleCustom,
            _ => ContinuationStyleNone,
        };
    }
}