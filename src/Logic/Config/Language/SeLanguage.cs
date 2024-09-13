namespace SubtitleAlchemist.Logic.Config.Language;

public class SeLanguage
{
    public LanguageFixCommonErrors FixCommonErrors { get; set; } = new();
    public LanguageSettings Settings { get; set; } = new();
}