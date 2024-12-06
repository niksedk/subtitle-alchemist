using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Logic;

public static class AssetHelper
{
    public static void CopyToAppData(string name)
    {
        var targetFile = Path.Combine(Se.BaseFolder, name);
        if (File.Exists(targetFile))
        {
            return;
        }

        using var outputStream = File.OpenWrite(targetFile);
        using var fs = FileSystem.Current.OpenAppPackageFileAsync(name).Result;
        using var writer = new BinaryWriter(outputStream);
        using var reader = new BinaryReader(fs);
        int bytesRead;
        const int bufferSize = 1024;

        do
        {
            var buffer = reader.ReadBytes(bufferSize);
            bytesRead = buffer.Length;
            writer.Write(buffer);
        }
        while (bytesRead > 0);
    }
}