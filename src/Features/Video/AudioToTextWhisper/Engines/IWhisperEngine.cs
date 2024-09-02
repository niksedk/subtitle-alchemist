﻿using Nikse.SubtitleEdit.Core.AudioToText;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

public interface IWhisperEngine
{
    string Name { get; }
    string Choice { get; }
    string Url { get; }
    List<WhisperLanguage> Languages { get; }
    List<WhisperModel> Models { get; }
    string Extension { get;  }
    string UnpackSkipFolder { get; }
    bool IsEngineInstalled();
    string GetAndCreateWhisperFolder();
    string GetAndCreateWhisperModelFolder(WhisperModel? whisperModel);
    string GetExecutable();
    bool IsModelInstalled(WhisperModel model);
    string GetModelForCmdLine(string modelName);
    Task<string> GetHelpText();
    string GetWhisperModelDownloadFileName(WhisperModel whisperModel, string url);
}
