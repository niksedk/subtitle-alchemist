using System.IO.Compression;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;

namespace SubtitleAlchemist.Features.Video.TextToSpeech.Engines;

public class AzureSpeech : ITtsEngine
{
    public string Name => "AzureSpeech";
    public string Description => "pay/fast/good";
    public bool HasLanguageParameter => true;
    public bool HasApiKey => true;
    public bool HasRegion => true;
    public bool HasModel => false;

    public Task<bool> IsInstalled()
    {
        return Task.FromResult(!string.IsNullOrEmpty(Se.Settings.Video.TextToSpeech.AzureApiKey));
    }

    private const string JsonFileName = "AzureVoices.json";
    private readonly ITtsDownloadService _ttsDownloadService;

    public AzureSpeech(ITtsDownloadService ttsDownloadService)
    {
        _ttsDownloadService = ttsDownloadService;
    }

    public override string ToString()
    {
        return $"{Name}";
    }

    public async Task<Voice[]> GetVoices()
    {
        var azureFolder = GetSetAzureFolder();

        var voiceFileName = Path.Combine(azureFolder, JsonFileName);
        if (!File.Exists(voiceFileName))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TtsAzureVoices.zip");
            using var reader = new StreamReader(stream);
            ZipFile.ExtractToDirectory(stream, azureFolder);
        }

        return Map(voiceFileName);
    }

    private static Voice[] Map(string voiceFileName)
    {
        if (!File.Exists(voiceFileName))
        {
            return Array.Empty<Voice>();
        }

        var result = new List<Voice>();
        var json = File.ReadAllText(voiceFileName);
        var parser = new SeJsonParser();
        var arr = parser.GetArrayElements(json);
        foreach (var item in arr)
        {
            var displayName = parser.GetFirstObject(item, "DisplayName");
            var localName = parser.GetFirstObject(item, "LocalName");
            var shortName = parser.GetFirstObject(item, "ShortName");
            var gender = parser.GetFirstObject(item, "Gender");
            var locale = parser.GetFirstObject(item, "Locale");

            result.Add(new Voice(new AzureVoice
            {
                DisplayName = displayName,
                LocalName = localName,
                ShortName = shortName,
                Gender = gender,
                Locale = locale,
            }));
        }

        return result.ToArray();
    }

    public static string GetSetAzureFolder()
    {
        if (!Directory.Exists(Se.TtsFolder))
        {
            Directory.CreateDirectory(Se.TtsFolder);
        }

        var azureFolder = Path.Combine(Se.TtsFolder, "Azure");
        if (!Directory.Exists(azureFolder))
        {
            Directory.CreateDirectory(azureFolder);
        }

        return azureFolder;
    }

    public bool IsVoiceInstalled(Voice voice)
    {
        return true;
    }

    public Task<TtsLanguage[]> GetLanguages(Voice voice)
    {
        return Task.FromResult(Array.Empty<TtsLanguage>());
    }

    public async Task<Voice[]> RefreshVoices(CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await _ttsDownloadService.DownloadElevenLabsVoiceList(ms, null, cancellationToken);
        await File.WriteAllBytesAsync(Path.Combine(GetSetAzureFolder(), JsonFileName), ms.ToArray(), cancellationToken);
        return await GetVoices();
    }

    public async Task<TtsResult> Speak(
        string text, 
        string outputFolder, 
        Voice voice, 
        TtsLanguage? language,
        string? region,
        string? model,
        CancellationToken cancellationToken)
    {
        // if (voice.EngineVoice is not PiperVoice piperVoice)
        // {
        //     throw new ArgumentException("Voice is not a PiperVoice");
        // }
        //
        // var fileNameOnly = Guid.NewGuid() + ".wav";
        // var process = StartPiperProcess(piperVoice, text, fileNameOnly);
        // await process.WaitForExitAsync();
        //
        // var fileName = Path.Combine(GetSetPiperFolder(), fileNameOnly);
        return new TtsResult();
    }

    public Task<string[]> GetRegions()
    {
        return Task.FromResult(new[]
        {
            "australiaeast",
            "brazilsouth",
            "canadacentral",
            "centralus",
            "eastasia",
            "eastus",
            "eastus2",
            "francecentral",
            "germanywestcentral",
            "centralindia",
            "japaneast",
            "japanwest",
            "jioindiawest",
            "koreacentral",
            "northcentralus",
            "northeurope",
            "norwayeast",
            "southcentralus",
            "southeastasia",
            "swedencentral",
            "switzerlandnorth",
            "switzerlandwest",
            "uaenorth",
            "usgovarizona",
            "usgovvirginia",
            "uksouth",
            "westcentralus",
            "westeurope",
            "westus",
            "westus2",
            "westus3"
        });
    }

    public Task<string[]> GetModels()
    {
        return Task.FromResult(Array.Empty<string>());
    }
}