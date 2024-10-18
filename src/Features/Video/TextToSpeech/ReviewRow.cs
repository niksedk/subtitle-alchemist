using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ReviewRow : ObservableObject
{
    [ObservableProperty] private bool _include;
    [ObservableProperty] private int _number;
    [ObservableProperty] private string _voice;
    [ObservableProperty] private string _cps;
    [ObservableProperty] private string _speed;
    [ObservableProperty] private Color  _speedBackgroundColor;
    [ObservableProperty] private string _text;

    public ReviewRow()
    {
        _include = true;
        _number = 0;
        _voice = string.Empty;
        _cps = string.Empty;
        _speed = string.Empty;
        _speedBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
        _text = string.Empty;
    }
}