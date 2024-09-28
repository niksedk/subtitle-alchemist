using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using System.Collections.ObjectModel;
using System.Web;
using Nikse.SubtitleEdit.Core.SpellCheck;

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

    [ObservableProperty]
    private FormattedString _currentFormattedText;

    private bool _loading = true;
    private SpellCheckWord _currentSpellCheckWord;
    private Subtitle _subtitle = new();
    private readonly ISpellCheckManager _spellCheckManager;
    private readonly IPopupService _popupService;
    private SpellCheckResult? _lastSpellCheckResult;
    private int _totalChangedWords;

    public SpellCheckerPageModel(ISpellCheckManager spellCheckManager, IPopupService popupService)
    {
        _spellCheckManager = spellCheckManager;
        _spellCheckManager.OnWordChanged += (sender, e) =>
        {
            UpdateChangedWordInUi(e.FromWord, e.ToWord, e.WordIndex);
            _totalChangedWords++;
        };

        _popupService = popupService;

        _currentSpellCheckWord = new SpellCheckWord();
        _wordNotFoundOriginal = string.Empty;
        _currentWord = string.Empty;
        _currentText = string.Empty;
        _currentFormattedText = new FormattedString();
    }

    private void UpdateChangedWordInUi(string fromWord, string toWord, int wordIndex)
    {
        //TODO: update subtitle list view
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
            if (!string.IsNullOrEmpty(Se.Settings.SpellCheck.LastLanguageDictionaryFile))
            {
                SelectedLanguage = Languages.FirstOrDefault(l => l.DictionaryFileName == Se.Settings.SpellCheck.LastLanguageDictionaryFile);
            }

            SelectedLanguage = Languages.FirstOrDefault(l => l.Name.Contains("English", StringComparison.OrdinalIgnoreCase));

            if (SelectedLanguage == null)
            {
                SelectedLanguage = Languages[0];
            }

            _spellCheckManager.Initialize(SelectedLanguage.DictionaryFileName);

            DoSpellCheck();
        }

        _loading = false;
    }

    private void DoSpellCheck()
    {
        var results = _spellCheckManager.CheckSpelling(_subtitle, _lastSpellCheckResult);
        if (results.Count > 0)
        {
            WordNotFoundOriginal = results[0].Word.Text;
            CurrentWord = results[0].Word.Text;
            CurrentText = results[0].Paragraph.Text;
            CurrentFormattedText = HighLightCurrentWord(results[0].Word, results[0].Paragraph);
            _currentSpellCheckWord = results[0].Word;
            _lastSpellCheckResult = results[0];

            var suggestions = _spellCheckManager.GetSuggestions(results[0].Word.Text);
            Suggestions = new ObservableCollection<string>(suggestions);
            SuggestionsAvailable = true;
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    { "Page", nameof(SpellCheckerPage) },
                    { "Subtitle", _subtitle },
                    { "TotalChangedWords", _totalChangedWords },
                });
            });
        }
    }

    private static FormattedString HighLightCurrentWord(SpellCheckWord word, Paragraph paragraph)
    {
        var text = paragraph.Text.Trim();
        var pre = string.Empty;
        if (word.Index > 0)
        {
            pre = text.Substring(0, word.Index);
        }

        var post = string.Empty;
        if (word.Index + word.Text.Length < text.Length)
        {
            post = text.Substring(word.Index + word.Text.Length);
        }

        var formattedString = new FormattedString();
        formattedString.Spans.Add(new Span { FontSize = 18, Text = pre });
        formattedString.Spans.Add(new Span { FontSize = 18, Text = word.Text, TextColor = Colors.Red, FontAttributes = FontAttributes.Bold });
        formattedString.Spans.Add(new Span { FontSize = 18, Text = post });

        return formattedString;
    }

    [RelayCommand]
    public void ChangeWord()
    {
        _spellCheckManager.ChangeWord(WordNotFoundOriginal, CurrentWord, _currentSpellCheckWord);
        DoSpellCheck();
    }

    [RelayCommand]
    public void ChangeAllWords()
    {
        _spellCheckManager.ChangeAllWord(WordNotFoundOriginal, CurrentWord, _currentSpellCheckWord);
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
        UiUtil.OpenUrl("https://www.google.com/search?q=" + HttpUtility.UrlEncode(word));
    }

    [RelayCommand]
    public async Task DownloadDictionary(string word)
    {
        var result = await _popupService.ShowPopupAsync<GetDictionaryPopupModel>(onPresenting: viewModel => viewModel.Initialize(), CancellationToken.None);
        if (result is SpellCheckDictionary dictionary)
        {
            var spellCheckLanguages = _spellCheckManager.GetDictionaryLanguages(Se.DictionariesFolder);
            Languages = new ObservableCollection<SpellCheckDictionaryDisplay>(spellCheckLanguages);
            if (Languages.Count > 0)
            {
                SelectedLanguage = Languages.FirstOrDefault(l => l.DictionaryFileName == dictionary.DictionaryFileName);
                if (SelectedLanguage == null)
                {
                    SelectedLanguage = Languages[0];
                }

                DoSpellCheck();
            }
        }
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

        if (!_loading)
        {
            _spellCheckManager.Initialize(SelectedLanguage.DictionaryFileName);
            Se.Settings.SpellCheck.LastLanguageDictionaryFile = SelectedLanguage.DictionaryFileName;
            DoSpellCheck();
        }
    }
}
