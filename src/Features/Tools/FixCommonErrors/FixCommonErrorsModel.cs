using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Enums;
using Nikse.SubtitleEdit.Core.Forms.FixCommonErrors;
using Nikse.SubtitleEdit.Core.Interfaces;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Config.Language;
using System.Collections.ObjectModel;
using System.Text;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public partial class FixCommonErrorsModel : ObservableObject, IQueryAttributable, IFixCallbacks
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<LanguageDisplayItem> _languages = new();

    [ObservableProperty]
    private LanguageDisplayItem? _selectedLanguage;

    [ObservableProperty]
    private ObservableCollection<FixRuleDisplayItem> _fixRules = new();

    [ObservableProperty]
    private ObservableCollection<FixDisplayItem> _fixes = new();

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs = new();

    [ObservableProperty]
    private DisplayParagraph? _selectedParagraph;

    [ObservableProperty]
    private string _editText;

    [ObservableProperty]
    private TimeSpan _editShow = new();

    [ObservableProperty]
    private TimeSpan _editDuration = new();

    public FixCommonErrorsPage? Page { get; set; }
    public Grid? Step1Grid { get; set; }
    public Grid? Step2Grid { get; set; }
    public Entry EntrySearch { get; set; } = new();

    public SubtitleFormat Format { get; set; } = new SubRip();
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    public string Language { get; set; } = "en";

    private List<FixRuleDisplayItem> _allFixRules = new();
    private Subtitle _originalSubtitle = new();
    private Subtitle _fixSubtitle = new();
    private readonly LanguageFixCommonErrors _language;
    private int _totalFixes;
    private int _totalErrors;
    private bool _previewMode = true;
    private List<FixDisplayItem> _oldFixes = new();


    public FixCommonErrorsModel()
    {
        _language = Se.Language.FixCommonErrors;
    }

    [RelayCommand]
    public void GoToStep2()
    {
        if (Page == null)
        {
            return;
        }

        Page.Content = Step2Grid;
        ApplyFixes();
        _previewMode = true;
    }

    private void ApplyFixes()
    {
        _totalErrors = 0;

        var subtitle = _previewMode ? new Subtitle(_fixSubtitle, false) : _fixSubtitle;
        foreach (var fix in _allFixRules)
        {
            if (fix.IsSelected)
            {
                var fixCommonError = fix.GetFixCommonErrorFunction();
                fixCommonError.Fix(subtitle, this);
            }
        }

        Paragraphs = new ObservableCollection<DisplayParagraph>(_fixSubtitle.Paragraphs.Select(p => new DisplayParagraph(p)));
    }

    [RelayCommand]
    public async Task Cancel()
    {
        if (Page == null)
        {
            return;
        }

        if (Page.Content == Step2Grid)
        {
            _previewMode = true;
            Page.Content = Step1Grid;
            _oldFixes = new List<FixDisplayItem>();
            Fixes.Clear();
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public void RulesSelectAll()
    {
        foreach (var rule in FixRules)
        {
            rule.IsSelected = true;
        }
    }

    [RelayCommand]
    public void RulesInverseSelected()
    {
        foreach (var rule in FixRules)
        {
            rule.IsSelected = !rule.IsSelected;
        }
    }


    [RelayCommand]
    public void FixesSelectAll()
    {
        foreach (var rule in FixRules)
        {
            rule.IsSelected = !rule.IsSelected;
        }
    }

    [RelayCommand]
    public void FixesInverseSelected()
    {
        foreach (var fix in Fixes)
        {
            fix.IsSelected = !fix.IsSelected;
        }
    }

    [RelayCommand]
    public void RefreshFixes()
    {
        _oldFixes = new List<FixDisplayItem>(Fixes);
        Fixes.Clear();
        _previewMode = true;
        ApplyFixes();
    }

    [RelayCommand]
    public void ApplySelectedFixes()
    {
        _previewMode = false;
        ApplyFixes();

        RefreshFixes();
    }

    [RelayCommand]
    public async Task Ok()
    {
        //SaveSettings?

        if (_totalFixes > 0)
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
            {
                { "Page", nameof(FixCommonErrorsPage) },
                { "Encoding", Encoding },
                { "Subtitle", _fixSubtitle },
                { "TotalFixes", _totalFixes },
            });
        }
        else
        {
            await Cancel();
        }
    }

    public void InitStep1()
    {
        var languages = new List<LanguageDisplayItem>();
        foreach (var ci in Utilities.GetSubtitleLanguageCultures(true))
        {
            languages.Add(new LanguageDisplayItem(ci, ci.EnglishName));
        }
        Languages = new ObservableCollection<LanguageDisplayItem>(languages.OrderBy(p => p.ToString()));

        var languageCode = LanguageAutoDetect.AutoDetectGoogleLanguage(_originalSubtitle); // Guess language based on subtitle contents
        SelectedLanguage = Languages.FirstOrDefault(p => p.Code.TwoLetterISOLanguageName == languageCode);
        if (SelectedLanguage != null)
        {
            SelectedLanguage = Languages.First(p => p.Code.TwoLetterISOLanguageName == "en");
        }

        _allFixRules = new List<FixRuleDisplayItem>
        {
            new (_language.RemovedEmptyLinesUnusedLineBreaks, "Has only one valid line!</br><i> -> Has only one valid line!", 1, true, nameof(FixEmptyLines)),
            new (_language.FixOverlappingDisplayTimes, string.Empty, 1, true, nameof(FixOverlappingDisplayTimes)),
            new (_language.FixShortDisplayTimes, string.Empty, 1, true, nameof(FixShortDisplayTimes)),
            new (_language.FixLongDisplayTimes, string.Empty, 1, true, nameof(FixLongDisplayTimes)),
            new (_language.FixShortGaps, string.Empty, 1, true, nameof(FixShortGaps)),
            new (_language.FixInvalidItalicTags, _language.FixInvalidItalicTagsExample, 1, true, nameof(FixInvalidItalicTags)),
            new (_language.RemoveUnneededSpaces, _language.RemoveUnneededSpacesExample,1, true, nameof(FixUnneededSpaces)),
            new (_language.FixMissingSpaces, _language.FixMissingSpacesExample, 1, true, nameof(FixMissingSpaces)),
            new (_language.RemoveUnneededPeriods, _language.RemoveUnneededPeriodsExample, 1, true, nameof(FixUnneededPeriods)),
            new (_language.FixCommas, ",, -> ,", 1, true, nameof(FixCommas)),
            new (_language.BreakLongLines, string.Empty, 1, true, nameof(FixLongLines)),
            new (_language.RemoveLineBreaks, "Foo</br>bar! -> Foo bar!", 1, true, nameof(FixShortLines)),
            new (_language.RemoveLineBreaksAll, string.Empty, 1, true, nameof(FixShortLinesAll)),
            //new (_language.RemoveLineBreaksPixelWidth, string.Empty, 1, true, nameof(FixShortLinesPixelWidth(TextWidth.CalcPixelWidth).Fix(Subtitle, this), ce.MergeShortLinesPixelWidthTicked),
            new (_language.FixDoubleApostrophes, "''Has double single quotes'' -> \"Has single double quote\"", 1, true, nameof(FixDoubleApostrophes)),
            new (_language.FixMusicNotation, _language.FixMusicNotationExample, 1, true, nameof(FixMusicNotation)),
            new (_language.AddPeriods, "Hello world -> Hello world.", 1, true, nameof(FixMissingPeriodsAtEndOfLine)),
            new (_language.StartWithUppercaseLetterAfterParagraph, "p1: Foobar! || p2: foobar! -> p1: Foobar! || p2: Foobar!", 1, true, nameof(FixStartWithUppercaseLetterAfterParagraph)),
            new (_language.StartWithUppercaseLetterAfterPeriodInsideParagraph, "Hello there! how are you?  -> Hello there! How are you?", 1, true, nameof(FixStartWithUppercaseLetterAfterPeriodInsideParagraph)),
            new (_language.StartWithUppercaseLetterAfterColon, "Speaker: hello world! -> Speaker: Hello world!", 1, true, nameof(FixStartWithUppercaseLetterAfterColon)),
            new (_language.AddMissingQuotes, _language.AddMissingQuotesExample, 1, true, nameof(AddMissingQuotes)),
            //new ( string.Format(_language.FixHyphensInDialogs, GetDialogStyle(Configuration.Settings.General.DialogStyle)), string.Empty, 1, true, nameof(FixHyphensInDialog)),
            new ( _language.RemoveHyphensSingleLine, "- Foobar. -> Foobar.", 1, true, nameof(FixHyphensRemoveDashSingleLine)),
            new (_language.Fix3PlusLines, "Foo</br>bar</br>baz! -> Foo bar baz!", 1, true, nameof(Fix3PlusLines)),
            new (_language.FixDoubleDash, _language.FixDoubleDashExample, 1, true, nameof(FixDoubleDash)),
            new (_language.FixDoubleGreaterThan, _language.FixDoubleGreaterThanExample, 1, true, nameof(FixDoubleGreaterThan)),
            //new ( string.Format(_language.FixContinuationStyleX, UiUtil.GetContinuationStyleName(Configuration.Settings.General.ContinuationStyle)), string.Empty, 1, true, nameof(FixContinuationStyle
            //{
            //    FixAction = string.Format(LanguageSettings.Current.FixCommonErrors.FixContinuationStyleX, UiUtil.GetContinuationStyleName(Configuration.Settings.General.ContinuationStyle))
            //}.Fix(Subtitle, this), ce.FixContinuationStyleTicked),
            new (_language.FixMissingOpenBracket, _language.FixMissingOpenBracketExample, 1, true, nameof(FixMissingOpenBracket)),
            //new (_language.FixCommonOcrErrors, _language.FixOcrErrorExample, 1, true, () => FixOcrErrorsViaReplaceList(threeLetterIsoLanguageName), ce.FixOcrErrorsViaReplaceListTicked),
            new (_language.FixUppercaseIInsideLowercaseWords, _language.FixUppercaseIInsideLowercaseWordsExample, 1, true, nameof(FixUppercaseIInsideWords)),
            new (_language.RemoveSpaceBetweenNumber, _language.FixSpaceBetweenNumbersExample, 1, true, nameof(RemoveSpaceBetweenNumbers)),
            new (_language.BreakDialogsOnOneLine, _language.FixDialogsOneLineExample, 1, true, nameof(FixDialogsOnOneLine)),
            new (_language.RemoveDialogFirstInNonDialogs, _language.RemoveDialogFirstInNonDialogsExample, 1, true, nameof(RemoveDialogFirstLineInNonDialogs)),
            new (_language.NormalizeStrings, string.Empty, 1, true, nameof(NormalizeStrings)),
    };

        if (Configuration.Settings.General.ContinuationStyle == ContinuationStyle.None)
        {
            _allFixRules.Add(
                new FixRuleDisplayItem(_language.FixEllipsesStart, _language.FixEllipsesStartExample, 1,
                true, nameof(FixEllipsesStart)));
        }

        if (Language == "en")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixLowercaseIToUppercaseI,
                _language.FixLowercaseIToUppercaseIExample, 1, true, nameof(FixAloneLowercaseIToUppercaseI)));
        }

        if (Language == "tr")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixTurkishAnsi,
                "Ý > İ, Ð > Ğ, Þ > Ş, ý > ı, ð > ğ, þ > ş", 1, true, nameof(FixTurkishAnsiToUnicode)));
        }

        if (Language == "da")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixDanishLetterI,
                "Jeg synes i er søde. -> Jeg synes I er søde.", 1, true, nameof(FixDanishLetterI)));
        }

        if (Language == "es")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixSpanishInvertedQuestionAndExclamationMarks,
                "Hablas bien castellano? -> ¿Hablas bien castellano?", 1, true,
                nameof(FixSpanishInvertedQuestionAndExclamationMarks)));
        }

        FixRules = new ObservableCollection<FixRuleDisplayItem>(_allFixRules);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _originalSubtitle = subtitle;
            _fixSubtitle = new Subtitle(subtitle, false);
        }

        if (query["Encoding"] is Encoding encoding)
        {
            Encoding = encoding;
        }

        if (query["Format"] is SubtitleFormat format)
        {
            Format = format;
        }

        InitStep1();
    }

    public void EntrySearch_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            FixRules = new ObservableCollection<FixRuleDisplayItem>(_allFixRules);
            return;
        }

        var searchText = e.NewTextValue.ToLower();
        var items = _allFixRules
            .Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                        p.Example.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        FixRules = new ObservableCollection<FixRuleDisplayItem>(items);
    }

    public bool AllowFix(Paragraph p, string action)
    {
        if (_previewMode)
        {
            return true;
        }

        var allowFix = Fixes.Any(f => f.Paragraph.Id == p.Id && f.Action == action && f.IsSelected);
        return allowFix;
    }

    public void AddFixToListView(Paragraph p, string action, string before, string after)
    {
        if (!_previewMode)
        {
            return;
        }

        var oldFix = _oldFixes.FirstOrDefault(f => f.Paragraph.Id == p.Id && f.Action == action);
        var isSelected = oldFix is not { IsSelected: false };

        Fixes.Add(new FixDisplayItem(p, p.Number, action, before, after, isSelected));
    }

    public void AddFixToListView(Paragraph p, string action, string before, string after, bool isChecked)
    {
        if (!_previewMode)
        {
            return;
        }

        var oldFix = _oldFixes.FirstOrDefault(f => f.Paragraph.Id == p.Id && f.Action == action);
        var isSelected = isChecked;
        if (oldFix is { IsSelected: false })
        {
            isSelected = false;
        }

        Fixes.Add(new FixDisplayItem(p, p.Number, action, before, after, isSelected));
    }

    public void LogStatus(string sender, string message)
    {
        //
    }

    public void LogStatus(string sender, string message, bool isImportant)
    {
        //
    }

    public void UpdateFixStatus(int fixes, string message)
    {
        if (_previewMode)
        {
            return;
        }

        if (fixes > 0)
        {
            _totalFixes += fixes;
            //            LogStatus(message, string.Format(LanguageSettings.Current.FixCommonErrors.XFixesApplied, fixes));
        }
    }

    public bool IsName(string candidate)
    {
        //TODO:
        return false;
    }

    public HashSet<string> GetAbbreviations()
    {
        //TODO:
        return new HashSet<string>();
    }

    public void AddToTotalErrors(int count)
    {
        _totalErrors += count;
    }

    public void AddToDeleteIndices(int index)
    {
        //TODO:
    }

    public void OnCollectionViewFixesSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection is not { Count: 1 } || e.CurrentSelection[0] is not FixDisplayItem fixItem)
        {
            SelectedParagraph = null;
            EditText = string.Empty;
            EditShow = new TimeSpan();
            EditDuration = new TimeSpan();
            return;
        }

        var p = Paragraphs.FirstOrDefault(p => p.P.Id == fixItem.Paragraph.Id);
        SelectedParagraph = p;
    }

    public void OnCollectionViewSubtitleSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection is not { Count: 1 } || e.CurrentSelection[0] is not DisplayParagraph dp)
        {
            SelectedParagraph = null;
            EditText = string.Empty;
            EditShow = new TimeSpan();
            EditDuration = new TimeSpan();
            return;
        }

        EditText = dp.Text;
        EditShow = dp.Start;
        EditDuration = dp.Duration;
    }

    public void EditorTextTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (SelectedParagraph != null)
        {
            SelectedParagraph.Text = e.NewTextValue;
            SelectedParagraph.P.Text = e.NewTextValue;
        }
    }

    public void EditShowChanged(object? sender, ValueChangedEventArgs e)
    {
        if (SelectedParagraph != null)
        {
            var dur = SelectedParagraph.Duration.TotalMilliseconds;
            SelectedParagraph.Start = TimeSpan.FromMilliseconds(e.NewValue);
            SelectedParagraph.End = TimeSpan.FromMilliseconds(SelectedParagraph.End.TotalMilliseconds + dur);
            SelectedParagraph.P.StartTime = new TimeCode(SelectedParagraph.Start);
            SelectedParagraph.P.EndTime = new TimeCode(SelectedParagraph.End);
        }
    }

    public void EditDurationChanged(object? sender, ValueChangedEventArgs e)
    {
        if (SelectedParagraph != null)
        {
            SelectedParagraph.Duration = TimeSpan.FromMilliseconds(e.NewValue);
            SelectedParagraph.End = TimeSpan.FromMilliseconds(SelectedParagraph.Start.TotalMilliseconds + e.NewValue);
            SelectedParagraph.P.EndTime = new TimeCode(SelectedParagraph.End);
        }
    }
}