using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public partial class WhisperAdvancedPopupModel : ObservableObject
{
    public WhisperAdvancedPopup? Popup { get; set; }
    public Dictionary<string, View> WhisperEngines { get; set; } = new();
    public Dictionary<string, Editor> WhisperHelpLabels { get; set; } = new();
    public Border EnginePage { get; set; } = new();
    private string _engineName = WhisperEngineCpp.StaticName;

    [ObservableProperty]
    private string _currentParameters = Configuration.Settings.Tools.WhisperExtraSettings;

    public VerticalStackLayout LeftMenu { get; set; } = new();
    public Editor LabelCppHelpText { get; set; } = new();
    public Editor LabelConstMeHelpText { get; set; } = new();
    public Editor LabelOpenAiHelpText { get; set; } = new();
    public Editor LabelPurfviewHelpText { get; set; } = new();
    public Editor LabelPurfviewXxlHelpText { get; set; } = new();

    public async Task LeftMenuTapped(string engineName)
    {
        _engineName = engineName;

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
    private void Close()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    [RelayCommand]
    private void Ok()
    {
        Popup?.Close(CurrentParameters);
    }

    [RelayCommand]
    private void Cancel()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }
}
