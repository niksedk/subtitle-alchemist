using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public partial class WhisperAdvancedModel : ObservableObject, IQueryAttributable
{
    public WhisperAdvancedPage? Page { get; set; }
    public Dictionary<string, View> WhisperEngines { get; set; } = new();
    public Dictionary<string, Editor> WhisperHelpText { get; set; } = new();
    public Dictionary<string, ScrollView> WhisperScrollViews { get; set; } = new();
    public Border EnginePage { get; set; } = new();
    public string DefaultWhisperEngineName { get; set; } = WhisperEngineCpp.StaticName;

    [ObservableProperty]
    private string _currentParameters = Se.Settings.Tools.AudioToText.WhisperCustomCommandLineArguments;

    public VerticalStackLayout LeftMenu { get; set; } = new();
    public Editor EditorCppHelpText { get; set; } = new();
    public Editor EditorConstMeHelpText { get; set; } = new();
    public Editor EditorOpenAiHelpText { get; set; } = new();
    public Editor EditorPurfviewHelpText { get; set; } = new();
    public Editor EditorPurfviewXxlHelpText { get; set; } = new();

    public ScrollView ScrollViewCppHelpText { get; set; } = new();
    public ScrollView ScrollViewConstMeHelpText { get; set; } = new();
    public ScrollView ScrollViewOpenAiHelpText { get; set; } = new();
    public ScrollView ScrollViewPurfviewHelpText { get; set; } = new();
    public ScrollView ScrollViewPurfviewXxlHelpText { get; set; } = new();

    private string _engineName;
    private readonly IPopupService _popupService;
    private static readonly SeAudioToText _settings = Se.Settings.Tools.AudioToText;

    public WhisperAdvancedModel(IPopupService popupService)
    {
        _engineName = DefaultWhisperEngineName;
        _popupService = popupService;
    }

    public async Task LeftMenuTapped(string engineName)
    {
        if (EnginePage.Content != null)
        {
            await EnginePage.Content.FadeTo(0, 200);
        }

        WhisperEngines[engineName].Opacity = 0;

        var engine = WhisperEngineFactory.MakeEngineFromStaticName(engineName);
        WhisperHelpText[engineName].Text = await engine.GetHelpText();

        EnginePage.Content = WhisperEngines[engineName];

        foreach (var child in LeftMenu.Children)
        {
            if (child is Label label)
            {
                if (label.ClassId == engineName)
                {
                    label.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
                }
                else
                {
                    label.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
                }
            }
        }

        await EnginePage.Content.FadeTo(1, 200);
    }

    [RelayCommand]
    private async Task Close()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Ok()
    {
        var param = CurrentParameters.Trim();
        if (!string.IsNullOrWhiteSpace(param) && !_settings.WhisperCustomCommandLineArguments.Contains(param))
        {
            _settings.WhisperExtraSettingsHistory =
                param + Environment.NewLine + _settings.WhisperExtraSettingsHistory;
        }

        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "Page", nameof(WhisperAdvancedPage) },
            { "Parameters", CurrentParameters },
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void OnSizeAllocated(double width, double height)
    {
        var newWidth = Math.Max(100, width - 350);
        var newHeight = Math.Max(100, height - 350);

        ScrollViewOpenAiHelpText.HeightRequest = newHeight;
        ScrollViewOpenAiHelpText.WidthRequest = newWidth;

        ScrollViewPurfviewHelpText.HeightRequest = newHeight;
        ScrollViewPurfviewHelpText.WidthRequest = newWidth;

        ScrollViewPurfviewXxlHelpText.HeightRequest = newHeight;
        ScrollViewPurfviewXxlHelpText.WidthRequest = newWidth;

        ScrollViewConstMeHelpText.HeightRequest = newHeight;
        ScrollViewConstMeHelpText.WidthRequest = newWidth;

        ScrollViewCppHelpText.HeightRequest = newHeight;
        ScrollViewCppHelpText.WidthRequest = newWidth;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("WhisperEngine") && query["WhisperEngine"] is string engineName)
        {
            DefaultWhisperEngineName = engineName;
        }
    }

    [RelayCommand]
    public async Task ShowHistory()
    {
        var engine = WhisperEngineFactory.MakeEngineFromStaticName(_engineName);
        _settings.WhisperChoice = engine.Choice;
        var command = await _popupService.ShowPopupAsync<WhisperAdvancedHistoryPopupModel>(CancellationToken.None);
        if (command is string commandString)
        {
            CurrentParameters = commandString;
        }
    }

    [RelayCommand]
    private void SetStandardParameter()
    {
        CurrentParameters = "--standard";
    }

    [RelayCommand]
    private void SetStandardAsiaParameter()
    {
        CurrentParameters = "--standard_asia";
    }

    [RelayCommand]
    private void SetSentenceParameter()
    {
        CurrentParameters = "--sentence";
    }

    [RelayCommand]
    private void SetSingleWordsParameter()
    {
        CurrentParameters = "--one_word 2";
    }

    [RelayCommand]
    private void SetHighlightWordParameter()
    {
        CurrentParameters = "--highlight_words true --max_line_width 43 --max_line_count 2";
    }
}
