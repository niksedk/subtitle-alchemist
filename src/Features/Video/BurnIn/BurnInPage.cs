using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.BurnIn;

public class BurnInPage : ContentPage
{
    public BurnInPage(BurnInPageModel vm)
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
            Text = Se.Language.BurnIn.Title,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            FontSize = 18,
        }.BindDynamicThemeTextColorOnly();
        pageGrid.Add(labelTitle, 0);
        pageGrid.SetColumnSpan(labelTitle, 3);

        var textView = MakeTextPropertiesView(vm);
        pageGrid.Add(textView, 0, 1);
        pageGrid.SetRowSpan(textView, 2);

        var videoView = MakeVideoPropertiesView(vm);
        pageGrid.Add(videoView, 0, 3);

        pageGrid.Add(MakeCutPropertiesView(vm), 1, 1);
        pageGrid.Add(MakePreviewView(vm), 1, 2);
        pageGrid.Add(MakeAudioPropertiesView(vm), 1, 3);
        pageGrid.Add(MakeTargetFilePropertiesView(vm), 0, 4);
        pageGrid.Add(MakeVideoPlayerView(vm), 1, 4);

        vm.BatchView = MakeBatchView(vm);
        pageGrid.Add(vm.BatchView, 0, 4);
        pageGrid.SetColumnSpan(vm.BatchView, 2);

        var labelHelp = new Label
        {
            Text = "Help",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicThemeTextColorOnly();
        var tapGestureRecognizerHelp = new TapGestureRecognizer();
        tapGestureRecognizerHelp.Tapped += vm.HelpTapped;
        labelHelp.GestureRecognizers.Add(tapGestureRecognizerHelp);
        var pointerGestureHelp = new PointerGestureRecognizer();
        pointerGestureHelp.PointerEntered += vm.LabelHelpMouseEntered;
        pointerGestureHelp.PointerExited += vm.LabelHelpMouseExited;
        labelHelp.GestureRecognizers.Add(pointerGestureHelp);
        vm.LabelHelp = labelHelp;

        pageGrid.Add(labelHelp, 0, 5);
        pageGrid.SetColumnSpan(labelHelp, 2);

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
        pageGrid.Add(progressBar, 0, 6);
        pageGrid.SetColumnSpan(progressBar, 2);


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

        pageGrid.Add(buttonBar, 0, 7);

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

    private static IView MakeTextPropertiesView(BurnInPageModel vm)
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
            Text = "Font factor",
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

    private static IView MakeVideoPropertiesView(BurnInPageModel vm)
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
        var controlWidth = 300;


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
        var labelX = new Label
        {
            Text = "x",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
        }.BindDynamicThemeTextColorOnly();
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
        var buttonResolution = new Button
        {
            Text = "...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            //Command = vm.GetResolutionCommand,
        }.BindDynamicTheme();

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

        var labelEncoding = new Label
        {
            Text = "Encoding",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerEncoding = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerEncoding.SetBinding(Picker.ItemsSourceProperty, nameof(vm.VideoEncodings));
        pickerEncoding.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVideoEncoding));
        pickerEncoding.SelectedIndexChanged += vm.VideoEncodingChanged;

        var stackEncoding = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelEncoding,
                pickerEncoding,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackEncoding);

        var labelPreset = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        labelPreset.SetBinding(Label.TextProperty, nameof(vm.VideoPresetText));
        var pickerPreset = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerPreset.SetBinding(Picker.ItemsSourceProperty, nameof(vm.VideoPresets));
        pickerPreset.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVideoPreset));
        var stackPreset = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelPreset,
                pickerPreset,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackPreset);

        var labelCrf = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        labelCrf.SetBinding(Label.TextProperty, nameof(vm.VideoCrfText));
        var pickerCrf = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 100,
        }.BindDynamicTheme();
        pickerCrf.SetBinding(Picker.ItemsSourceProperty, nameof(vm.VideoCrf));
        pickerCrf.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVideoCrf));
        var labelCrfHint = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 10,
        }.BindDynamicThemeTextColorOnly();
        labelCrfHint.SetBinding(Label.TextProperty, nameof(vm.VideoCrfHint));
        var stackCrf = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelCrf,
                pickerCrf,
                labelCrfHint,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackCrf);


        var labelPixelFormat = new Label
        {
            Text = "Pixel format",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerPixelFormat = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerPixelFormat.SetBinding(Picker.ItemsSourceProperty, nameof(vm.VideoPixelFormats));
        var stackPixelFormat = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelPixelFormat,
                pickerPixelFormat,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackPixelFormat);


        var labelTuneFor = new Label
        {
            Text = "Tune for",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerTuneFor = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = controlWidth,
        }.BindDynamicTheme();
        pickerTuneFor.SetBinding(Picker.ItemsSourceProperty, nameof(vm.VideoTuneFor));
        pickerTuneFor.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVideoTuneFor));
        var stackTuneFor = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelTuneFor,
                pickerTuneFor,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackTuneFor);


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

    private static IView MakeCutPropertiesView(BurnInPageModel vm)
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
        var subTimeUpDown = new SubTimeUpDown()
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
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
                subTimeUpDown,
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
        var subTimeUpDownTo = new SubTimeUpDown()
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
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

    private static IView MakePreviewView(BurnInPageModel vm)
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

    private static IView MakeAudioPropertiesView(BurnInPageModel vm)
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

        var labelEncoding = new Label
        {
            Text = "Encoding",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerEncoding = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
        pickerEncoding.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AudioEncodings));
        pickerEncoding.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAudioEncoding));
        var stackPicker = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelEncoding,
                pickerEncoding,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackPicker);


        var labelStereo = new Label
        {
            Text = "Stereo",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var switchStereo = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        switchStereo.SetBinding(Switch.IsToggledProperty, nameof(vm.AudioIsStereo));
        var stackStereo = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelStereo,
                switchStereo,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackStereo);


        var labelSampleRate = new Label
        {
            Text = "Sample rate",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerSampleRate = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
        pickerSampleRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AudioSampleRates));
        pickerSampleRate.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAudioSampleRate));
        var stackSampleRate = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelSampleRate,
                pickerSampleRate,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackSampleRate);

        var labelBitRate = new Label
        {
            Text = "Bit rate",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var pickerBitRate = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = textWidth,
        }.BindDynamicTheme();
        pickerBitRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AudioBitRates));
        pickerBitRate.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAudioBitRate));
        var stackBitRate = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelBitRate,
                pickerBitRate,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackBitRate);


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

    private IView MakeTargetFilePropertiesView(BurnInPageModel vm)
    {
        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
        };

        var textWidth = 250;


        var labelTargetFileSize = new Label
        {
            Text = "Target File Size (requires 2 pass encoding)",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var switchTargetFileSize = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        switchTargetFileSize.SetBinding(Switch.IsToggledProperty, nameof(vm.UseTargetFileSize));
        var stackTargetFileSize = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelTargetFileSize,
                switchTargetFileSize,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackTargetFileSize);

        var labelTargetFileSizeValue = new Label
        {
            Text = "File size (MB)",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = 16,
            WidthRequest = textWidth,
        }.BindDynamicThemeTextColorOnly();
        var entryTargetFileSizeValue = new Entry
        {
            Text = "100",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 120,
            Keyboard = Keyboard.Numeric,
        }.BindDynamicTheme();
        entryTargetFileSizeValue.SetBinding(Entry.TextProperty, nameof(vm.TargetFileSize));
        var stackTargetFileSizeValue = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 0),
            Spacing = 5,
            Children =
            {
                labelTargetFileSizeValue,
                entryTargetFileSizeValue,
            },
        }.BindDynamicTheme();
        stack.Children.Add(stackTargetFileSizeValue);


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

    private IView MakeVideoPlayerView(BurnInPageModel vm)
    {
        vm.VideoPlayer = new MediaElement { ZIndex = -10000 };

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
            Content = vm.VideoPlayer,
        }.BindDynamicTheme();

        return border;
    }

    private static Border MakeBatchView(BurnInPageModel vm)
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
                new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }, // Video file
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Resolution
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Size
                new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }, // Subtitle file
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Status
            },
        };

        // Add headers
        gridHeader.Add(
            new Label
            {
                Text = "Video file",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 0);
        gridHeader.Add(
            new Label
            {
                Text = "Resolution",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 1);
        gridHeader.Add(
            new Label
            {
                Text = "Size",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            }, 2);
        gridHeader.Add(
            new Label
            {
                Text = "Subtitle file",
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
        grid.Add(gridHeader, 0, 0);


        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 250,
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
                labelVideoFile.SetBinding(Label.TextProperty, nameof(BurnInJobItem.InputVideoFileNameShort));
                jobItemGrid.Add(labelVideoFile, 0, 0);

                var labelResolution = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelResolution.SetBinding(Label.TextProperty, nameof(BurnInJobItem.Resolution));
                jobItemGrid.Add(labelResolution, 1, 0);

                var labelSize = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelSize.SetBinding(Label.TextProperty, nameof(BurnInJobItem.Size));
                jobItemGrid.Add(labelSize, 2, 0);

                var labelSubtitleFile = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelSubtitleFile.SetBinding(Label.TextProperty, nameof(BurnInJobItem.SubtitleFileNameShort));
                jobItemGrid.Add(labelSubtitleFile, 3, 0);

                var labelStatus = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }.BindDynamicThemeTextColorOnly();
                labelStatus.SetBinding(Label.TextProperty, nameof(BurnInJobItem.Status));
                jobItemGrid.Add(labelStatus, 4, 0);

                return jobItemGrid;
            }),
        }.BindDynamicTheme();

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.JobItems), BindingMode.TwoWay);
        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty, nameof(vm.SelectedJobItem));
        //collectionView.BindingContext = vm;

        grid.Add(collectionView, 0, 1);


        var buttonAdd = new Button
        {
            Text = "Add",
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
            Text = "Pick subtitle file",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.BatchPickSubtitleFileCommand,
        }.BindDynamicTheme();

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

    public void Initialize(Subtitle subtitle, BurnInPageModel vm)
    {

    }
}
