﻿using SkiaSharp;
using SubtitleAlchemist.Logic.BluRaySup;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public class BluRayPcsDataList : IOcrSubtitle
{
    public int Count { get; private set; }

    private readonly List<BluRaySupParser.PcsData> _pcsDataList;

    public SKBitmap GetBitmap(int index)
    {
        return _pcsDataList[index].GetBitmap();
    }

    public TimeSpan GetStartTime(int index)
    {
        return TimeSpan.FromMilliseconds(_pcsDataList[index].StartTime / 90.0);
    }

    public TimeSpan GetEndTime(int index)
    {
        return TimeSpan.FromMilliseconds(_pcsDataList[index].EndTime / 90.0);
    }

    public BluRayPcsDataList(List<BluRaySupParser.PcsData> pcsDataList)
    {
        _pcsDataList = pcsDataList;
        Count = pcsDataList.Count;
    }

    public List<OcrSubtitleItem> MakeOcrSubtitleItems()
    {
        var ocrSubtitleItems = new List<OcrSubtitleItem>(Count);
        for (var i = 0; i < Count; i++)
        {
            ocrSubtitleItems.Add(new OcrSubtitleItem(this, i));
        }

        return ocrSubtitleItems;
    }
}