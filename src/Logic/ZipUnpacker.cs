﻿using System.IO.Compression;

namespace SubtitleAlchemist.Logic;

public class ZipUnpacker : IZipUnpacker
{
    public void UnpackZipStream(Stream zipStream, string outputPath)
    {
        UnpackZipStream(zipStream, outputPath, string.Empty, false, new List<string>(), null);
    }

    public void UnpackZipStream(
    Stream zipStream,
    string outputPath,
    string skipFolderLevel,
    bool allToOutputPath,
    List<string> allowedExtensions,
    List<string>? outputFileNames)
    {
        allowedExtensions = allowedExtensions.Select(x => x.ToLowerInvariant()).ToList();

        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            var entryFullName = entry.FullName;
            if (!string.IsNullOrEmpty(skipFolderLevel) && entryFullName.StartsWith(skipFolderLevel))
            {
                entryFullName = entryFullName[skipFolderLevel.Length..];
            }

            if (allToOutputPath)
            {
                entryFullName = Path.GetFileName(entryFullName);
            }

            var filePath = Path.Combine(outputPath, entryFullName);
            var directoryPath = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!string.IsNullOrEmpty(entry.Name))
            {

                if (allowedExtensions.Count > 0)
                {
                    var extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        continue;
                    }
                }

                using var entryStream = entry.Open();
                using var fileStream = File.Create(filePath);
                entryStream.CopyTo(fileStream);
                outputFileNames?.Add(filePath);
            }
        }
    }
}