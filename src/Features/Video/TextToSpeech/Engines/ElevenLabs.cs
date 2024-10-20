using System.IO.Compression;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public class ElevenLabs : ITtsEngine
{
    public string Name => "ElevenLabs";
    public string Description => "pay/fast/good";
    public bool HasLanguageParameter => true;
    public bool HasApiKey => true;
    public bool HasRegion => false;
    public bool HasModel => true;

    public Task<bool> IsInstalled(string? region)
    {
        return Task.FromResult(!string.IsNullOrEmpty(Se.Settings.Video.TextToSpeech.ElevenLabsApiKey));
    }

    private const string JsonFileName = "eleven-labs-voices.json";
    private readonly ITtsDownloadService _ttsDownloadService;

    public ElevenLabs(ITtsDownloadService ttsDownloadService)
    {
        _ttsDownloadService = ttsDownloadService;
    }

    public override string ToString()
    {
        return $"{Name}";
    }

    public async Task<Voice[]> GetVoices()
    {
        var elevenLabsFolder = GetSetElevenLabsFolder();

        var voiceFileName = Path.Combine(elevenLabsFolder, JsonFileName);
        if (!File.Exists(voiceFileName))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TtsElevenLabVoices.zip");
            using var reader = new StreamReader(stream);
            ZipFile.ExtractToDirectory(stream, elevenLabsFolder);
        }

        return Map(voiceFileName);
    }

    private static Voice[] Map(string voiceFileName)
    {
        if (!File.Exists(voiceFileName))
        {
            return Array.Empty<Voice>();
        }

        var result = new List<Voice>();
        var json = File.ReadAllText(voiceFileName);
        var parser = new SeJsonParser();
        var voices = parser.GetArrayElementsByName(json, "voices");
        foreach (var voice in voices)
        {
            var name = parser.GetFirstObject(voice, "name");
            var voiceId = parser.GetFirstObject(voice, "voice_id");
            var gender = parser.GetFirstObject(voice, "gender");
            var description = parser.GetFirstObject(voice, "description");
            var accent = parser.GetFirstObject(voice, "accent");
            var useCase = parser.GetFirstObject(voice, "use case");
            result.Add(new Voice(new ElevenLabVoice(string.Empty, name, gender, description, useCase, accent, voiceId)));
        }

        return result.ToArray();
    }

    public static string GetSetElevenLabsFolder()
    {
        if (!Directory.Exists(Se.TtsFolder))
        {
            Directory.CreateDirectory(Se.TtsFolder);
        }

        var elevenLabsFolder = Path.Combine(Se.TtsFolder, "ElevenLabs");
        if (!Directory.Exists(elevenLabsFolder))
        {
            Directory.CreateDirectory(elevenLabsFolder);
        }

        return elevenLabsFolder;
    }

    public bool IsVoiceInstalled(Voice voice)
    {
        return true;
    }

    public Task<TtsLanguage[]> GetLanguages(Voice voice)
    {
        // see https://help.elevenlabs.io/hc/en-us/articles/17883183930129-What-models-do-you-offer-and-what-is-the-difference-between-them

        var languages = new List<TtsLanguage>
        {
            new ("Arabic", "ar"),
            new ("Bulgarian", "bg"),
            new ("Chinese", "zh"),
            new ("Croatian", "hr"),
            new ("Czech", "cz"),
            new ("Danish", "da"),
            new ("Dutch", "nl"),
            new ("English", "en"),
            new ("Filipino", "ph"),
            new ("Finnish", "fi"),
            new ("French", "fr"),
            new ("German", "de"),
            new ("Greek", "el"),
            new ("Hindi", "hi"),
            new ("Hungarian", "hu"),
            new ("Indonesian", "id"),
            new ("Italian", "it"),
            new ("Japanese", "ja"),
            new ("Korean", "kr"),
            new ("Malay", "ms"),
            new ("Norwegian", "no"),
            new ("Polish", "pl"),
            new ("Portuguese", "pt"),
            new ("Romanian", "ro"),
            new ("Russian", "ru"),
            new ("Slovak", "sk"),
            new ("Spanish", "es"),
            new ("Swedish", "sv"),
            new ("Tamil", "ta"),
            new ("Turkish", "tr"),
            new ("Ukrainian", "uk"),
            new ("Vietnamese", "vi")
        };

        return Task.FromResult(languages.ToArray());
    }

    public async Task<Voice[]> RefreshVoices(CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await _ttsDownloadService.DownloadElevenLabsVoiceList(ms, null, cancellationToken);
        await File.WriteAllBytesAsync(Path.Combine(GetSetElevenLabsFolder(), JsonFileName), ms.ToArray(), cancellationToken);
        return await GetVoices();
    }

    public async Task<TtsResult> Speak(
        string text,
        string outputFolder,
        Voice voice,
        TtsLanguage? language,
        string? region,
        string? model,
        CancellationToken cancellationToken)
    {
        if (voice.EngineVoice is not ElevenLabVoice elevenLabVoice)
        {
            throw new ArgumentException("Voice is not an ElevenLabVoice");
        }

        var ms = new MemoryStream();
        var ok = await _ttsDownloadService.DownloadElevenLabsVoiceSpeak(text, elevenLabVoice, model, Se.Settings.Video.TextToSpeech.ElevenLabsApiKey, "en", ms, null, cancellationToken);
        if (!ok)
        {
            return new TtsResult { Text = text, FileName = string.Empty, Error = true };
        }

        var fileName = Path.Combine(GetSetElevenLabsFolder(), Guid.NewGuid() + ".mp3");
        await File.WriteAllBytesAsync(fileName, ms.ToArray(), cancellationToken);
        return new TtsResult { Text = text, FileName = fileName };
    }

    public Task<string[]> GetRegions()
    {
        return Task.FromResult(Array.Empty<string>());
    }

    public Task<string[]> GetModels()
    {
        return Task.FromResult(new[] { "eleven_turbo_v2_5", "eleven_multilingual_v2" });
    }
}