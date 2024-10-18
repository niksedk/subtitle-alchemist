﻿using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public class TtsStepResult
{
    public Paragraph Paragraph { get; set; }
    public string Text { get; set; }
    public string CurrentFileName { get; set; }
    public float SpeedFactor { get; set; }
    public Voice? Voice { get; set; }

    public TtsStepResult()
    {
        Paragraph = new Paragraph();
        Text = string.Empty;
        CurrentFileName = string.Empty;
        SpeedFactor = 1.0f;
    }
}