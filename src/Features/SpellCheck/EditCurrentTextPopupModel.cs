using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.SpellCheck;

public partial class EditCurrentTextPopupModel : ObservableObject
{
    public EditCurrentTextPopup? Popup { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Text { get; set; }

    public EditCurrentTextPopupModel()
    {
        Title = string.Empty;
        Text = string.Empty;
    }

    [RelayCommand]
    private void Ok()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(Text);
        });
    }

    [RelayCommand]
    public void Cancel()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void Initialize(string title, string text)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Title = title;
            Text = text;
        });
    }
}