using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public class WhisperEngineCpp : IWhisperEngine
{
    public static string StaticName => "Whisper CPP";
    public string Name => StaticName;
    public string Url => "https://github.com/ggerganov/whisper.cpp";

    public List<WhisperLanguage> Languages => WhisperLanguage.Languages.OrderBy(p => p.Name).ToList();

    public List<WhisperModel> Models
    {
        get
        {
            var models = new WhisperCppModel().Models;
            return models.ToList();
        }
    }

    public string Extension => ".bin";

    public bool IsEngineInstalled()
    {
        var folder = GetAndCreateWhisperFolder();
        var executableFile = Path.Combine(folder, GetExecutableFileName());
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

        var folder = Path.Combine(baseFolder, "Cpp");
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

    public bool IsModelInstalled(WhisperModel model)
    {
        var modelFileName = Path.Combine(GetAndCreateWhisperModelFolder(), model.Name + Extension);
        var fileExists = File.Exists(modelFileName);
        return fileExists;
    }

    public string GetExecutableFileName()
    {
        return "main.exe";
    }
}
