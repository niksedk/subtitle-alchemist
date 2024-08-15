using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.Translate;

public partial class ExcelRow : ObservableObject
{
    [ObservableProperty] private int _number;

    [ObservableProperty] private TimeSpan _startTime;

    [ObservableProperty] private string _originalText;

    [ObservableProperty] private string _translatedText;
}