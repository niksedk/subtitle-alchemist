﻿using System.Runtime.InteropServices;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Services;

public class TesseractDownloadService : ITesseractDownloadService
{
    private readonly HttpClient _httpClient;
    private const string WindowsUrl = "https://github.com/SubtitleEdit/support-files/releases/download/tesseract550/Tesseract550.zip";
    private const string MacUrl = "https://github.com/SubtitleEdit/support-files/releases/download/tesseract550/Tesseract550.zip"; //TODO:
    private readonly IZipUnpacker _zipUnpacker;

    public TesseractDownloadService(HttpClient httpClient, IZipUnpacker zipUnpacker)
    {
        _httpClient = httpClient;
        _zipUnpacker = zipUnpacker;
    }

    private static string GetTesseractUrl()
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

    public async Task DownloadTesseract(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, GetTesseractUrl(), stream, progress, cancellationToken);
    }

    public async Task DownloadTesseractModel(string modelUrl, Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, modelUrl, stream, progress, cancellationToken);
    }
}