using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.AudioVisualizerControl;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Sync.AdjustAllTimes;

public class AdjustAllTimesPage : ContentPage
{
    private readonly Grid _mainGrid;

    public AdjustAllTimesPage(AdjustAllTimesPageModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
        _mainGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }, // audio visualizer
                new RowDefinition { Height = GridLength.Star },
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
        _mainGrid.Add(labelGoToLineNumber, 0);


        var labelTime = new Label
        {
            Text = "Hour:minutes:seconds:milliseconds",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        _mainGrid.Add(labelTime, 0, 1);



        var timeUpDown = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        timeUpDown.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.AdjustTime), BindingMode.TwoWay);
        _mainGrid.Add(timeUpDown, 0, 2);


        var radioNormal = new RadioButton
        {
            Content = "All lines",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0,15,0,0),
            Padding = new Thickness(0),
        };
        radioNormal.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.AllLines));
        _mainGrid.Add(radioNormal, 0, 3);

        var radioCaseInsensitive = new RadioButton
        {
            Content = "Selected lines only",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0),
            Padding = new Thickness(0),
        };
        radioCaseInsensitive.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedLinesOnly));
        _mainGrid.Add(radioCaseInsensitive, 0, 4);

        var radioRegularExpression = new RadioButton
        {
            Content = "Selected and subsequent lines",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            Padding = new Thickness(0),
        };
        radioRegularExpression.SetBinding(RadioButton.IsCheckedProperty, nameof(vm.SelectedAndSubsequentLines));
        _mainGrid.Add(radioRegularExpression, 0, 5);


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
        _mainGrid.Add(buttonBar, 1, 3);
        _mainGrid.SetRowSpan(buttonBar, 5);

        var labelTotalAdjustmentInfo = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 15),
        }.BindDynamicTheme();
        labelTotalAdjustmentInfo.SetBinding(Label.TextProperty, nameof(vm.TotalAdjustmentInfo), BindingMode.TwoWay);
        _mainGrid.Add(labelTotalAdjustmentInfo, 0, 6);

        Content = _mainGrid;

        BindingContext = vm;
    }

    public void Initialize(Subtitle subtitle, string videoFileName, WavePeakData wavePeakData, AdjustAllTimesPageModel vm)
    {
        if (string.IsNullOrEmpty(videoFileName))
        {
            // no video player or waveform
            var subtitleGrid = MakeSubtitleGrid(vm); 
            _mainGrid.Add(subtitleGrid, 2, 0);
            _mainGrid.SetRowSpan(subtitleGrid, 8);

            vm.SubtitleList.BatchBegin();
            vm.Paragraphs = new ObservableCollection<DisplayParagraph>(subtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
            vm.SubtitleList.BatchCommit();

            return;
        }

        if (wavePeakData.Peaks == null || wavePeakData.Peaks.Count == 0)
        {
            // no waveform
            var mediaElementGridNoWaveform = MakeMediaElementGrid(vm);
            _mainGrid.Add(mediaElementGridNoWaveform, 2, 0);
            _mainGrid.SetRowSpan(mediaElementGridNoWaveform, 8);

            vm.VideoPlayer.Source = MediaSource.FromFile(videoFileName);

            return;
        }

        // video player and waveform
        var mediaElementGrid = MakeMediaElementGrid(vm);
        mediaElementGrid.Margin = new Thickness(0, 0, 0, 10);   
        _mainGrid.Add(mediaElementGrid, 2, 0);
        _mainGrid.SetRowSpan(mediaElementGrid, 7);

        vm.AudioVisualizer = new AudioVisualizer();
        _mainGrid.Add(vm.AudioVisualizer, 0, 7);
        _mainGrid.SetColumnSpan(vm.AudioVisualizer, 3);

        vm.AudioVisualizer.WavePeaks = wavePeakData;

        vm.VideoPlayer.Source = MediaSource.FromFile(videoFileName);
    }

    private MediaElement MakeMediaElementGrid(AdjustAllTimesPageModel vm)
    {
        vm.VideoPlayer = new MediaElement { ZIndex = -10000 };
        return vm.VideoPlayer;
    }

    private Border MakeSubtitleGrid(AdjustAllTimesPageModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
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


        
        vm.SubtitleList = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Each row will be a Grid
                var gridTexts = new Grid
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
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
        gridLayout.Add(vm.SubtitleList, 0, 1);

        vm.SubtitleList.SelectionMode = SelectionMode.Single;
        vm.SubtitleList.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs));
        vm.SubtitleList.SelectionChanged += vm.SubtitlesViewSelectionChanged;

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