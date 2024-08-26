using System.Diagnostics;
using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public interface IWhisperEngine
{
    string Name { get; }
    string Url { get; }
    List<WhisperLanguage> Languages { get; }
    List<WhisperModel> Models { get; }
    Process GetProcess(string audioFilePath, string language, string advancedSettings);
    void DownloadModel(string model);
    void DownloadEngine();
    bool IsEngineInstalled();
    string GetAndCreateWhisperFolder();
    bool IsModelInstalled(WhisperModel model);
}
