using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class BurnInJobItem : ObservableObject
{
    [ObservableProperty] 
    private string _inputVideoFileName;

    [ObservableProperty]
    private string _subtitleFileName;


    public string OutputVideoFileName { get; set; }
    public string AssaSubtitleFileName { get; set; }
    public bool UseTargetFileSize { get; set; }
    public long TargetFileSize { get; set; }


    [ObservableProperty]
    private string _resolution;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    public long TotalFrames { get; set; }

    [ObservableProperty]
    private string _size;

    [ObservableProperty]
    private string _status;

    public BurnInJobItem(string inputVideoFileName, int width, int height)
    {
        InputVideoFileName = inputVideoFileName;
        Width = width;
        Height = height;
        Resolution = $"{width}x{height}";
        Status = "Waiting";

        OutputVideoFileName = string.Empty;
        SubtitleFileName = string.Empty;
        AssaSubtitleFileName = string.Empty;
        Size = string.Empty;
    }
}
