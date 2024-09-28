using Nikse.SubtitleEdit.Core.Common;
using System.Text.RegularExpressions;
using SubtitleAlchemist.Logic.Dictionaries;
using WeCantSpell.Hunspell;
using System.Globalization;
using Nikse.SubtitleEdit.Core.SpellCheck;

namespace SubtitleAlchemist.Logic;

public class SpellCheckManager : ISpellCheckManager
{
    public delegate void SpellCheckWordChangedHandler(object sender, SpellCheckWordChangedEvent e);
    public event SpellCheckWordChangedHandler? OnWordChanged;

    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
    private static readonly Regex UrlRegex = new(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex HashtagRegex = new(@"^#[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    private static readonly Regex NumberRegex = new(@"^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$", RegexOptions.Compiled);
    private static readonly Regex PercentageRegex = new(@"^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?%$", RegexOptions.Compiled);
    private static readonly Regex CurrencyRegex = new(@"^[$£€]?-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$", RegexOptions.Compiled);

    private WordList? _hunspellDictionary;
    private SpellCheckResult? _currentResult;
    private readonly INamesList _namesList;
    private readonly HashSet<string> _ignoredWords;
    private readonly Dictionary<string, string> _replaceList;

    public SpellCheckManager(INamesList namesList)
    {
        _namesList = namesList;
        _ignoredWords = new HashSet<string>();
        _replaceList = new Dictionary<string, string>();
    }

    public List<SpellCheckDictionaryDisplay> GetDictionaryLanguages(string dictionaryFolder)
    {
        var list = new List<SpellCheckDictionaryDisplay>();
        if (!Directory.Exists(dictionaryFolder))
        {
            return list;
        }

        foreach (var dic in Directory.GetFiles(dictionaryFolder, "*.dic"))
        {
            var name = Path.GetFileNameWithoutExtension(dic);
            if (!name.StartsWith("hyph", StringComparison.Ordinal))
            {
                try
                {
                    var ci = CultureInfo.GetCultureInfo(name.Replace('_', '-'));
                    name = ci.DisplayName + " [" + name + "]";
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    name = "[" + name + "]";
                }

                var item = new SpellCheckDictionaryDisplay
                {
                    DictionaryFileName = dic,
                    Name = name,
                };
                list.Add(item);
            }
        }

        return list;
    }

    public bool Initialize(string dictionaryFile)
    {
        _ignoredWords.Clear();

        if (File.Exists(dictionaryFile))
        {
            _hunspellDictionary = WordList.CreateFromFiles(dictionaryFile);
            return true;
        }

        return false;
    }

    public bool Initialize(List<string> words)
    {
        _ignoredWords.Clear();

        _hunspellDictionary = WordList.CreateFromWords(words);
        return true;
    }

    public List<SpellCheckResult> CheckSpelling(Subtitle subtitle, SpellCheckResult? startFrom = null)
    {
        var results = new List<SpellCheckResult>();

        var startLineIndex = startFrom?.LineIndex ?? 0;
        var startWordIndex = startFrom?.WordIndex ?? 0;
        if (startFrom != null)
        {
            startWordIndex++;
        }

        for (var lineIndex = startLineIndex; lineIndex < subtitle.Paragraphs.Count; lineIndex++)
        {
            var p = subtitle.Paragraphs[lineIndex];
            var words = SpellCheckWordLists.Split(p.Text);

            for (var wordIndex = startWordIndex; wordIndex < words.Count; wordIndex++)
            {
                var word = words[wordIndex];
                if (!IsWordCorrect(word.Text, p.Text, words, wordIndex))
                {
                    results.Add(new SpellCheckResult
                    {
                        Paragraph = p,
                        LineIndex = lineIndex,
                        WordIndex = wordIndex,
                        Word = word,
                        Suggestions = GetSuggestions(word.Text),
                        IsCommonMisspelling = IsCommonMisspelling(word.Text),
                    });

                    _currentResult = results.Last();
                    return results;
                }
            }

            startWordIndex = 0;
        }

        return results;
    }

    public List<string> GetSuggestions(string word)
    {
        if (_hunspellDictionary == null)
        {
            return new List<string>();
        }

        var suggestions = _hunspellDictionary.Suggest(word);
        return suggestions.ToList();
    }

    public void AddIgnoreWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return;
        }

        if (!_ignoredWords.Contains(word))
        {
            _ignoredWords.Add(word);
        }

        var lowerWord = word.ToLowerInvariant();
        if (!_ignoredWords.Contains(lowerWord))
        {
            _ignoredWords.Add(lowerWord);
        }

        var upperWord = word.ToUpperInvariant();
        if (!_ignoredWords.Contains(upperWord))
        {
            _ignoredWords.Add(upperWord);
        }

        if (word.Length > 1)
        {
            var titleWord = char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();
            if (!_ignoredWords.Contains(titleWord))
            {
                _ignoredWords.Add(titleWord);
            }
        }
    }

    public void ChangeWord(string fromWord, string toWord, SpellCheckWord spellCheckWord)
    {
        if (_currentResult == null)
        {
            return;
        }

        var text = _currentResult.Paragraph.Text.Remove(spellCheckWord.Index, spellCheckWord.Length);
        text = text.Insert(spellCheckWord.Index, toWord);
        _currentResult.Paragraph.Text = text;

        OnWordChanged?.Invoke(this, new SpellCheckWordChangedEvent
        {
            Paragraph = _currentResult.Paragraph,
            FromWord = fromWord,
            ToWord = toWord,
            Word = spellCheckWord,
        });
    }

    public void ChangeAllWord(string fromWord, string toWord, SpellCheckWord spellCheckWord)
    {
        if (string.IsNullOrWhiteSpace(fromWord) || string.IsNullOrWhiteSpace(toWord))
        {
            return;
        }

        fromWord = fromWord.Trim();
        toWord = toWord.Trim();
        if (fromWord == toWord)
        {
            return;
        }

        if (!_replaceList.ContainsKey(fromWord))
        {
            _replaceList.Add(fromWord, toWord);
        }

        ChangeWord(fromWord, toWord, spellCheckWord);
    }

    public void AddToNames(string word)
    {
    }

    public void AdToUserDictionary(string word)
    {
    }

    private bool IsWordCorrect(string word, string text, List<SpellCheckWord> words, int wordIndex)
    {
        if (_ignoredWords.Contains(word))
        {
            return true;
        }

        if (IsEmailUrlOrHashTag(word))
        {
            return true;
        }

        if (IsNumber(word))
        {
            return true;
        }

        if (IsName(word))
        {
            return true;
        }

        if (_hunspellDictionary == null)
        {
            return false;
        }

        var spellCheck = _hunspellDictionary.Check(word);
        return spellCheck;
    }

    private static bool IsEmailUrlOrHashTag(string word)
    {
        return EmailRegex.IsMatch(word) || UrlRegex.IsMatch(word) || HashtagRegex.IsMatch(word);
    }

    private static bool IsNumber(string word)
    {
        return NumberRegex.IsMatch(word) || PercentageRegex.IsMatch(word) || CurrencyRegex.IsMatch(word);
    }

    private bool IsName(string word)
    {
        return _namesList.IsName(word);
    }

    private bool IsCommonMisspelling(string word) //TODO: some auto corrections?
    {
        return false;
    }
}