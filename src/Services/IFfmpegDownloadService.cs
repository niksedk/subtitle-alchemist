namespace SubtitleAlchemist.Services;

public interface IFfmpegDownloadService
{
    Task DownloadFfmpeg(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
}