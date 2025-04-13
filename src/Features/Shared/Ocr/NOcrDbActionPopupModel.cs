using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class NOcrDbActionPopupModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string NewName { get; set; }

    [ObservableProperty]
    public partial bool IsDeleteVisible { get; set; }

    [ObservableProperty]
    public partial bool IsEditVisible { get; set; }

    [ObservableProperty]
    public partial bool IsAddVisible { get; set; }

    [ObservableProperty]
    public partial bool IsNewVisible { get; set; }

    [ObservableProperty]
    public partial bool IsNewNameVisible { get; set; }

    [ObservableProperty]
    public partial bool IsOkVisible { get; set; }

    [ObservableProperty]
    public partial string ErrorFileName { get; set; }

    [ObservableProperty]
    public partial bool IsErrorFileNameVisible { get; set; }
    public NOcrDbActionPopup? Popup { get; set; }
    public Entry EntryNewName { get; set; }

    private readonly System.Timers.Timer _timer;


    public NOcrDbActionPopupModel()
    {
        EntryNewName = new Entry();
        Title = string.Empty;
        NewName = string.Empty;
        IsDeleteVisible = false;
        IsEditVisible = false;
        IsAddVisible = true;
        IsOkVisible = false;
        IsNewVisible = true;
        IsNewNameVisible = false;
        IsErrorFileNameVisible = false;
        ErrorFileName = string.Empty;
        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += TimerElapsed;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (NewName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            ErrorFileName = "Invalid characters";
            IsErrorFileNameVisible = true;
        }
        else
        {
            ErrorFileName = string.Empty;
            IsErrorFileNameVisible = false;
        }
    }

    [RelayCommand]
    private void Edit()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close("edit");
        });
    }

    [RelayCommand]
    private void New()
    {
        IsEditVisible = false;
        IsDeleteVisible = false;
        IsNewVisible = false;
        IsAddVisible = false;
        IsNewNameVisible = true;
        Title = "nOCR database - new";
        _timer.Start();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            EntryNewName.Focus();
        });
    }

    [RelayCommand]
    private void Delete()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close("delete");
        });
    }

    [RelayCommand]
    private void Ok()
    {
        if (string.IsNullOrWhiteSpace(NewName))
        {
            Cancel();
            return;
        }

        if (NewName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close("new:" + NewName);
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

    public void Initialize(string? selectedNOcrDatabase, ObservableCollection<string> nOcrDatabases)
    {
        if (string.IsNullOrEmpty(selectedNOcrDatabase))
        {
            Title = "nOCR database - new";
            IsDeleteVisible = false;
            IsEditVisible = false;
        }
        else
        {
            Title = $"nOCR database: {selectedNOcrDatabase}";
            IsDeleteVisible = true;
            IsEditVisible = true;
        }
    }
}