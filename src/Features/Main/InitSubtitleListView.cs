﻿using CommunityToolkit.Maui.Markup;
using SkiaSharp;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Main;

public static class InitSubtitleListView
{
    public static CollectionView MakeSubtitleListView(MainViewModel vm)
    {
        var view = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(10),
            Header = new Grid
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
                    new BoxView { BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(0),
                    new Label { Text = "#",  Margin = 5 }.Column(0).BindDynamicTheme(),
                    new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(1),
                    new Label { Text = "Show", Margin = 5 }.Column(1).BindDynamicTheme(),
                    new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(2),
                    new Label { Text = "Hide", Margin = 5 }.Column(2).BindDynamicTheme(),
                    new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(3),
                    new Label { Text = "Duration", Margin = 5 }.Column(3).BindDynamicTheme(),
                    new BoxView { BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BackgroundColor], Margin = 1, ZIndex = -1 }.Column(4),
                    new Label { Text = "Text", Margin = 5 }.Column(4).BindDynamicTheme()
                }
            },
            ItemTemplate = new DataTemplate(() => MakeGrid(vm))
        }.BindDynamicTheme();

        view.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs));
        view.SelectionChanged += vm.OnCollectionViewSelectionChanged;

        MakeContextMenu(vm, view);

        return view;
    }

    private static Grid MakeGrid(MainViewModel vm)
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

    private static void MakeContextMenu(MainViewModel vm, CollectionView view)
    {
        var imagePath = Path.Combine("Resources", "Images", "Menu");

        vm.SubtitleListViewContextMenu = new MenuFlyout();
        vm.SubtitleListViewContextMenuItems = new List<MenuFlyoutItem>
        {
            new MenuFlyoutItem
            {
                Text = "Delete x lines?",
                Command = vm.DeleteSelectedLinesCommand,
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath,"Delete.png")),
                IsEnabled = false,
            },
            new MenuFlyoutItem
            {
                Text = "Insert line before",
                Command = vm.InsertBeforeCommand,
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath, "Add.png")),
                IsEnabled = false,
            },
            new MenuFlyoutItem
            {
                Text = "Insert line after",
                Command = vm.InsertAfterCommand,
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath, "Add.png")),
                IsEnabled = false,
            },
            new MenuFlyoutSeparator(),
            new MenuFlyoutItem
            {
                Text = "Italic",
                Command = vm.ItalicCommand, KeyboardAccelerators =
                {
                    new KeyboardAccelerator
                    {
                        Modifiers = KeyboardAcceleratorModifiers.Ctrl,
                        Key = "I",
                    }
                },
                IconImageSource = ImageSource.FromFile(Path.Combine(imagePath, "Italic.png")),
                IsEnabled = false,
            },
        };

        foreach (var item in vm.SubtitleListViewContextMenuItems)
        {
            vm.SubtitleListViewContextMenu.Add(item);
        }

        FlyoutBase.SetContextFlyout(view, vm.SubtitleListViewContextMenu);
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