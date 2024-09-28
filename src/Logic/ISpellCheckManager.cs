using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic;

public interface ISpellCheckManager
{
    List<SpellCheckResult> CheckSpelling(Subtitle subtitle, SpellCheckResult? startFrom = null);
    bool Initialize(string dictionaryFile);
    bool Initialize(List<string> words);
    void AddIgnoreWord(string word);
    void AddChangeAllWord(string fromWord, string toWord);
    void AddToNames(string currentWord);
    void AdToUserDictionary(string currentWord);
    List<SpellCheckDictionaryDisplay> GetDictionaryLanguages(string dictionaryFolder);
    List<string> GetSuggestions(string word);
}
