using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Sync.ChangeFrameRate
{
    public partial class ChangeFrameRatePopupModel : ObservableObject
    {
        public ChangeFrameRatePopup? Popup { get; set; }

        [ObservableProperty]
        private int _lineNumber;

        [ObservableProperty]
        private ObservableCollection<double> _frameRates;

        [ObservableProperty]
        private double _selectedFromFrameRate;

        [ObservableProperty]
        private double _selectedToFrameRate;

        public ChangeFrameRatePopupModel()
        {
            // add list of common frame rates
            _frameRates = new ObservableCollection<double>()
            {
                23.976,
                24,
                25,
                29.97,
                30,
                48,
                59.94,
                60,
                120,
            };
        }

        [RelayCommand]
        private void Swap()
        {
            (SelectedFromFrameRate, SelectedToFrameRate) = (SelectedToFrameRate, SelectedFromFrameRate);
        }

        [RelayCommand]
        private void BrowseFromFrameRate()
        {

        }

        [RelayCommand]
        private void BrowseToFrameRate()
        {

        }

        [RelayCommand]
        private void Ok()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(LineNumber);
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
                    SelectedFromFrameRate = FrameRates[0];
                    SelectedToFrameRate = FrameRates[2];
                });

                return false;
            });
        }
    }
}
