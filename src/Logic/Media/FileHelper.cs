using System.Text;
using CommunityToolkit.Maui.Storage;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace SubtitleAlchemist.Logic.Media
{
    public class FileHelper : IFileHelper
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

        public async Task<string[]> PickAndShowSubtitleFiles(string title)
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

                var result = await FilePicker.Default.PickMultipleAsync(pickOptions);
                return result.Select(p => p.FullPath).ToArray();
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return Array.Empty<string>();
        }


        public async Task<string> PickAndShowSubtitleFile(string title, SubtitleFormat format)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "application/text" } }, // MIME type
                        { DevicePlatform.WinUI, new[] { format.Extension } }, // file extension
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { format.Extension.TrimStart('.') } }, // UTType values
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

        public async Task<string[]> PickAndShowVideoFiles(string title)
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

                var result = await FilePicker.Default.PickMultipleAsync(pickOptions);
                return result.Select(p => p.FullPath).ToArray();
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return Array.Empty<string>(); ;
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
                    var folderName = Path.GetDirectoryName(videoFileName);
                    if (folderName != null)
                    {
                        suggestedFileName = Path.Combine(folderName, Path.GetFileNameWithoutExtension(videoFileName) + format.Extension);
                    }
                }

                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(format.ToText(subtitle, string.Empty)));
#pragma warning disable CA1416 // Validate platform compatibility
                var fileLocation = await FileSaver.Default.SaveAsync(suggestedFileName, ms, cancellationToken);
#pragma warning restore CA1416 // Validate platform compatibility
                return fileLocation.FilePath ?? string.Empty;
            }
            catch
            {
                // The user canceled or something went wrong
            }

            return string.Empty;
        }

        public async Task<string> SaveStreamAs(Stream stream, string title, string fileName, SubtitleFormat format, CancellationToken cancellationToken = default)
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                        { DevicePlatform.Android, new[] { "application/text" } }, // MIME type
                        { DevicePlatform.WinUI, new [] { format.Extension } }, // file extension
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { format.Extension.TrimStart('.') } }, // UTType values
                    });

                var pickOptions = new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = title,
                };

                var suggestedFileName = string.Empty;
                if (!string.IsNullOrEmpty(fileName))
                {
                    var folderName = Path.GetDirectoryName(fileName);
                    if (folderName != null)
                    {
                        suggestedFileName = Path.Combine(folderName, Path.GetFileNameWithoutExtension(fileName) + format.Extension);
                    }
                }

#pragma warning disable CA1416 // Validate platform compatibility
                var fileLocation = await FileSaver.Default.SaveAsync(suggestedFileName, stream, cancellationToken);
#pragma warning restore CA1416 // Validate platform compatibility
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
