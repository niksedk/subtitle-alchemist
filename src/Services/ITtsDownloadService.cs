using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

namespace SubtitleAlchemist.Services;

public interface ITtsDownloadService
{
    Task DownloadPiper(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadPiper(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadPiperModel(string destinationFileName, PiperVoice voice, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadPiperVoice(string modelUrl, MemoryStream downloadStream, Progress<float> downloadProgress, CancellationToken token);
    Task DownloadPiperVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadAllTalkVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task<string> AllTalkVoiceSpeak(string text, AllTalkVoice voice, string language);
    Task<bool> AllTalkIsInstalled();
    Task DownloadElevenLabsVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}