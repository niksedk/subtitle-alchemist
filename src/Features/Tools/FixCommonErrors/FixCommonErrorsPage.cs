using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public class FixCommonErrorsPage : ContentPage
{
    public FixCommonErrorsPage(FixCommonErrorsModel vm)
    {
        this.BindDynamicTheme();
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
                new Label { Text = "Fix common errors", FontSize = 24, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
            },
        };
    }
}