using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Translate;

public partial class TranslateRow : ObservableObject
{
    [ObservableProperty] private int _number;

    [ObservableProperty] private TimeSpan _startTime;

    [ObservableProperty] private string _originalText;

    [ObservableProperty] private string _translatedText;

    [ObservableProperty] private Color  _backgroundColor;

    public TranslateRow()
    {
        _originalText = string.Empty;
        _translatedText = string.Empty;
        _backgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
    }
}