using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.SpellCheck;

public class SpellCheckerPage : ContentPage
{
    public SpellCheckerPage(SpellCheckerPageModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
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
                new Label { Text = "Spell check", FontSize = 24, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center },
            },
        };
    }
}