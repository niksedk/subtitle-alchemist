using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ReviewRow : ObservableObject
{
    [ObservableProperty] public partial bool Include { get; set; }
    [ObservableProperty] public partial int Number { get; set; }
    [ObservableProperty] public partial string Voice { get; set; }
    [ObservableProperty] public partial string Cps { get; set; }
    [ObservableProperty] public partial string Speed { get; set; }
    [ObservableProperty] public partial Color SpeedBackgroundColor { get; set; }
    [ObservableProperty] public partial string Text { get; set; }

    public TtsStepResult StepResult { get; set; }

    public ReviewRow()
    {
        Include = true;
        Number = 0;
        Voice = string.Empty;
        Cps = string.Empty;
        Speed = string.Empty;
        SpeedBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
        Text = string.Empty;
        StepResult = new TtsStepResult();
    }
}