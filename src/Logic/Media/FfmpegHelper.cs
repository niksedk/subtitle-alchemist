using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Media
{
    public static class FfmpegHelper
    {
        public static bool IsFfmpegInstalled()
        {
            Configuration.Settings.General.UseFFmpegForWaveExtraction = true;
            var ffmpegLocation = Configuration.Settings.General.FFmpegLocation;
            if (Configuration.IsRunningOnWindows && 
                (string.IsNullOrWhiteSpace(ffmpegLocation) || !File.Exists(ffmpegLocation)))
            {
                return false;
            }

            return true;
        }
    }
}
