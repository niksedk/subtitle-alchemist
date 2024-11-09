using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public class BatchConvertPage : ContentPage
{
    private readonly BatchConvertModel _vm;

    public BatchConvertPage(BatchConvertModel vm)
    {
        BindingContext = vm;
        _vm = vm;
        ThemeHelper.GetGridSelectionStyle();

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Title
                new RowDefinition { Height = GridLength.Star }, // File list
                new RowDefinition { Height = GridLength.Star }, // Settings
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
            Text = "Batch Convert",
            FontSize = ThemeHelper.TitleFontSize,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        pageGrid.Add(title, 0);

        pageGrid.Add(MakeFileList(vm), 0, 1);
        pageGrid.Add(MakeSettingsList(vm), 0, 2);

        var buttonConvert = new Button
        {
            Text = "Convert",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ConvertCommand,
        }.BindDynamicTheme();

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonConvert,
                buttonOk,
            },
        }.BindDynamicTheme();

        pageGrid.Add(okCancelBar, 0, 3);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private Border MakeFileList(BatchConvertModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // header
                new RowDefinition { Height = GridLength.Star }, // collection view of batch items
                new RowDefinition { Height = GridLength.Auto }, // buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(2),
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
                new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // File name
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Size
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Format
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Status
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "File name",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Size",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 1);
        gridHeader.Add(
            new Label
            {
                Text = "Format",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 2);
        gridHeader.Add(
            new Label
            {
                Text = "Status",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 3);

        // Add the header grid to the main grid
        grid.Add(gridHeader, 0, 0);


        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            HeightRequest = 250,
            ItemTemplate = new DataTemplate(() =>
            {
                var jobItemGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // File name
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Size
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Format
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }, // Status
                    },
                };

                var labelFileName = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                }.BindDynamicThemeTextColorOnly();
                labelFileName.SetBinding(Label.TextProperty, nameof(BatchConvertItem.FileName));
                jobItemGrid.Add(labelFileName, 0, 0);

                var labelSize = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelSize.SetBinding(Label.TextProperty, nameof(BatchConvertItem.Size));
                jobItemGrid.Add(labelSize, 1, 0);

                var labelFormat = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelFormat.SetBinding(Label.TextProperty, nameof(BatchConvertItem.Format));
                jobItemGrid.Add(labelFormat, 2, 0);

                var labelStatus = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelStatus.SetBinding(Label.TextProperty, nameof(BatchConvertItem.Status));
                jobItemGrid.Add(labelStatus, 3, 0);

                return jobItemGrid;
            }),
        }.BindDynamicTheme();

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.BatchItems), BindingMode.TwoWay);
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedBatchItem));
        collectionView.BindingContext = vm;

        grid.Add(collectionView, 0, 1);


        var buttonAdd = new Button
        {
            Text = "Add",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.FileAddCommand,
        }.BindDynamicTheme();

        var buttonRemove = new Button
        {
            Text = "Remove",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.FileRemoveCommand,
        }.BindDynamicTheme();

        var buttonClear = new Button
        {
            Text = "Clear",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.FileClearCommand,
        }.BindDynamicTheme();

        var boxSeparator1 = new BoxView
        {
            WidthRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            Opacity = 0.5,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BorderColor],
        };

        var buttonOutputProperties = new Button
        {
            Text = "Output properties",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchOutputPropertiesCommand,
        }.BindDynamicTheme();

        var labelOutputUseSource = new Label
        {
            Text = "Use source folder",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        //labelOutputUseSource.SetBinding(IsVisibleProperty, nameof(vm.UseSourceFolderVisible));

        var labelOutputFolder = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicThemeTextColorOnly();
        //labelOutputFolder.SetBinding(Label.TextProperty, nameof(vm.OutputSourceFolder));
        //labelOutputFolder.SetBinding(IsVisibleProperty, nameof(vm.UseOutputFolderVisible));

        //var pointerGesture = new PointerGestureRecognizer();
        //pointerGesture.PointerEntered += vm.OutputFolderLinkMouseEntered;
        //pointerGesture.PointerExited += vm.OutputFolderLinkMouseExited;
        //labelOutputFolder.GestureRecognizers.Add(pointerGesture);
        //var tapGesture = new TapGestureRecognizer();
        //tapGesture.Tapped += vm.OutputFolderLinkMouseClicked;
        //labelOutputFolder.GestureRecognizers.Add(tapGesture);
        //vm.LabelOutputFolder = labelOutputFolder;

        var boxSeparator2 = new BoxView
        {
            WidthRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            Opacity = 0.5,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BorderColor],
        };

        var labelTargetFormat = new Label
        {
            Text = "Target format",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();

        var pickerTargetFormat = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerTargetFormat.SetBinding(Picker.ItemsSourceProperty, nameof(vm.TargetFormats));
        pickerTargetFormat.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedTargetFormat));

        var boxSeparator3 = new BoxView
        {
            WidthRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            Opacity = 0.5,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BorderColor],
        };

        var labelTargetEncoding = new Label
        {
            Text = "Target encoding",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();

        var pickerTargetEncoding = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerTargetEncoding.SetBinding(Picker.ItemsSourceProperty, nameof(vm.TargetEncodings));
        pickerTargetEncoding.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedTargetEncoding));

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                buttonAdd,
                buttonRemove,
                buttonClear,
                boxSeparator2,
                labelTargetFormat,
                pickerTargetFormat,
                boxSeparator3,
                labelTargetEncoding,
                pickerTargetEncoding,
                boxSeparator1,
                buttonOutputProperties,
                labelOutputUseSource,
                labelOutputFolder,
            },
        }.BindDynamicTheme();

        grid.Add(stackButtons, 0, 2);


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

    private Border MakeSettingsList(BatchConvertModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto }, // List of functions
                new ColumnDefinition { Width = GridLength.Star }, // Settings
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
            VerticalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var functionGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) }, // Function name
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Is selected
                    },
                };

                var labelFunctionName = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(5),
                }.BindDynamicThemeTextColorOnly();
                labelFunctionName.SetBinding(Label.TextProperty, nameof(BatchConvertFunction.Name));
                functionGrid.Add(labelFunctionName, 0, 0);

                var switchIsSelected = new Switch
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                switchIsSelected.SetBinding(Switch.IsToggledProperty, nameof(BatchConvertFunction.IsSelected));
                functionGrid.Add(switchIsSelected, 1, 0);

                return functionGrid;
            }),
        }.BindDynamicTheme();
        collectionViewFunctions.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.BatchFunctions), BindingMode.TwoWay);
        collectionViewFunctions.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedBatchFunction));
        collectionViewFunctions.SelectionChanged += vm.FunctionSelectionChanged;

        grid.Add(collectionViewFunctions, 0, 0);

        vm.ViewRemoveFormatting = MakeRemoveFormattingSettings(vm);
        grid.Add(vm.ViewRemoveFormatting, 1, 0);

        vm.ViewOffsetTimeCodes = MakeOffsetTimeCodesSettings(vm);
        grid.Add(vm.ViewOffsetTimeCodes, 1, 0);

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

    private View MakeRemoveFormattingSettings(BatchConvertModel vm)
    {
        var stackRemoveFormatting = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                new Label
                {
                    Text = "Remove formatting",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = ThemeHelper.TitleFontSize,
                }.BindDynamicThemeTextColorOnly(),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove all",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveAll)),
                    },
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove italic",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveItalic)),
                    },
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove bold",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveBold)),
                    },
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove underline",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveUnderline)),
                    },
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove font tags",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveFontTags)),
                    },
                },

                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove alignment tags",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveAlignmentTags)),
                    },
                },

                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Remove color tags",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                        new Switch
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindToggledProperty(nameof(vm.FormattingRemoveColors)),
                    },
                },
            },

        }.BindDynamicTheme();

        return stackRemoveFormatting;
    }

    private View MakeOffsetTimeCodesSettings(BatchConvertModel vm)
    {
        var stackBar = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                new Label
                {
                    Text = "Offset time codes",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = ThemeHelper.TitleFontSize,
                }.BindDynamicThemeTextColorOnly(),
                new Label
                {
                    Text = "HH:MM:SS:MS",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly(),
                new SubTimeUpDown
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                },
                new Switch
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                },
                new Switch
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                },

            },
        }.BindDynamicTheme();

        return stackBar;
    }

    protected override void OnDisappearing()
    {
        _vm.OnDisappearing();
        base.OnDisappearing();
    }
}
