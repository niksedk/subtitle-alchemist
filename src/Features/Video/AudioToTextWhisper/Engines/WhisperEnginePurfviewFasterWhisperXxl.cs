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

    public Process GetProcess(string audioFilePath, string language, string advancedSettings)
    {
        throw new NotImplementedException();
    }

    public void DownloadModel(string model)
    {
        throw new NotImplementedException();
    }

    public void DownloadEngine()
    {
        throw new NotImplementedException();
    }

    public bool IsEngineInstalled()
    {
        throw new NotImplementedException();
    }

    public string GetAndCreateWhisperFolder()
    {
        throw new NotImplementedException();
    }

    public bool IsModelInstalled(WhisperModel model)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Name;
    }
}
