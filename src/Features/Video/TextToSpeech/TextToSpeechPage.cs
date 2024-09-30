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
        var buttonTestVoice = new Button
        {
            Text = "Test Voice",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            //Command = vm.TestVoiceCommand,
        }.BindDynamicTheme();
        var voiceStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Children =
            {
                labelVoice,
                pickerVoice,
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
