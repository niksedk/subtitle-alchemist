using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AudioToText;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Config;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper
{
    public partial class WhisperAdvancedHistoryPopupModel : ObservableObject
    {
        public WhisperAdvancedHistoryPopup? Popup { get; set; }

        public Picker Picker { get; set; } = new();

        [ObservableProperty]
        private ObservableCollection<string> _items = new();

        [ObservableProperty]
        private string? _selectedItem;

        public void SetItems()
        {
            var items = SeSettings.Settings.Tools.WhisperExtraSettingsHistory.SplitToLines();

            Items.Clear();
            Items.Add(string.Empty);

            if (SeSettings.Settings.Tools.WhisperChoice 
                is WhisperChoice.PurfviewFasterWhisper or WhisperChoice.PurfviewFasterWhisperXXL 
                && !items.Contains("--standard"))
            {
                Items.Add("--standard");
            }

            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        [RelayCommand]
        private void Ok()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(SelectedItem);
            });
        }
    }
}
