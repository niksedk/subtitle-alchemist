using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Help.About
{
    public partial class AboutPopupModel : ObservableObject
    {
        public AboutPopup? Popup { get; set; }

        [RelayCommand]
        private void Close()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        [RelayCommand]
        public void OpenSourceLink()
        {
            UiUtil.OpenUrl("https://github.com/niksedk/subtitle-alchemist");
        }

        [RelayCommand]
        public void OpenDonateLink()
        {
            UiUtil.OpenUrl("https://github.com/sponsors/niksedk?frequency=one-time");
        }
    }
}
