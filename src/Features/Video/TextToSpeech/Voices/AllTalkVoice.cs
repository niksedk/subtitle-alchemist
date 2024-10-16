namespace SubtitleAlchemist.Features.Video.TextToSpeech.Voices
{
    public class AllTalkVoice  
    {
        public string Voice { get; set; }

        public override string ToString()
        {
            return Voice;
        }

        public AllTalkVoice(string voice)
        {
            Voice = voice;
        }
   }
}