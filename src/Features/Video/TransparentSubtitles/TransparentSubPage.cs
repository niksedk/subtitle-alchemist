using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Features.Video.BurnIn;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.TransparentSubtitles;

public class TransparentSubPage : ContentPage
{
    public TransparentSubPage(TransparentSubPageModel vm)
    {
        BindingContext = vm;
        ThemeHelper.GetGridSelectionStyle();

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },  // title
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }, // help link
                new RowDefinition { Height = GridLength.Auto }, // progress bar
                new RowDefinition { Height = GridLength.Auto }, // buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }, // batch view
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = Se.Language.VideoTransparent.Title,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            FontSize = 18,
        }.BindDynamicThemeTextColorOnly();
        pageGrid.Add(labelTitle, 0);
        pageGrid.SetColumnSpan(labelTitle, 3);

        vm.FontPropertiesView = MakeFontPropertiesView(vm);
        pageGrid.Add(vm.FontPropertiesView, 0, 1);
        pageGrid.SetRowSpan(vm.FontPropertiesView, 2);

        vm.FontAssaView = MakeFontPropertiesAssaView(vm);
        pageGrid.Add(vm.FontAssaView, 0, 1);
        pageGrid.SetRowSpan(vm.FontAssaView, 2);

        var videoView = MakeVideoPropertiesView(vm);
        pageGrid.Add(videoView, 0, 3);

        pageGrid.Add(MakeCutPropertiesView(vm), 1, 1);
        pageGrid.Add(MakePreviewView(vm), 1, 2);
        pageGrid.Add(MakeAudioPropertiesView(vm), 1, 3);

        vm.BatchView = MakeBatchView(vm);
        pageGrid.Add(vm.BatchView, 2, 1);
        pageGrid.SetRowSpan(vm.BatchView, 3);

        var progressBar = new ProgressBar
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Progress = 0,
            ProgressColor = Colors.Orange,
            IsVisible = false,
        };
        progressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        vm.ProgressBar = progressBar;
        pageGrid.Add(progressBar, 0, 4);
        pageGrid.SetColumnSpan(progressBar, 3);


        var buttonGenerate = new Button
        {
            Text = "Generate",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.GenerateCommand,
        }.BindDynamicTheme();
        vm.ButtonGenerate = buttonGenerate;

        var buttonMode = new Button
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.ModeSwitchCommand,
        }.BindDynamicTheme();
        buttonMode.SetBinding(Button.TextProperty, nameof(vm.ButtonModeText));
        vm.ButtonMode = buttonMode;

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();
        vm.ButtonOk = buttonOk;

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var labelProgress = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(25, 0, 0, 0),
            FontSize = 16,
        }.BindDynamicThemeTextColorOnly();
        labelProgress.SetBinding(Label.TextProperty, nameof(vm.ProgressText));

        var buttonBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonGenerate,
                buttonMode,
                buttonOk,
                buttonCancel,
                labelProgress,
            },
        }.BindDynamicTheme();

        pageGrid.Add(buttonBar, 0, 5);
        pageGrid.SetColumnSpan(buttonBar, 3);

        var scrollView = new ScrollView
        {
            Content = pageGrid,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        Content = scrollView;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private static Border MakeFontPropertiesAssaView(TransparentSubPageModel vm)
    {
        var labelAssaInfo = new Label
        {
            Text = "Advanced Sub Station Alpha style from current subtitle will be used :)",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = labelAssaInfo,
        }.BindDynamicTheme();

        border.IsVisible = false;

        return border;
    }

    private static Border MakeFontPropertiesView(TransparentSubPageModel vm)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
        };

        var textWidth = 150;
        var controlWidth = 200;

        var labelFontName = new Label
        {
            Text = "Font name",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();

        var pickerFontName = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerFontName.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FontNames));
        pickerFontName.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFontName));
        pickerFontName.SelectedIndexChanged += vm.FontNameChanged;

        var stackFontName = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelFontName,
                pickerFontName,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackFontName);

        var labelFontFactor = new Label
        {
            Text = "Font size factor",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var entryFontFactor = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryFontFactor.SetBinding(Entry.TextProperty, nameof(vm.SelectedFontFactor));
        entryFontFactor.TextChanged += vm.FontFactorChanged;
        ToolTipProperties.SetText(entryFontFactor, "Set a factor between 0 and 1");

        var labelFontSize = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
            FontSize = 12,
        }.BindDynamicThemeTextColorOnly();
        labelFontSize.SetBinding(Label.TextProperty, nameof(vm.FontSizeText));

        var stackFontSize = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelFontFactor,
                entryFontFactor,
                labelFontSize,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackFontSize);

        var labelFontBold = new Label
        {
            Text = "Font bold",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var switchFontBold = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        switchFontBold.SetBinding(Switch.IsToggledProperty, nameof(vm.FontIsBold));
        switchFontBold.Toggled += vm.FontBoldToggled;
        var stackFontBold = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelFontBold,
                switchFontBold,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackFontBold);

        var labelTextColor = new Label
        {
            Text = "Text Color",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var boxViewTextColor = new BoxView
        {
            WidthRequest = 25,
            HeightRequest = 25,
        };
        boxViewTextColor.SetBinding(BoxView.ColorProperty, nameof(vm.FontTextColor));
        var tapGestureRecognizerTextColor = new TapGestureRecognizer();
        tapGestureRecognizerTextColor.Tapped += vm.FontTextColorTapped;
        boxViewTextColor.GestureRecognizers.Add(tapGestureRecognizerTextColor);

        var stackTextColor = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelTextColor,
                boxViewTextColor,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackTextColor);

        // Outline
        var labelOutline = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        labelOutline.SetBinding(Label.TextProperty, nameof(vm.FontOutlineText));

        var entryOutlineWidth = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryOutlineWidth.SetBinding(Entry.TextProperty, nameof(vm.SelectedFontOutline));
        entryOutlineWidth.TextChanged += vm.FontOutlineWidthChanged;
        var boxViewOutlineColor = new BoxView
        {
            WidthRequest = 25,
            HeightRequest = 25,
        };
        boxViewOutlineColor.SetBinding(BoxView.ColorProperty, nameof(vm.FontOutlineColor));
        var tapGestureRecognizerOutlineColor = new TapGestureRecognizer();
        tapGestureRecognizerOutlineColor.Tapped += vm.FontOutlineColorTapped;
        boxViewOutlineColor.GestureRecognizers.Add(tapGestureRecognizerOutlineColor);
        var stackOutline = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelOutline,
                entryOutlineWidth,
                boxViewOutlineColor,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackOutline);

        // Shadow
        var labelShadowColor = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        labelShadowColor.SetBinding(Label.TextProperty, nameof(vm.FontShadowText));
        var entryShadowWidth = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,

        }.BindDynamicTheme();
        entryShadowWidth.SetBinding(Entry.TextProperty, nameof(vm.SelectedFontShadowWidth));
        entryShadowWidth.TextChanged += vm.FontShadowWidthChanged;
        var boxViewShadowColor = new BoxView
        {
            WidthRequest = 25,
            HeightRequest = 25,
        };
        boxViewShadowColor.SetBinding(BoxView.ColorProperty, nameof(vm.FontShadowColor));
        var tapGestureRecognizerShadowColor = new TapGestureRecognizer();
        tapGestureRecognizerShadowColor.Tapped += vm.FontShadowColorTapped;
        boxViewShadowColor.GestureRecognizers.Add(tapGestureRecognizerShadowColor);
        var stackShadowColor = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelShadowColor,
                entryShadowWidth,
                boxViewShadowColor,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackShadowColor);

        // Box type
        var labelBoxType = new Label
        {
            Text = "Box type",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerBoxType = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerBoxType.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FontBoxTypes));
        pickerBoxType.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFontBoxType));
        pickerBoxType.SelectedIndexChanged += vm.FontBoxTypeChanged;
        var stackBoxType = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelBoxType,
                pickerBoxType,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackBoxType);

        var labelAlignRight = new Label
        {
            Text = "Alignment",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();

        var pickerAlignment = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerAlignment.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FontAlignments));
        pickerAlignment.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFontAlignment));

        var stackAlignRight = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelAlignRight,
                pickerAlignment,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackAlignRight);

        var labelMargin = new Label
        {
            Text = "Margin",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();

        var labelMarginHorizontal = new Label
        {
            Text = "Horizontal",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();

        var entryMarginHorizontal = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryMarginHorizontal.SetBinding(Entry.TextProperty, nameof(vm.FontMarginHorizontal));

        var labelMarginVertical = new Label
        {
            Text = "Vertical",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();

        var entryMarginVertical = new Entry
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryMarginVertical.SetBinding(Entry.TextProperty, nameof(vm.FontMarginVertical));

        var stackMargin = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelMargin,
                labelMarginHorizontal,
                entryMarginHorizontal,
                labelMarginVertical,
                entryMarginVertical,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackMargin);


        var labelFixRtl = new Label
        {
            Text = "Fix RTL",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var switchFixRtl = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        switchFixRtl.SetBinding(Switch.IsToggledProperty, nameof(vm.FontFixRtl));
        var stackFixRtl = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelFixRtl,
                switchFixRtl,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackFixRtl);

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = stack,
        }.BindDynamicTheme();

        return border;
    }

    private static IView MakeVideoPropertiesView(TransparentSubPageModel vm)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
        };

        var textWidth = 175;

        var labelResolution = new Label
        {
            Text = "Resolution",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var entryWidth = new Entry
        {
            Text = "1920",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryWidth.SetBinding(Entry.TextProperty, nameof(vm.VideoWidth));
        entryWidth.TextChanged += vm.VideoWidthChanged;
        vm.EntryWidth = entryWidth;
        var labelX = new Label
        {
            Text = "x",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
        }.BindDynamicThemeTextColorOnly();
        vm.LabelX = labelX;
        var entryHeight = new Entry
        {
            Text = "1080",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 75,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryHeight.SetBinding(Entry.TextProperty, nameof(vm.VideoHeight));
        entryHeight.TextChanged += vm.VideoHeightChanged;
        vm.EntryHeight = entryHeight;
        var buttonResolution = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Command = vm.PickResolutionCommand,
        }.BindDynamicTheme();
        vm.ButtonResolution = buttonResolution;

        var stackResolution = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelResolution,
                entryWidth,
                labelX,
                entryHeight,
                buttonResolution,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackResolution);

        var labelFrameRate = new Label
        {
            Text = "Frame rate",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerFrameRate = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerFrameRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FrameRates));
        pickerFrameRate.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedFrameRate));
        var stackFrameRate = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelFrameRate,
                pickerFrameRate,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackFrameRate);


        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = stack,
        }.BindDynamicTheme();

        return border;
    }

    private static IView MakeCutPropertiesView(TransparentSubPageModel vm)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
        };

        var textWidth = 120;

        var labelCutEnabled = new Label
        {
            Text = "Cut",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        var cutSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        cutSwitch.SetBinding(Switch.IsToggledProperty, nameof(vm.IsCutActive));
        var stackCutEnabled = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelCutEnabled,
                cutSwitch,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackCutEnabled);

        var labelFromTime = new Label
        {
            Text = "From time",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(25, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var subTimeUpDownFrom = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
        subTimeUpDownFrom.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.CutFrom));
        var buttonFromTime = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Command = vm.PickFromTimeCommand,
        }.BindDynamicTheme();
        buttonFromTime.SetBinding(Button.IsVisibleProperty, nameof(vm.IsSubtitleLoaded));
        ToolTipProperties.SetText(buttonFromTime, "Get position from subtitle line");
        var buttonFromVideoPosition = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Command = vm.PickFromVideoPositionCommand,
        }.BindDynamicTheme();
        ToolTipProperties.SetText(buttonFromVideoPosition, "Get position via video");

        var stackFromTime = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelFromTime,
                subTimeUpDownFrom,
                buttonFromTime,
                buttonFromVideoPosition,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackFromTime);

        var labelToTime = new Label
        {
            Text = "To time",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(25, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var subTimeUpDownTo = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
        subTimeUpDownTo.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.CutTo));
        var buttonToTime = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Command = vm.PickToTimeCommand,
        }.BindDynamicTheme();
        buttonToTime.SetBinding(Button.IsVisibleProperty, nameof(vm.IsSubtitleLoaded));
        ToolTipProperties.SetText(buttonToTime, "Get position from subtitle line");
        var buttonToVideoPosition = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            Command = vm.PickToVideoPositionCommand,
        }.BindDynamicTheme();
        ToolTipProperties.SetText(buttonToVideoPosition, "Get position from video");
        var stackToTime = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelToTime,
                subTimeUpDownTo,
                buttonToTime,
                buttonToVideoPosition,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackToTime);

        var imagePreview = new Image
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10),
        };
        vm.ImagePreview = imagePreview;
        stack.Children.Add(imagePreview);


        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = stack,
        }.BindDynamicTheme();

        return border;
    }

    private static IView MakePreviewView(TransparentSubPageModel vm)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
        };

        var labelCutEnabled = new Label
        {
            Text = "Preview",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicThemeTextColorOnly();
        stack.Children.Add(labelCutEnabled);

        var imagePreview = new Image
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(10),
        };
        vm.ImagePreview = imagePreview;
        stack.Children.Add(imagePreview);

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = stack,
        }.BindDynamicTheme();

        return border;
    }

    private static IView MakeAudioPropertiesView(TransparentSubPageModel vm)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
        };

        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = stack,
        }.BindDynamicTheme();

        return border;
    }

    private static Border MakeBatchView(TransparentSubPageModel vm)
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
            Padding = new Thickness(30, 20, 30, 10),
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
                new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // Subtitle file
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Size
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Resolution
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // Video file
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Status
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "Subtitle file",
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
                Text = "Resolution",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 2);
        gridHeader.Add(
            new Label
            {
                Text = "Video file",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 3);
        gridHeader.Add(
            new Label
            {
                Text = "Status",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 4);

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
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // Video file
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Resolution
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Size
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // Subtitle file
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Status
                    },
                };

                var labelVideoFile = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                }.BindDynamicThemeTextColorOnly();
                labelVideoFile.SetBinding(Label.TextProperty, nameof(BurnInJobItem.SubtitleFileName));
                jobItemGrid.Add(labelVideoFile, 0);

                var labelResolution = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelResolution.SetBinding(Label.TextProperty, nameof(BurnInJobItem.Size));
                jobItemGrid.Add(labelResolution, 1);

                var labelSize = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelSize.SetBinding(Label.TextProperty, nameof(BurnInJobItem.Resolution));
                jobItemGrid.Add(labelSize, 2);

                var labelSubtitleFile = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelSubtitleFile.SetBinding(Label.TextProperty, nameof(BurnInJobItem.InputVideoFileNameShort));
                jobItemGrid.Add(labelSubtitleFile, 3);

                var labelStatus = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelStatus.SetBinding(Label.TextProperty, nameof(BurnInJobItem.Status));
                jobItemGrid.Add(labelStatus, 4);

                return jobItemGrid;
            }),
        }.BindDynamicTheme();

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.JobItems), BindingMode.TwoWay);
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedJobItem));

        grid.Add(collectionView, 0, 1);


        var buttonAdd = new Button
        {
            Text = "Add...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchAddCommand,
        }.BindDynamicTheme();

        var buttonRemove = new Button
        {
            Text = "Remove",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchRemoveCommand,
        }.BindDynamicTheme();

        var buttonClear = new Button
        {
            Text = "Clear",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchClearCommand,
        }.BindDynamicTheme();

        var buttonPickSubtitleFile = new Button
        {
            Text = "Pick video file...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchPickVideoFileCommand,
        }.BindDynamicTheme();

        var buttonOutputProperties = new Button
        {
            Text = "Output properties...",
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
        labelOutputUseSource.SetBinding(IsVisibleProperty, nameof(vm.UseSourceFolderVisible));

        var labelOutputFolder = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicThemeTextColorOnly();
        labelOutputFolder.SetBinding(Label.TextProperty, nameof(vm.OutputSourceFolder));
        labelOutputFolder.SetBinding(IsVisibleProperty, nameof(vm.UseOutputFolderVisible));

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEntered += vm.OutputFolderLinkMouseEntered;
        pointerGesture.PointerExited += vm.OutputFolderLinkMouseExited;
        labelOutputFolder.GestureRecognizers.Add(pointerGesture);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += vm.OutputFolderLinkMouseClicked;
        labelOutputFolder.GestureRecognizers.Add(tapGesture);
        vm.LabelOutputFolder = labelOutputFolder;

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
                buttonPickSubtitleFile,
                buttonOutputProperties,
                labelOutputUseSource,
                labelOutputFolder,
            },
        }.BindDynamicTheme();

        grid.Add(stackButtons, 0, 2);


        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(15),
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
}
