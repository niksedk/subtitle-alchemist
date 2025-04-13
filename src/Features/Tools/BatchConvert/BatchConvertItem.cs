using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public partial class BatchConvertItem : ObservableObject
{
    public string FileName { get; set; }
    public long Size { get; set; }
    public string DisplaySize { get; set; }
    public string Format { get; set; }
    [ObservableProperty]
    public partial string Status { get; set; }
    public Subtitle? Subtitle { get; set; }

    public BatchConvertItem()
    {
        FileName = string.Empty;
        Format = string.Empty;
        Status = string.Empty;
        DisplaySize = string.Empty;
    }

    public BatchConvertItem(string fileName, long size, string format, Subtitle? subtitle)
    {
        FileName = fileName;
        Size = size;
        Format = format;
        Status = "-";
        Subtitle = subtitle;
        DisplaySize = Utilities.FormatBytesToDisplayFileSize(size);
    }
}
