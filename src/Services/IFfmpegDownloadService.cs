namespace SubtitleAlchemist.Services;

public interface IFfmpegDownloadService
{
    Task DownloadFfmpeg(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadFfmpeg(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}