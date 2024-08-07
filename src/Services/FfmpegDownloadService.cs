namespace SubtitleAlchemist.Services
{
    public class FfmpegDownloadService : IFfmpegDownloadService
    {
        private readonly HttpClient _httpClient;
        private const string WindowsUrl = "https://github.com/SubtitleEdit/support-files/releases/download/ffmpeg-2024-05/ffmpeg-2024-05-23.zip";

        public FfmpegDownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DownloadFfmpeg(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            await DownloadHelper.DownloadFileAsync(_httpClient, WindowsUrl, destinationFileName, progress, cancellationToken);
        }
    }
}
