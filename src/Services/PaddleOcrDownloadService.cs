﻿namespace SubtitleAlchemist.Services;

public class PaddleOcrDownloadService : IPaddleOcrDownloadService
{
    private readonly HttpClient _httpClient;
    private const string ModelsUrl = "https://github.com/SubtitleEdit/support-files/releases/download/PaddleOcr291/PP-OCRv4.zip";

    public PaddleOcrDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DownloadModels(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, ModelsUrl, stream, progress, cancellationToken);
        
    }
}