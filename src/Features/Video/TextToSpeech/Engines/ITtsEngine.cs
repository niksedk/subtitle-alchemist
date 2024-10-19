
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public interface ITtsEngine
{
    string Name { get; }
    string Description { get; }
    bool HasLanguageParameter { get; }
    bool HasApiKey { get; }
    bool HasRegion { get; }
    bool HasModel { get; }
    Task<bool> IsInstalled();
    string ToString();
    Task<Voice[]> GetVoices();
    bool IsVoiceInstalled(Voice voice);
    Task<TtsLanguage[]> GetLanguages(Voice voice);
    Task<Voice[]> RefreshVoices(CancellationToken cancellationToken);
    Task<TtsResult> Speak(string text, string outputFolder, Voice voice, TtsLanguage? language, CancellationToken cancellationToken);
}
