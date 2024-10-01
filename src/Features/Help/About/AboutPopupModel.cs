using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Help.About
{
    public partial class AboutPopupModel : ObservableObject
    {
        public AboutPopup? Popup { get; set; }
        public Label LabelSourceCodeLink { get; set; } = new();
        public Label LabelDonateLink { get; set; } = new();

        [RelayCommand]
        private void Close()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        public void SourceLinkMouseEntered(object? sender, PointerEventArgs e)
        {
            LabelSourceCodeLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
        }

        public void SourceLinkMouseExited(object? sender, PointerEventArgs e)
        {
            LabelSourceCodeLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
        }

        public void SourceLinkMouseClicked(object? sender, TappedEventArgs e)
        {
            UiUtil.OpenUrl("https://github.com/niksedk/subtitle-alchemist");
        }

        public void DonateLinkMouseEntered(object? sender, PointerEventArgs e)
        {
            LabelDonateLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
        }

        public void DonateLinkMouseExited(object? sender, PointerEventArgs e)
        {
            LabelDonateLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
        }

        public void DonateLinkMouseClicked(object? sender, TappedEventArgs e)
        {
            UiUtil.OpenUrl("https://github.com/sponsors/niksedk?frequency=one-time");
        }
    }
}
