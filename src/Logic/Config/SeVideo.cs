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
    public decimal Outline { get; set; }
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
    public Color NonAssaBoxColor { get; set; }
    public Color NonAssaTextColor { get; set; }
    public Color NonAssaShadowColor { get; set; }
    public Color NonAssaOutlineColor { get; internal set; }
    public bool NonAssaAlignRight { get; set; }
    public bool NonAssaFixRtlUnicode { get; set; }
    public string EmbedOutputExt { get; set; }
    public string EmbedOutputSuffix { get; set; }
    public string EmbedOutputReplace { get; set; }
    public bool DeleteInputVideoFile { get; set; }
    public bool UseOutputFolder { get; set; }
    public string OutputFolder { get; set; }
    public string OutputFileSuffix { get; set; }

    public VideoBurnIn()
    {
        FontFactor = 0.52;
        Encoding = "libx264";
        Preset = "medium";
        Crf = "23";
        Tune = "";
        AudioEncoding = "copy";
        AudioForceStereo = true;
        AudioSampleRate = "48000";
        FontBold = true;
        Outline = 6;
        NonAssaBox = true;
        NonAssaBoxColor = Color.FromArgb("#aa000000");
        NonAssaTextColor = Colors.White;
        NonAssaShadowColor = Colors.Black;
        NonAssaOutlineColor = Colors.Black;
        EmbedOutputSuffix = "embed";
        EmbedOutputReplace = "embed" + Environment.NewLine + "SoftSub" + Environment.NewLine + "SoftSubbed";
        OutputFileSuffix = "_new";
        GenTransparentVideoExtension = ".mkv";
    }
}