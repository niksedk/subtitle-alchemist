using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Media
{
    public class FfmpegMediaInfo2
    {
        public List<FfmpegTrackInfo> Tracks { get; set; }

        public Dimension Dimension { get; set; } = new(0, 0);
        public TimeCode Duration { get; set; } = new(0);
        public decimal FramesRate { get; set; } = new(0);

        private static readonly Regex ResolutionRegex = new(@"\d\d+x\d\d+", RegexOptions.Compiled);
        private static readonly Regex DurationRegex = new(@"Duration: \d+[:\.,]\d+[:\.,]\d+[:\.,]\d+", RegexOptions.Compiled);
        private static readonly Regex Fps1Regex = new(@" \d+\.\d+ fps", RegexOptions.Compiled);
        private static readonly Regex Fps2Regex = new(@" \d+ fps", RegexOptions.Compiled);

        private FfmpegMediaInfo2()
        {
            Tracks = new List<FfmpegTrackInfo>();
        }

        public static FfmpegMediaInfo2 Parse(string videoFileName)
        {
            if (string.IsNullOrEmpty(Configuration.Settings.General.FFmpegLocation) ||
                !File.Exists(Configuration.Settings.General.FFmpegLocation))
            {
                return new FfmpegMediaInfo2();
            }

            var log = GetFfmpegLog(videoFileName);
            return ParseLog(log);
        }

        public long GetTotalFrames()
        {
            return (long)((double)FramesRate * Duration.TotalMilliseconds / TimeCode.BaseUnit);
        }

        public bool HasFrontCenterAudio(int trackNumber)
        {
            if (trackNumber < 0)
            {
                trackNumber = 0;
            }

            var audioTracks = Tracks.Where(track => track.TrackType == FfmpegTrackType.Audio).ToList();
            if (trackNumber >= audioTracks.Count)
            {
                return false;
            }

            var info = audioTracks[trackNumber].TrackInfo;
            return info.Contains("5.1", StringComparison.Ordinal) ||
                   info.Contains("7.1", StringComparison.Ordinal) ||
                   info.Contains("9.1", StringComparison.Ordinal);
        }

        internal static FfmpegMediaInfo2 ParseLog(string log)
        {
            var info = new FfmpegMediaInfo2();

            var fpsMatch = Fps1Regex.Match(log);
            if (!fpsMatch.Success)
            {
                fpsMatch = Fps2Regex.Match(log);
            }
            if (fpsMatch.Success)
            {
                var fps = fpsMatch.Value.Trim().Split(' ')[0];
                if (double.TryParse(fps, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
                {
                    info.FramesRate = (decimal)f;
                }
            }

            foreach (var line in log.SplitToLines())
            {
                var s = line.Trim();
                if (s.StartsWith("Stream #", StringComparison.Ordinal))
                {
                    var resolutionMatch = ResolutionRegex.Match(s);
                    if (resolutionMatch.Success)
                    {
                        var parts = resolutionMatch.Value.Split('x');
                        if (info.Dimension.Width == 0 &&
                            parts.Length == 2 &&
                            int.TryParse(parts[0], out var w) &&
                            int.TryParse(parts[1], out var h))
                        {
                            info.Dimension = new Dimension(w, h); 
                        }
                    }

                    var arr = s.Replace(": ", "¤").Split('¤');
                    if (arr.Length == 3)
                    {
                        var trackType = arr[1].Trim();
                        var trackInfo = arr[2].Trim();
                        if (trackType == FfmpegTrackType.Audio.ToString())
                        {
                            info.Tracks.Add(new FfmpegTrackInfo { TrackType = FfmpegTrackType.Audio, TrackInfo = trackInfo });
                        }
                        else if (trackType == FfmpegTrackType.Video.ToString())
                        {
                            info.Tracks.Add(new FfmpegTrackInfo { TrackType = FfmpegTrackType.Video, TrackInfo = trackInfo });
                        }
                        else if (trackType == FfmpegTrackType.Subtitle.ToString())
                        {
                            info.Tracks.Add(new FfmpegTrackInfo { TrackType = FfmpegTrackType.Subtitle, TrackInfo = trackInfo });
                        }
                        else
                        {
                            info.Tracks.Add(new FfmpegTrackInfo { TrackType = FfmpegTrackType.Other, TrackInfo = trackInfo });
                        }
                    }
                }

                var match = DurationRegex.Match(line);
                if (match.Success)
                {
                    var timeCodeString = match.Value.Split(' ')[1];
                    info.Duration = new TimeCode(TimeCode.ParseToMilliseconds(timeCodeString));
                }
            }

            return info;
        }

        private static string GetFfmpegLog(string videoFileName)
        {
            var sb = new StringBuilder();
            using (var process = GetFFmpegProcess(videoFileName)!)
            {
                process.OutputDataReceived += (sender, args) =>
                {
                    sb.AppendLine(args.Data);
                };
                process.ErrorDataReceived += (sender, args) =>
                {
                    sb.AppendLine(args.Data);
                };
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
#pragma warning disable CA1416
                _ = process.Start();
#pragma warning restore CA1416
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(8000);
                System.Threading.Thread.Sleep(400);
            }

            System.Threading.Thread.Sleep(100);
            return sb.ToString();
        }

        public static Process GetFFmpegProcess(string inputFileName)
        {
            var ffmpegLocation = Configuration.Settings.General.FFmpegLocation;
            if (!Configuration.IsRunningOnWindows && (string.IsNullOrEmpty(ffmpegLocation) || !File.Exists(ffmpegLocation)))
            {
                ffmpegLocation = "ffmpeg";
            }

            return new Process
            {
                StartInfo =
                {
                    FileName = ffmpegLocation,
                    Arguments = $"-i \"{inputFileName}\" -hide_banner",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }
    }
}
