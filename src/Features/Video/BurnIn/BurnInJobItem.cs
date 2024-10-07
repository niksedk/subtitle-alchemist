using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public partial class BurnInJobItem : ObservableObject
{
    [ObservableProperty] 
    private string _inputVideoFileName;

    [ObservableProperty]
    private string _inputVideoFileNameShort;

    [ObservableProperty]
    private string _subtitleFileName;

    [ObservableProperty]
    private string _subtitleFileNameShort;


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
        _inputVideoFileName = string.Empty;
        _inputVideoFileNameShort = string.Empty;
        _subtitleFileName = string.Empty;
        _subtitleFileNameShort = string.Empty;
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
    }
}
