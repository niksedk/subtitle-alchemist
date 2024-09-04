using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Options.DownloadFfmpeg;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Logic.Media
{
    public static class FfmpegHelper
    {
        public static bool IsFfmpegInstalled()
        {
            Configuration.Settings.General.UseFFmpegForWaveExtraction = true;
            Se.Settings.FfmpegPath = DownloadFfmpegModel.GetFfmpegFileName();
            return File.Exists(Se.Settings.FfmpegPath);
        }
    }
}
