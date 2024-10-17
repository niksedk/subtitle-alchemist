using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Video.TextToSpeech
{
    public class TtsStepResult
    {
        public Paragraph Paragraph { get; set; }
        public string Text { get; set; }
        public string CurrentFileName { get; set; }
       public List<string> OldFileNames { get; set; }

        public TtsStepResult()
        {
            Paragraph = new Paragraph();
            Text = string.Empty;
            CurrentFileName = string.Empty;
            OldFileNames = new List<string>();
        }
    }
}
