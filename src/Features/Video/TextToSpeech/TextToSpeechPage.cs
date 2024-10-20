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
            Margin = new Thickness(0, 0, 0, 0),
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
        var stackEngine = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0,0,0,10),
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
            Margin = new Thickness(0, 0, 0, 00),
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
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        labelVoiceCount.SetBinding(Label.TextProperty, nameof(vm.VoiceCount));
        var stackVoice = new StackLayout
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
            Margin = new Thickness(5, 0, 0, 0),
            Command = vm.TestVoiceCommand,
        }.BindDynamicTheme();
        var stackVoiceTest = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
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
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 5, 0),
            MinimumWidthRequest = 90,
        }.BindDynamicTheme();
        var pickerLanguage = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        pickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages));
        pickerLanguage.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedLanguage));
        var stackLanguage = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                labelLanguage,
                pickerLanguage,
            },
        }.BindDynamicTheme();
        stackLanguage.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.HasLanguageParameter));

        var labelApiKey = new Label
        {
            Text = "API Key",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 5, 0),
            MinimumWidthRequest = 90,
        }.BindDynamicTheme();
        var entryApiKey = new Entry
        {
            Placeholder = "Enter API Key",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        entryApiKey.SetBinding(Entry.TextProperty, nameof(vm.ApiKey));

        var stackApiKey = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                labelApiKey,
                entryApiKey,
            },
        }.BindDynamicTheme();
        stackApiKey.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.HasApiKey));

        var labelModel = new Label
        {
            Text = "Model",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 5, 0),
            MinimumWidthRequest = 90,
        }.BindDynamicTheme();
        var pickerModel = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        pickerModel.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Models));
        pickerModel.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedModel));
        pickerModel.SelectedIndexChanged += vm.SelectedModelChanged;
        var stackModel = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                labelModel,
                pickerModel,
            },
        }.BindDynamicTheme();
        stackModel.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.HasModel));

        var labelRegion = new Label
        {
            Text = "Region",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 5, 0),
            MinimumWidthRequest = 90,
        }.BindDynamicTheme();
        var pickerRegion = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        pickerRegion.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Regions));
        pickerRegion.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedRegion));
        var stackRegion = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                labelRegion,
                pickerRegion,
            },
        }.BindDynamicTheme();
        stackRegion.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.HasRegion));


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
            },
        }.BindDynamicTheme();

        var labelReviewAudioClips = new Label
        {
            Text = "Review audio segments",
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
            Text = "Add audio to video file",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 10, 0),
        }.BindDynamicTheme();
        gridSwitch.Add(labelAddAudioToVideoFile, 0, 1);

        var switchAddAudioToVideoFile = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15, 0, 0),
        }.BindDynamicTheme();
        switchAddAudioToVideoFile.SetBinding(Switch.IsToggledProperty, nameof(vm.DoGenerateVideoFile));
        gridSwitch.Add(switchAddAudioToVideoFile, 1, 1);

        var labelCustomAudioEncoding = new Label
        {
            Text = "Custom audio encoding",
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 0, 10, 0),
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicTheme();

        var switchCustomAudioEncoding = new Switch
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();
        switchCustomAudioEncoding.SetBinding(Switch.IsToggledProperty, nameof(vm.UseCustomAudioEncoding));

        var customAudioEncodingStack = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
            Children =
            {
                labelCustomAudioEncoding,
                switchCustomAudioEncoding,
            },
        }.BindDynamicTheme();
        gridSwitch.Add(customAudioEncodingStack, 0, 2);
        gridSwitch.SetColumnSpan(customAudioEncodingStack, 2);


        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEntered += vm.LabelAudioEncodingSettingsMouseEntered;
        pointerGesture.PointerExited += vm.LabelAudioEncodingSettingsMouseExited;
        labelCustomAudioEncoding.GestureRecognizers.Add(pointerGesture);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += vm.LabelAudioEncodingSettingsMouseClicked;
        labelCustomAudioEncoding.GestureRecognizers.Add(tapGesture);
        vm.LabelAudioEncodingSettings = labelCustomAudioEncoding;


        var buttonGenerateSpeechFromText = new Button
        {
            Text = "Generate speech from text",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 10, 10),
            Command = vm.GenerateTtsCommand,
        }.BindDynamicTheme();
        
        var buttonImport = new Button
        {
            Text = "Import...",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 10),
            Command = vm.ImportCommand,
        }.BindDynamicTheme();

        var buttonDoneOrCancel = new Button
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.DoneOrCancelCommand,
        }.BindDynamicTheme();
        buttonDoneOrCancel.SetBinding(Button.TextProperty, nameof(vm.DoneOrCancelText));

        vm.Player.WidthRequest = 1;
        vm.Player.HeightRequest = 1;

        var stackButtons = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonGenerateSpeechFromText,
                buttonImport,
                buttonDoneOrCancel,
                vm.Player,
            },
        }.BindDynamicTheme();

        var labelProgressText = new Label
        {
            FontSize = 16,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 2),
        }.BindDynamicTheme();
        labelProgressText.SetBinding(Label.TextProperty, nameof(vm.ProgressText));

        var progressBar = new ProgressBar
        {
            Progress = 0.0,
            ProgressColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Fill,
        };
        progressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));

        var stackProgress = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
            Children =
            {
                labelProgressText,
                progressBar,
            },
        }.BindDynamicTheme();
        stackProgress.SetBinding(StackLayout.IsVisibleProperty, nameof(vm.IsGenerating));

        var pageStack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(25),
            Children =
            {
                labelTitle,
                stackEngine,
                stackVoice,
                stackVoiceTest,
                stackModel,
                stackLanguage,
                stackRegion,
                stackApiKey,
                gridSwitch,
                stackProgress,
                stackButtons,
            },
        }.BindDynamicTheme();

        Content = pageStack;

        this.BindDynamicTheme();

        vm.Page = this;
    }
}
