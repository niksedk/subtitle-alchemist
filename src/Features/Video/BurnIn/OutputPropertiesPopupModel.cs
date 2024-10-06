using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Video.BurnIn
{
    public partial class OutputPropertiesPopupModel : ObservableObject
    {
        public OutputPropertiesPopup? Popup { get; set; }

        [ObservableProperty]
        private bool _useSourceFolder;

        [ObservableProperty]
        private bool _useOutputFolder;

        [ObservableProperty]
        private string _outputFolder;

        [ObservableProperty]
        private string _videoOutputSuffix;

        public OutputPropertiesPopupModel()
        {
            _useSourceFolder = !Se.Settings.Video.BurnIn.UseOutputFolder;
            _useOutputFolder = Se.Settings.Video.BurnIn.UseOutputFolder;
            _outputFolder = Se.Settings.Video.BurnIn.OutputFolder;
            _videoOutputSuffix = Se.Settings.Video.BurnIn.BurnInSuffix;
        }

        [RelayCommand]
        private void Ok()
        {
            Se.Settings.Video.BurnIn.UseOutputFolder = UseOutputFolder;
            Se.Settings.Video.BurnIn.OutputFolder = OutputFolder;
            Se.Settings.Video.BurnIn.BurnInSuffix = VideoOutputSuffix;
            Se.SaveSettings();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(true);
            });
        }

        [RelayCommand]
        private async Task BrowseOutputFolder()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var result = await FolderPicker.Default.PickAsync(CancellationToken.None);
#pragma warning restore CA1416 // Validate platform compatibility
            if (result.IsSuccessful)
            {
                OutputFolder = result.Folder.Path;
            }
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

                });

                return false;
            });
        }
    }
}
