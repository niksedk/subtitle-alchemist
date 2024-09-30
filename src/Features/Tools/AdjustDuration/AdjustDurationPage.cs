using Microsoft.Maui.Controls.Shapes;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.NumberUpDownControl;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Converters;

namespace SubtitleAlchemist.Features.Tools.AdjustDuration;

public class AdjustDurationPage : ContentPage
{
    private readonly Grid _grid;

    public AdjustDurationPage(AdjustDurationModel vm)
    {
        BindingContext = vm;
        ThemeHelper.GetGridSelectionStyle();

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }, // Subtitle list
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        _grid = pageGrid;


        var topBar = MakeTopBar(vm);
        pageGrid.Add(topBar, 0);

        vm.ViewAdjustViaSeconds = MakeAdjustViaSecondsView(vm);
        pageGrid.Add(vm.ViewAdjustViaSeconds, 0, 1);
        pageGrid.SetColumnSpan(vm.ViewAdjustViaSeconds, 3);

        vm.ViewAdjustViaPercent = MakeAdjustViaPercentView(vm);
        vm.ViewAdjustViaPercent.IsVisible = false;
        pageGrid.Add(vm.ViewAdjustViaPercent, 0, 1);
        pageGrid.SetColumnSpan(vm.ViewAdjustViaPercent, 3);

        vm.ViewAdjustViaFixed = MakeAdjustViaFixedView(vm);
        vm.ViewAdjustViaFixed.IsVisible = false;
        pageGrid.Add(vm.ViewAdjustViaFixed, 0, 1);
        pageGrid.SetColumnSpan(vm.ViewAdjustViaFixed, 3);

        vm.ViewAdjustRecalculate = MakeAdjustRecalculateView(vm);
        vm.ViewAdjustRecalculate.IsVisible = false;
        pageGrid.Add(vm.ViewAdjustRecalculate, 0, 1);
        pageGrid.SetColumnSpan(vm.ViewAdjustRecalculate, 3);


        var labelEnforceDurationLimits = new Label
        {
            Text = Se.Language.AdjustDurations.EnforceDurationLimits,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        pageGrid.Add(labelEnforceDurationLimits, 0, 2);

        var switchEnforceDurationLimits = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
        }.BindDynamicTheme();
        switchEnforceDurationLimits.SetBinding(Switch.IsToggledProperty, nameof(vm.EnforceDurationLimits));
        pageGrid.Add(switchEnforceDurationLimits, 1, 2);


        var labelDoNotExtendPastShotChanges = new Label
        {
            Text = Se.Language.AdjustDurations.CheckShotChanges,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        pageGrid.Add(labelDoNotExtendPastShotChanges, 0, 3);

        var switchDoNotExtendPastShotChanges = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        switchDoNotExtendPastShotChanges.SetBinding(Switch.IsToggledProperty, nameof(vm.DoNotExtendPastShotChanges));
        pageGrid.Add(switchDoNotExtendPastShotChanges, 1, 3);


        var labelOverlapInfo = new Label
        {
            Text = Se.Language.AdjustDurations.Note,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        pageGrid.Add(labelOverlapInfo, 0, 4);


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
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        }.BindDynamicTheme();

        pageGrid.Add(okCancelBar, 0, 5);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    public void Initialize(Subtitle subtitle, AdjustDurationModel vm)
    {
        var subtitleGrid = MakeSubtitleGrid(vm);
        _grid.Add(subtitleGrid, 2);
        _grid.SetRowSpan(subtitleGrid, 7);
    }

    private static View MakeTopBar(AdjustDurationModel vm)
    {
        var topBar = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star }, // Title
                new ColumnDefinition { Width = GridLength.Star }, // Adjust via
            },
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = Se.Language.AdjustDurations.Title,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            FontSize = 18,
        }.BindDynamicThemeTextColorOnly();
        topBar.Add(labelTitle, 0);
        topBar.SetColumnSpan(labelTitle, 2);

        var labelAdjustVia = new Label
        {
            Text = Se.Language.AdjustDurations.AdjustVia,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        topBar.Add(labelAdjustVia, 0, 1);

        var pickerAdjustVia = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerAdjustVia.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AdjustViaItems));
        pickerAdjustVia.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAdjustViaItem));
        pickerAdjustVia.SelectedIndexChanged += vm.PickerAdjustVia_SelectedIndexChanged;
        topBar.Add(pickerAdjustVia, 1, 1);

        return topBar;
    }

    private static Grid MakeAdjustViaSecondsView(AdjustDurationModel vm)
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
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();


        var labelAddSeconds = new Label
        {
            Text = Se.Language.AdjustDurations.AddSeconds,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelAddSeconds, 0);

        var upDownViewAddSeconds = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        upDownViewAddSeconds.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.AdjustSeconds), BindingMode.TwoWay);
        grid.Add(upDownViewAddSeconds, 1);

        return grid;
    }

    private static Grid MakeAdjustViaPercentView(AdjustDurationModel vm)
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
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();

        var labelAdjustViaPercent = new Label
        {
            Text = Se.Language.AdjustDurations.SetAsPercent,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelAdjustViaPercent, 0);

        var upDownViewPercent = new NumberUpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 100,
            StepValue = 1,
            StepValueFast = 10,
            Postfix = "%",
            Value = 100,
        }.BindDynamicTheme();
        upDownViewPercent.SetBinding(NumberUpDownView.ValueProperty, nameof(vm.AdjustPercentage), BindingMode.TwoWay);
        grid.Add(upDownViewPercent, 1);

        return grid;
    }

    private static Grid MakeAdjustViaFixedView(AdjustDurationModel vm)
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
            },
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelAdjustViaFixed = new Label
        {
            Text = Se.Language.AdjustDurations.Fixed,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelAdjustViaFixed, 0);

        var upDownViewFixedValue = new NumberUpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 100,
            StepValue = 1,
            StepValueFast = 100,
        }.BindDynamicTheme();
        upDownViewFixedValue.SetBinding(NumberUpDownView.ValueProperty, nameof(vm.AdjustFixedValue), BindingMode.TwoWay);
        grid.Add(upDownViewFixedValue, 1);

        return grid;
    }

    private static Grid MakeAdjustRecalculateView(AdjustDurationModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            RowSpacing = 10,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelRecalculate = new Label
        {
            Text = Se.Language.AdjustDurations.Recalculate,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelRecalculate, 0);

        var upDownViewMaxChars = new NumberUpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 100,
            StepValue = 1,
            StepValueFast = 10,
        }.BindDynamicTheme();
        upDownViewMaxChars.SetBinding(NumberUpDownView.ValueProperty, nameof(vm.AdjustRecalculateMaximumCharacters), BindingMode.TwoWay);
        grid.Add(upDownViewMaxChars, 1);

        var labelRecalculateOptimalCharacters = new Label
        {
            Text = "Maximum characters",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelRecalculateOptimalCharacters, 0, 1);

        var upDownViewOptimalChars = new NumberUpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 100,
            StepValue = 1,
            StepValueFast = 10,
        }.BindDynamicTheme();
        upDownViewOptimalChars.SetBinding(NumberUpDownView.ValueProperty, nameof(vm.AdjustRecalculateOptimalCharacters), BindingMode.TwoWay);
        grid.Add(upDownViewOptimalChars, 1, 1);

        var labelExtendOnly = new Label
        {
            Text = Se.Language.AdjustDurations.ExtendOnly,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelExtendOnly, 0, 2);

        var checkBoxExtendOnly = new CheckBox
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        checkBoxExtendOnly.SetBinding(CheckBox.IsCheckedProperty, nameof(vm.AdjustRecalculateExtendOnly));

        return grid;
    }

    private static Border MakeSubtitleGrid(AdjustDurationModel vm)
    {
        // Create the header grid
        var headerGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.TableHeaderBackgroundColor],
            Padding = new Thickness(5),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
            },
        };

        // Add headers
        headerGrid.Add(new Label { Text = "#", FontAttributes = FontAttributes.Bold }, 0);
        headerGrid.Add(new Label { Text = "Show", FontAttributes = FontAttributes.Bold }, 1);
        headerGrid.Add(new Label { Text = "Duration", FontAttributes = FontAttributes.Bold }, 2);
        headerGrid.Add(new Label { Text = "Text", FontAttributes = FontAttributes.Bold }, 3);

        var subtitleList = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Each row will be a Grid
                var gridTexts = new Grid
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    }
                };

                // Bind each cell to the appropriate property
                var numberLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                numberLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Number));

                var startTimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                startTimeLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Start), BindingMode.Default, new TimeSpanToStringConverter());

                var labelDuration = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                labelDuration.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Duration), BindingMode.Default, new TimeSpanToShortStringConverter());

                var translatedTextLabel = new Label { VerticalTextAlignment = TextAlignment.Center }.BindDynamicThemeTextColorOnly();
                translatedTextLabel.SetBinding(Label.TextProperty, nameof(DisplayParagraph.Text));

                // Add labels to grid
                gridTexts.Add(numberLabel, 0);
                gridTexts.Add(startTimeLabel, 1);
                gridTexts.Add(labelDuration, 2);
                gridTexts.Add(translatedTextLabel, 3);

                return gridTexts;
            })
        }.BindDynamicTheme();


        var gridLayout = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Star) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
            }
        }.BindDynamicTheme();

        var labelPreviewInfo = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        labelPreviewInfo.SetBinding(Label.TextProperty, nameof(vm.PreviewInfo));

        gridLayout.Add(headerGrid, 0);
        gridLayout.Add(subtitleList , 0, 1);
        gridLayout.Add(labelPreviewInfo, 0, 2);

        subtitleList.SelectionMode = SelectionMode.Single;
        subtitleList.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Paragraphs), BindingMode.TwoWay);

        var border = new Border
        {
            Content = gridLayout,
            Padding = new Thickness(5),
            Margin = new Thickness(10, 10, 10, 50),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
        }.BindDynamicTheme();
        border.Content = gridLayout;

        return border;
    }
}
