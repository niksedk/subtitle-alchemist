using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public partial class RemoveTextForHiPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private bool _isRemoveBracketsOn;
    [ObservableProperty] private bool _isRemoveCurlyBracketsOn;
    [ObservableProperty] private bool _isRemoveParenthesesOn;
    [ObservableProperty] private bool _isRemoveCustomOn;
    [ObservableProperty] private string _customStart;
    [ObservableProperty] private string _customEnd;
    [ObservableProperty] private bool _isOnlySeparateLine;

    [ObservableProperty] private bool _isRemoveTextBeforeColonOn;
    [ObservableProperty] private bool _isRemoveTextBeforeColonUppercaseOn;
    [ObservableProperty] private bool _isRemoveTextBeforeColonSeparateLineOn;

    [ObservableProperty] private bool _isRemoveTextUppercaseLineOn;

    [ObservableProperty] private bool _isRemoveTextContainsOn;
    [ObservableProperty] private string _textContains;

    [ObservableProperty] private bool _isRemoveOnlyMusicSymbolsOn;

    [ObservableProperty] private bool _isRemoveInterjectionsOn;
    [ObservableProperty] private bool _isInterjectionsSeparateLineOn;

    [ObservableProperty] private DisplayFile? _selectedFile;

    [ObservableProperty] private ObservableCollection<string> _languages;
    [ObservableProperty] private string _selectedLanguage;

    [ObservableProperty] private ObservableCollection<RemoveItem> _fixes;


    public RemoveTextForHiPage? Page { get; set; }
    public Label LabelOpenFolder { get; set; } = new();

    private readonly IAutoBackup _autoBackup;

    public RemoveTextForHiPageModel(IAutoBackup autoBackup)
    {
        _autoBackup = autoBackup;
        _customStart = "?";
        _customEnd = "?";
        _textContains = string.Empty;
        _languages = new ObservableCollection<string> { "English" };
        _fixes = new ObservableCollection<RemoveItem>();
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
    //        { "SubtitleFileName", file.FullPath },
        });
    }


    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var page = query["Page"].ToString();


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
        var settings = Se.Settings.Tools.RemoveTextForHi;

        IsRemoveBracketsOn = settings.IsRemoveBracketsOn;
        IsRemoveCurlyBracketsOn = settings.IsRemoveCurlyBracketsOn;
        IsRemoveParenthesesOn = settings.IsRemoveParenthesesOn;
        IsRemoveCustomOn = settings.IsRemoveCustomOn;
        CustomStart = settings.CustomStart;
        CustomEnd = settings.CustomEnd;
        IsOnlySeparateLine = settings.IsOnlySeparateLine;

        IsRemoveTextBeforeColonOn = settings.IsRemoveTextBeforeColonOn;
        IsRemoveTextBeforeColonUppercaseOn = settings.IsRemoveTextBeforeColonUppercaseOn;
        IsRemoveTextBeforeColonSeparateLineOn = settings.IsRemoveTextBeforeColonSeparateLineOn;

        IsRemoveTextUppercaseLineOn = settings.IsRemoveTextUppercaseLineOn;

        IsRemoveTextContainsOn = settings.IsRemoveTextContainsOn;
        TextContains = settings.TextContains;

        IsRemoveOnlyMusicSymbolsOn = settings.IsRemoveOnlyMusicSymbolsOn;

        IsRemoveInterjectionsOn = settings.IsRemoveInterjectionsOn;
        IsInterjectionsSeparateLineOn = settings.IsInterjectionsSeparateLineOn;
    }

    private void SaveSettings()
    {
        var settings = Se.Settings.Tools.RemoveTextForHi;

        settings.IsRemoveBracketsOn = IsRemoveBracketsOn;
        settings.IsRemoveCurlyBracketsOn = IsRemoveCurlyBracketsOn;
        settings.IsRemoveParenthesesOn = IsRemoveParenthesesOn;
        settings.IsRemoveCustomOn = IsRemoveCustomOn;
        settings.CustomStart = CustomStart;
        settings.CustomEnd = CustomEnd;
        settings.IsOnlySeparateLine = IsOnlySeparateLine;

        settings.IsRemoveTextBeforeColonOn = IsRemoveTextBeforeColonOn;
        settings.IsRemoveTextBeforeColonUppercaseOn = IsRemoveTextBeforeColonUppercaseOn;
        settings.IsRemoveTextBeforeColonSeparateLineOn = IsRemoveTextBeforeColonSeparateLineOn;

        settings.IsRemoveTextUppercaseLineOn = IsRemoveTextUppercaseLineOn;

        settings.IsRemoveTextContainsOn = IsRemoveTextContainsOn;
        settings.TextContains = TextContains;

        settings.IsRemoveOnlyMusicSymbolsOn = IsRemoveOnlyMusicSymbolsOn;

        settings.IsRemoveInterjectionsOn = IsRemoveInterjectionsOn;
        settings.IsInterjectionsSeparateLineOn = IsInterjectionsSeparateLineOn;

        Se.SaveSettings();
    }
}
