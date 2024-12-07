using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public class NOcrCharacterInspectPage : ContentPage
{
    private readonly NOcrCharacterInspectPageModel _vm;

    public NOcrCharacterInspectPage(NOcrCharacterInspectPageModel vm)
    {
        BindingContext = vm;
        _vm = vm;
        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Title
                new RowDefinition { Height = GridLength.Auto }, // Letter list
                new RowDefinition { Height = GridLength.Star }, // Current image and match
                new RowDefinition { Height = GridLength.Auto }, // Status
                new RowDefinition { Height = GridLength.Auto }, // Buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var title = new Label
        {
            Text = "nOCR inspect image matches",
            FontSize = ThemeHelper.TitleFontSize,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        pageGrid.Add(title, 0);

        var collectionViewLetters = MakeLettersView(vm);
        pageGrid.Add(collectionViewLetters, 0, 1);

        var currentImageAndMatchView = MakeCurrentImageAndMatchView(vm);
        pageGrid.Add(currentImageAndMatchView, 0, 2);

        var labelStatus = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        };
        labelStatus.SetBinding(Label.TextProperty, nameof(NOcrCharacterInspectPageModel.StatusText));
        pageGrid.Add(labelStatus, 0, 3);

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        }.BindDynamicTheme();

        pageGrid.Add(okCancelBar, 0, 4);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private static Grid MakeLettersView(NOcrCharacterInspectPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var collectionViewFunctions = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal),
            ItemTemplate = new DataTemplate(() =>
            {
                var functionGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                    },
                };

                var label = new Label
                {
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5),
                };

                label.SetBinding(Label.TextProperty, nameof(NOcrCharacterInspectPageModel.LetterItem.Text));
                functionGrid.Add(label, 0);

                return functionGrid;
            }),
        }.BindDynamicTheme();
        collectionViewFunctions.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.LetterItems), BindingMode.TwoWay);
        collectionViewFunctions.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedLetterItem));
        collectionViewFunctions.SelectionChanged += vm.SelectedLetterItemChanged;

        grid.Add(PackIntoScrollViewAndBorder(collectionViewFunctions), 0);

        return grid;
    }

    private static IView MakeCurrentImageAndMatchView(NOcrCharacterInspectPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var gridCurrentImage = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        var labelCurrentImage = new Label
        {
            Text = "Current image",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        }.BindDynamicTheme();
        gridCurrentImage.Add(labelCurrentImage, 0);
        var imageCurrent = new Image
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Aspect = Aspect.AspectFit,
        };
        imageCurrent.SetBinding(Image.SourceProperty, nameof(NOcrCharacterInspectPageModel.CurrentImageSource));
        gridCurrentImage.Add(imageCurrent, 0, 1);
        var labelImageResolution = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        };
        labelImageResolution.SetBinding(Label.TextProperty, nameof(NOcrCharacterInspectPageModel.CurrentImageResolution));
        gridCurrentImage.Add(labelImageResolution, 0, 2);


        var gridMatch = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelMatch = new Label
        {
            Text = "Match",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        }.BindDynamicTheme();
        gridMatch.Add(labelMatch, 0);

        var drawingCanvas = new NOcrDrawingCanvasView
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 200,
            HeightRequest = 200,
        };
        drawingCanvas.SetStrokeWidth(1);
        vm.NOcrDrawingCanvas = drawingCanvas;
        gridMatch.Add(drawingCanvas, 0, 1);


        var labelMatchText = new Label
        {
            Text = "Text",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        };

        var entryMatchText = new Entry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
            WidthRequest = 50,
        };
        entryMatchText.SetBinding(Entry.TextProperty, nameof(NOcrCharacterInspectPageModel.MatchText));

        var stackMatchText = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelMatchText,
                entryMatchText,
            },
        };

        gridMatch.Add(stackMatchText, 0, 2);

        var labelItalic = new Label
        {
            Text = "Italic",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        };
        var checkBoxItalic = new CheckBox
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        };
        checkBoxItalic.SetBinding(CheckBox.IsCheckedProperty, nameof(NOcrCharacterInspectPageModel.MatchIsItalic));
        var stackItalic = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelItalic,
                checkBoxItalic,
            },
        };
        gridMatch.Add(stackItalic, 0, 3);

        var labelMatchInfo = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        };
        labelMatchInfo.SetBinding(Label.TextProperty, nameof(NOcrCharacterInspectPageModel.MatchInfo));
        gridMatch.Add(labelMatchInfo, 0, 4);

        var buttonUpdate = new Button
        {
            Text = "Update",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(25, 10, 15, 10),
            Command = vm.UpdateMatchCommand,
        }.BindDynamicTheme();

        var buttonDelete = new Button
        {
            Text = "Delete",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.DeleteMatchCommand,
        }.BindDynamicTheme();


        var labelNumberOfLines = new Label
        {
            Text = "Number of lines to auto draw",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        };
        var pickerNumberOfLines=  new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme().Bind(nameof(vm.NoOfLinesToAutoDrawList), nameof(vm.SelectedNoOfLinesToAutoDraw));
        var stackNumberOfLines = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelNumberOfLines,
                pickerNumberOfLines,
            },
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsNewMatch));

        var buttonAddBetterMatch = new Button
        {
            Text = "Add better match",
            Margin = new Thickness(0, 10, 0, 0),
            Command = vm.AddBetterMatchCommand,
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsAddBetterMatchVisible));

        var buttonDrawAll = new Button
        {
            Text = "Auto draw all",
            Margin = new Thickness(0, 10, 0, 0),
            Command = vm.DrawAutoAllCommand,
        }.BindDynamicTheme();
        var buttonDrawBackground = new Button
        {
            Text = "Auto draw background",
            Margin = new Thickness(10, 10, 0, 0),
            Command = vm.DrawAutoBackgroundCommand,
        }.BindDynamicTheme();
        var buttonDrawForeground = new Button
        {
            Text = "Auto draw foreground",
            Margin = new Thickness(10, 10, 0, 0),
            Command = vm.DrawAutoForegroundCommand,
        }.BindDynamicTheme();
        var stackDrawButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Children =
            {
                buttonDrawAll,
                buttonDrawBackground,
                buttonDrawForeground,
            },
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsNewMatch));

        var buttonClearAll = new Button
        {
            Text = "Clear all",
            Margin = new Thickness(0, 10, 0, 0),
            Command = vm.DrawClearAllCommand,
        }.BindDynamicTheme();
        var buttonClearBackground = new Button
        {
            Text = "Clear background",
            Margin = new Thickness(10, 10, 0, 0),
            Command = vm.DrawClearBackgroundCommand,
        }.BindDynamicTheme();
        var buttonClearForeground = new Button
        {
            Text = "Clear foreground",
            Margin = new Thickness(10, 10, 0, 0),
            Command = vm.DrawClearForegroundCommand,
        }.BindDynamicTheme();
        var stackClearButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Children =
            {
                buttonClearAll,
                buttonClearBackground,
                buttonClearForeground,
            },
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsNewMatch));

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(25, 0, 0, 0),
            Children =
            {
                buttonDelete,
                buttonAddBetterMatch,
                stackNumberOfLines,
                stackDrawButtons,
                stackClearButtons,
            },
        }.BindDynamicTheme();
        gridMatch.Add(stackButtons, 1, 1);

        gridMatch.Add(buttonUpdate, 1, 2);


        grid.Add(PackIntoScrollViewAndBorder(gridCurrentImage), 0);
        grid.Add(PackIntoScrollViewAndBorder(gridMatch), 1);

        return grid;
    }

    private static View PackIntoScrollViewAndBorder(View view)
    {
        var scrollView = new ScrollView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Content = view,
        }.BindDynamicTheme();

        var borderSettings = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(10, 5, 10, 5),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = scrollView,
        }.BindDynamicTheme();

        return borderSettings;
    }

    protected override void OnDisappearing()
    {
        _vm.OnDisappearing();
        base.OnDisappearing();
    }
}
