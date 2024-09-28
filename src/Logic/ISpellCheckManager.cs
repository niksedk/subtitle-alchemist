using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SpellCheck;

namespace SubtitleAlchemist.Logic;

public interface ISpellCheckManager
{
    event SpellCheckManager.SpellCheckWordChangedHandler? OnWordChanged;
    List<SpellCheckResult> CheckSpelling(Subtitle subtitle, SpellCheckResult? startFrom = null);
    bool Initialize(string dictionaryFile);
    bool Initialize(List<string> words);
    void AddIgnoreWord(string word);
    void ChangeWord(string fromWord, string toWord, SpellCheckWord spellCheckWord);
    void ChangeAllWord(string fromWord, string toWord, SpellCheckWord spellCheckWord);
    void AddToNames(string currentWord);
    void AdToUserDictionary(string currentWord);
    List<SpellCheckDictionaryDisplay> GetDictionaryLanguages(string dictionaryFolder);
    List<string> GetSuggestions(string word);
}
