using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Controls.PickerControl;

public partial class PickerPopupModel : ObservableObject
{

    [ObservableProperty] 
    private ObservableCollection<string> _items = new();

    [ObservableProperty] 
    private string? _selectedItem;

    public PickerPopup? Popup { get; set; }

    public void SetItems(List<string> items)
    {
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public void SetSelectedItem(string item)
    {
        SelectedItem = item;
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