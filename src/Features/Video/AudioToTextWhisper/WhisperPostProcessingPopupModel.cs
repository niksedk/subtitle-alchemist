using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper
{
    public partial class WhisperPostProcessingPopupModel : ObservableObject
    {
        public WhisperPostProcessingPopup? Popup { get; set; }

        public WhisperPostProcessingPopupModel()
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
}
