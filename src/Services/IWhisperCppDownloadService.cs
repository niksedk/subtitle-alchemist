namespace SubtitleAlchemist.Services;

public interface IWhisperCppDownloadService
{
    Task DownloadWhisperCpp(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadWhisperCpp(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}