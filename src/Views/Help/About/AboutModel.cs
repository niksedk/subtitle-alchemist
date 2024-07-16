using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Views.Help.About
{
    public partial class AboutModel : ObservableObject
    {
        public AboutPopup? Popup { get; set; }

        public AboutModel()
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
