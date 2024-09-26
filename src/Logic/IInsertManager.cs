using SubtitleAlchemist.Features.Main;

namespace SubtitleAlchemist.Logic;

public interface IInsertManager
{
    DisplayParagraph InsertAfter(List<DisplayParagraph> paragraphs, int[] selectedIndices, string text, int minGapBetweenLines);
    DisplayParagraph InsertBefore(List<DisplayParagraph> paragraphs, int[] selectedIndices, string text, int minGapBetweenLines);
}