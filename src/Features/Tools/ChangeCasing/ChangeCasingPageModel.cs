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

        Se.SaveSettings();

        var subtitle = new Subtitle(_subtitle, false);

        if (FixOnlyNames)
        {
            await ShowFixNames(subtitle, 0);
            return;
        }


        var info = string.Empty;

        var language = LanguageAutoDetect.AutoDetectGoogleLanguage(subtitle);
        var fixCasing = new FixCasing(language)
        {
            FixNormal = ToNormalCasing,
            FixNormalOnlyAllUppercase = OnlyChangeAllUppercaseLines,
            FixMakeUppercase = ToUppercase,
            FixMakeLowercase = ToLowercase,
            FixMakeProperCase = false,
            FixProperCaseOnlyAllUppercase = false,
            Format = subtitle.OriginalFormat,
        };
        fixCasing.Fix(subtitle);

        if (ToNormalCasing)
        {
            info = $"Normal Casing - lines changed: {fixCasing.NoOfLinesChanged}";
            if (FixNames)
            {
                await ShowFixNames(subtitle, fixCasing.NoOfLinesChanged);
                return;
            }
        }
        else if (ToUppercase)
        {
            info = $"Uppercase - lines changed: {fixCasing.NoOfLinesChanged}";
        }
        else if (ToLowercase)
        {
            info = $"Lowercase - lines changed: {fixCasing.NoOfLinesChanged}";
        }

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(ChangeCasingPage) },
            { "Subtitle", subtitle },
            { "Status", info },
            { "NoOfLinesChanged", fixCasing.NoOfLinesChanged },
        });
    }

    private async Task ShowFixNames(Subtitle subtitle, int noOfFixes)
    {
        await Shell.Current.GoToAsync(nameof(FixNamesPage), new Dictionary<string, object>
        {
            { "Page", nameof(ChangeCasingPage) },
            { "Subtitle", subtitle },
            { "NoOfFixes", noOfFixes },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
