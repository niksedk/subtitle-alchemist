using Nikse.SubtitleEdit.Core.Common.TextLengthCalculator;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class CpsLineLengthDisplay
{
    public string Name { get; set; }
    public ICalcLength Strategy { get; set; }

    public CpsLineLengthDisplay(string name, ICalcLength strategy)
    {
        Name = name;
        Strategy = strategy;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<CpsLineLengthDisplay> GetCpsLineLengthStrategies()
    {
        var l = Se.Language.Settings;
        return new List<CpsLineLengthDisplay>
        {
            new(l.CpsLineLengthStyleCalcAll, new CalcAll()),
            new(l.CpsLineLengthStyleCalcCjk, new CalcCjk()),
            new(l.CpsLineLengthStyleCalcCjkNoSpace, new CalcCjkNoSpace()),
            new(l.CpsLineLengthStyleCalcIncludeCompositionCharacters, new CalcIncludeCompositionCharacters()),
            new(l.CpsLineLengthStyleCalcIncludeCompositionCharactersNotSpace, new CalcIncludeCompositionCharactersNotSpace()),
            new(l.CpsLineLengthStyleCalcNoSpace, new CalcNoSpace()),
            new(l.CpsLineLengthStyleCalcNoSpaceCpsOnly, new CalcNoSpaceCpsOnly()),
            new(l.CpsLineLengthStyleCalcNoSpaceOrPunctuation, new CalcNoSpaceOrPunctuation()),
            new(l.CpsLineLengthStyleCalcNoSpaceOrPunctuationCpsOnly, new CalcNoSpaceOrPunctuationCpsOnly()),
        };
    }
}
