﻿using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public class WhisperEngineOpenAi : IWhisperEngine
{
    public static string StaticName => "Whisper Open AI";
    public string Name => StaticName;
    public string Choice => WhisperChoice.OpenAi;
    public string Url => "https://github.com/openai/whisper";

    public List<WhisperLanguage> Languages => WhisperLanguage.Languages.OrderBy(p => p.Name).ToList();

    public List<WhisperModel> Models
    {
        get
        {
            var models = new WhisperModel().Models;
            return models.ToList();
        }
    }

    public string Extension => ".pt";
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
        var folder = WhisperHelper.GetWhisperFolder(WhisperChoice.OpenAi);
        return folder;
    }

    public string GetAndCreateWhisperModelFolder()
    {
        var folder = new WhisperModel().ModelFolder;
        return folder;
    }

    public string GetExecutable()
    {
        var fullPath = WhisperHelper.GetExecutableFileName(WhisperChoice.OpenAi);
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

    public async Task<string> GetHelpText()
    {
        var assetName = $"{StaticName.Replace(" ", string.Empty)}.txt";
        await using var stream = await FileSystem.OpenAppPackageFileAsync(assetName);
        using var reader = new StreamReader(stream);

        var contents = await reader.ReadToEndAsync();

        return contents;
    }
}
