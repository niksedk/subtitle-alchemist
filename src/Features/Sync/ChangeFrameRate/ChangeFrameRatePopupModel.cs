using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Logic.Media;

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

        private Subtitle _subtitle = new();
        private readonly IFileHelper _fileHelper;

        public ChangeFrameRatePopupModel(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;

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
        private async Task BrowseFromFrameRate()
        {
            var oldToFrameRate = SelectedToFrameRate;
            var frameRate = await BrowseFrameRate();
            if (frameRate > 0.001)
            {
                SelectedFromFrameRate = frameRate;
                SelectedToFrameRate = oldToFrameRate;
            }
        }

        [RelayCommand]
        private async Task BrowseToFrameRate()
        {
            var oldFromFrameRate = SelectedFromFrameRate;
            var frameRate = await BrowseFrameRate();
            if (frameRate > 0.001)
            {
                SelectedToFrameRate = frameRate;
                SelectedFromFrameRate = oldFromFrameRate;
            }
        }

        private async Task<double> BrowseFrameRate()
        {
            var videoFileName = await _fileHelper.PickAndShowVideoFile("Open video file");
            if (string.IsNullOrEmpty(videoFileName) || !File.Exists(videoFileName))
            {
                return 0;
            }

            var mediaInfo = FfmpegMediaInfo2.Parse(videoFileName);
            if (mediaInfo.FramesRate > 0.001m)
            {
                var frameRate = (double)mediaInfo.FramesRate;

                if (!FrameRates.Contains(frameRate))
                {
                    FrameRates.Add(frameRate);
                    FrameRates = new ObservableCollection<double>(FrameRates.OrderBy(x => x));
                }

                return frameRate;
            }

            return 0;
        }


        [RelayCommand]
        private void Ok()
        {
            if (SelectedFromFrameRate < 0.1 || SelectedToFrameRate < 0.1)
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

        public void Initialize(Subtitle subtitle, VideoInfo videoInfo)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                _subtitle = subtitle;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectedFromFrameRate = FrameRates[0];
                    SelectedToFrameRate = FrameRates[2];

                    if (videoInfo.FramesPerSecond > 0.001)
                    {
                        if (!FrameRates.Contains(videoInfo.FramesPerSecond))
                        {
                            FrameRates.Add(videoInfo.FramesPerSecond);
                            FrameRates = new ObservableCollection<double>(FrameRates.OrderBy(x => x));
                        }

                        SelectedFromFrameRate = videoInfo.FramesPerSecond;
                    }
                });

                return false;
            });
        }
    }
}
