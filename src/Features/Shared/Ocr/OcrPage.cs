using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public class OcrPage : ContentPage
{
    public OcrPage(OcrPageModel vm)
    {
        BindingContext = vm;
        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = GridLength.Auto }, // OCR Engine
                new() { Height = GridLength.Star }, // List and Image
                new() { Height = 250 }, // Text and OCR corrections
                new() { Height = GridLength.Auto }, // Buttons
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = GridLength.Star },
                new() { Width = GridLength.Star },
            },
            Margin = new Thickness(20, 20, 20, 20)
        };

        var row = 0;
        var labelTitle = new Label
        {
            Text = "OCR engine",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var pickerOcrEngine = new Picker
        {
            ItemsSource = vm.OcrEngines,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,

        }.BindDynamicTheme();
        pickerOcrEngine.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedOcrEngine));
        var stackOcrEngine = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                labelTitle,
                pickerOcrEngine,
            }
        };
        grid.Add(stackOcrEngine, 0, row);
        grid.SetColumnSpan(stackOcrEngine, 2);

        row++;
        grid.Add(MakeSubtitleList(vm), 0, row);
        grid.Add(MakeImageView(vm), 1, row);

        // Text and OCR correction settings
        row++;
        grid.Add(MakeTextAndButtonsView(vm), 0, row);
        grid.Add(MakeOcrSettingsView(vm), 1, row);

        // OK and Cancel buttons
        row++;
        var buttonOk = new Button
        {
            Text = "Ok",
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 5, 0, 20),
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        };
        grid.Add(stackButtons, 0, row);

        Content = grid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private static Border MakeSubtitleList(OcrPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // header
                new RowDefinition { Height = GridLength.Star }, // collection view of batch items
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 2,
            ColumnSpacing = 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();


        // Create the header grid
        var gridHeader = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 65 }, // number
                new ColumnDefinition { Width = 100 }, // start time
                new ColumnDefinition { Width = 100 }, // duration
                new ColumnDefinition { Width = GridLength.Star }, // text
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "#",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Show",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 1);
        gridHeader.Add(
            new Label
            {
                Text = "Duration",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 2);
        gridHeader.Add(
            new Label
            {
                Text = "Text",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 3);

        // Add the header grid to the main grid
        grid.Add(gridHeader, 0);

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var jobItemGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 65 }, // number
                        new ColumnDefinition { Width = 100 }, // start time
                        new ColumnDefinition { Width = 100 }, // duration
                        new ColumnDefinition { Width = GridLength.Star }, // text
                    },
                }.BindDynamicTheme();

                var labelNumber = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelNumber.SetBinding(Label.TextProperty, nameof(OcrSubtitleItem.Number));
                jobItemGrid.Add(labelNumber, 0);


                var labelShowTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelShowTime.SetBinding(Label.TextProperty, nameof(OcrSubtitleItem.StartTime), BindingMode.Default, new TimeSpanToStringConverter());
                jobItemGrid.Add(labelShowTime, 1, 0);

                var labelDuration = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly(); 
                labelDuration.SetBinding(Label.TextProperty, nameof(OcrSubtitleItem.Duration), BindingMode.Default, new TimeSpanToShortStringConverter());
                jobItemGrid.Add(labelDuration, 2, 0);

                var labelText = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelText.SetBinding(Label.TextProperty, nameof(OcrSubtitleItem.Text));
                jobItemGrid.Add(labelText, 3, 0);

                return jobItemGrid;
            }),
        }.BindDynamicTheme();

        grid.Add(collectionView, 0, 1);
        collectionView.BindingContext = vm;
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.OcrSubtitleItems), BindingMode.TwoWay);
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedOcrSubtitleItem), BindingMode.TwoWay);
        collectionView.SelectionChanged += vm.OnCollectionViewSelectionChanged;
        vm.ListView = collectionView;

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(5),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
        }.BindDynamicTheme();

        return border;
    }

    private static Border MakeImageView(OcrPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // image info
                new RowDefinition { Height = GridLength.Star }, // collection view of batch items
                new RowDefinition { Height = GridLength.Auto }, // settings
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 2,
            ColumnSpacing = 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelImageInfo = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(5),
        }.BindDynamicTheme().BindText(nameof(vm.CurrentBitmapInfo));
        grid.Add(labelImageInfo, 0, 0);

        var imageCurrent = new Image
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Aspect = Aspect.AspectFit,
        };
        imageCurrent.SetBinding(Image.SourceProperty, nameof(vm.CurrentImageSource));
        grid.Add(imageCurrent, 0, 1);

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(5),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
        }.BindDynamicTheme();

        return border;
    }

    private static IView MakeTextAndButtonsView(OcrPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = 100 }, // text box
                new RowDefinition { Height = GridLength.Auto }, // buttons 
                new RowDefinition { Height = GridLength.Auto },  // progress
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 2,
            ColumnSpacing = 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();


        var entryText = new Entry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme().BindText(nameof(vm.CurrentText));
        grid.Add(entryText, 0, 0);

        var buttonRunOcr = new Button
        {
            Text = "Run OCR",
            Command = vm.RunOcrCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        buttonRunOcr.SetBinding(IsEnabledProperty, nameof(vm.IsRunActive));

        var labelFromNumber = new Label
        {
            Text = "From",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var pickerFromNumber = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerFromNumber.SetBinding(Picker.ItemsSourceProperty, nameof(vm.StartFromNumbers));
        pickerFromNumber.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedStartFromNumber));

        var buttonPause = new Button
        {
            Text = "Pause",
            Command = vm.PauseCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        buttonPause.SetBinding(IsEnabledProperty, nameof(vm.IsPauseActive));

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 5, 0, 20),
            Children =
            {
                buttonRunOcr,
                labelFromNumber,
                pickerFromNumber,
                buttonPause,
            }
        };
        grid.Add(stackButtons, 0, 1);

        var stackProgress = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                new Label
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Start,
                    }.BindDynamicTheme()
                    .BindIsVisible(nameof(vm.IsProgressVisible))
                    .BindText(nameof(vm.ProgressText)),

                new ProgressBar
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Start,
                    ProgressColor = (Color)Application.Current!.Resources[ThemeNames.ProgressColor],
                }.BindIsVisible(nameof(vm.IsProgressVisible)).BindProgress(nameof(vm.ProgressValue)),
            },
        }.BindDynamicTheme();
        grid.Add(stackProgress, 0, 2);

        return grid;
    }

    private IView MakeOcrSettingsView(OcrPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // image info
                new RowDefinition { Height = GridLength.Star }, // collection view of batch items
                new RowDefinition { Height = GridLength.Auto }, // settings
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 2,
            ColumnSpacing = 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(5),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
        }.BindDynamicTheme();

        return border;
    }
}
