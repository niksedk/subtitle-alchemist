using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper
{
    public partial class WhisperPostProcessingPopupModel : ObservableObject
    {
        public WhisperPostProcessingPopup? Popup { get; set; }
        public Switch MergeShortLinesSwitch { get; set; } = new();
        public Switch BreakSplitLongLinesSwitch { get; set; } = new();
        public Switch FixShortDurationsSwitch { get; set; } = new();
        public Switch FixCasingSwitch { get; set; } = new();
        public Switch AddPeriodsSwitch { get; set; } = new();

        public void LoadSettings()
        {
            MergeShortLinesSwitch.IsToggled = Configuration.Settings.Tools.WhisperPostProcessingMergeLines;
            AddPeriodsSwitch.IsToggled = Configuration.Settings.Tools.WhisperPostProcessingAddPeriods;
            BreakSplitLongLinesSwitch.IsToggled = Configuration.Settings.Tools.WhisperPostProcessingSplitLines;
            FixShortDurationsSwitch.IsToggled = Configuration.Settings.Tools.WhisperPostProcessingFixShortDuration;
            FixCasingSwitch.IsToggled = Configuration.Settings.Tools.WhisperPostProcessingFixCasing;
        }

        [RelayCommand]
        private void Cancel()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        [RelayCommand]
        private void Ok()
        {
            Configuration.Settings.Tools.WhisperPostProcessingMergeLines = MergeShortLinesSwitch.IsToggled;
            Configuration.Settings.Tools.WhisperPostProcessingAddPeriods = AddPeriodsSwitch.IsToggled;
            Configuration.Settings.Tools.WhisperPostProcessingSplitLines = BreakSplitLongLinesSwitch.IsToggled;
            Configuration.Settings.Tools.WhisperPostProcessingFixShortDuration = FixShortDurationsSwitch.IsToggled;
            Configuration.Settings.Tools.WhisperPostProcessingFixCasing = FixCasingSwitch.IsToggled;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }
    }
}
