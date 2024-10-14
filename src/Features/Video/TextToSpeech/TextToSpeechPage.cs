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
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();


        var labelEngine = new Label
        {
            Text = "Engine",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var pickerEngine = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        pickerEngine.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Engines));
        pickerEngine.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedEngine));
        pickerEngine.SelectedIndexChanged += vm.SelectedEngineChanged;
        var engineStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
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
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var pickerVoice = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        pickerVoice.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Voices));
        pickerVoice.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedVoice));
        var labelVoiceCount = new Label
        {
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        labelVoiceCount.SetBinding(Label.TextProperty, nameof(vm.VoiceCount));
        var voiceStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
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
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 200,
        }.BindDynamicTheme();
        entryTestVoice.SetBinding(Entry.TextProperty, nameof(vm.VoiceTestText));
        var buttonTestVoice = new Button
        {
            Text = "Test Voice",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.TestVoiceCommand,
        }.BindDynamicTheme();
        var voiceTestStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
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
            Children =
            {
                labelLanguage,
                pickerLanguage,
            },
        }.BindDynamicTheme();
        languageStack.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.HasLanguageParameter));

        var labelReviewAudioClips = new Label
        {
            Text = "Review Audio Clips",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var switchReviewAudioClips = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        switchReviewAudioClips.SetBinding(Switch.IsToggledProperty, nameof(vm.DoReviewAudioClips));
        var reviewAudioClipsStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                labelReviewAudioClips,
                switchReviewAudioClips,
            },
        }.BindDynamicTheme();

        var labelAddAudioToVideoFile = new Label
        {
            Text = "Add Audio to Video File",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var switchAddAudioToVideoFile = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        switchAddAudioToVideoFile.SetBinding(Switch.IsToggledProperty, nameof(vm.DoGenerateVideoFile));
        var addAudioToVideoFileStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                labelAddAudioToVideoFile,
                switchAddAudioToVideoFile,
            },
        }.BindDynamicTheme();

        var labelCustomAudioEncoding = new Label
        {
            Text = "Custom Audio Encoding",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var switchCustomAudioEncoding = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();
        switchCustomAudioEncoding.SetBinding(Switch.IsToggledProperty, nameof(vm.UseCustomAudioEncoding));
        var labelAudioEncodingSettings = new Label
        {
            Text = "Audio Encoding Settings",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 10, 0, 0),
        }.BindDynamicTheme();
        var stackAudioEncodingSettings = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                labelCustomAudioEncoding,
                switchCustomAudioEncoding,
                labelAudioEncodingSettings,
            },
        }.BindDynamicTheme();


        var buttonGenerateSpeechFromText = new Button
        {
            Text = "Generate Speech from Text",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 15, 10),
            //Command = vm.GenerateSpeechFromTextCommand,
        }.BindDynamicTheme();
        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();
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
                reviewAudioClipsStack,
                addAudioToVideoFileStack,
                stackAudioEncodingSettings,
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
