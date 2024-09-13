namespace SubtitleAlchemist.Logic.Dictionaries;

public interface INamesList
{
    void Load(string dictionaryFolder, string languageName);
    bool IsName(string candidate);
    HashSet<string> GetAbbreviations();
}