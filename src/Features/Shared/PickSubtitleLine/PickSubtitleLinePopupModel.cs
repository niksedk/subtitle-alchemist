using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Features.Shared.PickSubtitleLine
{
    public partial class PickSubtitleLinePopupModel : ObservableObject
    {
        public PickSubtitleLinePopup? Popup { get; set; }
        public CollectionView SubtitleList { get; set; }

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private ObservableCollection<DisplayParagraph> _paragraphs;

        [ObservableProperty]
        private DisplayParagraph? _selectedParagraph;

        public PickSubtitleLinePopupModel()
        {
            SubtitleList = new();
            _title = string.Empty;
            _paragraphs = new ObservableCollection<DisplayParagraph>();
            _selectedParagraph = null;
        }

        [RelayCommand]
        private void Ok()
        {
            var result = string.Empty;

            MainThread.BeginInvokeOnMainThread(() =>
            {
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

        public void Initialize(Subtitle subtitle, string title)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Title = title;
                    Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p=> new DisplayParagraph(p)));
                });

                return false;
            });
        }
    }
}
