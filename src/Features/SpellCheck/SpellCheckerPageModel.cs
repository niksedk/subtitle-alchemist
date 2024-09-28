using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.SpellCheck;

public partial class SpellCheckerPageModel : ObservableObject, IQueryAttributable
{
    public SpellCheckerPage? Page { get; set; }

    [ObservableProperty]
    private ObservableCollection<string> _suggestions = new();

    [ObservableProperty]
    private ObservableCollection<SpellCheckDictionaryDisplay> _languages = new();

    [ObservableProperty]
    private SpellCheckDictionaryDisplay? _selectedLanguage;

    [ObservableProperty]
    private bool _suggestionsAvailable;

    [ObservableProperty]
    private bool _editWholeText;

    [ObservableProperty]
    private bool _isPrompting;

    [ObservableProperty]
    private string _wordNotFoundOriginal;

    [ObservableProperty]
    private string _currentWord;

    [ObservableProperty]
    private string _currentText;


    private Subtitle _subtitle = new();
    private readonly ISpellCheckManager _spellCheckManager;
    private SpellCheckResult? _lastSpellCheckResult;

    public SpellCheckerPageModel(ISpellCheckManager spellCheckManager)
    {
        _spellCheckManager = spellCheckManager;

        _wordNotFoundOriginal = string.Empty;
        _currentWord = string.Empty;
        _currentText = string.Empty;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is not Subtitle subtitle)
        {
            return;
        }

        _subtitle = subtitle;
        var spellCheckLanguages = _spellCheckManager.GetDictionaryLanguages(Se.DictionariesFolder);
        Languages = new ObservableCollection<SpellCheckDictionaryDisplay>(spellCheckLanguages);
        if (Languages.Count > 0)
        {
            SelectedLanguage = Languages[0];
            _spellCheckManager.Initialize(SelectedLanguage.DictionaryFileName);

            DoSpellCheck();
        }
    }

    private void DoSpellCheck()
    {
        var results = _spellCheckManager.CheckSpelling(_subtitle, _lastSpellCheckResult);
        if (results.Count > 0)
        {
            WordNotFoundOriginal = results[0].Word;
            CurrentWord = results[0].Word;
            CurrentText = results[0].Paragraph.Text;
            _lastSpellCheckResult = results[0];

            var suggestions = _spellCheckManager.GetSuggestions(results[0].Word);
            Suggestions = new ObservableCollection<string>(suggestions);
            SuggestionsAvailable = true;
        }
    }

    [RelayCommand]
    public void ChangeWord()
    {
        DoSpellCheck();
    }

    [RelayCommand]
    public void ChangeAllWords()
    {
        _spellCheckManager.AddChangeAllWord(WordNotFoundOriginal, CurrentWord);
        DoSpellCheck();
    }

    [RelayCommand]
    public void SkipWord()
    {
        DoSpellCheck();
    }

    [RelayCommand]
    public void SkipAllWord()
    {
        _spellCheckManager.AddIgnoreWord(WordNotFoundOriginal);
        DoSpellCheck();
    }

    [RelayCommand]
    public void AddToNames()
    {
        _spellCheckManager.AddToNames(CurrentWord);
        DoSpellCheck();
    }

    [RelayCommand]
    public void AddToUserDictionary(string word)
    {
        _spellCheckManager.AdToUserDictionary(CurrentWord);
        DoSpellCheck();
    }

    [RelayCommand]
    public void GoogleIt(string word)
    {
    }

    [RelayCommand]
    public void DownloadDictionary(string word)
    {
    }

    [RelayCommand]
    public void SuggestionUseOnce(string word)
    {
    }

    [RelayCommand]
    public void SuggestionUseAlways(string word)
    {
    }

    public void LanguageChanged(object? sender, EventArgs e)
    {
        if (SelectedLanguage == null)
        {
            return;
        }

        _spellCheckManager.Initialize(SelectedLanguage.DictionaryFileName);
        DoSpellCheck();
    }
}
