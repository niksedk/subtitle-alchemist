using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Edit.Find;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Edit.Replace;

public partial class ReplacePopupModel : ObservableObject
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _wholeWord;

    [ObservableProperty]
    private bool _normal;

    [ObservableProperty]
    private bool _caseInsensitive;

    [ObservableProperty]
    private bool _regularExpression;

    public ReplacePopup? Popup { get; set; }
    public SearchBar SearchBar { get; set; } = new SearchBar();

    private IFindService? _findService;


    [RelayCommand]
    private void Find()
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_findService != null)
            {
                _findService.SearchText = SearchText;
                Popup?.Close(_findService);
            }
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void Initialize(IFindService findService)
    {
        _findService = findService;

        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                WholeWord = findService.WholeWord;

                if (findService.CurrentFindMode == FindService.FindMode.Normal)
                {
                    Normal = true;
                }
                else if (findService.CurrentFindMode == FindService.FindMode.CaseInsensitive)
                {
                    CaseInsensitive = true;
                }
                else if (findService.CurrentFindMode == FindService.FindMode.RegularExpression)
                {
                    RegularExpression = true;
                }

                SearchBar.Focus();
            });

            return false;
        });
    }

    internal void SearchButtonPressed(object? sender, EventArgs e)
    {
        Find();
    }
}
