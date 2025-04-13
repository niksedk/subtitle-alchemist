using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class BurnInJobItem : ObservableObject
{
    [ObservableProperty]
    public partial string InputVideoFileName { get; set; }

    [ObservableProperty]
    public partial string InputVideoFileNameShort { get; set; }

    [ObservableProperty]
    public partial string SubtitleFileName { get; set; }

    [ObservableProperty]
    public partial string SubtitleFileNameShort { get; set; }

    public string OutputVideoFileName { get; set; }
    public string AssaSubtitleFileName { get; set; }
    public bool UseTargetFileSize { get; set; }
    public long TargetFileSize { get; set; }


    [ObservableProperty]
    public partial string Resolution { get; set; }

    [ObservableProperty]
    public partial int Width { get; set; }

    [ObservableProperty]
    public partial int Height { get; set; }
    public long TotalFrames { get; set; }
    public double TotalSeconds { get; set; }
    public string VideoBitRate { get; set; }

    [ObservableProperty]
    public partial string Size { get; set; }

    [ObservableProperty]
    public partial string Status { get; set; }

    public void AddSubtitleFileName(string subtitleFileName)
    {
        if (string.IsNullOrEmpty(subtitleFileName))
        {
            SubtitleFileName = string.Empty;
            SubtitleFileNameShort = string.Empty;
            return;
        }

        SubtitleFileName = subtitleFileName;
        SubtitleFileNameShort = Path.GetFileName(subtitleFileName);
    }

    public void AddInputVideoFileName(string inputVideoFileName)
    {
        InputVideoFileName = inputVideoFileName;
        if (inputVideoFileName.Length > 75)
        {
            InputVideoFileNameShort = Path.GetFileName(inputVideoFileName);
        }
        else
        {
            InputVideoFileNameShort = inputVideoFileName;
        }
    }

    public BurnInJobItem(string inputVideoFileName, int width, int height)
    {
        InputVideoFileName = string.Empty;
        InputVideoFileNameShort = string.Empty;
        SubtitleFileName = string.Empty;
        SubtitleFileNameShort = string.Empty;
        AddInputVideoFileName(inputVideoFileName);
        Width = width;
        Height = height;
        Resolution = $"{width}x{height}";
        Status = "Waiting";

        OutputVideoFileName = string.Empty;
        SubtitleFileName = string.Empty;
        SubtitleFileNameShort = string.Empty;
        AssaSubtitleFileName = string.Empty;
        Size = string.Empty;
        VideoBitRate = string.Empty;
    }
}
