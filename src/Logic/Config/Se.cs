using System.Globalization;
using System.Text;
using System.Text.Json;
using Nikse.SubtitleEdit.Core.Common;
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

            UpdateLibSeSettings();
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
        Configuration.Settings.Tools.WhisperExtraSettings = Settings.Tools.WhisperCustomCommandLineArguments;
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