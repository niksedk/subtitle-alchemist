using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public sealed class ResolutionPopup : Popup
{
    public ResolutionPopup(ResolutionPopupModel vm)
    {
        BindingContext = vm;

        CanBeDismissedByTappingOutsideOfPopup = true;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star }, // items (collection view)
                new RowDefinition { Height = GridLength.Auto }, // buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 700,
            HeightRequest = 730,
        }.BindDynamicTheme();

        var collectionView = new CollectionView
        {
            WidthRequest = 600,
            HeightRequest = 630,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemsSource = vm.ResolutionItems,
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(5),
                };
                label.SetBinding(Label.TextProperty, nameof(ResolutionItem.DisplayName));
                label.SetBinding(VisualElement.BackgroundColorProperty, nameof(ResolutionItem.BackgroundColor));
                label.SetBinding(Label.TextColorProperty, nameof(ResolutionItem.TextColor));

                // add mouse entered gesture
                var mouseGesture = new PointerGestureRecognizer();
                mouseGesture.PointerEntered += vm.PointerEntered;
                mouseGesture.PointerEnteredCommandParameter = ".";
                mouseGesture.PointerExited += vm.PointerExited;
                mouseGesture.PointerExitedCommandParameter = ".";
                label.GestureRecognizers.Add(mouseGesture);

                var tapGesture = new TapGestureRecognizer();
                tapGesture.CommandParameter = ".";
                tapGesture.Tapped += vm.TappedSingle;
                label.GestureRecognizers.Add(tapGesture);

                return label;
            }),
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, new Binding(nameof(ResolutionPopupModel.SelectedResolution)));
        grid.Add(collectionView, 0, 0);


        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                buttonCancel,
            },
        };
        grid.Add(buttonBar, 0, 1);


        var windowBorder = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5),
            },
            BackgroundColor = Colors.Transparent,
            Content = grid,
        }.BindDynamicTheme();

        Content = windowBorder;

        this.BindDynamicTheme();

        vm.Popup = this;
    }
}
