using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public class NOcrCharacterAddPage : ContentPage
{
    private readonly NOcrCharacterAddPageModel _vm;

    public NOcrCharacterAddPage(NOcrCharacterAddPageModel vm)
    {
        BindingContext = vm;
        _vm = vm;
        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Title + collapse/expand buttons
                new RowDefinition { Height = GridLength.Auto }, // Image
                new RowDefinition { Height = GridLength.Star }, // border with settings
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
            Text = "nOCR character",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        }.AsTitle();
        pageGrid.Add(title, 0);

        var buttonCollapse = new Button
        {
            Text = "Collapse",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ShrinkCommand,
        }.BindDynamicTheme().BindIsVisible(nameof(vm.CanShrink));

        var buttonExpand = new Button
        {
            Text = "Expand",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ExpandCommand,
        }.BindDynamicTheme().BindIsVisible(nameof(vm.CanExpand));

        var stackCollapseAndExpand = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                buttonCollapse,
                buttonExpand,
            },
        }.BindDynamicTheme();
        pageGrid.Add(stackCollapseAndExpand, 0);

        var image = new Image
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Aspect = Aspect.AspectFit,
            MaximumHeightRequest = 300,
        };
        image.SetBinding(Image.SourceProperty, nameof(vm.SentenceImageSource));
        pageGrid.Add(image, 0, 1);

        var settings = MakeImageDraw(vm);
        pageGrid.Add(settings, 0, 2);

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var buttonUseOnce = new Button
        {
            Text = "Use once",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.UseOnceCommand,
        }.BindDynamicTheme();

        var buttonSkip = new Button
        {
            Text = "Skip",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.SkipCommand,
        }.BindDynamicTheme();

        var buttonAbort = new Button
        {
            Text = "Abort",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.AbortCommand,
        }.BindDynamicTheme();

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonOk,
                buttonUseOnce,
                buttonSkip,
                buttonAbort,
            },
        }.BindDynamicTheme();
        pageGrid.Add(stackButtons, 0, 3);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }
    
    private static Grid MakeImageDraw(NOcrCharacterAddPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto }, // text settings + image
                new ColumnDefinition { Width = GridLength.Auto }, // drawing settings
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();


        var labelCurrentImage = new Label { Text = "Current image" }.BindDynamicTheme();

        var imageLetter = new Image
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Aspect = Aspect.AspectFit,
        };
        imageLetter.SetBinding(Image.SourceProperty, nameof(vm.ItemImageSource));

        var labelNewText = new Label
        {
            Text = "New text",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 10),
        }.BindDynamicTheme();

        var entryNewText = new Entry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            ReturnCommand = vm.OkCommand,
        };
        entryNewText.SetBinding(Entry.TextProperty, nameof(vm.NewText));
        vm.EntryNewText = entryNewText;

        var labelIsNewTextItalic = new Label
        {
            Text = "Italic",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();

        var checkBoxIsNewTextItalic = new CheckBox
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        };
        checkBoxIsNewTextItalic.SetBinding(CheckBox.IsCheckedProperty, nameof(vm.IsNewTextItalic));
        checkBoxIsNewTextItalic.CheckedChanged += vm.CheckBoxIsNewTextItalic_CheckedChanged;

        var stackItalic = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelIsNewTextItalic,
                checkBoxIsNewTextItalic,
            },
        }.BindDynamicTheme();


        var stackMiddle = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                labelCurrentImage,
                imageLetter,
                labelNewText,
                entryNewText,
                stackItalic,
            },
        }.BindDynamicTheme();

        grid.Add(stackMiddle, 0);

        
        // right column


        var stackRight = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(20, 0, 0, 0),
            Children =
            {
                new Label { Text = "#Lines to draw" }.BindDynamicTheme(),
                new Picker
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme().Bind(nameof(vm.NoOfLinesToAutoDrawList), nameof(vm.SelectedNoOfLinesToAutoDraw)),
                new Button
                {
                    Text = "Auto draw again", Command = vm.AutoGuessLinesCommand,
                    Margin = new Thickness(0, 10, 0, 0),
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "Clear", Command = vm.ClearLinesCommand,
                    Margin = new Thickness(0, 10, 0, 0),
                }.BindDynamicTheme(),
            },
        }.BindDynamicTheme();

        grid.Add(stackRight, 1);

        var stackZoom = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                new Button
                {
                    Text = "-",
                    Command = vm.ZoomOutCommand,
                    Margin = new Thickness(25, 0, 0, 0),
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "+",
                    Command = vm.ZoomInCommand,
                    Margin = new Thickness(10, 0, 0, 0),
                }.BindDynamicTheme(),
            },
        }.BindDynamicTheme();

        var drawingCanvas = new NOcrDrawingCanvasView
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 200,
            HeightRequest = 200,
        };
        drawingCanvas.SetStrokeWidth(1);
        vm.NOcrDrawingCanvas = drawingCanvas;

        var stackDrawing = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(10, 0, 0, 0),
            Children =
            {
                stackZoom,
                drawingCanvas,
            },
        }.BindDynamicTheme();

        grid.Add(stackDrawing, 2);

        return grid;
    }
    
    protected override void OnDisappearing()
    {
        _vm.OnDisappearing();
        base.OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.OnAppearing();
    }
}
