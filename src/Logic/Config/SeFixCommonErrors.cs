namespace SubtitleAlchemist.Logic.Config;

public class SeFixCommonErrors
{
    public string LastLanguageCode { get; set; } = string.Empty;
    public List<SeFixCommonErrorsProfile> Profiles { get; set; } = new();
    public bool SkipStep1 { get; set; }
}