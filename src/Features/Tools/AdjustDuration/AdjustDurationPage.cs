using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Tools.AdjustDuration;

public class AdjustDurationPage : ContentPage
{
    public AdjustDurationPage(AdjustDurationModel vm)
    {
        BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
        Content = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Star) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
            Children =
            {
                new Label { Text = "Adjust duration for all subtitle lines", FontSize = 24, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
            },
        };
    }
}