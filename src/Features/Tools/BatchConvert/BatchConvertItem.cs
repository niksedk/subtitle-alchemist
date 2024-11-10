using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public partial class BatchConvertItem : ObservableObject
{
    public string FileName { get; set; }
    public long Size { get; set; }
    public string Format { get; set; }
    [ObservableProperty] private string _status;
    public Subtitle? Subtitle { get; set; }

    public BatchConvertItem()
    {
        FileName = string.Empty;
        Format = string.Empty;
        _status = string.Empty;
    }

    public BatchConvertItem(string fileName, long size, string format, Subtitle? subtitle)
    {
        FileName = fileName;
        Size = size;
        Format = format;
        _status = "-";
        Subtitle = subtitle;
    }
}
