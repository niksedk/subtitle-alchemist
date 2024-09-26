using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Sync.AdjustAllTimes;

public class AdjustAllTimesPage : ContentPage
{
    public AdjustAllTimesPage(AdjustAllTimesPageModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto }, // input
                new ColumnDefinition { Width = GridLength.Auto }, // buttons
                new ColumnDefinition { Width = GridLength.Star }, // subtitles
            },
            Margin = new Thickness(25),
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelGoToLineNumber = new Label
        {
            Text = "Adjust all times (show earlier/later)",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5, 0, 0, 15),
        }.BindDynamicTheme();
        grid.Add(labelGoToLineNumber, 0);


        var labelTime = new Label
        {
            Text = "Hour:minutes:seconds:milliseconds",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        grid.Add(labelTime, 0, 1);



        var timeUpDown = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        timeUpDown.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.AdjustTime), BindingMode.TwoWay);
        grid.Add(timeUpDown, 0, 2);


        var radioNormal = new RadioButton
        {
            Content = "All lines",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0,15,0,0),
            Padding = new Thickness(0),
        };
        radioNormal.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.AllLines));
        grid.Add(radioNormal, 0, 3);

        var radioCaseInsensitive = new RadioButton
        {
            Content = "Selected lines only",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0),
            Padding = new Thickness(0),
        };
        radioCaseInsensitive.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedLinesOnly));
        grid.Add(radioCaseInsensitive, 0, 4);

        var radioRegularExpression = new RadioButton
        {
            Content = "Selected and subsequent lines",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            Padding = new Thickness(0),
        };
        radioRegularExpression.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedAndSubsequentLines));
        grid.Add(radioRegularExpression, 0, 5);


        var buttonOk = new Button
        {
            Text = "Show earlier",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.ShowEarlierCommand,
            Margin = new Thickness(0, 0, 10, 10),
            WidthRequest = 175,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Show later",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Command = vm.ShowLaterCommand,
            Margin = new Thickness(0, 0, 10, 10),
            WidthRequest = 175,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(15, 0, 0, 0),
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        grid.Add(buttonBar, 1, 3);
        grid.SetRowSpan(buttonBar, 5);

        var labelTotalAdjustmentInfo = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 15),
        }.BindDynamicTheme();
        labelTotalAdjustmentInfo.SetBinding(Label.TextProperty, nameof(vm.TotalAdjustmentInfo), BindingMode.TwoWay);
        grid.Add(labelTotalAdjustmentInfo, 0, 6);

        var subtitleGrid = MakeSubtitleGrid(vm); // no, use video player instead
        grid.Add(subtitleGrid, 2, 0);
        grid.SetRowSpan(subtitleGrid, 7);

        //TODo: add waveform

        Content = grid;

        this.BindingContext = vm;

        this.BindDynamicTheme();
    }

    private Border MakeSubtitleGrid(AdjustAllTimesPageModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            BackgroundColor = Color.FromRgb(22, 22, 22), //TODO: Add to resources, header background color
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "Number", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 0, 0);
        headerGrid.Add(new Label { Text = "Show", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 1, 0);
        headerGrid.Add(new Label { Text = "Hide", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 2, 0);
        headerGrid.Add(new Label { Text = "Text", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center }, 3, 0);


        var collectionView = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Each row will be a Grid
                var gridTexts = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    }
                };

                // Bind each cell to the appropriate property
                var numberLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                numberLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Number));
                //numberLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                var startTimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                startTimeLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Start), BindingMode.Default, new TimeSpanToStringConverter());
                //startTimeLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                var originalTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                originalTextLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.End));
                //originalTextLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                var translatedTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                translatedTextLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Text));
                //translatedTextLabel.SetBinding(BackgroundColorProperty, "BackgroundColor");

                // Add labels to grid
                gridTexts.Add(numberLabel, 0, 0);
                gridTexts.Add(startTimeLabel, 1, 0);
                gridTexts.Add(originalTextLabel, 2, 0);
                gridTexts.Add(translatedTextLabel, 3, 0);

                return gridTexts;
            })
        }.BindDynamicTheme();


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
        gridLayout.Add(collectionView, 0, 1);

        collectionView.SelectionMode = SelectionMode.Single;
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs));
        collectionView.SelectionChanged += vm.SubtitlesViewSelectionChanged;

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
        border.Content = gridLayout;

        return border;
    }
}