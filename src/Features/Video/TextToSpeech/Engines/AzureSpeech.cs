using System.IO.Compression;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public class AzureSpeech : ITtsEngine
{
    public string Name => "AzureSpeech";
    public string Description => "pay/fast/good";
    public bool HasLanguageParameter => true;
    public bool HasApiKey => true;
    public bool HasRegion => true;
    public bool HasModel => false;

    public Task<bool> IsInstalled()
    {
        return Task.FromResult(!string.IsNullOrEmpty(Se.Settings.Video.TextToSpeech.AzureApiKey));
    }

    private const string JsonFileName = "AzureVoices.json";
    private readonly ITtsDownloadService _ttsDownloadService;

    public AzureSpeech(ITtsDownloadService ttsDownloadService)
    {
        _ttsDownloadService = ttsDownloadService;
    }

    public override string ToString()
    {
        return $"{Name}";
    }

    public async Task<Voice[]> GetVoices()
    {
        var elevenLabsFolder = GetSetAzureFolder();

        var voiceFileName = Path.Combine(elevenLabsFolder, JsonFileName);
        if (!File.Exists(voiceFileName))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TtsAzureVoices.zip");
            using var reader = new StreamReader(stream);
            ZipFile.ExtractToDirectory(stream, elevenLabsFolder);
        }

        return Map(voiceFileName);
    }

    private Voice[] Map(string voiceFileName)
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

    public static string GetSetAzureFolder()
    {
        if (!Directory.Exists(Se.TtsFolder))
        {
            Directory.CreateDirectory(Se.TtsFolder);
        }

        var azureFolder = Path.Combine(Se.TtsFolder, "Azure");
        if (!Directory.Exists(azureFolder))
        {
            Directory.CreateDirectory(azureFolder);
        }

        return azureFolder;
    }

    public bool IsVoiceInstalled(Voice voice)
    {
        return true;
    }

    public Task<TtsLanguage[]> GetLanguages(Voice voice)
    {
        return Task.FromResult(Array.Empty<TtsLanguage>());
    }

    public async Task<Voice[]> RefreshVoices(CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await _ttsDownloadService.DownloadElevenLabsVoiceList(ms, null, cancellationToken);
        await File.WriteAllBytesAsync(Path.Combine(GetSetAzureFolder(), JsonFileName), ms.ToArray(), cancellationToken);
        return await GetVoices();
    }

    public async Task<TtsResult> Speak(string text, string outputFolder, Voice voice, TtsLanguage? language, CancellationToken cancellationToken)
    {
        // if (voice.EngineVoice is not PiperVoice piperVoice)
        // {
        //     throw new ArgumentException("Voice is not a PiperVoice");
        // }
        //
        // var fileNameOnly = Guid.NewGuid() + ".wav";
        // var process = StartPiperProcess(piperVoice, text, fileNameOnly);
        // await process.WaitForExitAsync();
        //
        // var fileName = Path.Combine(GetSetPiperFolder(), fileNameOnly);
        return new TtsResult();
    }
}