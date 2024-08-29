using System.Diagnostics;
using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public interface IWhisperEngine
{
    string Name { get; }
    string Url { get; }
    List<WhisperLanguage> Languages { get; }
    List<WhisperModel> Models { get; }
    string Extension { get;  }
    bool IsEngineInstalled();
    string GetAndCreateWhisperFolder();
    string GetAndCreateWhisperModelFolder();
    bool IsModelInstalled(WhisperModel model);
}
