using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Views.Options.DownloadFfmpeg
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
