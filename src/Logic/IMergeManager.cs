using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic;

public interface IMergeManager
{
    Subtitle MergeSelectedLines(Subtitle subtitle, int[] selectedIndices, MergeManager.BreakMode breakMode = MergeManager.BreakMode.Normal);
}