namespace SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

public class Voice
{
    public string Name { get; set; }
    public object? EngineVoice { get; set; }

    public Voice()
    {
        Name = string.Empty;
    }

    public override string ToString()
    {
        return Name;
    }
}
