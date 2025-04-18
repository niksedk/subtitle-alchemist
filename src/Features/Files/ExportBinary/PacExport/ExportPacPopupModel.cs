﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Files.ExportBinary.PacExport
{
    public partial class ExportPacPopupModel : ObservableObject
    {
        [ObservableProperty] public partial ObservableCollection<string> PacCodePages { get; set; }
        [ObservableProperty] public partial string? SelectedPacCodePage { get; set; }

        public ExportPacPopup? Popup { get; set; }

        public ExportPacPopupModel()
        {
            PacCodePages = new ObservableCollection<string>
            {
                "Latin",
                "Greek",
                "Latin Czech",
                "Arabic",
                "Hebrew",
                "Thai",
                "Cyrillic",
                "Chinese Traditional (Big5)",
                "Chinese Simplified (gb2312)",
                "Korean",
                "Japanese",
                "Portuguese",
            };

            SelectedPacCodePage = PacCodePages[0];
        }

        [RelayCommand]
        private void Ok()
        {
            if (SelectedPacCodePage is null)
            {
                return;
            }

            Popup?.Close(PacCodePages.IndexOf(SelectedPacCodePage));
        }


        [RelayCommand]
        private void Cancel()
        {
            Popup?.Close();
        }
    }
}
