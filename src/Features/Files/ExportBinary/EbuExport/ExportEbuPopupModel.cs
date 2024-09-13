using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Files.ExportBinary.EbuExport
{
    public partial class ExportEbuPopupModel : ObservableObject
    {
        public ExportEbuPopup? Popup { get; set; }

        public ExportEbuPopupModel()
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
