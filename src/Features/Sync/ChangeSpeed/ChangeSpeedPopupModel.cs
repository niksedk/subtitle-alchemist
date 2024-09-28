using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Sync.ChangeSpeed
{
    public partial class ChangeSpeedPopupModel : ObservableObject
    {
        public ChangeSpeedPopup? Popup { get; set; }
        public Entry EntrySpeedInPercent { get; set; } = new();

        [ObservableProperty]
        private double _speedPercent;

        [ObservableProperty]
        private bool _modeCustom;

        [ObservableProperty]
        private bool _modeFromDropFrame;

        [ObservableProperty]
        private bool _modeToDropFrame;

        [ObservableProperty]
        private bool _allLines;

        [ObservableProperty]
        private bool _selectedLinesOnly;

        [ObservableProperty]
        private bool _allowOverlap;

        private Subtitle _subtitle;

        public ChangeSpeedPopupModel()
        {
            _speedPercent = 100;
            _modeCustom = true;
            _modeFromDropFrame = false;
            _modeToDropFrame = false;
            _allLines = true;
            _selectedLinesOnly = false;
            _allowOverlap = false;

            _subtitle = new Subtitle();
        }

        [RelayCommand]
        private void FromDropFrame()
        {
            SpeedPercent = 99.9889;
        }

        [RelayCommand]
        private void ToDropFrame()
        {
            SpeedPercent = 100.1001;
        }

        [RelayCommand]
        private void Ok()
        {
            if (SpeedPercent < 0.001)
            {
                return;
            }

            if (Math.Abs(SpeedPercent - 100) < 0.00001)
            {
                Cancel();
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var adjustFactor = SpeedPercent / 100.0;
                var adjustedSubtitle = AdjustAllParagraphs(_subtitle, adjustFactor);
                Popup?.Close(adjustedSubtitle);
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _subtitle = subtitle;
                });

                return false;
            });
        }

        public Subtitle AdjustAllParagraphs(Subtitle subtitle, double adjustFactor)
        {
            foreach (var p in subtitle.Paragraphs)
            {
                AdjustParagraph(p, adjustFactor);
            }

            if (!AllowOverlap)
            {
                for (var i = 0; i < subtitle.Paragraphs.Count; i++)
                {
                    var p = subtitle.Paragraphs[i];
                    var next = subtitle.GetParagraphOrDefault(i + 1);
                    if (next != null)
                    {
                        if (p.EndTime.TotalMilliseconds >= next.StartTime.TotalMilliseconds)
                        {
                            p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                        }
                    }
                }
            }

            return subtitle;
        }

        public void AdjustParagraph(Paragraph p, double adjustFactor)
        {
            p.StartTime.TotalMilliseconds = Math.Round(p.StartTime.TotalMilliseconds * adjustFactor, MidpointRounding.AwayFromZero);
            p.EndTime.TotalMilliseconds = Math.Round(p.EndTime.TotalMilliseconds * adjustFactor, MidpointRounding.AwayFromZero);
        }
    }
}
