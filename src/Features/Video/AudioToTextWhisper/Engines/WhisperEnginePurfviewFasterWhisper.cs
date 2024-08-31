﻿using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public class WhisperEnginePurfviewFasterWhisper : IWhisperEngine
{
    public static string StaticName => "Purfview Faster Whisper";
    public string Name => StaticName;
    public string Choice => WhisperChoice.PurfviewFasterWhisper;
    public string Url => "https://github.com/Purfview/whisper-standalone-win";
    public List<WhisperLanguage> Languages => WhisperLanguage.Languages.OrderBy(p => p.Name).ToList();
    public List<WhisperModel> Models
    {
        get
        {
            var models = new WhisperPurfviewFasterWhisperModel().Models;
            return models.ToList();
        }
    }

    public string Extension => string.Empty;

    public string UnpackSkipFolder => "Whisper-Faster/";

    public bool IsEngineInstalled()
    {
        var executableFile = GetExecutable();
        return File.Exists(executableFile);
    }

    public string GetAndCreateWhisperFolder()
    {
        var baseFolder = Path.Combine(FileSystem.Current.AppDataDirectory, "Whisper");
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }

        var folder = Path.Combine(baseFolder, "Purfview-Whisper-Faster");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return folder;
    }

    public string GetAndCreateWhisperModelFolder()
    {
        var baseFolder = GetAndCreateWhisperFolder();

        var folder = Path.Combine(baseFolder, "_models");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return folder;
    }

    public string GetExecutable()
    {
        var fullPath = Path.Combine(GetAndCreateWhisperFolder(), GetExecutableFileName());

        return fullPath;
    }

    public bool IsModelInstalled(WhisperModel model)
    {
        var modelFileName = Path.Combine(GetAndCreateWhisperModelFolder(), model.Name);
        if (Extension.Length > 0 && !modelFileName.EndsWith(Extension))
        {
            modelFileName += Extension;
        }
        var fileExists = File.Exists(modelFileName);

        return fileExists;
    }

    public string GetModelForCmdLine(string modelName)
    {
        return modelName;
    }

    public override string ToString()
    {
        return Name;
    }

    public async Task<string> GetHelpText()
    {
        var assetName = $"{StaticName.Replace(" ", string.Empty)}.txt";
        await using var stream = await FileSystem.OpenAppPackageFileAsync(assetName);
        using var reader = new StreamReader(stream);

        var contents = await reader.ReadToEndAsync();

        return contents;
    }

    public string GetExecutableFileName()
    {
        return "whisper-faster.exe";
    }
}
