using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Options.DownloadFfmpeg
{
    public partial class DownloadFfmpegModel : ObservableObject
    {
        public DownloadFfmpegPopup? Popup { get; set; }

        public DownloadFfmpegModel()
        {
        }

        private void Close()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }
    }
}
