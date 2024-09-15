namespace SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;

public class LanguageItem
{
    public string Code { get; set; }
    public string Language { get; set; }

    public override string ToString()
    {
        return $"{Code} - {Language}";
    }

    public LanguageItem(string code, string language)
    {
        Code = code;
        Language = language;
    }
}
