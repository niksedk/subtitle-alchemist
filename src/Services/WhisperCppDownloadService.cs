﻿using System.Runtime.InteropServices;

namespace SubtitleAlchemist.Services;

public class WhisperCppDownloadService : IWhisperCppDownloadService
{
    private readonly HttpClient _httpClient;
    private const string WindowsUrl = "https://github.com/ggerganov/whisper.cpp/releases/download/v1.6.0/whisper-blas-clblast-bin-x64.zip";
    private const string MacUrl = "https://github.com/ggerganov/whisper.cpp/releases/download/v1.6.0/whisper-blas-clblast-bin-x64.zip";

    public WhisperCppDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DownloadWhisperCpp(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, GetUrl(), destinationFileName, progress, cancellationToken);
    }

    public async Task DownloadFile(string url, string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, url, destinationFileName, progress, cancellationToken);
    }

    public async Task DownloadWhisperCpp(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, GetUrl(), stream, progress, cancellationToken);
    }

    private static string GetUrl()
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
}