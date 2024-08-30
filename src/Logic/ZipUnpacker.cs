using System.IO.Compression;

namespace SubtitleAlchemist.Logic
{
    public static class ZipUnpacker
    {
        public static void UnpackZipStream(Stream zipStream, string outputPath, string skipFolderLevel)
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                var entryFullName = entry.FullName;
                if (!string.IsNullOrEmpty(skipFolderLevel) && entryFullName.StartsWith(skipFolderLevel))
                {
                    entryFullName = entryFullName[skipFolderLevel.Length..];
                }

                var filePath = Path.Combine(outputPath, entryFullName);
                var directoryPath = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!string.IsNullOrEmpty(entry.Name))
                {
                    using var entryStream = entry.Open();
                    using var fileStream = File.Create(filePath);
                    entryStream.CopyTo(fileStream);
                }
            }
        }
    }
}
