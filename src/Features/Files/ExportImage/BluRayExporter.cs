using SkiaSharp;
using SkiaSharp.Views.Maui;
using SubtitleAlchemist.Logic.BluRaySup;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Files.ExportImage;

public class BluRayExporter : IImageExporter
{
    public bool Export(Stream output, ExportImages settings, List<ImageExportLine> lines, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        var finalImages = new List<byte[]>();
        var counter = 0;
        Parallel.ForEach(lines, (line, loopState) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                loopState.Stop();
                return;
            }

            var brSub = new BluRaySupPicture
            {
                StartTime = (long)Math.Round(line.Paragraph.StartTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                EndTime = (long)Math.Round(line.Paragraph.EndTime.TotalMilliseconds, MidpointRounding.AwayFromZero),
                Width = 1920, // settings.ScreenWidth,
                Height = 1080, //param.ScreenHeight,
                IsForced = line.Paragraph.Forced,
                CompositionNumber = line.Paragraph.Number * 2,
            };

            //if (param.OverridePosition != null &&
            //                   param.OverridePosition.Value.X >= 0 && param.OverridePosition.Value.X < param.ScreenWidth &&
            //                   param.OverridePosition.Value.Y >= 0 && param.OverridePosition.Value.Y < param.ScreenHeight)
            //{
            //    param.LeftMargin = param.OverridePosition.Value.X;
            //    param.BottomMargin = param.ScreenHeight - param.OverridePosition.Value.Y - param.Bitmap.Height;
            //}

            //var margin = (param.Alignment == ContentAlignment.TopRight ||
            //              param.Alignment == ContentAlignment.MiddleRight ||
            //              param.Alignment == ContentAlignment.BottomRight)
            //    ? param.RightMargin
            //    : param.LeftMargin;

            SKColor.TryParse(settings.FontColor, out var textColor);
            SKColor.TryParse(settings.BorderColor, out var borderColor);
            SKColor.TryParse(settings.ShadowColor, out var shadowColor);
            var bitmap = TextToImageGenerator.GenerateImage(line.Paragraph.Text, settings.FontName, settings.FontSize, settings.IsBold, textColor, borderColor, shadowColor , SKColors.Transparent, 2, settings.ShadowWidth, 0);

            finalImages.Add(BluRaySupPicture.CreateSupFrame(brSub, bitmap, (double)settings.FrameRate, settings.BottomMargin, settings.LeftRightMargin, BluRayContentAlignment.BottomCenter, null));

            Interlocked.Increment(ref counter);
            progress?.Report((float)counter / lines.Count * 100);
        });

        foreach (var image in finalImages)
        {
            output.Write(image, 0, image.Length);
        }

        return true;
    }
}
