using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Edit
{
    public partial class GoToLineNumberPopupModel : ObservableObject
    {
        public GoToLineNumberPopup? Popup { get; set; }

        public GoToLineNumberPopupModel()
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
