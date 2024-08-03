using SubtitleAlchemist.Features.Video.AudioToTextWhisper;

namespace SubtitleAlchemist.Features.Translate;

public class TranslatePage : ContentPage
{
    public TranslatePage(AudioToTextWhisperModel vm)
    {
        BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
        Content = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            },
            Children =
            {
                new Label { Text = "Translate", FontSize = 24, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.CenterAndExpand },
            },
        };
    }
}