using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Logic.Dictionaries;

public static class DictionaryLoader
{
    public static async Task UnpackIfNotFound()
    {
        if (!Directory.Exists(Se.DictionariesFolder))
        {
            Directory.CreateDirectory(Se.DictionariesFolder);
            await UnpackDictionaries();
        }
    }

    private static async Task UnpackDictionaries()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("Dictionaries.zip");
        using var reader = new StreamReader(stream);
        var zipUnpacker = new ZipUnpacker();
        zipUnpacker.UnpackZipStream(stream, Se.DictionariesFolder);
    }
}

