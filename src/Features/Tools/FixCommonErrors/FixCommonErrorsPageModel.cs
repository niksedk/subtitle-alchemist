using CommunityToolkit.Maui.Core;
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
using System.Globalization;
using System.Text;
using SubtitleAlchemist.Logic.Dictionaries;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public partial class FixCommonErrorsPageModel : ObservableObject, IQueryAttributable, IFixCallbacks
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
    private string _editText = string.Empty;

    [ObservableProperty]
    private TimeSpan _editShow;

    [ObservableProperty]
    private TimeSpan _editDuration;

    [ObservableProperty]
    private ObservableCollection<string> _profiles = new();

    [ObservableProperty]
    private string? _selectedProfile;

    public FixCommonErrorsPage? Page { get; set; }
    public Grid? Step1Grid { get; set; }
    public Grid? Step2Grid { get; set; }

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
    private LanguageDisplayItem _oldSelectedLanguage = new(CultureInfo.InvariantCulture, "English");
    private string? _oldSelectedProfile;

    private readonly IPopupService _popupService;
    private readonly INamesList _namesList;

    public FixCommonErrorsPageModel(IPopupService popupService, INamesList namesList)
    {
        _popupService = popupService;
        _namesList = namesList;
        _language = Se.Language.FixCommonErrors;

        Profiles = new ObservableCollection<string>(Se.Settings.Tools.FixCommonErrors.Profiles.Select(p => p.ProfileName));
        if (Profiles.Count == 0)
        {
            Profiles.Add("Default");
        }

        var profileName = Se.Settings.Tools.FixCommonErrors.LastProfileName;
        SelectedProfile = Profiles.Contains(profileName)
            ? profileName
            : Profiles.First();
    }

    private void UpdateRulesSelection()
    {
        if (string.IsNullOrEmpty(SelectedProfile))
        {
            return;
        }

        var profile = Se.Settings.Tools.FixCommonErrors.Profiles.FirstOrDefault(p => p.ProfileName == SelectedProfile);
        if (profile == null)
        {
            return;
        }

        foreach (var rule in FixRules)
        {
            rule.IsSelected = profile.SelectedRules.Contains(rule.Name);
        }
    }

    private void SaveRulesSelection()
    {
        if (string.IsNullOrEmpty(SelectedProfile))
        {
            return;
        }

        SaveRulesSelection(SelectedProfile);
    }

    private void SaveRulesSelection(string profileName)
    {
        var profile = Se.Settings.Tools.FixCommonErrors.Profiles.FirstOrDefault(p => p.ProfileName == profileName);
        if (profile == null)
        {
            return;
        }

        profile.SelectedRules.Clear();
        foreach (var rule in FixRules)
        {
            if (rule.IsSelected)
            {
                profile.SelectedRules.Add(rule.Name);
            }
        }

        Se.Settings.Tools.FixCommonErrors.LastProfileName = profileName;
        Se.SaveSettings();
    }

    [RelayCommand]
    public void GoToStep2()
    {
        if (Page == null)
        {
            return;
        }

        SaveRulesSelection();
        _oldSelectedLanguage = SelectedLanguage!;
        _oldSelectedProfile = SelectedProfile!;

        Page.Content = Step2Grid;
        ApplyFixes();
        _previewMode = true;
    }

    private void ApplyFixes()
    {
        _totalErrors = 0;

        var subtitle = _previewMode ? new Subtitle(_fixSubtitle, false) : _fixSubtitle;
        foreach (var paragraph in subtitle.Paragraphs)
        {
            paragraph.Text = string.Join(Environment.NewLine, paragraph.Text.SplitToLines());
        }

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
    public async Task EditProfiles()
    {
        var result = await _popupService
            .ShowPopupAsync<FixCommonErrorsProfilePopupModel>(onPresenting: viewModel
                => viewModel.SetValues(Profiles.ToList()), CancellationToken.None);

        if (result is List<string> profiles)
        {
            Profiles = new ObservableCollection<string>(profiles);
            if (Profiles.Count == 0)
            {
                Profiles.Add("Default");
            }

            SelectedProfile = SelectedProfile != null && Profiles.Contains(SelectedProfile)
                ? SelectedProfile
                : Profiles.First();

            var settingProfiles = Se.Settings.Tools.FixCommonErrors.Profiles
                .Where(p => Profiles.Contains(p.ProfileName)).ToList();
            foreach (var profile in profiles)
            {
                if (settingProfiles.All(p => p.ProfileName != profile))
                {
                    settingProfiles.Add(new SeFixCommonErrorsProfile
                    {
                        ProfileName = profile,
                        SelectedRules = new List<string>(),
                    });
                }
            }
            Se.Settings.Tools.FixCommonErrors.Profiles = settingProfiles;

            UpdateRulesSelection();
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
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _previewMode = true;
                Page.Content = Step1Grid;
                _oldFixes = new List<FixDisplayItem>();
                Fixes.Clear();
                SelectedProfile = _oldSelectedProfile ?? Profiles.FirstOrDefault();
                SelectedLanguage = _oldSelectedLanguage;
            });
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
        foreach (var fix in Fixes)
        {
            fix.IsSelected = true;
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
            await Shell.Current.GoToAsync("..");
        }
    }

    public void InitStep1(string languageCode)
    {
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
            new (_language.RemoveLineBreaksPixelWidth, string.Empty, 1, true, nameof(FixShortLinesPixelWidth)),
            new (_language.FixDoubleApostrophes, "''Has double single quotes'' -> \"Has single double quote\"", 1, true, nameof(FixDoubleApostrophes)),
            new (_language.FixMusicNotation, _language.FixMusicNotationExample, 1, true, nameof(FixMusicNotation)),
            new (_language.AddPeriods, "Hello world -> Hello world.", 1, true, nameof(FixMissingPeriodsAtEndOfLine)),
            new (_language.StartWithUppercaseLetterAfterParagraph, "p1: Foobar! || p2: foobar! -> p1: Foobar! || p2: Foobar!", 1, true, nameof(FixStartWithUppercaseLetterAfterParagraph)),
            new (_language.StartWithUppercaseLetterAfterPeriodInsideParagraph, "Hello there! how are you?  -> Hello there! How are you?", 1, true, nameof(FixStartWithUppercaseLetterAfterPeriodInsideParagraph)),
            new (_language.StartWithUppercaseLetterAfterColon, "Speaker: hello world! -> Speaker: Hello world!", 1, true, nameof(FixStartWithUppercaseLetterAfterColon)),
            new (_language.AddMissingQuotes, _language.AddMissingQuotesExample, 1, true, nameof(AddMissingQuotes)),
            new (_language.BreakDialogsOnOneLine, _language.FixDialogsOneLineExample, 1, true, nameof(FixDialogsOnOneLine)),
            new ( string.Format(_language.FixHyphensInDialogs, GetDialogStyle(Configuration.Settings.General.DialogStyle)), string.Empty, 1, true, nameof(FixHyphensInDialog)),
            new ( _language.RemoveHyphensSingleLine, "- Foobar. -> Foobar.", 1, true, nameof(FixHyphensRemoveDashSingleLine)),
            new (_language.Fix3PlusLines, "Foo</br>bar</br>baz! -> Foo bar baz!", 1, true, nameof(Fix3PlusLines)),
            new (_language.FixDoubleDash, _language.FixDoubleDashExample, 1, true, nameof(FixDoubleDash)),
            new (_language.FixDoubleGreaterThan, _language.FixDoubleGreaterThanExample, 1, true, nameof(FixDoubleGreaterThan)),
            new ( string.Format(_language.FixContinuationStyleX, Se.Language.Settings.GetContinuationStyleName(Configuration.Settings.General.ContinuationStyle)), string.Empty, 1, true, nameof(FixContinuationStyle)),
            new (_language.FixMissingOpenBracket, _language.FixMissingOpenBracketExample, 1, true, nameof(FixMissingOpenBracket)),
            //new (_language.FixCommonOcrErrors, _language.FixOcrErrorExample, 1, true, () => FixOcrErrorsViaReplaceList(threeLetterIsoLanguageName), ce.FixOcrErrorsViaReplaceListTicked),
            new (_language.FixUppercaseIInsideLowercaseWords, _language.FixUppercaseIInsideLowercaseWordsExample, 1, true, nameof(FixUppercaseIInsideWords)),
            new (_language.RemoveSpaceBetweenNumber, _language.FixSpaceBetweenNumbersExample, 1, true, nameof(RemoveSpaceBetweenNumbers)),
            new (_language.RemoveDialogFirstInNonDialogs, _language.RemoveDialogFirstInNonDialogsExample, 1, true, nameof(RemoveDialogFirstLineInNonDialogs)),
            new (_language.NormalizeStrings, string.Empty, 1, true, nameof(NormalizeStrings)),
    };

        if (Configuration.Settings.General.ContinuationStyle == ContinuationStyle.None)
        {
            _allFixRules.Add(
                new FixRuleDisplayItem(_language.FixEllipsesStart, _language.FixEllipsesStartExample, 1,
                true, nameof(FixEllipsesStart)));
        }

        if (languageCode == "en")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixLowercaseIToUppercaseI,
                _language.FixLowercaseIToUppercaseIExample, 1, true, nameof(FixAloneLowercaseIToUppercaseI)));
        }

        if (languageCode == "tr")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixTurkishAnsi,
                "Ý > İ, Ð > Ğ, Þ > Ş, ý > ı, ð > ğ, þ > ş", 1, true, nameof(FixTurkishAnsiToUnicode)));
        }

        if (languageCode == "da")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixDanishLetterI,
                "Jeg synes i er søde. -> Jeg synes I er søde.", 1, true, nameof(FixDanishLetterI)));
        }

        if (languageCode == "es")
        {
            _allFixRules.Add(new FixRuleDisplayItem(_language.FixSpanishInvertedQuestionAndExclamationMarks,
                "Hablas bien castellano? -> ¿Hablas bien castellano?", 1, true,
                nameof(FixSpanishInvertedQuestionAndExclamationMarks)));
        }

        FixRules = new ObservableCollection<FixRuleDisplayItem>(_allFixRules);
    }

        private static string GetDialogStyle(DialogType dialogStyle)
        {
            if (dialogStyle == DialogType.DashSecondLineWithoutSpace)
            {
                return Se.Language.Settings.DialogStyleDashSecondLineWithoutSpace;
            }

            if (dialogStyle == DialogType.DashSecondLineWithSpace)
            {
                return Se.Language.Settings.DialogStyleDashSecondLineWithSpace;
            }

            if (dialogStyle == DialogType.DashBothLinesWithoutSpace)
            {
                return Se.Language.Settings.DialogStyleDashBothLinesWithoutSpace;
            }

            return Se.Language.Settings.DialogStyleDashBothLinesWithSpace;
        }

    private string InitLanguage()
    {
        var languages = new List<LanguageDisplayItem>();
        foreach (var ci in Utilities.GetSubtitleLanguageCultures(true))
        {
            languages.Add(new LanguageDisplayItem(ci, ci.EnglishName));
        }

        Languages = new ObservableCollection<LanguageDisplayItem>(languages.OrderBy(p => p.ToString()));

        var languageCode =
            LanguageAutoDetect.AutoDetectGoogleLanguage(_originalSubtitle); // Guess language based on subtitle contents
        Language = languageCode;

        SelectedLanguage = Languages.FirstOrDefault(p => p.Code.TwoLetterISOLanguageName == languageCode);
        if (SelectedLanguage != null)
        {
            SelectedLanguage = Languages.First(p => p.Code.TwoLetterISOLanguageName == "en");
        }

        return languageCode;
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

        var languageCode = InitLanguage();
        InitStep1(languageCode);
        UpdateRulesSelection();
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
        return _namesList.IsName(candidate);
    }

    public HashSet<string> GetAbbreviations()
    {
        return _namesList.GetAbbreviations();
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
        if (SelectedParagraph == null)
        {
            return;
        }

        var dur = SelectedParagraph.Duration.TotalMilliseconds;
        SelectedParagraph.Start = TimeSpan.FromMilliseconds(e.NewValue);
        SelectedParagraph.End = TimeSpan.FromMilliseconds(SelectedParagraph.End.TotalMilliseconds + dur);
        SelectedParagraph.P.StartTime = new TimeCode(SelectedParagraph.Start);
        SelectedParagraph.P.EndTime = new TimeCode(SelectedParagraph.End);
    }

    public void EditDurationChanged(object? sender, ValueChangedEventArgs e)
    {
        if (SelectedParagraph == null)
        {
            return;
        }

        SelectedParagraph.Duration = TimeSpan.FromMilliseconds(e.NewValue);
        SelectedParagraph.End = TimeSpan.FromMilliseconds(SelectedParagraph.Start.TotalMilliseconds + e.NewValue);
        SelectedParagraph.P.EndTime = new TimeCode(SelectedParagraph.End);
    }

    public void PickerProfileSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_oldSelectedProfile != null)
        {
            SaveRulesSelection(_oldSelectedProfile);
        }

        UpdateRulesSelection();

        _oldSelectedProfile = SelectedProfile;
        Se.Settings.Tools.FixCommonErrors.LastProfileName = SelectedProfile ?? Profiles.First();
    }

    public void PickerLanguageSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (SelectedLanguage == null)
        {
            return;
        }

        Language = SelectedLanguage.Code.TwoLetterISOLanguageName;
        InitStep1(Language);
        UpdateRulesSelection();
    }

    public void AutoBreak(object? sender, TappedEventArgs e)
    {
        if (SelectedParagraph != null)
        {
            var text = Utilities.AutoBreakLine(Utilities.UnbreakLine(SelectedParagraph.Text));
            EditText = text;
        }
    }

    public void Unbreak(object? sender, TappedEventArgs e)
    {
        if (SelectedParagraph != null)
        {
            var text = Utilities.UnbreakLine(SelectedParagraph.Text);
            EditText = text;
        }
    }
}