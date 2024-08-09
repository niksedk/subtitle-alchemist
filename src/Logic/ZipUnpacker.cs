using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleAlchemist.Logic
{
    public static class ZipUnpacker
    {
        public static void UnpackZipStream(Stream zipStream, string outputPath)
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                var filePath = Path.Combine(outputPath, entry.FullName);
                var directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
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
