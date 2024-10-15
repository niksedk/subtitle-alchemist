

using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public interface ITtsEngine
{
    string Name { get; }
    string Description { get; }
    bool HasLanguageParameter { get; }
    bool IsInstalled { get; }
    string ToString();
    Task<Voice[]> GetVoices();
    bool IsVoiceInstalled(Voice voice);
    Task<string[]> GetLanguages(Voice voice);
    Task<Voice[]> RefreshVoices(CancellationToken cancellationToken);
    Task<TtsResult> Speak(string text, Voice voice);
}
