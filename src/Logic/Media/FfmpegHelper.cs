using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;

namespace SubtitleAlchemist.Logic.Media
{
    public static class FfmpegHelper
    {
        public static bool IsFfmpegInstalled()
        {
            Configuration.Settings.General.UseFFmpegForWaveExtraction = true;
            Configuration.Settings.General.FFmpegLocation = DownloadFfmpegModel.GetFfmpegFileName();
            return File.Exists(Configuration.Settings.General.FFmpegLocation);
        }
    }
}
