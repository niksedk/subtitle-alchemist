using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Files.ExportImage;

public class ImageExportLine
{
    public Paragraph Paragraph { get; set;  }

    public ImageExportLine()
    {
        Paragraph = new Paragraph();     
    }
}