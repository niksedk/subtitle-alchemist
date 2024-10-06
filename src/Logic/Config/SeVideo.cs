namespace SubtitleAlchemist.Logic.Config;

public class SeVideo
{
    public VideoBurnIn BurnIn { get; set; } = new();

    public SeVideo()
    {
        BurnIn = new();
    }
}

public class VideoBurnIn
{
    public string FontName { get; set; }
    public bool FontBold { get; set; }
    public decimal OutlineWidth { get; set; }
    public decimal ShadowWidth { get; set; }
    public double FontFactor { get; set; }
    public string Encoding { get; set; }
    public string Preset { get; set; }
    public string PixelFormat { get; set; }
    public string Crf { get; set; }
    public string Tune { get; set; }
    public string AudioEncoding { get; set; }
    public bool AudioForceStereo { get; set; }
    public string AudioSampleRate { get; set; }
    public bool TargetFileSize { get; set; }
    public bool NonAssaBox { get; set; }
    public bool GenTransparentVideoNonAssaBox { get; set; }
    public bool GenTransparentVideoNonAssaBoxPerLine { get; set; }
    public string GenTransparentVideoExtension { get; set; }
    public string NonAssaBoxColor { get; set; }
    public string NonAssaTextColor { get; set; }
    public string NonAssaShadowColor { get; set; }
    public string NonAssaOutlineColor { get; set; }
    public string NonAssaAlignment { get; set; }
    public bool NonAssaFixRtlUnicode { get; set; }
    public decimal NonAssaMarginVertical { get; set; }
    public decimal NonAssaMarginHorizontal { get; set; }
    public string EmbedOutputExt { get; set; }
    public string EmbedOutputSuffix { get; set; }
    public string EmbedOutputReplace { get; set; }
    public bool DeleteInputVideoFile { get; set; }
    public bool UseOutputFolder { get; set; }
    public string OutputFolder { get; set; }
    public string BurnInSuffix { get; set; }

    public VideoBurnIn()
    {
        FontName = "Arial";
        PixelFormat = string.Empty;
        EmbedOutputExt = ".mkv";
        OutputFolder = string.Empty;
        FontFactor = 0.52;
        Encoding = "libx264";
        Preset = "medium";
        Crf = "23";
        Tune = "film";
        AudioEncoding = "copy";
        AudioForceStereo = true;
        AudioSampleRate = "48000";
        FontBold = true;
        OutlineWidth = 6;
        ShadowWidth = 3;
        NonAssaBox = true;
        NonAssaBoxColor = Colors.Black.ToArgbHex();
        NonAssaTextColor = Colors.White.ToArgbHex();
        NonAssaShadowColor = Colors.Black.ToArgbHex();
        NonAssaOutlineColor = Colors.Black.ToArgbHex();
        EmbedOutputSuffix = "embed";
        EmbedOutputReplace = "embed" + Environment.NewLine + "SoftSub" + Environment.NewLine + "SoftSubbed";
        BurnInSuffix = "_new";
        GenTransparentVideoExtension = ".mkv";
        NonAssaAlignment = "2";
    }
}