using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Sync.ChangeFrameRate
{
    public partial class ChangeFrameRatePopupModel : ObservableObject
    {
        public ChangeFrameRatePopup? Popup { get; set; }

        [ObservableProperty]
        private ObservableCollection<double> _frameRates;

        [ObservableProperty]
        private double _selectedFromFrameRate;

        [ObservableProperty]
        private double _selectedToFrameRate;

        private Subtitle _subtitle = new Subtitle();

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
            if (SelectedFromFrameRate < 1 || SelectedToFrameRate < 1)
            {
                return;
            }

            if (Math.Abs(SelectedFromFrameRate - SelectedToFrameRate) < 0.001)
            {
                //TODO: show warning
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _subtitle.ChangeFrameRate(SelectedFromFrameRate, SelectedToFrameRate);
                var result = new ChangeFrameRateResult(_subtitle, SelectedFromFrameRate, SelectedToFrameRate);
                Popup?.Close(result);
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

        public void Initialize(Subtitle subtitle)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                _subtitle = subtitle;

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
