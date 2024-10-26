﻿using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Enums;
using System.Text;

namespace SubtitleAlchemist.Logic
{
    public class MergeManager : IMergeManager
    {
        public enum BreakMode
        {
            AutoBreak,
            Normal,
            Unbreak,
            UnbreakNoSpace
        }

        public Subtitle MergeSelectedLines(Subtitle inputSubtitle, int[] selectedIndices, BreakMode breakMode = BreakMode.Normal)
        {
            if (inputSubtitle.Paragraphs.Count <= 0 || selectedIndices.Length <= 1)
            {
                return inputSubtitle;
            }

            var subtitle = new Subtitle(inputSubtitle, false);
            var sb = new StringBuilder();
            var deleteIndices = new List<int>();
            var first = true;
            var firstIndex = 0;
            double endMilliseconds = 0;
            var next = 0;
            foreach (var index in selectedIndices)
            {
                if (first)
                {
                    firstIndex = index;
                    next = index + 1;
                    first = !first;
                }
                else
                {
                    deleteIndices.Add(index);
                    if (next != index)
                    {
                        return subtitle; 
                    }

                    next++;
                }

                var continuationStyle = Configuration.Settings.General.ContinuationStyle;
                if (continuationStyle != ContinuationStyle.None)
                {
                    var continuationProfile = ContinuationUtilities.GetContinuationProfile(continuationStyle);
                    if (next < firstIndex + selectedIndices.Length)
                    {
                        var mergeResult = ContinuationUtilities.MergeHelper(subtitle.Paragraphs[index].Text, subtitle.Paragraphs[index + 1].Text, continuationProfile, LanguageAutoDetect.AutoDetectGoogleLanguage(subtitle));
                        subtitle.Paragraphs[index].Text = mergeResult.Item1;
                        subtitle.Paragraphs[index + 1].Text = mergeResult.Item2;
                    }
                }
                var addText = subtitle.Paragraphs[index].Text;

                if (firstIndex != index)
                {
                    // addText = RemoveAssStartAlignmentTag(addText);
                }

                if (breakMode == BreakMode.UnbreakNoSpace)
                {
                    sb.Append(addText);
                }
                else
                {
                    sb.AppendLine(addText);
                }

                endMilliseconds = subtitle.Paragraphs[index].EndTime.TotalMilliseconds;
            }

            var currentParagraph = subtitle.Paragraphs[firstIndex];
            var text = sb.ToString().TrimEnd();
            text = HtmlUtil.FixInvalidItalicTags(text);
            //text = FixAssaTagsAfterMerge(text);
            //text = ChangeAllLinesTagsToSingleTag(text, "i");
            //text = ChangeAllLinesTagsToSingleTag(text, "b");
            //text = ChangeAllLinesTagsToSingleTag(text, "u");
            if (breakMode == BreakMode.Unbreak)
            {
                text = Utilities.UnbreakLine(text);
            }
            else if (breakMode == BreakMode.UnbreakNoSpace)
            {
                text = text.Replace(" " + Environment.NewLine + " ", string.Empty)
                    .Replace(Environment.NewLine + " ", string.Empty)
                    .Replace(" " + Environment.NewLine, string.Empty)
                    .Replace(Environment.NewLine, string.Empty);
            }
            else
            {
                text = Utilities.AutoBreakLine(text, LanguageAutoDetect.AutoDetectGoogleLanguage(subtitle));
            }

            currentParagraph.Text = text;

            //display time
            currentParagraph.EndTime.TotalMilliseconds = endMilliseconds;

            var nextParagraph = subtitle.GetParagraphOrDefault(next);
            if (nextParagraph != null && currentParagraph.EndTime.TotalMilliseconds > nextParagraph.StartTime.TotalMilliseconds && currentParagraph.StartTime.TotalMilliseconds < nextParagraph.StartTime.TotalMilliseconds)
            {
                currentParagraph.EndTime.TotalMilliseconds = nextParagraph.StartTime.TotalMilliseconds - 1;
            }

            for (var i = deleteIndices.Count - 1; i >= 0; i--)
            {
                subtitle.Paragraphs.RemoveAt(deleteIndices[i]);
            }

            subtitle.Renumber();
            return subtitle;
        }
    }
}
