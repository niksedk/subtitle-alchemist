using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public class ReviewSpeechPage : ContentPage
{
    public ReviewSpeechPage(ReviewSpeechPageModel vm)
    {
        this.BindDynamicTheme();

        BindingContext = vm;

        vm.Page = this;

        var grid = new Grid
        {
            Margin = new Thickness(10, 10, 10, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // title
                new() { Height = new GridLength(1, GridUnitType.Star) }, // collection view / audio segments
                new() { Height = new GridLength(200, GridUnitType.Absolute) }, // waveform
                new() { Height = new GridLength(1, GridUnitType.Auto) }, // buttons (done)  
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) }, 
                new() { Width = new GridLength(1, GridUnitType.Auto) }, // buttons
            },
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Margin = new Thickness(5, 15, 15, 0),
            Text = "Review audio segments",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 20,
        }.BindDynamicTheme();
        grid.Add(labelTitle, 0);

        var audioSegmentsBorder = MakeAudioSegmentsView(vm);
        grid.Add(audioSegmentsBorder, 0, 2);
        Grid.SetColumnSpan(audioSegmentsBorder, 2);


        var buttonEditText = new Button
        {
            Text = "Edit text",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var buttonRegenerate = new Button
        {
            Text = "Regenerate audio",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var buttonPlay = new Button
        {
            Text = "Play",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var buttonStop = new Button
        {
            Text = "Stop",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var stackPlayStop = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            Children =
            {
                buttonPlay,
                buttonStop,
            }
        };


        var labelAutoContinue = new Label
        {
            Text = "Auto continue",
            Margin = new Thickness(10,25,0,0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var switchAutoContinue = new Switch
        {
            IsToggled = vm.AutoContinue,
            Margin = new Thickness(10, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();


        var buttonRow = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            Children =
            {
                buttonEditText,
                buttonRegenerate,
                stackPlayStop,
                labelAutoContinue,
                switchAutoContinue,
            }
        };

        grid.Add(buttonRow, 1, 1);

        var waveformView = MakeWaveformView(vm);
        grid.Add(waveformView, 0, 2);
        Grid.SetColumnSpan(waveformView, 2);

        var buttonDone = new Button
        {
            Text = "Done",
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(buttonDone, 0, 3);

        Content = grid;
    }

    private View MakeWaveformView(ReviewSpeechPageModel vm)
    {
        var boxView = new BoxView
        {
            BackgroundColor = Colors.Yellow,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        return boxView;
    }

    private Border MakeAudioSegmentsView(ReviewSpeechPageModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "Number", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 0, 0);
        headerGrid.Add(new Label { Text = "Show", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
        headerGrid.Add(new Label { Text = "Original Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 2, 0);
        headerGrid.Add(new Label { Text = "Translated Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 3, 0);

        // Add content
        var gridLayout = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            }
        }.BindDynamicTheme();

        gridLayout.Add(headerGrid, 0, 0);
        gridLayout.Add(vm.CollectionView, 0, 1);

        vm.CollectionView.SelectionMode = SelectionMode.Single;
        vm.CollectionView.SelectionChanged += vm.CollectionViewSelectionChanged;
        vm.CollectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Lines));

        var border = new Border
        {
            Content = gridLayout,
            Padding = new Thickness(5),
            Margin = new Thickness(10),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        return border;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
