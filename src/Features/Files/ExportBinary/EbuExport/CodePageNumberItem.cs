namespace SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;

public class CodePageNumberItem
{
    public string CodePage { get; set; }
    public string CodePageName { get; set; }

    public CodePageNumberItem(string codePage, string codePageName)
    {
        CodePage = codePage;
        CodePageName = codePageName;
    }

    public override string ToString()
    {
        return $"{CodePage} - {CodePageName}";
    }

    public static List<CodePageNumberItem> GetCodePageNumberItems()
    {
        return new List<CodePageNumberItem>
        {
            new("437", "United States"),
            new("850", "Multilingual"),
            new("860", "Portugal"),
            new("863", "Canada-French"),
            new("865", "Nordic"),
        };
    }
}
