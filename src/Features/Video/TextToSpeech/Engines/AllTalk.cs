﻿using System.IO.Compression;
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
    public bool HasApiKey => false;
    public bool HasRegion => false;
    public bool HasModel => false;

    private bool _isInstalled;
    public async Task<bool> IsInstalled(string? region)
    {
        if (_isInstalled)
        {
            return true;
        }

        try
        {
            _isInstalled = await _ttsDownloadService.AllTalkIsInstalled();
            return _isInstalled;
        }
        catch
        {
            return false;
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

    public async Task<Voice[]> GetVoices(string language)
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

    public Task<string[]> GetRegions()
    {
        return Task.FromResult(Array.Empty<string>());
    }

    public Task<string[]> GetModels()
    {
        return Task.FromResult(Array.Empty<string>());
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

    public Task<TtsLanguage[]> GetLanguages(Voice voice, string? model)
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

    public async Task<Voice[]> RefreshVoices(string language,CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await _ttsDownloadService.DownloadAllTalkVoiceList(ms, null, cancellationToken);
        await File.WriteAllBytesAsync(Path.Combine(GetSetAllTalkFolder(), JsonFileName), ms.ToArray(), cancellationToken);
        return await GetVoices(language);
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
        if (voice.EngineVoice is not AllTalkVoice allTalkVoice)
        {
            throw new ArgumentException("Voice is not a AllTalkVoice");
        }

        var languageCode = language != null ? language.Code : "en";
        var allTalkFileNameOutputFileName = await _ttsDownloadService.AllTalkVoiceSpeak(text, allTalkVoice, languageCode);

        var outputFileName = Path.Combine(GetSetAllTalkFolder(), Guid.NewGuid() + ".wav");
        File.Move(allTalkFileNameOutputFileName, outputFileName);

        return new TtsResult { FileName = outputFileName, Text = text };
    }
}