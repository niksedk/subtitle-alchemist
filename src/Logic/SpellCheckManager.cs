﻿using HunspellSharp;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Interfaces;
using Nikse.SubtitleEdit.Core.SpellCheck;
using System.Globalization;
using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;

namespace SubtitleAlchemist.Logic;

public class SpellCheckManager : ISpellCheckManager, IDoSpell
{
    public delegate void SpellCheckWordChangedHandler(object sender, SpellCheckWordChangedEvent e);
    public event SpellCheckWordChangedHandler? OnWordChanged;
    public int NoOfChangedWords { get; set; }
    public int NoOfSkippedWords { get; set; }

    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
    private static readonly Regex UrlRegex = new(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex HashtagRegex = new(@"^#[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    private static readonly Regex NumberRegex = new(@"^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$", RegexOptions.Compiled);
    private static readonly Regex PercentageRegex = new(@"^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?%$", RegexOptions.Compiled);
    private static readonly Regex CurrencyRegex = new(@"^[$£€]?-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$", RegexOptions.Compiled);

    private WordList? _hunspellWeCantSpell;
    private Hunspell? _hunspellSharp;
    private SpellCheckResult? _currentResult;
    private SpellCheckWordLists? _spellCheckWordLists;
    private readonly List<string> _skipAllList;
    private readonly Dictionary<string, string> _changeAllDictionary;

    public SpellCheckManager()
    {
        _skipAllList = new List<string>();
        _changeAllDictionary = new Dictionary<string, string>();
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

    public bool Initialize(string dictionaryFile, string twoLetterLanguageCode)
    {
        _skipAllList.Clear();

        if (!File.Exists(dictionaryFile))
        {
            return false;
        }

        var affixFile = Path.ChangeExtension(dictionaryFile, ".aff");
        _hunspellSharp = new Hunspell(affixFile, dictionaryFile);
        _hunspellWeCantSpell = WordList.CreateFromFiles(dictionaryFile, affixFile);

        if (string.IsNullOrEmpty(twoLetterLanguageCode))
        {
            _spellCheckWordLists = null;
        }
        else
        {
            var dictionariesFolder = Path.GetDirectoryName(dictionaryFile) ?? string.Empty;
            _spellCheckWordLists = new SpellCheckWordLists(dictionariesFolder, twoLetterLanguageCode, this);
        }

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
                if (!IsWordCorrect(word, p, words, wordIndex))
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
        if (_hunspellWeCantSpell == null)
        {
            return new List<string>();
        }

        var suggestions = _hunspellWeCantSpell.Suggest(word);
        return suggestions.ToList();
    }

    public void AddIgnoreWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return;
        }

        NoOfSkippedWords++;

        if (!_skipAllList.Contains(word))
        {
            _skipAllList.Add(word);
        }

        var lowerWord = word.ToLowerInvariant();
        if (!_skipAllList.Contains(lowerWord))
        {
            _skipAllList.Add(lowerWord);
        }

        var upperWord = word.ToUpperInvariant();
        if (!_skipAllList.Contains(upperWord))
        {
            _skipAllList.Add(upperWord);
        }

        if (word.Length > 1)
        {
            var titleWord = char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();
            if (!_skipAllList.Contains(titleWord))
            {
                _skipAllList.Add(titleWord);
            }
        }
    }

    public void ChangeWord(string fromWord, string toWord, SpellCheckWord spellCheckWord, Paragraph p)
    {
        if (_currentResult == null)
        {
            return;
        }

        var text = p.Text.Remove(spellCheckWord.Index, spellCheckWord.Length);
        text = text.Insert(spellCheckWord.Index, toWord);
        p.Text = text;

        NoOfChangedWords++;

        OnWordChanged?.Invoke(this, new SpellCheckWordChangedEvent
        {
            Paragraph = p,
            FromWord = fromWord,
            ToWord = toWord,
            Word = spellCheckWord,
        });
    }

    public void ChangeAllWord(string fromWord, string toWord, SpellCheckWord spellCheckWord, Paragraph p)
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

        if (!_changeAllDictionary.ContainsKey(fromWord))
        {
            _changeAllDictionary.Add(fromWord, toWord);
        }

        ChangeWord(fromWord, toWord, spellCheckWord, p);
    }

    public void AddToNames(string word)
    {
        _spellCheckWordLists?.AddName(word);
    }

    public void AdToUserDictionary(string word)
    {
        _spellCheckWordLists?.AddUserWord(word);
    }

    public bool DoSpell(string word)
    {
        var isCorrect = false;
        if (_hunspellWeCantSpell != null)
        {
            isCorrect = _hunspellWeCantSpell.Check(word);
        }

        if (!isCorrect && _hunspellSharp != null)
        {
            isCorrect = _hunspellSharp.Spell(word);
        }

        return isCorrect;
    }

    private bool IsWordCorrect(SpellCheckWord spellCheckWord, Paragraph p, List<SpellCheckWord> words, int wordIndex)
    {
        var word = spellCheckWord.Text;
        var text = p.Text;

        if (_skipAllList.Contains(word.ToUpperInvariant()) ||
            (word.StartsWith('\'') || word.EndsWith('\'')) && _skipAllList.Contains(word.Trim('\'').ToUpperInvariant()))
        {
            NoOfSkippedWords++;
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

        if (IsName(word, text))
        {
            return true;
        }

        if (_spellCheckWordLists != null && _spellCheckWordLists.HasUserWord(word))
        {
            return true;
        }

        if (_spellCheckWordLists != null && _spellCheckWordLists.IsWordInUserPhrases(wordIndex, words))
        {
            return true;
        }

        var isCorrect = false;
        if (_hunspellWeCantSpell != null)
        {
            isCorrect = _hunspellWeCantSpell.Check(word);
        }

        if (!isCorrect && _hunspellSharp != null)
        {
            isCorrect = _hunspellSharp.Spell(word);
        }

        if (_changeAllDictionary.ContainsKey(word) && NotSameSpecialEnding(words[wordIndex], _changeAllDictionary[word], text))
        {
            ChangeWord(word, _changeAllDictionary[word], words[wordIndex], p);
            return true;
        }
        
        if (word.EndsWith('\'') && _changeAllDictionary.ContainsKey(word.TrimEnd('\'')))
        {
            ChangeWord(word, _changeAllDictionary[word] + word.Remove(0, word.TrimEnd('\'').Length), words[wordIndex], p);
            return true;
        }

        return isCorrect;
    }

    private static bool IsEmailUrlOrHashTag(string word)
    {
        return EmailRegex.IsMatch(word) || UrlRegex.IsMatch(word) || HashtagRegex.IsMatch(word);
    }

    private static bool IsNumber(string word)
    {
        return NumberRegex.IsMatch(word) || PercentageRegex.IsMatch(word) || CurrencyRegex.IsMatch(word);
    }

    private bool IsName(string word, string text)
    {
        if (_spellCheckWordLists == null)
        {
            return false;
        }

        return _spellCheckWordLists.HasNameExtended(word, text);
    }

    private bool IsCommonMisspelling(string word) //TODO: some auto corrections?
    {
        return false;
    }

    /// <summary>
    /// Do not allow changing "Who is lookin' at X" with "lokin" word to "lokin'" via repalce word.
    /// </summary>
    private static bool NotSameSpecialEnding(SpellCheckWord spellCheckWord, string replaceWord, string text)
    {
        if (spellCheckWord.Index + spellCheckWord.Length + 1 >= text.Length)
        {
            return true;
        }

        var wordPlusOne = text.Substring(spellCheckWord.Index, spellCheckWord.Length + 1).TrimStart();
        if (replaceWord.EndsWith('\'') && !replaceWord.EndsWith("''", StringComparison.Ordinal) && wordPlusOne == replaceWord)
        {
            return false;
        }

        if (replaceWord.EndsWith('"') && !replaceWord.EndsWith("\"\"", StringComparison.Ordinal) && wordPlusOne == replaceWord)
        {
            return false;
        }

        return true;
    }

}