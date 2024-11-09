using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

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
        return s;
    }

    private Subtitle ProcessRemoveFormatting(Subtitle subtitle)
    {
        if (!_config.RemoveFormatting.IsActive)
        {
            return subtitle;
        }

        if (_config.RemoveFormatting.RemoveAll)
        {
            //subtitle.RemoveFormatting();
        }
        else
        {
            if (_config.RemoveFormatting.RemoveItalic)
            {
                //subtitle.RemoveItalic();
            }

            if (_config.RemoveFormatting.RemoveBold)
            {
                // subtitle.RemoveBold();
            }

            if (_config.RemoveFormatting.RemoveUnderline)
            {
                //subtitle.RemoveUnderline();
            }

            if (_config.RemoveFormatting.RemoveColor)
            {
                // subtitle.RemoveColor();
            }

            if (_config.RemoveFormatting.RemoveFontName)
            {
                // subtitle.RemoveFontName();
            }

            if (_config.RemoveFormatting.RemoveAlignment)
            {
               // subtitle.RemoveAlignment();
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