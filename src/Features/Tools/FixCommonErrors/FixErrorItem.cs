using Nikse.SubtitleEdit.Core.Forms.FixCommonErrors;
using Nikse.SubtitleEdit.Core.Interfaces;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public class FixErrorItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsSelected { get; set; }
    public string FixCommonErrorFunctionName { get; set; }

    public FixErrorItem(string name, string description, int sortOrder, bool isSelected, string fixCommonErrorFunctionName)
    {
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        IsSelected = isSelected;
        FixCommonErrorFunctionName = fixCommonErrorFunctionName;
    }

    public IFixCommonError GetFixCommonErrorFunction()
    {
        var function = GetFixCommonErrorItems()
            .First(p => p.GetType().Name == FixCommonErrorFunctionName);
        return function;
    }

    public static List<IFixCommonError> GetFixCommonErrorItems()
    {
        var list = new List<IFixCommonError>
        {
            new AddMissingQuotes(),
            new Fix3PlusLines(),
            new FixAloneLowercaseIToUppercaseI(),
            new FixCommas(),
            new FixContinuationStyle(),
            new FixDanishLetterI(),
            new FixDialogsOnOneLine(),
            new FixDoubleApostrophes(),
            new FixDoubleDash(),
            new FixDoubleGreaterThan(),
            new FixEllipsesStart(),
            new FixEmptyLines(),
            new FixHyphensInDialog(),
            new FixHyphensRemoveDashSingleLine(),
            new FixInvalidItalicTags(),
            new FixLongDisplayTimes(),
            new FixLongLines(),
            new FixMissingOpenBracket(),
            new FixMissingPeriodsAtEndOfLine(),
            new FixMissingSpaces(),
            new FixMusicNotation(),
            new FixOverlappingDisplayTimes(),
            new FixShortDisplayTimes(),
            new FixShortGaps(),
            new FixShortLines(),
            new FixShortLinesAll(),
            //new FixShortLinesPixelWidth(),
            new FixSpanishInvertedQuestionAndExclamationMarks(),
            new FixStartWithUppercaseLetterAfterColon(),
            new FixStartWithUppercaseLetterAfterParagraph(),
            new FixStartWithUppercaseLetterAfterPeriodInsideParagraph(),
            new FixTurkishAnsiToUnicode(),
            new FixUnnecessaryLeadingDots(),
            new FixUnneededPeriods(),
            new FixUnneededSpaces(),
            new FixUppercaseIInsideWords(),
            new NormalizeStrings(),
            new RemoveDialogFirstLineInNonDialogs(),
            new RemoveSpaceBetweenNumbers(),
        };

        return list;
    }
}
