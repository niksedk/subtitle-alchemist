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
    public Dictionary<string, Editor> WhisperHelpLabels { get; set; } = new();
    public Border EnginePage { get; set; } = new();

    [ObservableProperty]
    private string _currentParameters = SeSettings.Settings.Tools.WhisperExtraSettings;

    public VerticalStackLayout LeftMenu { get; set; } = new();
    public Editor LabelCppHelpText { get; set; } = new();
    public Editor LabelConstMeHelpText { get; set; } = new();
    public Editor LabelOpenAiHelpText { get; set; } = new();
    public Editor LabelPurfviewHelpText { get; set; } = new();
    public Editor LabelPurfviewXxlHelpText { get; set; } = new();

    private string _videoFileName = string.Empty;

    public async Task LeftMenuTapped(string engineName)
    {
        if (EnginePage.Content != null)
        {
            await EnginePage.Content.FadeTo(0, 200);
        }

        WhisperEngines[engineName].Opacity = 0;

        var engine = WhisperEngineFactory.MakeEngineFromStaticName(engineName);
        WhisperHelpLabels[engineName].Text = await engine.GetHelpText();

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
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "Page", nameof(WhisperAdvancedPage) },
            { "Parameters", CurrentParameters },
            { "VideoFileName", _videoFileName },
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["VideoFileName"] is string videoFileName)
        {
            _videoFileName = videoFileName;
        }
    }
}
