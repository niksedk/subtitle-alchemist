using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

namespace SubtitleAlchemist.Services;

public interface ITtsDownloadService
{
    Task DownloadPiper(string destinationFileName, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadPiper(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadPiperModel(string destinationFileName, PiperVoice voice, IProgress<float>? progress, CancellationToken cancellationToken);
    Task DownloadPiperVoiceList(Stream stream, IProgress<float>? progress, CancellationToken cancellationToken);
}