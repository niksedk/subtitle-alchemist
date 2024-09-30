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

        public void SourceLinkMouseEnteredPoweredBy(object? sender, PointerEventArgs e)
        {
            LabelSourceCodeLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
        }

        public void SourceLinkMouseExitedPoweredBy(object? sender, PointerEventArgs e)
        {
            LabelSourceCodeLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
        }

        public void SourceLinkMouseClickedPoweredBy(object? sender, TappedEventArgs e)
        {
            UiUtil.OpenUrl("https://github.com/niksedk/subtitle-alchemist");
        }

        public void DonateLinkMouseEnteredPoweredBy(object? sender, PointerEventArgs e)
        {
            LabelDonateLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
        }

        public void DonateLinkMouseExitedPoweredBy(object? sender, PointerEventArgs e)
        {
            LabelDonateLink.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
        }

        public void DonateLinkMouseClickedPoweredBy(object? sender, TappedEventArgs e)
        {
            UiUtil.OpenUrl("https://www.paypal.com/donate?hosted_button_id=4XEHVLANCQBCU");
        }
    }
}
