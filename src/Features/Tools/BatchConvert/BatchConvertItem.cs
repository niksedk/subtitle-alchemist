﻿using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;
public class BatchConvertItem
{
    public string FileName { get; set; }
    public long Size { get; set; }
    public string Format { get; set; }
    public string Status { get; set; }
    public Subtitle? Subtitle { get; set; }

    public BatchConvertItem()
    {
        FileName = string.Empty;
        Format = string.Empty;
        Status = string.Empty;
    }

    public BatchConvertItem(string fileName, long size, string format, Subtitle? subtitle)
    {
        FileName = fileName;
        Size = size;
        Format = format;
        Status = "-";
        Subtitle = subtitle;
    }
}
