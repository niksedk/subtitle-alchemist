using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Controls.PickerControl;

public partial class PickerPopupModel : ObservableObject
{

    [ObservableProperty] public partial ObservableCollection<string> Items { get; set; } = new();
    [ObservableProperty] public partial string? SelectedItem { get; set; }

    public PickerPopup? Popup { get; set; }
    public Picker Picker { get; set; } = new();

    public void SetItems(List<string> items, string? currentModel)
    {
        Items.Clear();

        if (!string.IsNullOrEmpty(currentModel) && !items.Contains(currentModel))
        {
            Items.Add(currentModel);
        }

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public void SetSelectedItem(string item)
    {
        SelectedItem = item;

        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectedItem = item;
                Picker.SelectedItem = item;
            });
            return false;
        });
    }

    [RelayCommand]
    public void Ok()
    {
        Popup?.Close(SelectedItem);
    }

    [RelayCommand]
    public void Cancel()
    {
        Popup?.Close();
    }
}