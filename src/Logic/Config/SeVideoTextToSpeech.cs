namespace SubtitleAlchemist.Logic.Config;

public class SeVideoTextToSpeech
{
    public string Engine { get; set; }
    public string Voice { get; set; }
    public string ElevenLabsApiKey { get; set; }
    public string AzureApiKey { get; set; }
    public string AzureRegion { get; set; }
    public string ElevenLabsModel { get; set; }
    public string ElevenLabsLanguage { get; set; }
    public bool ReviewAudioClips { get; set; }
    public bool CustomAudio { get; set; }
    public bool CustomAudioStereo { get; set; }
    public string CustomAudioEncoding { get; set; }
    public bool GenerateVideoFile { get; set; }
    public string VoiceTestText { get; set; }

    public SeVideoTextToSpeech()
    {
        Engine = "Piper";
        Voice = string.Empty;
        ElevenLabsApiKey = string.Empty;
        AzureApiKey = string.Empty;
        AzureRegion = string.Empty;
        ElevenLabsModel = string.Empty;
        ElevenLabsLanguage = string.Empty;
        ReviewAudioClips = true;
        CustomAudio = false;
        CustomAudioStereo = true;
        CustomAudioEncoding = string.Empty;
        GenerateVideoFile = true;
        VoiceTestText = "Hello, how are you doing?";
    }
}