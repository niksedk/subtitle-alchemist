using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.SpellCheck;

public class SpellCheckDictionary : ObservableObject
{
    public string EnglishName { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string DownloadLink { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the ".dic" dictionary file.
    /// </summary>
    public string DictionaryFileName { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{EnglishName} ({NativeName})";
    }
}
