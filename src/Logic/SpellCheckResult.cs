﻿using System.Diagnostics;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SpellCheck;

namespace SubtitleAlchemist.Logic;

[DebuggerDisplay("Line {LineIndex + 1}, Word {WordIndex + 1}: '{Word}'")]
public class SpellCheckResult
{
    public int LineIndex { get; set; }
    public int WordIndex { get; set; }
    public SpellCheckWord Word { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public bool IsCommonMisspelling { get; set; }
    public Paragraph Paragraph { get; set; } = new();
}
