﻿using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SpellCheck;

namespace SubtitleAlchemist.Logic;

public class SpellCheckWordChangedEvent : EventArgs
{
    public int WordIndex { get; set; }
    public string FromWord { get; set; } = string.Empty;
    public string ToWord { get; set; } = string.Empty;
    public Paragraph Paragraph { get; set; } = new ();
    public SpellCheckWord Word { get; set; } = new();
}