namespace SubtitleAlchemist.Logic.Config;

public class SeOcr
{
    public string Engine { get; set; }
    public string NOcrDatabase { get; set; }
    public int NOcrMaxWrongPixels { get; set; }
    public int NOcrPixelsAreSpace { get; set; }
    public bool NOcrDrawUnknownText { get; set; }

    public SeOcr()
    {
        Engine = "nOCR";

        NOcrDatabase = "Latin";
        NOcrMaxWrongPixels = 20;
        NOcrPixelsAreSpace = 12;
        NOcrDrawUnknownText = true;
    }
}