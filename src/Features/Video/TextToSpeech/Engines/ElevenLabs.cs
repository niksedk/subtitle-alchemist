﻿using System.IO.Compression;
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
    public bool IsInstalled => !string.IsNullOrEmpty(Se.Settings.Video.TextToSpeech.ElevenLabsApiKey);

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
        return Task.FromResult(Array.Empty<TtsLanguage>());
    }

    public async Task<Voice[]> RefreshVoices(CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await _ttsDownloadService.DownloadElevenLabsVoiceList(ms, null, cancellationToken);
        await File.WriteAllBytesAsync(Path.Combine(GetSetElevenLabsFolder(), JsonFileName), ms.ToArray(), cancellationToken);
        return await GetVoices();
    }

    public async Task<TtsResult> Speak(string text, Voice voice)
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