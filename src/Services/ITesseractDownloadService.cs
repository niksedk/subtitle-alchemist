using SubtitleAlchemist.Features.Shared.Ocr;

namespace SubtitleAlchemist.Services;

public interface ITesseractDownloadService
{
    Task DownloadTesseract(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadTesseractModel(string modelUrl, Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}