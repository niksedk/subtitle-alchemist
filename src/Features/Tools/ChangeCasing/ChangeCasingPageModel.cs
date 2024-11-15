using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public partial class ChangeCasingPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private bool _toNormalCasing;

    [ObservableProperty]
    private bool _fixNames;

    [ObservableProperty]
    private bool _onlyChangeAllUppercaseLines;

    [ObservableProperty]
    private bool _fixOnlyNames;

    [ObservableProperty]
    private bool _toUppercase;

    [ObservableProperty]
    private bool _toLowercase;


    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs = new();

    public ChangeCasingPage? Page { get; set; }

    private Subtitle _subtitle = new();

    public ChangeCasingPageModel()
    {
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
            Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ToNormalCasing = Se.Settings.Tools.ChangeCasing.ToNormalCasing;
                FixNames = Se.Settings.Tools.ChangeCasing.FixNames;
                OnlyChangeAllUppercaseLines = Se.Settings.Tools.ChangeCasing.OnlyChangeAllUppercaseLines;
                FixOnlyNames = Se.Settings.Tools.ChangeCasing.FixOnlyNames;
                ToUppercase = Se.Settings.Tools.ChangeCasing.ToUppercase;
                ToLowercase = Se.Settings.Tools.ChangeCasing.ToLowercase;
            });
            return false;
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        Se.Settings.Tools.ChangeCasing.ToNormalCasing = ToNormalCasing;
        Se.Settings.Tools.ChangeCasing.FixNames = FixNames;
        Se.Settings.Tools.ChangeCasing.OnlyChangeAllUppercaseLines = OnlyChangeAllUppercaseLines;
        Se.Settings.Tools.ChangeCasing.FixOnlyNames = FixOnlyNames;
        Se.Settings.Tools.ChangeCasing.ToUppercase = ToUppercase;
        Se.Settings.Tools.ChangeCasing.ToLowercase = ToLowercase;

        var subtitle = new Subtitle(_subtitle, false);
        var info = string.Empty;

        Se.SaveSettings();

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Subtitle", subtitle },
            { "Status", info },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
