using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public class AudioToTextWhisperPage : ContentPage
{
    private readonly AudioToTextWhisperModel _vm;

    public AudioToTextWhisperPage(AudioToTextWhisperModel vm)
    {
        this.BindDynamicTheme();
        vm.Page = this;
        _vm = vm;
        BindingContext = vm;

        var grid = new Grid
        {
            Margin = new Thickness(10, 10, 10, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
        };

        var poweredByLabel = new Label
        {
            Margin = new Thickness(15, 15, 0, 15),
            Text = "Powered by ",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 12,
        }.BindDynamicTheme();

        vm.TitleLabel = new Label
        {
            Margin = new Thickness(5, 15, 15, 15),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 12,
            TextDecorations = TextDecorations.Underline,
        }.BindDynamicTheme();

        var titleTexts = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                poweredByLabel,
                vm.TitleLabel,
            }
        };

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEnteredCommand = new Command(vm.MouseEnteredPoweredBy);
        pointerGesture.PointerExitedCommand = new Command(vm.MouseExitedPoweredBy);
        vm.TitleLabel.GestureRecognizers.Add(pointerGesture);
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += vm.MouseClickedPoweredBy;
        vm.TitleLabel.GestureRecognizers.Add(tapGesture);

        grid.Add(titleTexts, 0, 0);
        Grid.SetColumnSpan(titleTexts, 2);

        vm.PickerEngine = new Picker
        {
            Margin = new Thickness(15),
            Title = "Engine",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.PickerEngine.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedWhisperEngine), BindingMode.TwoWay);
        vm.PickerEngine.SelectedIndexChanged += vm.PickerEngine_SelectedIndexChanged;

        vm.PickerLanguage = new Picker
        {
            Margin = new Thickness(15),
            Title = "Language",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.PickerLanguage.SetBinding(Picker.ItemsSourceProperty, nameof(vm.Languages), BindingMode.TwoWay);

        vm.PickerModel = new Picker
        {
            Margin = new Thickness(15),
            Title = "Model",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.PickerModel.SetBinding(Picker.ItemsSourceProperty, new Binding(nameof(vm.Models)));

        vm.ButtonModel = new Button
        {
            Margin = new Thickness(15),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Text = "...",
            Command = vm.DownloadModelCommand,
        }.BindDynamicTheme();

        var pickerBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                vm.PickerEngine,
                vm.PickerLanguage,
                vm.PickerModel,
                vm.ButtonModel,
            }
        };

        grid.Add(pickerBar, 0, 1);
        grid.SetColumnSpan(pickerBar, 2);

        var labelTranslateToEnglish = new Label
        {
            Text = "Translate to English",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 5, 15, 5),
        }.BindDynamicTheme();
        grid.Add(labelTranslateToEnglish, 0, 2);

        vm.SwitchTranslateToEnglish = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        grid.Add(vm.SwitchTranslateToEnglish, 1, 2);



        var labelAdjustTimings = new Label
        {
            Text = "Adjust timings",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 5, 15, 5),
        }.BindDynamicTheme();
        grid.Add(labelAdjustTimings, 0, 3);

        vm.SwitchAdjustTimings = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        grid.Add(vm.SwitchAdjustTimings, 1, 3);



        var labelPostProcessing = new Label
        {
            Text = "Post-processing",
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 5, 15, 5),
        }.BindDynamicTheme();
        grid.Add(labelPostProcessing, 0, 4);

        vm.SwitchPostProcessing = new Switch
        {
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        grid.Add(vm.SwitchPostProcessing, 1, 4);


        var buttonAdvancedSettings = new Button
        {
            VerticalOptions = LayoutOptions.Center,
            Command = vm.ShowAdvancedWhisperSettingsCommand,
            Text = "Advanced settings",
        }.BindDynamicTheme();


        vm.LabelAdvancedSettings = new Label
        {
            Text = Configuration.Settings.Tools.WhisperExtraSettings,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(15, 5, 15, 5),
        }.BindDynamicTheme();

        var advancedBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                buttonAdvancedSettings,
                vm.LabelAdvancedSettings,
            }
        };

        grid.Add(advancedBar, 0, 5);
        grid.SetColumnSpan(advancedBar, 2);

        vm.PickerEngine.ItemsSource = vm.WhisperEngines;
        vm.PickerEngine.SetBinding(Picker.SelectedItemProperty, new Binding(nameof(vm.SelectedWhisperEngine), BindingMode.TwoWay));


        vm.ProgressBar = new ProgressBar
        {
            Progress = 0.5,
            ProgressColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Fill,
        };
        vm.ProgressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        grid.Add(vm.ProgressBar, 0, 6);
        grid.SetColumnSpan(vm.ProgressBar, 2);

        vm.LabelProgress = new Label
        {
            Text = string.Empty,
            FontAttributes = FontAttributes.Bold,
        }.BindDynamicTheme();
        grid.Add(vm.LabelProgress, 0, 7);
        grid.SetColumnSpan(vm.LabelProgress, 2);


        var transcribeButton = new Button
        {
            Text = "Transcribe",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.TranscribeCommand,
            Margin = new Thickness(5, 15, 5, 5),
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
            Margin = new Thickness(5, 15, 5, 5),
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                transcribeButton,
                cancelButton,
            }
        };

        grid.Add(buttonBar, 0, 8);
        grid.SetColumnSpan(buttonBar, 2);

        Content = grid;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var engine = _vm.WhisperEngines.FirstOrDefault(e => e.Choice == Configuration.Settings.Tools.WhisperChoice);
                if (engine != null)
                {
                    _vm.SelectedWhisperEngine = engine;
                    _vm.PickerEngine.SelectedItem = engine;
                }
                else
                {
                    _vm.SelectedWhisperEngine = _vm.WhisperEngines.FirstOrDefault();
                    _vm.PickerEngine.SelectedItem = _vm.WhisperEngines.FirstOrDefault();
                }

                _vm.SetDefaultLanguageAndModel();
            });

            return false;
        });
    }
}