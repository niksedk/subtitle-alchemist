using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Controls.UpDownControl;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Tools.AdjustDuration;

public class AdjustDurationPage : ContentPage
{
    public AdjustDurationPage(AdjustDurationModel vm)
    {
        BindingContext = vm;

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // top bar
                new RowDefinition { Height = GridLength.Auto }, // main content
                new RowDefinition { Height = GridLength.Auto }, // bottom bar with buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();


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


        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
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

        var upDownViewAddSeconds = new UpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        upDownViewAddSeconds.SetBinding(UpDownView.ValueProperty, nameof(vm.AdjustSeconds));
        grid.Add(upDownViewAddSeconds, 1, 0);

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

        var upDownViewPercent = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        upDownViewPercent.SetBinding(UpDownView.ValueProperty, nameof(vm.AdjustPercentage));
        grid.Add(upDownViewPercent, 1, 0);

        return grid;
    }

    private static Grid MakeAdjustViaFixedView(AdjustDurationModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
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

        var upDownViewFixedValue = new UpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        upDownViewFixedValue.SetBinding(UpDownView.ValueProperty, nameof(vm.AdjustFixedValue));
        grid.Add(upDownViewFixedValue, 1, 0);

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
                new ColumnDefinition { Width = GridLength.Star },
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

        var upDownViewMaxChars = new UpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        upDownViewMaxChars.SetBinding(UpDownView.ValueProperty, nameof(vm.AdjustRecalculateMaximumCharacters));
        grid.Add(upDownViewMaxChars, 1, 0);

        var labelRecalculateOptimalCharacters = new Label
        {
            Text = "Maximum characters",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelRecalculateOptimalCharacters, 0, 1);

        var upDownViewOptimalChars = new UpDownView
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        upDownViewOptimalChars.SetBinding(UpDownView.ValueProperty, nameof(vm.AdjustRecalculateOptimalCharacters));
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
}
