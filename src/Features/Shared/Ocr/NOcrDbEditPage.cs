using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public class NOcrDbEditPage : ContentPage
{
    private readonly NOcrDbEditPageModel _vm;

    public NOcrDbEditPage(NOcrDbEditPageModel vm)
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
                new RowDefinition { Height = GridLength.Star }, // Letter nOCR character instances
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
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        }.AsTitle().BindText(nameof(vm.Title));
        pageGrid.Add(title, 0);

        var labelCharacters = new Label
        {
            Text = "Characters",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        }.BindDynamicTheme();
        var pickerCharacters = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5),
        }.BindDynamicTheme().Bind(nameof(vm.CharacterList), nameof(vm.SelectedCharacter));
        pickerCharacters.SelectedIndexChanged += (sender, args) => vm.SelectedCharacterChanged();
        var stackCharacters = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelCharacters,
                pickerCharacters,
            },
        }.BindDynamicTheme();
        pageGrid.Add(stackCharacters, 0, 1);

        var currentImageAndMatchView = MakeCurrentNOcrView(vm);
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

    private static Grid MakeCurrentNOcrView(NOcrDbEditPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto }, // Character list
                new ColumnDefinition { Width = GridLength.Auto }, // Character info + edit
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var gridCharacterList = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Title
                new RowDefinition { Height = GridLength.Auto }, // Character list
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
        var labelCurrentCharacter = new Label
        {
            Text = "Current character(s)",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        }.BindDynamicTheme();
        gridCharacterList.Add(labelCurrentCharacter, 0);

        var collectionViewCharacterItems = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(5),
                };
                label.SetBinding(Label.TextProperty, nameof(NOcrChar.Text));
                return label;
            }),
        }.BindDynamicTheme();
        collectionViewCharacterItems.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.NOcrCharList));
        collectionViewCharacterItems.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedNOcrChar));
        collectionViewCharacterItems.SelectionChanged += (sender, args) => vm.SelectedNOcrCharChanged();
        gridCharacterList.Add(collectionViewCharacterItems, 0, 1);

        var gridOcrCharDetails = MakeOcrCharDetails(vm);

        grid.Add(PackIntoScrollViewAndBorder(gridCharacterList), 0);
        grid.Add(PackIntoScrollViewAndBorder(gridOcrCharDetails), 1);

        return grid;
    }

    private static Grid MakeOcrCharDetails(NOcrDbEditPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto }, // Character info
                new ColumnDefinition { Width = GridLength.Auto }, // Buttons
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();

        var labelMatch = new Label
        {
            Text = "Match",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        }.BindDynamicTheme();
        var buttonZoomOut = new Button
        {
            Text = "-",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ZoomOutCommand,
        }.BindDynamicTheme();
        var buttonZoomIn = new Button
        {
            Text = "+",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ZoomInCommand,
        }.BindDynamicTheme();
        var stackZoom = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                labelMatch,
                buttonZoomOut,
                buttonZoomIn,
            },
        }.BindDynamicTheme();

        grid.Add(stackZoom, 0);

        var drawingCanvas = new NOcrDrawingCanvasView
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 200,
            HeightRequest = 200,
        };
        drawingCanvas.SetStrokeWidth(1);
        vm.NOcrDrawingCanvas = drawingCanvas;

        var labelResolution = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(5),
        }.BindText(nameof(vm.CurrentImageResolution));

        var stackCanvasAndResolution = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                drawingCanvas,
                labelResolution,
            },
        };

        grid.Add(stackCanvasAndResolution, 0, 1);


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
        entryMatchText.SetBinding(Entry.TextProperty, nameof(vm.CurrentText));

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

        grid.Add(stackMatchText, 0, 2);

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
        checkBoxItalic.SetBinding(CheckBox.IsCheckedProperty, nameof(vm.CurrentItalic));
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
        grid.Add(stackItalic, 0, 3);

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

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(25, 0, 0, 0),
            Children =
            {
                buttonDelete,
            },
        }.BindDynamicTheme();
        grid.Add(stackButtons, 1, 1);

        grid.Add(buttonUpdate, 1, 2);

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
