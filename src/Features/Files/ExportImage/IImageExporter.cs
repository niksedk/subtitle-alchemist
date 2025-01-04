using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Files.ExportImage;

public interface IImageExporter
{
    bool Export(Stream output, ExportImages settings, List<ImageExportLine> lines, IProgress<float>? progress, CancellationToken cancellation);
}