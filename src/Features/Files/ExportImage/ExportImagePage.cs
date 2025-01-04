using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Files.ExportImage;

public class ExportImagePage : ContentPage
{
    public ExportImagePage(ExportImagePageModel vm)
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
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // title
                new RowDefinition { Height = GridLength.Star }, // settings + subtitle collection view
                new RowDefinition { Height = GridLength.Auto }, // buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star }, // settings
                new ColumnDefinition { Width = GridLength.Star }, // subtitle collection view
            }
        }.BindDynamicTheme();

        var titleLabel = new Label()
        .AsTitle()
        .BindDynamicTheme()
        .BindText(nameof(vm.Title));
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);


        var settings = MakeSettingsView(vm);
        grid.Add(settings, 0, 1);

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
        //gridSubtitleFiles.Add(header, 0, 0);
        gridSubtitleFiles.Add(collectionView, 0, 1);

        var border = new Border
        {
            Padding = new Thickness(10),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Content = gridSubtitleFiles,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();

        grid.Add(border, 1, 1);
        Grid.SetColumnSpan(border, 2);


        var buttonOk = new Button
        {
            Text = "Export...",
            Command = vm.ExportCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        buttonOk.SetBinding(IsEnabledProperty, nameof(vm.IsExportButtonEnabled));

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        }.BindDynamicTheme();

        grid.Add(buttonBar, 1, 2);

        Content = grid;
    }

    private CollectionView MakeCollectionView(ExportImagePageModel vm)
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


        return collectionView;
    }

    private static Grid MakeSettingsView(ExportImagePageModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            },
        }.BindDynamicTheme();

        var row = 0;

        var labelFontName = new Label
        {
            Text = "Font Name",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelFontName, 0, row);

        var pickerFontName = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerFontName.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FontNames));
        pickerFontName.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFontName));
        grid.Add(pickerFontName, 1, row++);


        var labelFontSize = new Label
        {
            Text = "Font Size",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelFontSize, 0, row);

        var pickerFontSize = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerFontSize.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FontSizes));
        pickerFontSize.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFontSize));
        grid.Add(pickerFontSize, 1, row++);

        
        var labelFontColor = new Label
        {
            Text = "Font Color",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelFontColor, 0, row);

        var boxViewTextColor = new BoxView
        {
            WidthRequest = 25,
            HeightRequest = 25,
        };
        boxViewTextColor.SetBinding(BoxView.ColorProperty, nameof(vm.FontColor));
        var tapGestureRecognizerTextColor = new TapGestureRecognizer();
        tapGestureRecognizerTextColor.Tapped += vm.FontColorTapped;
        boxViewTextColor.GestureRecognizers.Add(tapGestureRecognizerTextColor);
        grid.Add(boxViewTextColor, 1, row++);


        var labelBorderColor = new Label
        {
            Text = "Border Color",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelBorderColor, 0, row);

        var boxViewBorderColor = new BoxView
        {
            WidthRequest = 25,
            HeightRequest = 25,
        };
        boxViewBorderColor.SetBinding(BoxView.ColorProperty, nameof(vm.BorderColor));
        var tapGestureRecognizerBorderColor = new TapGestureRecognizer();
        tapGestureRecognizerBorderColor.Tapped += vm.BorderColorTapped;
        boxViewBorderColor.GestureRecognizers.Add(tapGestureRecognizerBorderColor);
        grid.Add(boxViewBorderColor, 1, row++);


        var labelShadowColor = new Label
        {
            Text = "Shadow Color",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelShadowColor, 0, row);

        var boxViewShadowColor = new BoxView
        {
            WidthRequest = 25,
            HeightRequest = 25,
        };
        boxViewShadowColor.SetBinding(BoxView.ColorProperty, nameof(vm.ShadowColor));
        var tapGestureRecognizerShadowColor = new TapGestureRecognizer();
        tapGestureRecognizerShadowColor.Tapped += vm.ShadowColorTapped;
        boxViewShadowColor.GestureRecognizers.Add(tapGestureRecognizerShadowColor);
        grid.Add(boxViewShadowColor, 1, row++);



        var labelResolution = new Label
        {
            Text = "Resolution",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelResolution, 0, row);

        var pickerResolution = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerResolution.SetBinding(Picker.ItemsSourceProperty, nameof(vm.ResolutionItems));
        pickerResolution.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedResolutionItem));
        grid.Add(pickerResolution, 1, row++);


        var labelAlignment = new Label
        {
            Text = "Alignment",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelAlignment, 0, row);

        var pickerAlignment = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerAlignment.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AlignmentItems));
        pickerAlignment.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAlignmentItem));
        grid.Add(pickerAlignment, 1, row++);


        var labelBorderStyle = new Label
        {
            Text = "Border Style",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelBorderStyle, 0, row);

        var pickerBorderStyle = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerBorderStyle.SetBinding(Picker.ItemsSourceProperty, nameof(vm.BorderStyleItems));
        pickerBorderStyle.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedBorderStyleItem));
        grid.Add(pickerBorderStyle, 1, row++);


        var labelProfile = new Label
        {
            Text = "Profile",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelProfile, 0, row);

        var pickerProfile = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerProfile.SetBinding(Picker.ItemsSourceProperty, nameof(vm.ProfileItems));
        pickerProfile.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedProfileItem));
        grid.Add(pickerProfile, 1, row++);


        var labelBottomMargin = new Label
        {
            Text = "Bottom Margin",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelBottomMargin, 0, row);

        var pickerBottomMargin = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerBottomMargin.SetBinding(Picker.ItemsSourceProperty, nameof(vm.BottomMarginItems));
        pickerBottomMargin.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedBottomMarginItem));
        grid.Add(pickerBottomMargin, 1, row++);


        var labelLeftRightMargin = new Label
        {
            Text = "Left/Right Margin",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        grid.Add(labelLeftRightMargin, 0, row);

        var pickerLeftRightMargin = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        pickerLeftRightMargin.SetBinding(Picker.ItemsSourceProperty, nameof(vm.LeftRightMarginItems));
        pickerLeftRightMargin.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLeftRightMarginItem));
        grid.Add(pickerLeftRightMargin, 1, row++);

        return grid;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as RestoreAutoBackupPageModel)?.Initialize();
    }
}
