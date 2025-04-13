using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Translate;

public partial class TranslateRow : ObservableObject
{
    [ObservableProperty]
    public partial int Number { get; set; }

    [ObservableProperty]
    public partial TimeSpan StartTime { get; set; }

    [ObservableProperty]
    public partial string OriginalText { get; set; }

    [ObservableProperty]
    public partial string TranslatedText { get; set; }

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }

    public TranslateRow()
    {
        OriginalText = string.Empty;
        TranslatedText = string.Empty;
        BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
    }
}