using SkiaSharp;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public interface IOcrSubtitle
{
    int Count { get; }
    SKBitmap GetBitmap(int index);
    TimeSpan GetStartTime(int index);
    TimeSpan GetEndTime(int index);
    List<OcrSubtitleItem> MakeOcrSubtitleItems();
}