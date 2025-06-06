﻿using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubtitleAlchemist.Features.Video.BurnIn
{
    public partial class OutputPropertiesPopupModel : ObservableObject
    {
        public OutputPropertiesPopup? Popup { get; set; }

        [ObservableProperty]
        public partial bool UseSourceFolder { get; set; }

        [ObservableProperty]
        public partial bool UseOutputFolder { get; set; }

        [ObservableProperty]
        public partial string OutputFolder { get; set; }

        [ObservableProperty]
        public partial string OutputSuffix { get; set; }

        public OutputPropertiesPopupModel()
        {
            UseSourceFolder = false;
            UseOutputFolder = true;
            OutputFolder = string.Empty;
            OutputSuffix = string.Empty;
        }

        [RelayCommand]
        private void Ok()
        {
            var result = new OutputProperties
            {
                OutputFolder = OutputFolder,
                UseOutputFolder = UseOutputFolder,
                Suffix = OutputSuffix,
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(result);
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

        public void Initialize(OutputProperties outputProperties)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UseSourceFolder = !outputProperties.UseOutputFolder;
                    UseOutputFolder = outputProperties.UseOutputFolder;
                    OutputFolder = outputProperties.OutputFolder;
                    OutputSuffix = outputProperties.Suffix;
                });

                return false;
            });
        }
    }
}
