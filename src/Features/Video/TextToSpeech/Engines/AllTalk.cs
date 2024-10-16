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
    public bool HasLanguageParameter => true;

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

    public Task<TtsLanguage[]> GetLanguages(Voice voice)
    {
        var languagePairs = new List<TtsLanguage>()
        {
            new ("English", "en"),
            new ("Arabic", "ar"),
            new ("Chinese", "zh-cn"),
            new ("Czech", "cs"),
            new ("Dutch", "nl"),
            new ("French", "fr"),
            new ("German", "de"),
            new ("Hindi", "hi"),
            new ("Hungarian", "hu"),
            new ("Italian", "it"),
            new ("Japanese", "ja"),
            new ("Turkish", "tr"),
            new ("Korean", "ko"),
            new ("Polish", "pl"),
            new ("Portuguese", "pt"),
            new ("Russian", "ru"),
            new ("Spanish", "es"),
        };

        return Task.FromResult(languagePairs.ToArray());
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