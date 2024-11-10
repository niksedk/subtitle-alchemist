﻿using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public class BatchConverter : IBatchConverter
{
    private BatchConvertConfig _config;
    private List<SubtitleFormat> _subtitleFormats;

    public BatchConverter()
    {
        _config = new BatchConvertConfig();
        _subtitleFormats = new List<SubtitleFormat>();
    }

    public void Initialize(BatchConvertConfig config)
    {
        _config = config;
        _subtitleFormats = SubtitleFormat.AllSubtitleFormats.ToList();
    }

    public async Task Convert(BatchConvertItem item)
    {
        if (_subtitleFormats.Count == 0)
        {
            throw new InvalidOperationException("Initialize not called?");
        }

        foreach (var format in _subtitleFormats)
        {
            if (format.Name == _config.TargetFormatName && item.Subtitle != null)
            {
                item.Status = "Converting...";
                try
                {
                    var processedSubtitle = RunConvertFunctions(item);
                    var converted = format.ToText(processedSubtitle, _config.TargetEncoding);
                    var path = MakeOutputFileName(item, format);
                    await File.WriteAllTextAsync(path, converted);
                    item.Status = "Converted";
                }
                catch (Exception exception)
                {
                    item.Status = "Error: " + exception.Message;
                }

                break;
            }
        }
    }

    private Subtitle RunConvertFunctions(BatchConvertItem item)
    {
        var s = new Subtitle(item.Subtitle, false);
        s = ProcessRemoveFormatting(s);
        s = ProcessOffsetTimeCodes(s);
        s = ChangeFrameRate(s);
        s = RemoveLineBreaks(s);
        s = DeleteLines(s);
        s = AdjustDisplayDuration(s);
        return s;
    }

    private Subtitle ProcessRemoveFormatting(Subtitle subtitle)
    {
        if (!_config.RemoveFormatting.IsActive)
        {
            return subtitle;
        }

        foreach (var p in subtitle.Paragraphs)
        {
            if (_config.RemoveFormatting.RemoveAll)
            {
                p.Text = HtmlUtil.RemoveHtmlTags(p.Text, true);
            }
            else
            {
                if (_config.RemoveFormatting.RemoveItalic)
                {
                    p.Text = HtmlUtil.RemoveOpenCloseTags(p.Text, HtmlUtil.TagItalic);
                    p.Text = p.Text
                        .Replace("{\\i}", string.Empty)
                        .Replace("{\\i0}", string.Empty)
                        .Replace("{\\i1}", string.Empty);
                }

                if (_config.RemoveFormatting.RemoveBold)
                {
                    p.Text = HtmlUtil.RemoveOpenCloseTags(p.Text, HtmlUtil.TagBold);
                    p.Text = p.Text
                        .Replace("{\\b}", string.Empty)
                        .Replace("{\\b0}", string.Empty)
                        .Replace("{\\b1}", string.Empty);
                }

                if (_config.RemoveFormatting.RemoveUnderline)
                {
                    p.Text = HtmlUtil.RemoveOpenCloseTags(p.Text, HtmlUtil.TagUnderline);
                    p.Text = p.Text
                        .Replace("{\\u}", string.Empty)
                        .Replace("{\\u0}", string.Empty)
                        .Replace("{\\u1}", string.Empty);
                }

                if (_config.RemoveFormatting.RemoveColor)
                {
                    p.Text = HtmlUtil.RemoveColorTags(p.Text);
                    if (p.Text.Contains("\\c") || p.Text.Contains("\\1c"))
                    {
                        p.Text = HtmlUtil.RemoveAssaColor(p.Text);
                    }
                }

                if (_config.RemoveFormatting.RemoveFontName)
                {
                    p.Text = HtmlUtil.RemoveFontName(p.Text);
                }

                if (_config.RemoveFormatting.RemoveAlignment)
                {
                    if (p.Text.Contains('{'))
                    {
                        p.Text = HtmlUtil.RemoveAssAlignmentTags(p.Text);
                    }
                }
            }
        }

        return subtitle;
    }

    private Subtitle ProcessOffsetTimeCodes(Subtitle subtitle)
    {
        if (!_config.OffsetTimeCodes.IsActive)
        {
            return subtitle;
        }

        var totalMilliseconds = _config.OffsetTimeCodes.Milliseconds;
        if (!_config.OffsetTimeCodes.Forward)
        {
            totalMilliseconds *= -1;
        }

        subtitle.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(totalMilliseconds));
        return subtitle;
    }

    private Subtitle ChangeFrameRate(Subtitle subtitle)
    {
        if (!_config.ChangeFrameRate.IsActive)
        {
            return subtitle;
        }

        subtitle.ChangeFrameRate(_config.ChangeFrameRate.FromFrameRate, _config.ChangeFrameRate.ToFrameRate);

        return subtitle;
    }

    private Subtitle RemoveLineBreaks(Subtitle subtitle)
    {
        if (!_config.OffsetTimeCodes.IsActive)
        {
            return subtitle;
        }

        foreach (var paragraph in subtitle.Paragraphs)
        {
            paragraph.Text = Utilities.UnbreakLine(paragraph.Text);
        }

        return subtitle;
    }

    public Subtitle DeleteLines(Subtitle subtitle)
    {
        if (!_config.DeleteLines.IsActive)
        {
            return subtitle;
        }

        var c = _config.DeleteLines;
        if (c.DeleteXFirst == 0 && c.DeleteXLast == 0 && string.IsNullOrWhiteSpace(c.DeleteContains))
        {
            return subtitle;
        }

        var paragraphs = subtitle.Paragraphs.Skip(c.DeleteXFirst).ToList();
        paragraphs = paragraphs.Take(paragraphs.Count - c.DeleteXLast).ToList();
        if (!string.IsNullOrWhiteSpace(c.DeleteContains))
        {
            paragraphs = paragraphs.Where(p => !p.Text.Contains(c.DeleteContains)).ToList();
        }

        subtitle.Paragraphs.Clear();
        subtitle.Paragraphs.AddRange(paragraphs);
        subtitle.Renumber();

        return subtitle;
    }

    public Subtitle AdjustDisplayDuration(Subtitle subtitle)
    {
        if (!_config.AdjustDisplayDuration.IsActive)
        {
            return subtitle;
        }

        //TODO: Implement this, use enum?
        var c = _config.AdjustDisplayDuration;
        var adjustmentType = _config.AdjustDisplayDuration.AdjustmentType;
        if (adjustmentType == Se.Language.AdjustDurations.Percent)
        {
            //subtitle.AdjustDisplayTimeUsingPercent(c.Percentage, null, shotChanges, checkBoxEnforceDurationLimits.Checked);
        }
        else if (adjustmentType == Se.Language.AdjustDurations.Recalculate)
        {
            //subtitle.RecalculateDisplayTimes((double)numericUpDownMaxCharsSec.Value, null, (double)numericUpDownOptimalCharsSec.Value, checkBoxExtendOnly.Checked, shotChanges, checkBoxEnforceDurationLimits.Checked);
        }
        else if (adjustmentType == Se.Language.AdjustDurations.Fixed)
        {
            //subtitle.SetFixedDuration(null, (double)numericUpDownFixedMilliseconds.Value);
        }
        else if (adjustmentType == Se.Language.AdjustDurations.Seconds)
        {
            //subtitle.AdjustDisplayTimeUsingSeconds((double)numericUpDownSeconds.Value, null, shotChanges, checkBoxEnforceDurationLimits.Checked);
        }

        return subtitle;
    }


    private string MakeOutputFileName(BatchConvertItem item, SubtitleFormat format)
    {
        var outputFolder = _config.SaveInSourceFolder || string.IsNullOrEmpty(_config.OutputFolder)
                ? Path.GetDirectoryName(item.FileName)
                : _config.OutputFolder;
        if (string.IsNullOrEmpty(outputFolder))
        {
            throw new InvalidOperationException("Output folder is not set");
        }

        var fileName = Path.GetFileNameWithoutExtension(item.FileName);
        var targetExtension = format.Extension;
        var outputFileName = Path.Combine(outputFolder, fileName + targetExtension);
        if (!File.Exists(outputFileName))
        {
            return outputFileName;
        }

        if (_config.Overwrite)
        {
            File.Delete(outputFileName);
        }
        else
        {
            var counter = 1;
            do
            {
                outputFileName = Path.Combine(outputFolder, fileName + $"_{counter}" + targetExtension);
                counter++;
            } while (File.Exists(outputFileName));
        }

        return outputFileName;
    }
}
