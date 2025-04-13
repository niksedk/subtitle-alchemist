using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Video.OpenFromUrl
{
    public partial class OpenFromUrlPopupModel : ObservableObject
    {
        public OpenFromUrlPopup? Popup { get; set; }
        public Entry EntryVideoUrl { get; set; } = new Entry();

        [ObservableProperty]
        public partial string VideoUrl { get; set; } = string.Empty;

        [RelayCommand]
        private void Ok()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(VideoUrl);
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

        public void Initialize(Subtitle updatedSubtitle)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                   
                });

                return false;
            });
        }
    }
}
