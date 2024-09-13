using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Files.ExportBinary.PacExport
{
    public partial class ExportPacPopupModel : ObservableObject
    {
        public ExportPacPopup? Popup { get; set; }

        public ExportPacPopupModel()
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
