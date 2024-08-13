namespace SubtitleAlchemist.Logic;

public static class AssetHelper
{
    public static void CopyToAppData(string name)
    {
        var targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, name);
        if (File.Exists(targetFile))
        {
            return;
        }

        using var outputStream = File.OpenWrite(targetFile);
        using var fs = FileSystem.Current.OpenAppPackageFileAsync(name).Result;
        using var writer = new BinaryWriter(outputStream);
        using var reader = new BinaryReader(fs);
        var bytesRead = 0;
        var bufferSize = 1024;
        var buffer = new byte[bufferSize];

        do
        {
            buffer = reader.ReadBytes(bufferSize);
            bytesRead = buffer.Length;
            writer.Write(buffer);
        }
        while (bytesRead > 0);
    }
}