using System.Runtime.InteropServices;

namespace SubtitleAlchemist.Services;

public class FfmpegDownloadService : IFfmpegDownloadService
{
    private readonly HttpClient _httpClient;
    private const string WindowsUrl = "https://github.com/SubtitleEdit/support-files/releases/download/ffmpeg-2025-03-31/ffmpeg-2025-03-31.zip";
    private const string MacUrl = "https://github.com/SubtitleEdit/support-files/releases/download/ffmpeg-v7-1/ffmpeg-mac-7.1.1.zip";

    public FfmpegDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static string GetFfmpegUrl()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return WindowsUrl;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return MacUrl;
        }

        throw new PlatformNotSupportedException();
    }

    public async Task DownloadFfmpeg(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, GetFfmpegUrl(), destinationFileName, progress, cancellationToken);
    }

    public async Task DownloadFfmpeg(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, GetFfmpegUrl(), stream, progress, cancellationToken);
    }
}