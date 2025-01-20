namespace SubtitleAlchemist.Services;

public interface IPaddleOcrDownloadService
{
    Task DownloadModels(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}