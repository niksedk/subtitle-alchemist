using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Dictionaries;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public partial class FixNamesPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<FixNameItem> _names;

    [ObservableProperty]
    private ObservableCollection<FixNameHitItem> _hits;

    public FixNamesPage? Page { get; set; }

    private Subtitle _subtitle;

    private NameList? _nameList;
    private List<string> _nameListInclMulti;
    private string _language;

    public FixNamesPageModel()
    {
        _names = new ObservableCollection<FixNameItem>();
        _hits = new ObservableCollection<FixNameHitItem>();

        _nameListInclMulti = new List<string>();
        _language = "en";
        _subtitle = new Subtitle();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
               
            });
            return false;
        });
    }

    [RelayCommand]
    public void NamesSelectAll()
    {
        foreach (var name in Names)
        {
            name.IsChecked = true;
        }
    }

    [RelayCommand]
    public void NamesInvertSelection()
    {
        foreach (var name in Names)
        {
            name.IsChecked = !name.IsChecked;
        }
    }


    [RelayCommand]
    private async Task Ok()
    {
        var subtitle = new Subtitle(_subtitle, false);

        await Shell.Current.GoToAsync("../..", new Dictionary<string, object>
        {
            { "Page", nameof(FixNamesPage) },
            { "Subtitle", subtitle },
            { "NoOfLinesChanged", 1 },
            { "Status", "status" },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("../..", new Dictionary<string, object>
        {
            { "Page", nameof(FixNamesPage) },
        });
    }
}
