﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Forms.FixCommonErrors;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Config.Language;
using System.Collections.ObjectModel;
using System.Text;
using Nikse.SubtitleEdit.Core.Interfaces;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public partial class FixCommonErrorsModel : ObservableObject, IQueryAttributable, IFixCallbacks
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<LanguageDisplayItem> _languages = new();

    [ObservableProperty]
    private ObservableCollection<FixRuleDisplayItem> _fixItems = new();

    private List<FixRuleDisplayItem> _allFixItems = new();


    public FixCommonErrorsPage? Page { get; set; }
    public Grid? Step1Grid { get; set; }
    public Grid? Step2Grid { get; set; }
    public Entry EntrySearch { get; set; } = new();

    private Subtitle _originalSubtitle = new();
    private Subtitle _fixSubtitle = new();
    private readonly LanguageFixCommonErrors _language;
    private int _totalFixes;
    private int _totalErrors;

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
    }

    private void ApplyFixes()
    {
        _totalFixes = 0;
        _totalErrors = 0;

        foreach (var fix in _allFixItems)
        {
            if (fix.IsSelected)
            {
                var fixCommonError = fix.GetFixCommonErrorFunction();
                fixCommonError.Fix(_originalSubtitle, this);
            }
        }
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
            Page.Content = Step1Grid;
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    public void InitStep1()
    {
        var languages = new List<LanguageDisplayItem>();
        foreach (var ci in Utilities.GetSubtitleLanguageCultures(true))
        {
            languages.Add(new LanguageDisplayItem(ci, ci.EnglishName));
        }
        Languages = new ObservableCollection<LanguageDisplayItem>(languages);


        _allFixItems = new List<FixRuleDisplayItem>
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
//                new FixErrorDisplayItem(_language.FixCommonOcrErrors, _language.FixOcrErrorExample, () => FixOcrErrorsViaReplaceList(threeLetterIsoLanguageName), ce.FixOcrErrorsViaReplaceListTicked),
            new (_language.FixUppercaseIInsideLowercaseWords, _language.FixUppercaseIInsideLowercaseWordsExample, 1, true, nameof(FixUppercaseIInsideWords)),
            new (_language.RemoveSpaceBetweenNumber, _language.FixSpaceBetweenNumbersExample, 1, true, nameof(RemoveSpaceBetweenNumbers)),
            new (_language.BreakDialogsOnOneLine, _language.FixDialogsOneLineExample, 1, true, nameof(FixDialogsOnOneLine)),
            new (_language.RemoveDialogFirstInNonDialogs, _language.RemoveDialogFirstInNonDialogsExample, 1, true, nameof(RemoveDialogFirstLineInNonDialogs)),
            new (_language.NormalizeStrings, string.Empty, 1, true, nameof(NormalizeStrings)),
    };

        //if (Configuration.Settings.General.ContinuationStyle == ContinuationStyle.None)
        //{
        //    _fixActions.Add(new FixErrorDisplayItem(_language.FixEllipsesStart, _language.FixEllipsesStartExample, 1, true, nameof(FixEllipsesStart)),
        //}
        //if (Language == "en")
        //{
        //    _indexAloneLowercaseIToUppercaseIEnglish = _fixActions.Count;
        //    _fixActions.Add(new FixErrorDisplayItem(_language.FixLowercaseIToUppercaseI, _language.FixLowercaseIToUppercaseIExample, 1, true, nameof(FixAloneLowercaseIToUppercaseI)),
        //}
        //if (Language == "tr")
        //{
        //    _turkishAnsiIndex = _fixActions.Count;
        //    _fixActions.Add(new FixErrorDisplayItem(_language.FixTurkishAnsi, "Ý > İ, Ð > Ğ, Þ > Ş, ý > ı, ð > ğ, þ > ş", 1, true, nameof(FixTurkishAnsiToUnicode)),
        //}

        //if (Language == "da")
        //{
        //    _danishLetterIIndex = _fixActions.Count;
        //    _fixActions.Add(new FixErrorDisplayItem(_language.FixDanishLetterI, "Jeg synes i er søde. -> Jeg synes I er søde.", 1, true, nameof(FixDanishLetterI)),
        //}

        //if (Language == "es")
        //{
        //    _spanishInvertedQuestionAndExclamationMarksIndex = _fixActions.Count;
        //    _fixActions.Add(new FixErrorDisplayItem(_language.FixSpanishInvertedQuestionAndExclamationMarks, "Hablas bien castellano? -> ¿Hablas bien castellano?", 1, true, nameof(FixSpanishInvertedQuestionAndExclamationMarks)),
        //}

        FixItems = new ObservableCollection<FixRuleDisplayItem>(_allFixItems);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _originalSubtitle = subtitle;
            _fixSubtitle = new Subtitle(subtitle, false);
        }
    }

    public void EntrySearch_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            FixItems = new ObservableCollection<FixRuleDisplayItem>(_allFixItems);
            return;
        }

        var searchText = e.NewTextValue.ToLower();
        var items = _allFixItems
            .Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                        p.Example.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        FixItems = new ObservableCollection<FixRuleDisplayItem>(items);
    }

    public bool AllowFix(Paragraph p, string action)
    {
        return true;
    }

    public void AddFixToListView(Paragraph p, string action, string before, string after)
    {
        //
    }

    public void AddFixToListView(Paragraph p, string action, string before, string after, bool isChecked)
    {
        //
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

    public SubtitleFormat Format
    {
        get
        {
            return new SubRip();
        }
    }


    public Encoding Encoding
    {
        get
        {
            return Encoding.UTF8;
        }
    }

    public string Language
    {
        get
        {
            return "en";
        }
    }
}