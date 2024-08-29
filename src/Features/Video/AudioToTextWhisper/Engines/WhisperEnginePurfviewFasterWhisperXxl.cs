using System.Diagnostics;
using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public class WhisperEnginePurfviewFasterWhisperXxl : IWhisperEngine
{
    public static string StaticName => "Purfview Faster Whisper XXL";
    public string Name => StaticName;
    public string Url => "https://github.com/Purfview/whisper-standalone-win";
    public List<WhisperLanguage> Languages => WhisperLanguage.Languages.OrderBy(p => p.Name).ToList();
    public List<WhisperModel> Models { get; } = new();

    public string Extension => string.Empty;

    public bool IsEngineInstalled()
    {
        var folder = GetAndCreateWhisperFolder();
        var executableFile = Path.Combine(folder, GetExecutableFileName());
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

    public bool IsModelInstalled(WhisperModel model)
    {
        var modelFileName = Path.Combine(GetAndCreateWhisperModelFolder(), model.Name + Extension);
        var fileExists = File.Exists(modelFileName);
        return fileExists;
    }

    public override string ToString()
    {
        return Name;
    }

    public string GetExecutableFileName()
    {
        return "faster-whisper-xxl.exe";
    }
}
