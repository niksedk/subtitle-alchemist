using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SpellCheck;

namespace SubtitleAlchemist.Logic;

public interface ISpellCheckManager
{
    event SpellCheckManager.SpellCheckWordChangedHandler? OnWordChanged;
    List<SpellCheckResult> CheckSpelling(Subtitle subtitle, SpellCheckResult? startFrom = null);
    int NoOfChangedWords { get; set; }
    int NoOfSkippedWords { get; set; }
    bool Initialize(string dictionaryFile, string twoLetterLanguageCode);
    void AddIgnoreWord(string word);
    void ChangeWord(string fromWord, string toWord, SpellCheckWord spellCheckWord, Paragraph p);
    void ChangeAllWord(string fromWord, string toWord, SpellCheckWord spellCheckWord, Paragraph p);
    void AddToNames(string currentWord);
    void AdToUserDictionary(string currentWord);
    List<SpellCheckDictionaryDisplay> GetDictionaryLanguages(string dictionaryFolder);
    List<string> GetSuggestions(string word);
}
