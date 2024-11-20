using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Ocr;

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
                new ColumnDefinition { Width = GridLength.Auto }, // lines
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

        var collectionViewLinesForeground = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var functionGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                    },
                };

                var labelPoint = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5),
                }.BindDynamicThemeTextColorOnly();
                labelPoint.SetBinding(Label.TextProperty, nameof(NOcrPoint.DisplayName));
                functionGrid.Add(labelPoint, 0);

                return functionGrid;
            }),
        }.BindDynamicTheme();
        collectionViewLinesForeground.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.LinesForeground), BindingMode.TwoWay);
        collectionViewLinesForeground.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedLineForeground));
        //collectionViewLinesForeground.SelectionChanged += vm

        grid.Add(PackIntoScrollViewAndBorder(collectionViewLinesForeground), 0);


        var collectionViewLinesBackground = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var functionGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                    },
                };

                var labelPoint = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5),
                }.BindDynamicThemeTextColorOnly();
                labelPoint.SetBinding(Label.TextProperty, nameof(NOcrPoint.DisplayName));
                functionGrid.Add(labelPoint, 0);

                return functionGrid;
            }),
        }.BindDynamicTheme();
        collectionViewLinesBackground.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.LinesBackground), BindingMode.TwoWay);
        collectionViewLinesBackground.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedLineBackground));


        // middle column

        var stackZoomOutAndZoomIn = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                new Label { Text = "Current image" }.BindDynamicTheme(),
                new Button
                {
                    Text = "-",
                    Command = vm.ZoomOutCommand,
                }.BindDynamicTheme(),
                new Button
                {
                    Text = "+",
                    Command = vm.ZoomInCommand,
                }.BindDynamicTheme(),
            },
        }.BindDynamicTheme();

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
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();

        var entryNewText = new Entry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        };
        entryNewText.SetBinding(Entry.TextProperty, nameof(vm.NewText));


        var stackMiddle = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                stackZoomOutAndZoomIn,
                imageLetter,
                labelNewText,
                entryNewText,
            },
        }.BindDynamicTheme();

        grid.Add(stackMiddle, 1);

        
        // right column


        var stackRight = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                new Label { Text = "#Lines to draw" }.BindDynamicTheme(),
                new Picker
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme().Bind(nameof(vm.NoOfLinesToAutoDrawList), nameof(vm.SelectedNoOfLinesToAutoDraw)),
                new Button() { Text = "Auto draw again", Command = vm.AutoGuessLinesCommand }.BindDynamicTheme(),
            },
        }.BindDynamicTheme();

        grid.Add(stackRight, 2);


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
