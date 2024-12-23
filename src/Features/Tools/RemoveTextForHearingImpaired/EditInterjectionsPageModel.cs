using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HunspellSharp;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public partial class EditInterjectionsPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private string _interjections;
    [ObservableProperty] private string _skipList;
    [ObservableProperty] private string _language;

    public EditInterjectionsPage? Page { get; set; }

    private readonly System.Timers.Timer _timer;

    public EditInterjectionsPageModel()
    {
        _interjections = string.Empty;
        _skipList = string.Empty;

        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += TimerElapsed;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        try
        {
            MainThread.BeginInvokeOnMainThread(GeneratePreview);
        }
        catch
        {
            return;
        }

        _timer.Start();
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Ok()
    {
        SaveSettings();

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(RemoveTextForHiPage) },
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var page = query["Page"].ToString();

        if (query.ContainsKey("Language"))
        {
            var twoLetterLanguageCode = query["Language"].ToString();
            if (!string.IsNullOrEmpty(twoLetterLanguageCode))
            {
                var interjections = InterjectionsRepository.LoadInterjections(twoLetterLanguageCode);
                Interjections = string.Join("\n", interjections.Interjections);
                SkipList = string.Join("\n", interjections.SkipIfStartsWith);
                Language = twoLetterLanguageCode;
            }
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadSettings();
                GeneratePreview();
                _timer.Start();
            });
            return false;
        });
    }

    private void GeneratePreview()
    {
    }

    private void LoadSettings()
    {
        var settings = Se.Settings.Tools.RemoveTextForHi;
    }

    private void SaveSettings()
    {
        var settings = Se.Settings.Tools.RemoveTextForHi;
        Se.SaveSettings();
    }
}
