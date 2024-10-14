﻿using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public class Piper : ITtsEngine
{
    public string Name => "Piper";
    public string Description => "free/fast/good";
    public bool HasLanguageParameter => false;
    public bool IsInstalled => File.Exists(GetPiperExecutableFileName());

    public override string ToString()
    {
        return $"{Name}";
    }

    public async Task<Voice[]> GetVoices()
    {
        var piperFolder = GetSetPiperFolder();

        var voiceFileName = Path.Combine(piperFolder, "voices.json");
        if (!File.Exists(voiceFileName))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TtsPiperVoices.zip");
            using var reader = new StreamReader(stream);
            ZipFile.ExtractToDirectory(stream, piperFolder);
        }

        return Map(voiceFileName);
    }

    private static string GetPiperExecutableFileName()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(GetSetPiperFolder(), "piper.exe");
        }

        return "piper";
    }

    private static string GetSetPiperFolder()
    {
        if (!Directory.Exists(Se.TtsFolder))
        {
            Directory.CreateDirectory(Se.TtsFolder);
        }

        var piperFolder = Path.Combine(Se.TtsFolder, "Piper");
        if (!Directory.Exists(piperFolder))
        {
            Directory.CreateDirectory(piperFolder);
        }

        return piperFolder;
    }

    public async Task<string[]> GetLanguages(Voice voice)
    {
        return Array.Empty<string>();
    }

    private static Voice[] Map(string voiceFileName)
    {
        if (string.IsNullOrWhiteSpace(voiceFileName))
        {
            return Array.Empty<Voice>();
        }

        var result = new List<Voice>();
        var json = File.ReadAllText(voiceFileName);
        var parser = new SeJsonParser();
        var arr = parser.GetRootElements(json);

        foreach (var element in arr)
        {
            var elements = parser.GetRootElements(element.Json);
            var name = elements.FirstOrDefault(p => p.Name == "name");
            var quality = elements.FirstOrDefault(p => p.Name == "quality");
            var language = elements.FirstOrDefault(p => p.Name == "language");
            var files = elements.FirstOrDefault(p => p.Name == "files");

            if (name != null && quality != null && language != null && files != null)
            {
                var languageDisplay = parser.GetFirstObject(language.Json, "name_english");
                var languageFamily = parser.GetFirstObject(language.Json, "family");
                var languageCode = parser.GetFirstObject(language.Json, "code");

                var filesElements = parser.GetRootElements(files.Json);
                var model = filesElements.FirstOrDefault(p => p.Name.EndsWith(".onnx"));
                var config = filesElements.FirstOrDefault(p => p.Name.EndsWith("onnx.json"));
                if (model != null && config != null)
                {
                    var modelUrl = "https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/" + model.Name;
                    var configUrl = "https://huggingface.co/rhasspy/piper-voices/resolve/v1.0.0/" + config.Name;

                    var piperVoice = new PiperVoice(name.Json, languageDisplay, quality.Json, modelUrl, configUrl);
                    result.Add(new Voice() { Name = piperVoice.Voice, EngineVoice = piperVoice });
                }
            }
        }

        return result.ToArray();
    }

    public async Task<Voice[]> RefreshVoices()
    {
        throw new NotImplementedException();
    }

    public async Task<List<object>> Speak(string text, Voice voice)
    {
        var piperVoice = voice.EngineVoice as PiperVoice;
        var process = StartPiperProcess(piperVoice, text);
        return null;
    }

    private Process StartPiperProcess(PiperVoice voice, string inputText)
    {
        var processPiper = new Process
        {
            StartInfo =
            {
                WorkingDirectory = GetSetPiperFolder(),
                FileName = GetPiperExecutableFileName(),
                Arguments = $"-m \"{voice.ModelShort}\" -c \"{voice.ConfigShort}\" -f out.wav",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
            }
        };

        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsIOS())
        {
            _ = processPiper.Start();
        }
        else
        {
            throw new PlatformNotSupportedException("Process.Start() is not supported on this platform.");
        }

        var streamWriter = new StreamWriter(processPiper.StandardInput.BaseStream, Encoding.UTF8);
        var text = Utilities.UnbreakLine(inputText);
        streamWriter.Write(text);
        streamWriter.Flush();
        streamWriter.Close();

        return processPiper;
    }
}