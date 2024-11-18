using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.NumberUpDownControl;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Features.Tools.BatchConvert;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

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

        var labelFilesInfo = new Label
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 15, 1),
            FontSize = 12,
        }.BindDynamicTheme().BindText(nameof(vm.BatchItemsInfo));
        pageGrid.Add(labelFilesInfo, 0);

        pageGrid.Add(MakeFileList(vm), 0, 1);
        pageGrid.Add(MakeSettingsList(vm), 0, 2);

        var buttonConvert = new Button
        {
            Text = "Convert",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ConvertCommand,
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.AreControlsEnabled));

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.CancelConvertCommand,
        }.BindDynamicTheme().BindIsVisible(nameof(vm.IsConverting));

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.AreControlsEnabled));

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
                }.BindIsVisible(nameof(vm.IsProgressVisible)).BindProgress(nameof(vm.Progress)),

                vm.LabelStatusText.BindText(nameof(vm.StatusText)),
            },
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonConvert,
                buttonCancel,
                buttonOk,
                stackProgress,
            },
        }.BindDynamicTheme();

        pageGrid.Add(okCancelBar, 0, 3);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private static Border MakeFileList(NOcrCharacterAddPageModel vm)
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
                jobItemGrid.Add(labelFileName);

                var labelSize = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelSize.SetBinding(Label.TextProperty, nameof(BatchConvertItem.DisplaySize));
                jobItemGrid.Add(labelSize, 1);

                var labelFormat = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelFormat.SetBinding(Label.TextProperty, nameof(BatchConvertItem.Format));
                jobItemGrid.Add(labelFormat, 2);

                var labelStatus = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelStatus.SetBinding(Label.TextProperty, nameof(BatchConvertItem.Status));
                jobItemGrid.Add(labelStatus, 3);

                return jobItemGrid;
            }),
        }.BindDynamicTheme();

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.BatchItems), BindingMode.TwoWay);
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedBatchItem));
        collectionView.BindingContext = vm;

        grid.Add(collectionView, 0, 1);

        // Context menu
        var flyout = new MenuFlyout();
        flyout.Add(new MenuFlyoutItem
        {
            Text = "Add...",
            Command = vm.FileAddCommand,
        });
        flyout.Add(new MenuFlyoutItem
        {
            Text = "Remove",
            Command = vm.FileRemoveCommand,
        });
        flyout.Add(new MenuFlyoutItem
        {
            Text = "Clear",
            Command = vm.FileClearCommand,
        });
        FlyoutBase.SetContextFlyout(collectionView, flyout);

        // Buttons
        var buttonAdd = new Button
        {
            Text = "Add",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.FileAddCommand,
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.AreControlsEnabled));

        var buttonRemove = new Button
        {
            Text = "Remove",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.FileRemoveCommand,
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.AreControlsEnabled));

        var buttonClear = new Button
        {
            Text = "Clear",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.FileClearCommand,
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.AreControlsEnabled));

        var boxSeparator1 = new BoxView
        {
            WidthRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            Opacity = 0.5,
            BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BorderColor],
        };

        var buttonOutputProperties = new Button
        {
            Text = "Output properties",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchOutputPropertiesCommand,
        }.BindDynamicTheme().BindIsEnabled(nameof(vm.AreControlsEnabled));

        var labelOutputUseSource = new Label
        {
            Text = "Use source folder",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        labelOutputUseSource.SetBinding(IsVisibleProperty, nameof(vm.UseSourceFolderVisible));

        var labelOutputFolder = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicThemeTextColorOnly();
        labelOutputFolder.SetBinding(Label.TextProperty, nameof(vm.OutputFolder));
        labelOutputFolder.SetBinding(IsVisibleProperty, nameof(vm.UseOutputFolderVisible));
        labelOutputFolder.WithLinkLabel(vm.OpenOutputFolderCommand);

        var boxSeparator2 = new BoxView
        {
            WidthRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            Opacity = 0.5,
            BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BorderColor],
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
            BackgroundColor = (Color)Application.Current.Resources[ThemeNames.BorderColor],
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

    private static Grid MakeSettingsList(NOcrCharacterAddPageModel vm)
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
                functionGrid.Add(labelFunctionName, 0);

                var switchIsSelected = new Switch
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicTheme();
                switchIsSelected.SetBinding(Switch.IsToggledProperty, nameof(BatchConvertFunction.IsSelected));
                functionGrid.Add(switchIsSelected, 1);

                return functionGrid;
            }),
        }.BindDynamicTheme();
        collectionViewFunctions.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.BatchFunctions), BindingMode.TwoWay);
        collectionViewFunctions.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedBatchFunction));
        collectionViewFunctions.SelectionChanged += vm.FunctionSelectionChanged;

        grid.Add(PackIntoScrollViewAndBorder(collectionViewFunctions), 0);

        return grid;
    }

    private static View MakeRemoveFormattingSettings(NOcrCharacterAddPageModel vm)
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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
                            WidthRequest = 150,
                            Margin = new Thickness(0, 0, 5, 0),
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

        return PackIntoScrollViewAndBorder(stackRemoveFormatting);
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

    private static View MakeOffsetTimeCodesSettings(BatchConvertPageModel vm)
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
                }.BindTime(nameof(vm.OffsetTimeCodesTime)).BindDynamicTheme(),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Forward",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            WidthRequest = 80,
                        }.BindDynamicThemeTextColorOnly(),
                        new RadioButton
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            GroupName = "OffsetTimeCodes"
                        }.BindIsChecked(nameof(vm.OffsetTimeCodesForward)),
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
                            Text = "Back",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            WidthRequest = 80,
                        }.BindDynamicThemeTextColorOnly(),

                        new RadioButton
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            GroupName = "OffsetTimeCodes"
                        }.BindIsChecked(nameof(vm.OffsetTimeCodesBack)),
                     },
                },
            },
        }.BindDynamicTheme();

        return PackIntoScrollViewAndBorder(stackBar);
    }

    private static View MakeAdjustDurationSettings(BatchConvertPageModel vm)
    {
        var pickerAdjustVia = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.Bind(nameof(vm.AdjustTypes), nameof(vm.SelectedAdjustType));
        pickerAdjustVia.SelectedIndexChanged += vm.PickerAdjustVia_SelectedIndexChanged;

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
                    Text = "Adjust durations",
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
                            Text = "Adjust via",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 0, 5, 0),
                        }.BindDynamicThemeTextColorOnly(),
                        pickerAdjustVia
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
                            Text = Se.Language.AdjustDurations.AddSeconds,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            Padding = new Thickness(0, 0, 5, 0),
                        }.BindDynamicThemeTextColorOnly(),
                        new SubTimeUpDown
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            MinimumWidthRequest = 100,
                        }.BindDynamicTheme().BindTime(nameof(vm.AdjustSeconds))
                    },
                }.BindIsVisible(nameof(vm.AdjustIsSecondsVisible)),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = Se.Language.AdjustDurations.SetAsPercent,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            Padding = new Thickness(0, 0, 5, 0),
                        }.BindDynamicThemeTextColorOnly(),
                        new NumberUpDownView
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            MinimumWidthRequest = 100,
                        }.BindDynamicTheme().BindValue(nameof(vm.AdjustPercentage)),
                        new Label
                        {
                            Text = "%",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicThemeTextColorOnly(),
                     },
                }.BindIsVisible(nameof(vm.AdjustIsPercentVisible)),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = Se.Language.AdjustDurations.Fixed,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            Padding = new Thickness(0, 0, 5, 0),
                        }.BindDynamicThemeTextColorOnly(),
                        new SubTimeUpDown
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            MinimumWidthRequest = 100,
                        }.BindDynamicTheme().BindTime(nameof(vm.AdjustFixedValue))
                    },
                }.BindIsVisible(nameof(vm.AdjustIsFixedVisible)),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Optimal chars/sec",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            Padding = new Thickness(0, 0, 5, 0),
                        }.BindDynamicThemeTextColorOnly(),
                        new NumberUpDownView
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            MinimumWidthRequest = 100,
                            StepValue = 1,
                            StepValueFast = 10,
                        }.BindDynamicTheme().BindValue(nameof(vm.AdjustRecalculateOptimalCharacters))
                    },
                }.BindIsVisible(nameof(vm.AdjustIsRecalculateVisible)),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Max chars/sec",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            Padding = new Thickness(0, 0, 5, 0),
                        }.BindDynamicThemeTextColorOnly(),
                        new NumberUpDownView
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            MinimumWidthRequest = 100,
                            StepValue = 1,
                            StepValueFast = 10,
                        }.BindDynamicTheme().BindValue(nameof(vm.AdjustRecalculateMaximumCharacters))
                    },
                }.BindIsVisible(nameof(vm.AdjustIsRecalculateVisible)),

            },
        }.BindDynamicTheme();

        return PackIntoScrollViewAndBorder(stackBar);
    }

    private static View MakeDeleteLinesSettings(BatchConvertPageModel vm)
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
                    Text = "Delete lines",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = ThemeHelper.TitleFontSize,
                }.BindDynamicThemeTextColorOnly(),
                new Label
                {
                    Text = "Delete where line contains",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                }.BindDynamicThemeTextColorOnly(),
                new Entry
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 200,
                    Margin = new Thickness(0,0,0,15),
                }.BindText(nameof(vm.DeleteLinesContains)).BindDynamicTheme(),
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = "Delete first number of lines",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            WidthRequest = 180,
                        }.BindDynamicThemeTextColorOnly(),
                        new Picker
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicTheme().Bind(nameof(vm.DeleteLineNumbers), nameof(vm.DeleteXFirstLines)),
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
                            Text = "Delete last number of lines",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            WidthRequest = 180,
                        }.BindDynamicThemeTextColorOnly(),

                        new Picker
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicTheme().Bind(nameof(vm.DeleteLineNumbers), nameof(vm.DeleteXLastLines)),
                     },
                },
            },
        }.BindDynamicTheme();

        return PackIntoScrollViewAndBorder(stackBar);
    }

    private static View MakeChangeFrameRateSettings(BatchConvertPageModel vm)
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
                    Text = "Change frame rate",
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
                            Text = "From frame rate",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            WidthRequest = 180,
                        }.BindDynamicThemeTextColorOnly(),
                        new Picker
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicTheme().Bind(nameof(vm.FrameRates), nameof(vm.SelectedFromFrameRate)),
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
                            Text = "To frame rate",
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            WidthRequest = 180,
                        }.BindDynamicThemeTextColorOnly(),

                        new Picker
                        {
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                        }.BindDynamicTheme().Bind(nameof(vm.FrameRates), nameof(vm.SelectedToFrameRate)),
                     },
                },
            },
        }.BindDynamicTheme();

        return PackIntoScrollViewAndBorder(stackBar);
    }

    protected override void OnDisappearing()
    {
        _vm.OnDisappearing();
        base.OnDisappearing();
    }
}
