namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public class BatchConvertOutputProperties
{
    public string OutputFolder { get; set; }
    public bool UseOutputFolder { get; set; }
    public bool Overwrite { get; set; }

    public BatchConvertOutputProperties()
    {
        OutputFolder = string.Empty;
        UseOutputFolder = false;
        Overwrite = false;
    }
}
