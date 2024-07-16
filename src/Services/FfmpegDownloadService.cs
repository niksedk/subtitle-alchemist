namespace SubtitleAlchemist.Services
{
    public class FfmpegDownloadService : IFfmpegDownloadService
    {
        private readonly HttpClient _httpClient;
        private const string _remoteServiceBaseUrl = "url";

        public FfmpegDownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadFfmpeg()
        {
            var responseString = await _httpClient.GetStringAsync("uri");
            return new byte[] { 0 };
        }
    }
}
