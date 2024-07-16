namespace SubtitleAlchemist.Views.Video.AudioToTextWhisper;

public class AudioToTextWhisperPage : ContentPage
{
    public AudioToTextWhisperPage(AudioToTextWhisperModel vm)
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
                new Label { Text = "Audio to text", FontSize = 24, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.CenterAndExpand },
            },
        };
    }
}