using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Files.ExportBinary.Cavena890Export
{
    public partial class ExportCavena890PopupModel : ObservableObject
    {
        public ExportCavena890Popup? Popup { get; set; }

        public ExportCavena890PopupModel()
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
