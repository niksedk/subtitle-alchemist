using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace SubtitleAlchemist.Logic.Media;

public interface IFileHelper
{
    Task<string> PickAndShowSubtitleFile(string title);

    Task<string> PickAndShowSubtitleFile(string title, SubtitleFormat format);
    Task<string> PickAndShowVideoFile(string title);

    Task<string> SaveSubtitleFileAs(string title, string videoFileName, SubtitleFormat format, Subtitle subtitle,
        CancellationToken cancellationToken = default);

    Task<string> SaveStreamAs(Stream stream, string title, string fileName, SubtitleFormat format, CancellationToken cancellationToken = default);
}