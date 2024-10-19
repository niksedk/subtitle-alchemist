using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using System.Net.Http;
using System.Text;

namespace SubtitleAlchemist.Services;

public class TtsDownloadService : ITtsDownloadService
{
    private readonly HttpClient _httpClient;
    private const string WindowsPiperUrl = "https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_windows_amd64.zip";
    private const string MacPiperUrl = "https://github.com/rhasspy/piper/releases/download/2023.11.14-2/piper_macos_x64.tar.gz";

    public TtsDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DownloadPiper(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        var url = OperatingSystem.IsWindows() ? WindowsPiperUrl : MacPiperUrl;
        await DownloadHelper.DownloadFileAsync(_httpClient, WindowsPiperUrl, destinationFileName, progress, cancellationToken);
    }

    public async Task DownloadPiper(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        var url = OperatingSystem.IsWindows() ? WindowsPiperUrl : MacPiperUrl;
        await DownloadHelper.DownloadFileAsync(_httpClient, url, stream, progress, cancellationToken);
    }

    public async Task DownloadPiperModel(string destinationFileName, PiperVoice voice, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, voice.Model, destinationFileName, progress, cancellationToken);
        await DownloadHelper.DownloadFileAsync(_httpClient, voice.Config, destinationFileName, progress, cancellationToken);
    }

    public async Task DownloadPiperVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        var url = "https://huggingface.co/rhasspy/piper-voices/resolve/main/voices.json?download=true";
        await DownloadHelper.DownloadFileAsync(_httpClient, url, stream, progress, cancellationToken);
    }

    public async Task DownloadPiperVoice(string url, MemoryStream stream, Progress<float> progress, CancellationToken cancellationToken)
    {
        await DownloadHelper.DownloadFileAsync(_httpClient, url, stream, progress, cancellationToken);
    }

    public async Task DownloadAllTalkVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        var url = Se.Settings.Video.TextToSpeech.AllTalkUrl.TrimEnd('/') + "/api/voices";
        await DownloadHelper.DownloadFileAsync(_httpClient, url, stream, progress, cancellationToken);
    }

    public async Task<string> AllTalkVoiceSpeak(string inputText, AllTalkVoice voice, string language)
    {
        var multipartContent = new MultipartFormDataContent();
        var text = Utilities.UnbreakLine(inputText);
        multipartContent.Add(new StringContent(Json.EncodeJsonText(text)), "text_input");
        multipartContent.Add(new StringContent("standard"), "text_filtering");
        multipartContent.Add(new StringContent(voice.Voice), "character_voice_gen");
        multipartContent.Add(new StringContent("false"), "narrator_enabled");
        multipartContent.Add(new StringContent(voice.Voice), "narrator_voice_gen");
        multipartContent.Add(new StringContent("character"), "text_not_inside");
        multipartContent.Add(new StringContent(language), "language");
        multipartContent.Add(new StringContent("output"), "output_file_name");
        multipartContent.Add(new StringContent("false"), "output_file_timestamp");
        multipartContent.Add(new StringContent("false"), "autoplay");
        multipartContent.Add(new StringContent("1.0"), "autoplay_volume");
        var result = await _httpClient.PostAsync(Se.Settings.Video.TextToSpeech.AllTalkUrl.TrimEnd('/') + "/api/tts-generate", multipartContent);
        var bytes = await result.Content.ReadAsByteArrayAsync();
        var resultJson = Encoding.UTF8.GetString(bytes);

        if (!result.IsSuccessStatusCode)
        {
            SeLogger.Error($"All Talk TTS failed calling API as base address {_httpClient.BaseAddress} : Status code={result.StatusCode}" + Environment.NewLine + resultJson);
        }

        var jsonParser = new SeJsonParser();
        var allTalkOutput = jsonParser.GetFirstObject(resultJson, "output_file_path");
        return allTalkOutput.Replace("\\\\", "\\");
    }

    public async Task<bool> AllTalkIsInstalled()
    {
        var timeout = Task.Delay(2000); // 2 seconds timeout
        var request = _httpClient.GetAsync(Se.Settings.Video.TextToSpeech.AllTalkUrl);

        await Task.WhenAny(timeout, request); // wait for either timeout or the request

        if (timeout.IsCompleted) // if the timeout ended first, then handle it
        {
            return false;
        }

        return true;
    }

    public async Task DownloadElevenLabsVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken)
    {
        var url = "https://api.elevenlabs.io/v1/voices";
        await DownloadHelper.DownloadFileAsync(_httpClient, url, stream, progress, cancellationToken);
    }

}