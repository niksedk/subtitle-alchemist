using System.Text;
using CommunityToolkit.Maui.Storage;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace SubtitleAlchemist.Logic.Media
{
    public class FileHelper
    {
        public async Task<string> PickAndShowSubtitleFile(string title)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "application/text" } }, // MIME type
                        { DevicePlatform.WinUI, new[] { ".srt", ".ass" } }, // file extension
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "srt", "ass" } }, // UTType values
                    });

                var pickOptions = new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = title,
                };

                var result = await FilePicker.Default.PickAsync(pickOptions);
                return result?.FullPath ?? string.Empty;
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return string.Empty;
        }

        public async Task<string> PickFfmpeg(string title)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "application/text" } }, // MIME type
                        { DevicePlatform.WinUI, new[] { ".exe" } }, // file extension
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "ffmpeg" } }, // UTType values
                    });

                var pickOptions = new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = title,
                };

                var result = await FilePicker.Default.PickAsync(pickOptions);
                return result?.FullPath ?? string.Empty;
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return string.Empty;
        }

        public async Task<string> PickAndShowVideoFile(string title)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "application/text" } }, // MIME type
                        { DevicePlatform.WinUI, Utilities.VideoFileExtensions }, // file extension
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "srt", "ass" } }, // UTType values
                    });

                var pickOptions = new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = title,
                };

                var result = await FilePicker.Default.PickAsync(pickOptions);
                return result?.FullPath ?? string.Empty;
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return string.Empty;
        }

        public async Task<string> SaveSubtitleFileAs(string title, string videoFileName, SubtitleFormat format, Subtitle subtitle, CancellationToken cancellationToken = default)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "application/text" } }, // MIME type
                        { DevicePlatform.WinUI, Utilities.VideoFileExtensions }, // file extension
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "srt", "ass" } }, // UTType values
                    });

                var pickOptions = new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = title,
                };

                var suggestedFileName = string.Empty;
                if (!string.IsNullOrEmpty(videoFileName))
                {
                    suggestedFileName = Path.Combine(Path.GetDirectoryName(videoFileName), Path.GetFileNameWithoutExtension(videoFileName) + format.Extension);
                }

                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(format.ToText(subtitle, string.Empty)));
                var fileLocation = await FileSaver.Default.SaveAsync(suggestedFileName, ms, cancellationToken);
                return fileLocation.FilePath ?? string.Empty;
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return string.Empty;
        }
    }
}
