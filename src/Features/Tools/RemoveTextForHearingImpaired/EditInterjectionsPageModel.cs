using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public partial class EditInterjectionsPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private string _interjections;
    [ObservableProperty] private string _skipList;
    [ObservableProperty] private string _language;

    public EditInterjectionsPage? Page { get; set; }

    private string _twoLetterLanguageCode;

    public EditInterjectionsPageModel()
    {
        _interjections = string.Empty;
        _skipList = string.Empty;
        _language = "en";
        _twoLetterLanguageCode = "en";
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
            var language = query["Language"].ToString();
            if (!string.IsNullOrEmpty(language))
            {
                Language = language;
            }
        }

        if (query.ContainsKey("TwoLetterLanguageCode"))
        {
            var twoLetterLanguageCode = query["TwoLetterLanguageCode"].ToString();
            if (!string.IsNullOrEmpty(twoLetterLanguageCode))
            {
                _twoLetterLanguageCode = twoLetterLanguageCode;
            }
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadSettings();
            });
            return false;
        });
    }

    private void LoadSettings()
    {
        var interjections = InterjectionsRepository.LoadInterjections(_twoLetterLanguageCode);
        Interjections = string.Join("\n", interjections.Interjections);
        SkipList = string.Join("\n", interjections.SkipIfStartsWith);
    }

    private void SaveSettings()
    {
        var interjections = new List<string>();
        foreach (var line in Interjections.SplitToLines().OrderBy(p => p))
        {
            var text = line.Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                interjections.Add(text);
            }
        }

        var skipList = new List<string>();
        foreach (var line in SkipList.SplitToLines().OrderBy(p => p))
        {
            var text = line.Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                skipList.Add(text);
            }
        }

        InterjectionsRepository.SaveInterjections(_twoLetterLanguageCode, interjections, skipList);
    }
}
