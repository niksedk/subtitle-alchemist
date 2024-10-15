using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public class TextToSpeechPage : ContentPage
{
    public TextToSpeechPage(TextToSpeechPageModel vm)
    {
        BindingContext = vm;
        ThemeHelper.GetGridSelectionStyle();

        var labelTitle = new Label
        {
            Text = "Text to Speech",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 35),
        }.BindDynamicTheme();


        var labelEngine = new Label
        {
            Text = "Engine",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 0, 10),
            WidthRequest = 100,
        }.BindDynamicTheme();
        var pickerEngine = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        pickerEngine.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Engines));
        pickerEngine.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedEngine));
        pickerEngine.SelectedIndexChanged += vm.SelectedEngineChanged;
        var engineStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0,0,0,20),
            Children =
            {
                labelEngine,
                pickerEngine,
            },
        }.BindDynamicTheme();


        var labelVoice = new Label
        {
            Text = "Voice",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 0, 10),
            WidthRequest = 100,
        }.BindDynamicTheme();
        var pickerVoice = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        pickerVoice.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Voices));
        pickerVoice.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVoice));
        var labelVoiceCount = new Label
        {
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
        }.BindDynamicTheme();
        labelVoiceCount.SetBinding(Label.TextProperty, nameof(vm.VoiceCount));
        var voiceStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                labelVoice,
                pickerVoice,
                labelVoiceCount,
            },
        }.BindDynamicTheme();

        var entryTestVoice = new Entry
        {
            Placeholder = "Enter sample text",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 200,
        }.BindDynamicTheme();
        entryTestVoice.SetBinding(Entry.TextProperty, nameof(vm.VoiceTestText));
        var buttonTestVoice = new Button
        {
            Text = "Test Voice",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0),
            Command = vm.TestVoiceCommand,
        }.BindDynamicTheme();
        var voiceTestStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                entryTestVoice,
                buttonTestVoice,
            },
        }.BindDynamicTheme();

        var labelLanguage = new Label
        {
            Text = "Language",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        var languageStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 25),
            Children =
            {
                labelLanguage,
                pickerLanguage,
            },
        }.BindDynamicTheme();
        languageStack.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.HasLanguageParameter));

        
        var gridSwitch = new Grid
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 40, 0, 20),
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
        }.BindDynamicTheme();

        var labelReviewAudioClips = new Label
        {
            Text = "Review Audio Clips",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        gridSwitch.Add(labelReviewAudioClips, 0);

        var switchReviewAudioClips = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        switchReviewAudioClips.SetBinding(Switch.IsToggledProperty, nameof(vm.DoReviewAudioClips));
        gridSwitch.Add(switchReviewAudioClips, 1);

        var labelAddAudioToVideoFile = new Label
        {
            Text = "Add Audio to Video File",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        gridSwitch.Add(labelAddAudioToVideoFile, 0, 1);

        var switchAddAudioToVideoFile = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        switchAddAudioToVideoFile.SetBinding(Switch.IsToggledProperty, nameof(vm.DoGenerateVideoFile));
        gridSwitch.Add(switchAddAudioToVideoFile, 1, 1);

        var labelCustomAudioEncoding = new Label
        {
            Text = "Custom Audio Encoding",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();
        gridSwitch.Add(labelCustomAudioEncoding, 0, 2);

        var switchCustomAudioEncoding = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        switchCustomAudioEncoding.SetBinding(Switch.IsToggledProperty, nameof(vm.UseCustomAudioEncoding));
        gridSwitch.Add(switchCustomAudioEncoding, 1, 2);

        var labelAudioEncodingSettings = new Label
        {
            Text = "Audio Encoding Settings",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicTheme();
        gridSwitch.Add(labelAudioEncodingSettings, 2,2);

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEntered += vm.LabelAudioEncodingSettingsMouseEntered;
        pointerGesture.PointerExited += vm.LabelAudioEncodingSettingsMouseExited;
        labelAudioEncodingSettings.GestureRecognizers.Add(pointerGesture);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += vm.LabelAudioEncodingSettingsMouseClicked;
        labelAudioEncodingSettings.GestureRecognizers.Add(tapGesture);
        vm.LabelAudioEncodingSettings = labelAudioEncodingSettings;


        var buttonGenerateSpeechFromText = new Button
        {
            Text = "Generate Speech from Text",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.GenerateTtsCommand,
        }.BindDynamicTheme();
        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();
        vm.Player.WidthRequest = 1;
        vm.Player.HeightRequest = 1;
        var buttonBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonGenerateSpeechFromText,
                buttonCancel,
                vm.Player,
            },
        }.BindDynamicTheme();

        var pageStack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(25),
            Children =
            {
                labelTitle,
                engineStack,
                voiceStack,
                voiceTestStack,
                languageStack,
                gridSwitch,
                buttonBar,
            },
        }.BindDynamicTheme();

        Content = pageStack;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    public void Initialize(Subtitle subtitle, TextToSpeechPageModel vm)
    {

    }
}
