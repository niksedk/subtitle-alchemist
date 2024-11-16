using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;
using Path = System.IO.Path;

namespace SubtitleAlchemist.Features.Main;

public static class InitSubtitleListView
{
    public static Border MakeSubtitleListView(MainPageModel vm)
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
            Margin = new Thickness(0),
            Padding = new Thickness(0, 0, 0, 0),
            RowSpacing = 5,
            ColumnSpacing = 5,
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
                new(50), // #
                new(95), // Show
                new(95), // Hide
                new(95), // Duration
                new(GridLength.Star) // Text
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
                Text = "Hide",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 2);
        gridHeader.Add(
            new Label
            {
                Text = "Duration",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 3);
        gridHeader.Add(
            new Label
            {
                Text = "Text",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 4);

        // Add the header grid to the main grid
        grid.Add(gridHeader, 0, 0);

        IValueConverter converter = new TimeSpanToStringConverter();
        IValueConverter converterShort = new TimeSpanToShortStringConverter();

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.None,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            ItemTemplate = new DataTemplate(() =>
            {
                var subtitleGrid = new Grid
                {
                    Padding = new Thickness(0),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 50 }, // #
                        new ColumnDefinition { Width = 95 }, // Show
                        new ColumnDefinition { Width = 95 }, // Hide
                        new ColumnDefinition { Width = 95 }, // Duration
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Text
                    },
                };

                var labelNumber = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5,0,0,0),
                }.BindDynamicThemeTextColorOnly();
                labelNumber.SetBinding(Label.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor));
                labelNumber.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Number));
                subtitleGrid.Add(labelNumber, 0, 0);

                var labelShow = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                }.BindDynamicThemeTextColorOnly();
                labelShow.SetBinding(Label.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor));
                labelShow.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Start), BindingMode.Default, converter);
                subtitleGrid.Add(labelShow, 1, 0);

                var labelHide = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                }.BindDynamicThemeTextColorOnly();
                labelHide.SetBinding(Label.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor));
                labelHide.SetBinding(Label.TextProperty, nameof(DisplayParagraph.End), BindingMode.Default, converter);
                subtitleGrid.Add(labelHide, 2, 0);

                var labelDuration = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                }.BindDynamicThemeTextColorOnly();
                labelDuration.SetBinding(Label.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor));
                labelDuration.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Duration), BindingMode.Default, converterShort);
                subtitleGrid.Add(labelDuration, 3, 0);

                var labelText = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5, 0, 0, 0),
                }.BindDynamicThemeTextColorOnly();
                labelText.SetBinding(Label.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor));
                labelText.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Text));
                subtitleGrid.Add(labelText, 4, 0);

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.NumberOfTapsRequired = 1;
                tapGestureRecognizer.Tapped += vm.SubtitleListSingleTapped;
                tapGestureRecognizer.CommandParameter = "";
                tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
                subtitleGrid.GestureRecognizers.Add(tapGestureRecognizer);

                return subtitleGrid;
            }),
        }.BindDynamicTheme();

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs), BindingMode.TwoWay);
       // collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedParagraph));
      //  collectionView.SetBinding(SelectableItemsView.SelectedItemsProperty, nameof(vm.SelectedParagraph));
        //collectionView.SelectionChanged += vm.OnCollectionViewSelectionChanged;
        collectionView.BindingContext = vm;
        vm.SubtitleList = collectionView;
        //MakeContextMenu(vm, collectionView);

        grid.Add(collectionView, 0, 1);

        var border = new Border
        {
            StrokeThickness = 1,
            Margin = new Thickness(8,2,2,2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
        }.BindDynamicTheme();

        return border;






        //var view = new CollectionView
        //{
        //    SelectionMode = SelectionMode.Single,
        //    HorizontalOptions = LayoutOptions.Fill,
        //    Margin = new Thickness(10),
        //    Header = new Grid
        //    {
        //        ColumnDefinitions = new ColumnDefinitionCollection
        //        {
        //            new(50),
        //            new(95),
        //            new(95),
        //            new(95),
        //            new(GridLength.Star)
        //        },
        //        Children =
        //        {
        //            new BoxView { BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(0),
        //            new Label { Text = "#",  Margin = 5 }.Column(0).BindDynamicTheme(),
        //            new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(1),
        //            new Label { Text = "Show", Margin = 5 }.Column(1).BindDynamicTheme(),
        //            new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(2),
        //            new Label { Text = "Hide", Margin = 5 }.Column(2).BindDynamicTheme(),
        //            new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(3),
        //            new Label { Text = "Duration", Margin = 5 }.Column(3).BindDynamicTheme(),
        //            new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(4),
        //            new Label { Text = "Text", Margin = 5 }.Column(4).BindDynamicTheme()
        //        }
        //    },
        //    ItemTemplate = new DataTemplate(() => MakeGrid(vm))
        //}.BindDynamicTheme();

        //view.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs));
        //view.SelectionChanged += vm.OnCollectionViewSelectionChanged;

        //MakeContextMenu(vm, view);

        //return view;
    }

    private static Grid MakeGrid(MainPageModel vm)
    {
        IValueConverter converter = new TimeSpanToStringConverter();
        IValueConverter converterShort = new TimeSpanToShortStringConverter();

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new(50),
                new(95),
                new(95),
                new(95),
                new(GridLength.Star)
            },
            Children =
            {
                new Label {  Margin = 1, Padding = 5 }.Column(0).Bind("Number").BindDynamicThemeTextColorOnly().Bind(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor),
                new Label {  Margin = 1, Padding = 5 }.Column(1).Bind("Start", BindingMode.Default, converter).BindDynamicThemeTextColorOnly().Bind(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor),
                new Label { Margin = 1, Padding = 5 }.Column(2).Bind("End", BindingMode.Default, converter).BindDynamicThemeTextColorOnly().Bind(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor),
                new Label { Margin = 1, Padding = 5 }.Column(3).Bind("Duration", BindingMode.Default, converterShort).BindDynamicThemeTextColorOnly().Bind(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor),
                new Label { Margin = 1, Padding = 5 }.Column(4).Bind("Text").BindDynamicThemeTextColorOnly().Bind(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor),
            }
        };

        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.NumberOfTapsRequired = 2;
        tapGestureRecognizer.Tapped += vm.ListViewDoubleTapped;
        tapGestureRecognizer.CommandParameter = "";
        tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
        grid.GestureRecognizers.Add(tapGestureRecognizer);

        return grid;
    }


    private static ImageSource ConvertSvgToImageSource(byte[] svgData)
    {
        using var stream = new MemoryStream(svgData);
        var svgDocument = new SkiaSharp.Extended.Svg.SKSvg();
        svgDocument.Load(stream);

        var bitmap = new SKBitmap((int)svgDocument.Picture.CullRect.Width, (int)svgDocument.Picture.CullRect.Height);

        using (var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            canvas.DrawPicture(svgDocument.Picture);
            surface.Canvas.Flush();

            var image = surface.Snapshot();
            bitmap = SKBitmap.FromImage(image);
            var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);

            var imageStream = new MemoryStream(data.ToArray());
            var imageSource = ImageSource.FromStream(() => imageStream);

            return imageSource;
        }
    }
}