namespace SubtitleAlchemist.Services;

public interface IWhisperDownloadService
{
    Task DownloadWhisperCpp(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadFile(string url, string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadWhisperCpp(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadWhisperConstMe(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadWhisperPurfviewFasterWhisper(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}