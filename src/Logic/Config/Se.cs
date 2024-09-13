using System.Globalization;
using System.Text;
using System.Text.Json;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Forms.FixCommonErrors;
using SubtitleAlchemist.Logic.Config.Language;
namespace SubtitleAlchemist.Logic.Config;

public class Se
{
    public string FfmpegPath { get; set; }
    public string Theme { get; set; }
    public SeFile File { get; set; }
    public SeTools Tools { get; set; }
    public static SeLanguage Language { get; set; } = new();
    public static Se Settings { get; set; } = new();
    public static string DictionariesFolder => Path.Combine(FileSystem.Current.AppDataDirectory, "Dictionaries");
    public static string AutoBackupFolder => Path.Combine(FileSystem.Current.AppDataDirectory, "AutoBackup");

    public Se()
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
            Settings = JsonSerializer.Deserialize<Se>(json)!;

            SetDefaultValues();

            UpdateLibSeSettings();
        }
    }

    private static void SetDefaultValues()
    {
        if (Settings.Tools.FixCommonErrors.Profiles.Count == 0)
        {
            Settings.Tools.FixCommonErrors.Profiles.Add(new SeFixCommonErrorsProfile
            {
                ProfileName = "Default",
                SelectedRules = new()
                {
                    nameof(FixEmptyLines),
                    nameof(FixOverlappingDisplayTimes),
                    nameof(FixLongDisplayTimes),
                    nameof(FixShortDisplayTimes),
                    nameof(FixShortGaps),
                    nameof(FixInvalidItalicTags),
                    nameof(FixUnneededSpaces),
                    nameof(FixMissingSpaces),
                    nameof(FixUnneededPeriods),
                },
            });
            Settings.Tools.FixCommonErrors.LastProfileName = "Default";
        }
    }

    public static void WriteWhisperLog(string log)
    {
        try
        {
            var filePath = GetWhisperLogFilePath();
            using var writer = new StreamWriter(filePath, true, Encoding.UTF8);
            writer.WriteLine("-----------------------------------------------------------------------------");
            writer.WriteLine($"Date: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
            writer.WriteLine($"SE: {GetSeInfo()}");
            writer.WriteLine(log);
            writer.WriteLine();
        }
        catch
        {
            // ignore
        }
    }

    private static string GetSeInfo()
    {
        try
        {
            return $"{System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version} - {Environment.OSVersion} - {IntPtr.Size * 8}-bit";
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string GetWhisperLogFilePath()
    {
        return Path.Combine(FileSystem.Current.AppDataDirectory, "whisper_log.txt");
    }


    private static void UpdateLibSeSettings()
    {
        Configuration.Settings.General.FFmpegLocation = Settings.FfmpegPath;
        Configuration.Settings.General.UseDarkTheme = Settings.Theme == "Dark";

        var tts = Settings.Tools.AudioToText;
        Configuration.Settings.Tools.WhisperChoice = tts.WhisperChoice;
        Configuration.Settings.Tools.WhisperIgnoreVersion = tts.WhisperIgnoreVersion;
        Configuration.Settings.Tools.WhisperDeleteTempFiles = tts.WhisperDeleteTempFiles;
        Configuration.Settings.Tools.WhisperModel = tts.WhisperModel;
        Configuration.Settings.Tools.WhisperLanguageCode = tts.WhisperLanguageCode;
        Configuration.Settings.Tools.WhisperLocation = tts.WhisperLocation;
        Configuration.Settings.Tools.WhisperCtranslate2Location = tts.WhisperCtranslate2Location;
        Configuration.Settings.Tools.WhisperPurfviewFasterWhisperLocation = tts.WhisperPurfviewFasterWhisperLocation;
        Configuration.Settings.Tools.WhisperPurfviewFasterWhisperDefaultCmd = tts.WhisperPurfviewFasterWhisperDefaultCmd;
        Configuration.Settings.Tools.WhisperXLocation = tts.WhisperXLocation;
        Configuration.Settings.Tools.WhisperStableTsLocation = tts.WhisperStableTsLocation;
        Configuration.Settings.Tools.WhisperCppModelLocation = tts.WhisperCppModelLocation;
        Configuration.Settings.Tools.WhisperExtraSettings = tts.WhisperCustomCommandLineArguments;
        Configuration.Settings.Tools.WhisperExtraSettingsHistory = tts.WhisperExtraSettingsHistory;
        Configuration.Settings.Tools.WhisperAutoAdjustTimings = tts.WhisperAutoAdjustTimings;
        Configuration.Settings.Tools.WhisperUseLineMaxChars = tts.WhisperUseLineMaxChars;
        Configuration.Settings.Tools.WhisperPostProcessingAddPeriods = tts.WhisperPostProcessingAddPeriods;
        Configuration.Settings.Tools.WhisperPostProcessingMergeLines = tts.WhisperPostProcessingMergeLines;
        Configuration.Settings.Tools.WhisperPostProcessingSplitLines = tts.WhisperPostProcessingSplitLines;
        Configuration.Settings.Tools.WhisperPostProcessingFixCasing = tts.WhisperPostProcessingFixCasing;
        Configuration.Settings.Tools.WhisperPostProcessingFixShortDuration = tts.WhisperPostProcessingFixShortDuration;
        Configuration.Settings.Tools.VoskPostProcessing = tts.PostProcessing;

        Configuration.Settings.Tools.AutoTranslateLastName = Settings.Tools.AutoTranslateLastName;
    }
}