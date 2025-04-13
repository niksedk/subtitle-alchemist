using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SubtitleAlchemist.Features.SpellCheck;

public partial class GetDictionaryPopupModel : ObservableObject
{
    public GetDictionaryPopup? Popup { get; set; }

    [ObservableProperty]
    public partial float ProgressValue { get; set; }

    [ObservableProperty]
    public partial string Progress { get; set; }

    [ObservableProperty]
    public partial string Error { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SpellCheckDictionary> Dictionaries { get; set; } = new();

    [ObservableProperty]
    public partial SpellCheckDictionary? SelectedDictionary { get; set; }

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsDownloading { get; set; }

    private Task? _downloadTask;
    private readonly Timer _timer = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly MemoryStream _downloadStream;
    private readonly ISpellCheckDictionaryDownloadService _spellCheckDictionaryDownloadService;
    private readonly IZipUnpacker _zipUnpacker;

    public GetDictionaryPopupModel(ISpellCheckDictionaryDownloadService spellCheckDictionaryDownloadService, IZipUnpacker zipUnpacker)
    {
        _spellCheckDictionaryDownloadService = spellCheckDictionaryDownloadService;
        _zipUnpacker = zipUnpacker;

        _cancellationTokenSource = new CancellationTokenSource();

        _downloadStream = new MemoryStream();

        Progress = "Starting...";
        Error = string.Empty;

        _timer.Interval = 500;
        _timer.Elapsed += OnTimerOnElapsed;
        _timer.Start();
    }

    private void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
    {
        if (_downloadTask is { IsCompleted: true })
        {
            _timer.Stop();

            if (_downloadStream.Length == 0)
            {
                Progress = "Download failed";
                Error = "No data received";
                return;
            }

            var dicFileName = UnpackDictionary();
            if (SelectedDictionary != null)
            {
                SelectedDictionary.DictionaryFileName = dicFileName ?? string.Empty;
            }

            if (Popup != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Popup.Close(SelectedDictionary);
                });
            }
        }
        else if (_downloadTask is { IsFaulted: true })
        {
            _timer.Stop();
            var ex = _downloadTask.Exception?.InnerException ?? _downloadTask.Exception;
            if (ex is OperationCanceledException)
            {
                Progress = "Download canceled";
                Close();
            }
            else
            {
                Progress = "Download failed";
                Error = ex?.Message ?? "Unknown error";
            }
        }
    }

    private string? UnpackDictionary()
    {
        var folder = Se.DictionariesFolder;

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var outputFileNames = new List<string>();

        _downloadStream.Position = 0;
        _zipUnpacker.UnpackZipStream(
            _downloadStream, 
            folder, 
            string.Empty, 
            true, 
            new List<string> { ".dic", ".aff"},
            outputFileNames);

        _downloadStream.Dispose();

        return outputFileNames.FirstOrDefault(p => p.EndsWith(".dic", StringComparison.OrdinalIgnoreCase));
    }

    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    [RelayCommand]
    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
        Close();
    }

    [RelayCommand]
    public void Download()
    {
        if (SelectedDictionary == null)
        {
            return;
        }

        Description = string.Empty;
        IsDownloading = true;
        StartDownload(SelectedDictionary.DownloadLink);
    }

    [RelayCommand]
    public void OpenFolder()
    {
        var folder = Se.DictionariesFolder;
        UiUtil.OpenFolder(folder);
    }

    private void StartDownload(string downloadLink)
    {
        var downloadProgress = new Progress<float>(number =>
        {
            var percentage = (int)Math.Round(number * 100.0, MidpointRounding.AwayFromZero);
            var pctString = percentage.ToString(CultureInfo.InvariantCulture);
            ProgressValue = number;
            Progress = $"Downloading... {pctString}%";
        });

        _downloadTask = _spellCheckDictionaryDownloadService.DownloadDictionary(_downloadStream, downloadLink, downloadProgress, _cancellationTokenSource.Token);
    }

    public void SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (SelectedDictionary == null)
        {
            return;
        }

        Description = SelectedDictionary.Description;
    }

    public void Initialize()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Dictionaries = new ObservableCollection<SpellCheckDictionary>(await SpellCheckDictionaries.GetDictionaryListAsync());

            TryToSelectUsersLanguage();
        });
    }

    private void TryToSelectUsersLanguage()
    {
        var language = CultureInfo.CurrentCulture.EnglishName;
        var countryName = Regex.Match(language, @"\(([^)]*)\)").Groups[1].Value;
        var languageTags = language.Split(' ', ',', '(', ')', '_', '-').ToList();
        languageTags.Insert(0, countryName);

        if (CountryLanguageMap.TryGetValue(countryName, out var languageName) && !string.IsNullOrEmpty(languageName))
        {
            languageTags.Insert(0, languageName);
        }

        foreach (var tag in languageTags.Where(p => p.Length > 2))
        {
            foreach (var dictionary in Dictionaries)
            {
                if (dictionary.EnglishName.Contains(tag, StringComparison.OrdinalIgnoreCase) ||
                    dictionary.NativeName.Contains(tag, StringComparison.OrdinalIgnoreCase) ||
                    dictionary.Description.Contains(tag, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedDictionary = dictionary;
                    return;
                }
            }
        }

        SelectedDictionary = Dictionaries[0];
    }

    private static readonly Dictionary<string, string> CountryLanguageMap = new()
    {
        {"United States", "English"},
        {"United Kingdom", "English"},
        {"Australia", "English"},
        {"Canada", "English"},
        {"France", "French"},
        {"Germany", "German"},
        {"Italy", "Italian"},
        {"Spain", "Spanish"},
        {"Mexico", "Spanish"},
        {"Japan", "Japanese"},
        {"China", "Chinese"},
        {"Russia", "Russian"},
        {"Brazil", "Portuguese"},
        {"Portugal", "Portuguese"},
        {"India", "Hindi"},
        {"Netherlands", "Dutch"},
        {"Sweden", "Swedish"},
        {"Norway", "Norwegian"},
        {"Denmark", "Danish"},
        {"Finland", "Finnish"},
        {"South Korea", "Korean"},
        {"Turkey", "Turkish"},
        {"Greece", "Greek"},
        {"Poland", "Polish"},
        {"Ukraine", "Ukrainian"},
        {"Argentina", "Spanish"},
        {"Egypt", "Arabic"},
        {"Saudi Arabia", "Arabic"},
        {"Israel", "Hebrew"},
        {"Thailand", "Thai"},
        {"Vietnam", "Vietnamese"},
        {"Indonesia", "Indonesian"},
        {"Malaysia", "Malay"},
        {"Philippines", "Filipino"},
        {"Ireland", "Irish"},
    };
}