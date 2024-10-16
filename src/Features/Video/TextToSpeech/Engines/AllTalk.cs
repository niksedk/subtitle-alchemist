using System.IO.Compression;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public class AllTalk : ITtsEngine
{
    public string Name => "AllTalk";
    public string Description => "free/fast/good";
    public bool HasLanguageParameter => false;

    private bool _isInstalled;
    public bool IsInstalled
    {
        get
        {
            return true;
            //if (_isInstalled)
            //{
            //    return true;
            //}

            //try
            //{
            //    var voices = GetVoices().Result;
            //    _isInstalled = voices.Length > 0;
            //    return _isInstalled;
            //}
            //catch 
            //{
            //    return false;
            //}
        }
    }

    private const string JsonFileName = "AllTalkVoices.json";

    public override string ToString()
    {
        return $"{Name}";
    }

    private readonly ITtsDownloadService _ttsDownloadService;

    public AllTalk(ITtsDownloadService ttsDownloadService)
    {
        _ttsDownloadService = ttsDownloadService;
    }

    public async Task<Voice[]> GetVoices()
    {
        var allTalkFolder = GetSetAllTalkFolder();
        var jsonFileName = Path.Combine(allTalkFolder, JsonFileName);

        if (!File.Exists(jsonFileName))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TtsAllTalkVoices.zip");
            using var reader = new StreamReader(stream);
            ZipFile.ExtractToDirectory(stream, allTalkFolder);
        }

        var result = new List<Voice>();
        if (File.Exists(jsonFileName))
        {
            var json = await File.ReadAllTextAsync(jsonFileName);
            var parser = new SeJsonParser();
            var voices = parser.GetArrayElementsByName(json, "voices");
            foreach (var voice in voices)
            {
                result.Add(new Voice(new AllTalkVoice(voice)));
            }
        }

        return result.ToArray();
    }

    public static string GetSetAllTalkFolder()
    {
        if (!Directory.Exists(Se.TtsFolder))
        {
            Directory.CreateDirectory(Se.TtsFolder);
        }

        var allTalkFolder = Path.Combine(Se.TtsFolder, "AllTalk");
        if (!Directory.Exists(allTalkFolder))
        {
            Directory.CreateDirectory(allTalkFolder);
        }

        return allTalkFolder;
    }

    public bool IsVoiceInstalled(Voice voice)
    {
        return true;
    }

    public Task<string[]> GetLanguages(Voice voice)
    {
        return Task.FromResult(Array.Empty<string>());
    }

    public async Task<Voice[]> RefreshVoices(CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await _ttsDownloadService.DownloadAllTalkVoiceList(ms, null, cancellationToken);
        await File.WriteAllBytesAsync(Path.Combine(GetSetAllTalkFolder(), JsonFileName), ms.ToArray(), cancellationToken);
        return await GetVoices();
    }

    public async Task<TtsResult> Speak(string text, Voice voice)
    {
        if (voice.EngineVoice is not AllTalkVoice allTalkVoice)
        {
            throw new ArgumentException("Voice is not a AllTalkVoice");
        }

        var fileNameOnly = Guid.NewGuid() + ".wav";
        var outputFileName = Path.Combine(GetSetAllTalkFolder(), fileNameOnly);

        var language = "en";
        await _ttsDownloadService.AllTalkVoiceSpeak(text, allTalkVoice, language, outputFileName);

        return new TtsResult();
    }
}