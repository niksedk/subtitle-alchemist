using System.Xml.Serialization;

namespace SubtitleAlchemist.Features.SpellCheck;

[XmlRoot("Dictionaries")]
public class SpellCheckDictionaries
{
    [XmlElement("Dictionary")] 
    public List<SpellCheckDictionary> DictionaryList { get; set; } = new();

    public static SpellCheckDictionaries Deserialize(string xml)
    {
        var serializer = new XmlSerializer(typeof(SpellCheckDictionaries));
        using var reader = new StringReader(xml);
        return (SpellCheckDictionaries)serializer.Deserialize(reader)!;
    }

    public static async Task<List<SpellCheckDictionary>> GetDictionaryListAsync()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("HunspellDictionaries.xml");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();
        var dictionaries = Deserialize(contents);
        return dictionaries.DictionaryList;
    }
}
