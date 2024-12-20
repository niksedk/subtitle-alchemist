﻿using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Files;

public class RestoreAutoBackupPage : ContentPage
{
    public RestoreAutoBackupPage(RestoreAutoBackupPageModel vm)
    {
        this.BindDynamicTheme();
        Padding = new Thickness(10);
        vm.Page = this;
        BindingContext = vm;

        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // title
                new RowDefinition { Height = GridLength.Star }, // files header + files collection view
                new RowDefinition { Height = GridLength.Auto }, // link label and buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Restore auto-back",
        }.AsTitle();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);


        var header = MakeHeader(vm);
        var collectionView = MakeCollectionView(vm);

        var gridSubtitleFiles = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
        };
        gridSubtitleFiles.Add(header, 0, 0);
        gridSubtitleFiles.Add(collectionView, 0, 1);

        var border = new Border
        {
            Padding = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content =  gridSubtitleFiles,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        grid.Add(border, 0, 1);
        Grid.SetColumnSpan(border, 2);

        vm.LabelOpenFolder = new Label
        {
            TextDecorations = TextDecorations.Underline,
            Text = "Open containing folder",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        };
        grid.Add(vm.LabelOpenFolder, 0, 2);
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += (s, e) => vm.OpenContainingFolderTapped();
        vm.LabelOpenFolder.GestureRecognizers.Add(tapGestureRecognizer);
        var pointerGestureRecognizers = new PointerGestureRecognizer();
        pointerGestureRecognizers.PointerEntered += vm.OpenContainingFolderPointerEntered;
        pointerGestureRecognizers.PointerExited += vm.OpenContainingFolderPointerExited;
        vm.LabelOpenFolder.GestureRecognizers.Add(pointerGestureRecognizers);


        var buttonOk = new Button
        {
            Text = "Restore selected file",
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        buttonOk.SetBinding(IsEnabledProperty, nameof(RestoreAutoBackupPageModel.IsOkButtonEnabled));

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        }.BindDynamicTheme();

        grid.Add(buttonBar, 1, 2);

        Content = grid;
    }

    private CollectionView MakeCollectionView(RestoreAutoBackupPageModel vm)
    {
        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var rulesItemsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Date and time
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // File name
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Extension
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Size
                    },
                };

                var labelDateAndTime = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelDateAndTime.SetBinding(Label.TextProperty, nameof(DisplayFile.DateAndTime));
                rulesItemsGrid.Add(labelDateAndTime, 0, 0);

                var labelFileName = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelFileName.SetBinding(Label.TextProperty, nameof(DisplayFile.FileName));
                rulesItemsGrid.Add(labelFileName, 1, 0);

                var labelExtension = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelExtension.SetBinding(Label.TextProperty, nameof(DisplayFile.Extension));
                rulesItemsGrid.Add(labelExtension, 2, 0);

                var labelSize = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly();
                labelSize.SetBinding(Label.TextProperty, nameof(DisplayFile.Size));
                rulesItemsGrid.Add(labelSize, 3, 0);

                return rulesItemsGrid;
            }),
        }; 
        
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(RestoreAutoBackupPageModel.Files));
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(RestoreAutoBackupPageModel.SelectedFile));

        collectionView.SelectionChanged += vm.SelectionChanged;

        return collectionView;
    }

    private static Grid MakeHeader(RestoreAutoBackupPageModel vm)
    {
        // Create the header grid
        var gridHeader = new Grid
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Date and time
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // File name
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Extension
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Size
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "Date and time",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
        gridHeader.Add(
            new Label
            {
                Text = "File name",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Extension",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 2, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Size",
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            }, 3, 0);

        return gridHeader;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as RestoreAutoBackupPageModel)?.Initialize();
    }
}
