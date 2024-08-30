﻿using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public class WhisperEngineConstMe : IWhisperEngine
{
    public static string StaticName => "Whisper Const-me";
    public string Name => StaticName;
    public string Choice => WhisperChoice.Cpp;
    public string Url => "https://github.com/Const-me/Whisper";

    public List<WhisperLanguage> Languages => WhisperLanguage.Languages.OrderBy(p => p.Name).ToList();

    public List<WhisperModel> Models
    {
        get
        {
            var models = new WhisperConstMeModel().Models;
            return models.ToList();
        }
    }

    public string Extension => ".bin";
    public string UnpackSkipFolder => string.Empty;

    public bool IsEngineInstalled()
    {
        var executableFile = GetExecutable();
        return File.Exists(executableFile);
    }

    public override string ToString()
    {
        return Name;
    }

    public string GetAndCreateWhisperFolder()
    {
        var baseFolder = Path.Combine(FileSystem.Current.AppDataDirectory, "Whisper");
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }

        var folder = Path.Combine(baseFolder, "Const-me");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return folder;
    }

    public string GetAndCreateWhisperModelFolder()
    {
        var baseFolder = GetAndCreateWhisperFolder();

        var folder = Path.Combine(baseFolder, "Models");
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
        var modelFileName = Path.Combine(GetAndCreateWhisperModelFolder(), modelName);
        if (Extension.Length > 0 && !modelFileName.EndsWith(Extension))
        {
            modelFileName += Extension;
        }
        return modelFileName;
    }

    public string GetExecutableFileName()
    {
        return "main.exe";
    }
}