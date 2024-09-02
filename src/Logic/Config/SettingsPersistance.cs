using System.Text;
using System.Text.Json;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;

namespace SubtitleAlchemist.Logic.Config;

public class RecentFile
{
    public string SubtitleFileName { get; set; } = string.Empty;
    public string VideoFileName { get; set; } = string.Empty;
    public int SelectedLine { get; set; }
    public string Encoding { get; set; } = string.Empty;
}

public class SeFile
{
    public bool ShowRecentFiles { get; set; } = true;
    public int RecentFilesMaximum { get; set; } = 25;
    public List<RecentFile> RecentFiles { get; set; } = new();

    public void AddToRecentFiles(string subtitleFileName, string videoFileName, int selectedLine, string encoding)
    {
        RecentFiles.RemoveAll(rf => rf.SubtitleFileName == subtitleFileName);
        RecentFiles.Insert(0, new RecentFile { SubtitleFileName = subtitleFileName, VideoFileName = videoFileName, SelectedLine = selectedLine, Encoding = encoding });
        if (RecentFiles.Count > RecentFilesMaximum)
        {
            RecentFiles.RemoveAt(RecentFiles.Count - 1);
        }
    }
}

public class SeTools
{
    public bool VoskPostProcessing { get; set; } = true;

    public string WhisperChoice { get; set; } = WhisperEngineCpp.StaticName;

    public bool WhisperIgnoreVersion { get; set; } = false;

    public bool WhisperDeleteTempFiles { get; set; } = true;

    public string? WhisperModel { get; set; } = string.Empty;

    public string WhisperLanguageCode { get; set; } = string.Empty;

    public string WhisperLocation { get; set; } = string.Empty;

    public string WhisperCtranslate2Location { get; set; } = string.Empty;

    public string WhisperPurfviewFasterWhisperLocation { get; set; } = string.Empty;

    public string WhisperPurfviewFasterWhisperDefaultCmd { get; set; } = string.Empty;

    public string WhisperXLocation { get; set; } = string.Empty;

    public string WhisperStableTsLocation { get; set; } = string.Empty;

    public string WhisperCppModelLocation { get; set; } = string.Empty;

    public string WhisperExtraSettings { get; set; } = string.Empty;

    public string WhisperExtraSettingsHistory { get; set; } = string.Empty;

    public bool WhisperAutoAdjustTimings { get; set; } = true;

    public bool WhisperUseLineMaxChars { get; set; } = true;

    public bool WhisperPostProcessingAddPeriods { get; set; } = false;

    public bool WhisperPostProcessingMergeLines { get; set; } = true;

    public bool WhisperPostProcessingSplitLines { get; set; } = true;

    public bool WhisperPostProcessingFixCasing { get; set; } = false;

    public bool WhisperPostProcessingFixShortDuration { get; set; } = true;

    public string AutoTranslateLastName { get; set; } = string.Empty;
}

public class SeSettings
{
    public string FfmpegPath { get; set; }
    public string Theme { get; set; }
    public SeFile File { get; set; }
    public SeTools Tools { get; set; }

    public static SeSettings Settings { get; set; } = new();

    public SeSettings()
    {
        FfmpegPath = string.Empty;
        Theme = "Dark";
        File = new SeFile();
        Tools = new SeTools();
    }

    public static void SaveSettings()
    {
        var settingsFileName = Path.Combine(FileSystem.Current.AppDataDirectory, "Settings.json");
        SaveSettings(settingsFileName);
    }

    public static void SaveSettings(string settingsFileName)
    {
        var settings = Settings;
        var json = JsonSerializer.Serialize(settings);
        System.IO.File.WriteAllText(settingsFileName, json);

        UpdateLibSeSettings();
    }

    public static void LoadSettings()
    {
        var settingsFileName = Path.Combine(FileSystem.Current.AppDataDirectory, "Settings.json");
        LoadSettings(settingsFileName);
    }

    public static void LoadSettings(string settingsFileName)
    {
        if (System.IO.File.Exists(settingsFileName))
        {
            var json = System.IO.File.ReadAllText(settingsFileName);
            Settings = JsonSerializer.Deserialize<SeSettings>(json)!;

            UpdateLibSeSettings();
        }
    }

    private static void UpdateLibSeSettings()
    {
        Configuration.Settings.General.FFmpegLocation = Settings.FfmpegPath;
        Configuration.Settings.General.UseDarkTheme = Settings.Theme == "Dark";

        Configuration.Settings.Tools.WhisperChoice = Settings.Tools.WhisperChoice;
        Configuration.Settings.Tools.WhisperIgnoreVersion = Settings.Tools.WhisperIgnoreVersion;
        Configuration.Settings.Tools.WhisperDeleteTempFiles = Settings.Tools.WhisperDeleteTempFiles;
        Configuration.Settings.Tools.WhisperModel = Settings.Tools.WhisperModel;
        Configuration.Settings.Tools.WhisperLanguageCode = Settings.Tools.WhisperLanguageCode;
        Configuration.Settings.Tools.WhisperLocation = Settings.Tools.WhisperLocation;
        Configuration.Settings.Tools.WhisperCtranslate2Location = Settings.Tools.WhisperCtranslate2Location;
        Configuration.Settings.Tools.WhisperPurfviewFasterWhisperLocation = Settings.Tools.WhisperPurfviewFasterWhisperLocation;
        Configuration.Settings.Tools.WhisperPurfviewFasterWhisperDefaultCmd = Settings.Tools.WhisperPurfviewFasterWhisperDefaultCmd;
        Configuration.Settings.Tools.WhisperXLocation = Settings.Tools.WhisperXLocation;
        Configuration.Settings.Tools.WhisperStableTsLocation = Settings.Tools.WhisperStableTsLocation;
        Configuration.Settings.Tools.WhisperCppModelLocation = Settings.Tools.WhisperCppModelLocation;
        Configuration.Settings.Tools.WhisperExtraSettings = Settings.Tools.WhisperExtraSettings;
        Configuration.Settings.Tools.WhisperExtraSettingsHistory = Settings.Tools.WhisperExtraSettingsHistory;
        Configuration.Settings.Tools.WhisperAutoAdjustTimings = Settings.Tools.WhisperAutoAdjustTimings;
        Configuration.Settings.Tools.WhisperUseLineMaxChars = Settings.Tools.WhisperUseLineMaxChars;
        Configuration.Settings.Tools.WhisperPostProcessingAddPeriods = Settings.Tools.WhisperPostProcessingAddPeriods;
        Configuration.Settings.Tools.WhisperPostProcessingMergeLines = Settings.Tools.WhisperPostProcessingMergeLines;
        Configuration.Settings.Tools.WhisperPostProcessingSplitLines = Settings.Tools.WhisperPostProcessingSplitLines;
        Configuration.Settings.Tools.WhisperPostProcessingFixCasing = Settings.Tools.WhisperPostProcessingFixCasing;
        Configuration.Settings.Tools.WhisperPostProcessingFixShortDuration = Settings.Tools.WhisperPostProcessingFixShortDuration;
        Configuration.Settings.Tools.VoskPostProcessing = Settings.Tools.VoskPostProcessing;

        Configuration.Settings.Tools.AutoTranslateLastName = Settings.Tools.AutoTranslateLastName;
    }
}
