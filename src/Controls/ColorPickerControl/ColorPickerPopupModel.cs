using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public partial class ColorPickerPopupModel : ObservableObject
{
    public ColorPickerPopup? Popup { get; set; }

    public Color? CurrentColor { get; set; }
    public ColorPickerView? ColorPickerView { get; set; }

    [RelayCommand]
    private void Ok()
    {
        Popup?.Close(CurrentColor);
    }

    [RelayCommand]
    private void Cancel()
    {
        Close();
    }

    [RelayCommand]
    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void SetCurrentColor(Color color)
    {
        ColorPickerView?.SetCurrentColor(color);
    }
}