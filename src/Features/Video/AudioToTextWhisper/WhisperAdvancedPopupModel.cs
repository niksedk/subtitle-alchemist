﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic.Constants;
using static SubtitleAlchemist.Features.Video.AudioToTextWhisper.WhisperAdvancedPopup;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public partial class WhisperAdvancedPopupModel : ObservableObject
{
    public WhisperAdvancedPopup? Popup { get; set; }
    public Dictionary<WhisperEngineNames, View> WhisperEngines { get; set; } = new();
    public Border EnginePage { get; set; } = new();
    private WhisperEngineNames _engineName = WhisperEngineNames.WhisperCpp;

    [ObservableProperty]
    private string _currentParameters = Configuration.Settings.Tools.WhisperExtraSettings;

    public VerticalStackLayout LeftMenu { get; set; } = new();

    public async Task LeftMenuTapped(object? sender, TappedEventArgs tappedEventArgs, WhisperEngineNames engineName)
    {
        _engineName = engineName;

        if (EnginePage.Content != null)
        {
            await EnginePage.Content.FadeTo(0, 200);
        }

        WhisperEngines[engineName].Opacity = 0;
        EnginePage.Content = WhisperEngines[engineName];

        foreach (var child in LeftMenu.Children)
        {
            if (child is Label label)
            {
                if (label.ClassId == engineName.ToString())
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