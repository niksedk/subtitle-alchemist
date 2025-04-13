using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Features.Shared.PickSubtitleLine;

public partial class PickSubtitleLinePopupModel : ObservableObject
{
    public PickSubtitleLinePopup? Popup { get; set; }
    public CollectionView SubtitleList { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DisplayParagraph> Paragraphs { get; set; }

    [ObservableProperty]
    public partial DisplayParagraph? SelectedParagraph { get; set; }

    public PickSubtitleLinePopupModel()
    {
        SubtitleList = new();
        Title = string.Empty;
        Paragraphs = new ObservableCollection<DisplayParagraph>();
        SelectedParagraph = null;
    }

    [RelayCommand]
    private void Ok()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(SelectedParagraph?.P);
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