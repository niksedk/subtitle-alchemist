using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Video.TextToSpeech
{
    public partial class AudioSettingsPopupModel : ObservableObject
    {
        public AudioSettingsPopup? Popup { get; set; }

        [ObservableProperty]
        private ObservableCollection<string> _encodings;

        [ObservableProperty]
        private string? _selectedEncoding;

        [ObservableProperty]
        private bool _isStereo;

        public AudioSettingsPopupModel()
        {
            _encodings = new ObservableCollection<string>
            {
                "copy",
                "aac",
                "ac3",
                "eac3",
                "truehd",
                "libvorbis",
                "libmp3lame",
                "libopus",
            };

            _selectedEncoding = _encodings[0];
            _isStereo = false;
        }

        private void Close()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        [RelayCommand]
        public void Ok()
        {
            Close();
        }

        [RelayCommand]
        public void Cancel()
        {
            Close();
        }
    }
}
