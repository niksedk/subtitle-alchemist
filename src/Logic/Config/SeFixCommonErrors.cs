using Nikse.SubtitleEdit.Core.Forms.FixCommonErrors;

namespace SubtitleAlchemist.Logic.Config;

public class SeFixCommonErrors
{
    public string LastLanguageCode { get; set; } = string.Empty;
    public List<SeFixCommonErrorsProfile> Profiles { get; set; } = new();
    public string LastProfileName { get; set; } = "Default";
    public bool SkipStep1 { get; set; }
}