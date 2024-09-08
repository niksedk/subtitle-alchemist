using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

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
            RowDefinitions = new()
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            ColumnDefinitions = new()
            {
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Auto) },
                new() { Width = new GridLength(1, GridUnitType.Star) },
            },
        };

        MakeConsoleLogWindow(grid, vm);

        MakeContextMenu(vm);

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
            Margin = new Thickness(5),
            Title = "Model",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        vm.PickerModel.SetBinding(Picker.ItemsSourceProperty, new Binding(nameof(vm.Models)));

        vm.ButtonModel = new Button
        {
            Margin = new Thickness(5),
            VerticalOptions = LayoutOptions.End,
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
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();

        vm.LinkLabelProcessingSettings = new Label
        {
            Text = "Post-processing settings",
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();

        var pgPp = new PointerGestureRecognizer();
        pgPp.PointerEnteredCommand = new Command(vm.MouseEnteredPostProcessingSettings);
        pgPp.PointerExitedCommand = new Command(vm.MouseExitedProcessingSettings);
        vm.LinkLabelProcessingSettings.GestureRecognizers.Add(pgPp);
        var tgPp = new TapGestureRecognizer();
        tgPp.Tapped += async (o, args) => await vm.MouseClickedProcessingSettings(o, args);
        vm.LinkLabelProcessingSettings.GestureRecognizers.Add(tgPp);

        var processingBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(0, 0, 0, 0),
            Children =
            {
                vm.SwitchPostProcessing,
                vm.LinkLabelProcessingSettings,
            }
        };

        grid.Add(processingBar, 1, 4);

        var labelAdvancedSettings = new Label
        {
            Text = "Advanced settings",
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(15, 10, 10, 10),
        }.BindDynamicTheme();
        grid.Add(labelAdvancedSettings, 0, 5);

        var buttonAdvancedSettings = new Button
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Command = vm.ShowAdvancedWhisperSettingsCommand,
            Text = "...",
            Margin = new Thickness(0, 0, 0, 0),
        }.BindDynamicTheme();

        vm.LabelAdvancedSettings = new Label
        {
            Text = Se.Settings.Tools.AudioToText.WhisperCustomCommandLineArguments,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 5, 0, 25),
            FontSize = 10,
            FontFamily = "RobotoMono",
        }.BindDynamicTheme();

        var advancedBar = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Margin = new Thickness(0, 0, 0, 25),
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Children =
            {
                buttonAdvancedSettings,
                vm.LabelAdvancedSettings,
            }
        };

        grid.Add(advancedBar, 1, 5);
        

        vm.PickerEngine.ItemsSource = vm.WhisperEngines;
        vm.PickerEngine.SetBinding(Picker.SelectedItemProperty, new Binding(nameof(vm.SelectedWhisperEngine), BindingMode.TwoWay));


        vm.ProgressBar = new ProgressBar
        {
            Progress = 0.5,
            ProgressColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(15, 2, 15, 2),
            IsVisible = false
        };
        vm.ProgressBar.SetBinding(ProgressBar.ProgressProperty, nameof(vm.ProgressValue));
        grid.Add(vm.ProgressBar, 0, 6);
        grid.SetColumnSpan(vm.ProgressBar, 2);

        vm.LabelProgress = new Label
        {
            Text = string.Empty,
            IsVisible = false,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(15, 2, 2, 2),
            FontSize = 16,
        }.BindDynamicTheme();

        var labelElapsedText = new Label
        {
            Text = "Time elapsed: ",
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(15, 10, 2, 2),
            FontSize = 16,
        }.BindDynamicTheme();
        labelElapsedText.SetBinding(Label.TextProperty, nameof(vm.ElapsedText));

        var labelEstimatedText = new Label
        {
            Text = "Time estimated: ",
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(15, 2, 2, 2),
            FontSize = 16,
        }.BindDynamicTheme();
        labelEstimatedText.SetBinding(Label.TextProperty, nameof(vm.EstimatedText));

        var progressLine = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
            Children =
            {
                vm.LabelProgress,
                labelElapsedText,
                labelEstimatedText,
            }
        };

        grid.Add(progressLine, 0, 7);
        grid.SetColumnSpan(progressLine, 2);


        vm.TranscribeButton = new Button
        {
            Text = "Transcribe",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.TranscribeCommand,
            Margin = new Thickness(5, 5, 5, 5),
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
            Margin = new Thickness(5, 5, 5, 5),
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                vm.TranscribeButton,
                cancelButton,
            }
        };

        grid.Add(buttonBar, 0, 8);
        grid.SetColumnSpan(buttonBar, 2);

        Content = grid;
    }

    private void MakeContextMenu(AudioToTextWhisperModel vm)
    {
        var menuFlyoutMain = new MenuFlyout();
        var flyoutItem = new MenuFlyoutItem();
        flyoutItem.Text = "Show whisper log";
        flyoutItem.Command = vm.ShowWhisperLogCommand;
        menuFlyoutMain.Add(flyoutItem);

        var separator = new MenuFlyoutSeparator();
        menuFlyoutMain.Add(separator);

        // Add reDownload Whisper
        flyoutItem = new MenuFlyoutItem();
        flyoutItem.Command = vm.DownloadWhisperCommand;
        flyoutItem.SetBinding(MenuFlyoutItem.TextProperty, new Binding(nameof(vm.ReDownloadWhisper)));
        menuFlyoutMain.Add(flyoutItem);

        FlyoutBase.SetContextFlyout(this, menuFlyoutMain);
    }

    private static void MakeConsoleLogWindow(Grid grid, AudioToTextWhisperModel vm)
    {
        vm.ConsoleTextScrollView = new ScrollView
        {
            Margin = new Thickness(10, 0, 10, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Always,
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
        };

        var consoleLabel = new Label
        {
            Text = "Console log",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(10, 10, 10, 10),
        }.BindDynamicTheme();

        vm.ConsoleText = new Editor
        {
            Margin = new Thickness(0, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            IsReadOnly = true,
            Text = string.Empty,
            FontSize = 10,
            FontFamily = "RobotoMono",
            AutoSize = EditorAutoSizeOption.TextChanges,
        }.BindDynamicTheme();

        vm.ConsoleTextScrollView.Content = vm.ConsoleText;

        grid.Add(consoleLabel, 2, 0);
        grid.Add(vm.ConsoleTextScrollView, 2, 1);
        Grid.SetRowSpan(vm.ConsoleTextScrollView, 8);
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

                _vm.LoadSettings();
                _vm.Loading = false;
            });

            return false;
        });
    }

    protected override void OnDisappearing()
    {
        _vm.DeleteTempFiles();
        base.OnDisappearing();
    }
}