using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public partial class ColorPickerPopupModel : ObservableObject
{
    public ColorPickerPopup? Popup { get; set; }

    public ColorPickerPopupModel()
    {
    }

    [RelayCommand]
    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }
}