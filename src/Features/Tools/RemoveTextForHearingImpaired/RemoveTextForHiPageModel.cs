using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Forms;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Logic.Config;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public partial class RemoveTextForHiPageModel : ObservableObject, IQueryAttributable
{
    public class LanguageItem
    {
        public CultureInfo Code { get; }
        public string Name { get; }

        public LanguageItem(CultureInfo code, string name)
        {
            Code = code;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [ObservableProperty] public partial bool IsRemoveBracketsOn { get; set; }
    [ObservableProperty]
    public partial bool IsRemoveCurlyBracketsOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveParenthesesOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveCustomOn { get; set; }

    [ObservableProperty]
    public partial string CustomStart { get; set; }

    [ObservableProperty]
    public partial string CustomEnd { get; set; }

    [ObservableProperty]
    public partial bool IsOnlySeparateLine { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveTextBeforeColonOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveTextBeforeColonUppercaseOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveTextBeforeColonSeparateLineOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveTextUppercaseLineOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveTextContainsOn { get; set; }

    [ObservableProperty]
    public partial string TextContains { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveOnlyMusicSymbolsOn { get; set; }

    [ObservableProperty]
    public partial bool IsRemoveInterjectionsOn { get; set; }

    [ObservableProperty]
    public partial bool IsInterjectionsSeparateLineOn { get; set; }

    [ObservableProperty]
    public partial DisplayFile? SelectedFile { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<LanguageItem> Languages { get; set; }

    [ObservableProperty]
    public partial LanguageItem? SelectedLanguage { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<RemoveItem> Fixes { get; set; }

    [ObservableProperty]
    public partial RemoveItem? SelectedFix { get; set; }

    [ObservableProperty]
    public partial string FixText { get; set; }

    [ObservableProperty]
    public partial bool FixTextEnabled { get; set; }
    public RemoveTextForHiPage? Page { get; set; }

    private Subtitle _subtitle;
    private RemoveTextForHI? _removeTextForHiLib;
    private readonly System.Timers.Timer _timer;
    private readonly List<Paragraph> _edited;

    public RemoveTextForHiPageModel()
    {
        CustomStart = "?";
        CustomEnd = "?";
        TextContains = string.Empty;
        Languages = new ObservableCollection<LanguageItem>();
        Fixes = new ObservableCollection<RemoveItem>();
        _subtitle = new Subtitle();
        FixText = string.Empty;
        _edited = new List<Paragraph>();
        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += TimerElapsed;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        try
        {
            MainThread.BeginInvokeOnMainThread(GeneratePreview);
        }
        catch
        {
            return;
        }

        _timer.Start();
    }

    [RelayCommand]
    public async Task EditInterjections()
    {
        await Shell.Current.GoToAsync(nameof(EditInterjectionsPage), new Dictionary<string, object>
        {
            { "Page", nameof(RemoveTextForHiPage) },
            { "Language", SelectedLanguage?.Name ?? "English" },
            { "TwoLetterLanguageCode", SelectedLanguage?.Code.TwoLetterISOLanguageName ?? "en" },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Ok()
    {
        _timer.Stop();
        SaveSettings();
        GeneratePreview();

        foreach (var fix in Fixes.Where(f => f.Apply))
        {
            fix.Paragraph.Text = fix.After;
        }

        var i = _subtitle.Paragraphs.Count - 1;
        while (i >= 0)
        {
            var fix = Fixes
                .FirstOrDefault(p =>
                    p.Paragraph.Id == _subtitle.Paragraphs[i].Id &&
                    p.Apply &&
                    string.IsNullOrEmpty(p.After));

            if (fix != null)
            {
                _subtitle.Paragraphs.RemoveAt(i);
            }

            i--;
        }

        _subtitle.Renumber();

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(RemoveTextForHiPage) },
            { "Subtitle", _subtitle },
            { "Count", Fixes.Count(p => p.Apply ) },
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var page = query["Page"].ToString();

        if (page == nameof(EditInterjectionsPage))
        {
            Fixes.Clear();
            _timer.Start();
            return;
        }

        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                InitializeLanguages();
                LoadSettings();
                _removeTextForHiLib = new RemoveTextForHI(GetSettings(_subtitle));
                GeneratePreview();
                _timer.Start();
            });
            return false;
        });
    }

    private void InitializeLanguages()
    {
        Languages.Clear();
        var language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
        foreach (var ci in Utilities.GetSubtitleLanguageCultures(true).OrderBy(p => p.EnglishName))
        {
            Languages.Add(new LanguageItem(ci, ci.EnglishName));
            if (ci.TwoLetterISOLanguageName == language)
            {
                SelectedLanguage = Languages.LastOrDefault();
            }
        }

        if (SelectedLanguage == null)
        {
            SelectedLanguage = Languages.FirstOrDefault();
        }
    }

    private void GeneratePreview()
    {
        if (_removeTextForHiLib == null)
        {
            return;
        }

        _removeTextForHiLib.Settings = GetSettings(_subtitle);
        _removeTextForHiLib.Warnings = new List<int>();

        _removeTextForHiLib.ReloadInterjection(SelectedLanguage?.Code.TwoLetterISOLanguageName ?? "en");

        var count = 0;
        var newFixes = new List<RemoveItem>();
        for (var index = 0; index < _subtitle.Paragraphs.Count; index++)
        {
            var p = _subtitle.Paragraphs[index];
            _removeTextForHiLib.WarningIndex = index - 1;
            if (_edited.Contains(p))
            {
                var editedParagraph = _edited.First(x => x.Id == p.Id);
                var newText = editedParagraph.Text;

                var apply = true;
                var oldItem = Fixes.FirstOrDefault(f => f.Index == index);
                if (oldItem != null)
                {
                    apply = oldItem.Apply;
                }

                var item = new RemoveItem(apply, index, p.Text, newText, p);
                newFixes.Add(item); ;
                count++;
            }
            else
            {
                var newText = _removeTextForHiLib.RemoveTextFromHearImpaired(p.Text, _subtitle, index, SelectedLanguage == null ? "en" : SelectedLanguage.Code.TwoLetterISOLanguageName);
                if (p.Text.RemoveChar(' ') != newText.RemoveChar(' '))
                {
                    var apply = true;
                    var oldItem = Fixes.FirstOrDefault(f => f.Index == index);
                    if (oldItem != null)
                    {
                        apply = oldItem.Apply;
                    }

                    var item = new RemoveItem(apply, index, p.Text, newText, p);
                    newFixes.Add(item);
                    count++;
                }
            }
        }

        if (newFixes.Count == Fixes.Count)
        {
            var same = true;
            for (var i = 0; i < newFixes.Count; i++)
            {
                if (newFixes[i].Index != Fixes[i].Index ||
                    newFixes[i].Before != Fixes[i].Before ||
                    newFixes[i].After != Fixes[i].After)
                {
                    same = false;
                    break;
                }
            }

            if (same)
            {
                return; // no changes
            }
        }

        Fixes = new ObservableCollection<RemoveItem>(newFixes);

        //groupBoxLinesFound.Text = string.Format(_language.LinesFoundX, count);
    }

    public RemoveTextForHISettings GetSettings(Subtitle subtitle)
    {
        var textContainsList = TextContains.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

        var settings = new RemoveTextForHISettings(subtitle)
        {
            OnlyIfInSeparateLine = IsOnlySeparateLine,
            RemoveIfAllUppercase = IsRemoveTextUppercaseLineOn,
            RemoveTextBeforeColon = IsRemoveTextBeforeColonOn,
            RemoveTextBeforeColonOnlyUppercase = IsRemoveTextBeforeColonUppercaseOn,
            ColonSeparateLine = IsRemoveTextBeforeColonSeparateLineOn,
            RemoveWhereContains = IsRemoveTextContainsOn,
            RemoveIfTextContains = textContainsList,
            RemoveTextBetweenCustomTags = IsRemoveCustomOn,
            RemoveInterjections = IsRemoveInterjectionsOn,
            RemoveInterjectionsOnlySeparateLine = IsRemoveInterjectionsOn && IsInterjectionsSeparateLineOn,
            RemoveTextBetweenSquares = IsRemoveBracketsOn,
            RemoveTextBetweenBrackets = IsRemoveCurlyBracketsOn,
            RemoveTextBetweenQuestionMarks = false,
            RemoveTextBetweenParentheses = IsRemoveParenthesesOn,
            RemoveIfOnlyMusicSymbols = IsRemoveOnlyMusicSymbolsOn,
            CustomStart = CustomStart,
            CustomEnd = CustomEnd,
        };

        foreach (var item in TextContains.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            settings.RemoveIfTextContains.Add(item.Trim());
        }

        return settings;
    }


    private void LoadSettings()
    {
        var settings = Se.Settings.Tools.RemoveTextForHi;

        IsRemoveBracketsOn = settings.IsRemoveBracketsOn;
        IsRemoveCurlyBracketsOn = settings.IsRemoveCurlyBracketsOn;
        IsRemoveParenthesesOn = settings.IsRemoveParenthesesOn;
        IsRemoveCustomOn = settings.IsRemoveCustomOn;
        CustomStart = settings.CustomStart;
        CustomEnd = settings.CustomEnd;
        IsOnlySeparateLine = settings.IsOnlySeparateLine;

        IsRemoveTextBeforeColonOn = settings.IsRemoveTextBeforeColonOn;
        IsRemoveTextBeforeColonUppercaseOn = settings.IsRemoveTextBeforeColonUppercaseOn;
        IsRemoveTextBeforeColonSeparateLineOn = settings.IsRemoveTextBeforeColonSeparateLineOn;

        IsRemoveTextUppercaseLineOn = settings.IsRemoveTextUppercaseLineOn;

        IsRemoveTextContainsOn = settings.IsRemoveTextContainsOn;
        TextContains = settings.TextContains;

        IsRemoveOnlyMusicSymbolsOn = settings.IsRemoveOnlyMusicSymbolsOn;

        IsRemoveInterjectionsOn = settings.IsRemoveInterjectionsOn;
        IsInterjectionsSeparateLineOn = settings.IsInterjectionsSeparateLineOn;
    }

    private void SaveSettings()
    {
        var settings = Se.Settings.Tools.RemoveTextForHi;

        settings.IsRemoveBracketsOn = IsRemoveBracketsOn;
        settings.IsRemoveCurlyBracketsOn = IsRemoveCurlyBracketsOn;
        settings.IsRemoveParenthesesOn = IsRemoveParenthesesOn;
        settings.IsRemoveCustomOn = IsRemoveCustomOn;
        settings.CustomStart = CustomStart;
        settings.CustomEnd = CustomEnd;
        settings.IsOnlySeparateLine = IsOnlySeparateLine;

        settings.IsRemoveTextBeforeColonOn = IsRemoveTextBeforeColonOn;
        settings.IsRemoveTextBeforeColonUppercaseOn = IsRemoveTextBeforeColonUppercaseOn;
        settings.IsRemoveTextBeforeColonSeparateLineOn = IsRemoveTextBeforeColonSeparateLineOn;

        settings.IsRemoveTextUppercaseLineOn = IsRemoveTextUppercaseLineOn;

        settings.IsRemoveTextContainsOn = IsRemoveTextContainsOn;
        settings.TextContains = TextContains;

        settings.IsRemoveOnlyMusicSymbolsOn = IsRemoveOnlyMusicSymbolsOn;

        settings.IsRemoveInterjectionsOn = IsRemoveInterjectionsOn;
        settings.IsInterjectionsSeparateLineOn = IsInterjectionsSeparateLineOn;

        Se.SaveSettings();
    }

    internal void FixSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is RemoveItem item)
        {
            FixText = item.After;
            FixTextEnabled = true;
        }
        else
        {
            FixText = string.Empty;
            FixTextEnabled = false;
        }
    }

    internal void FixTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (SelectedFix is RemoveItem item)
        {
            var p = new Paragraph(item.Paragraph, false);
            if (!_edited.Contains(p) && FixText != item.After)
            {
                p.Text = FixText;
                _edited.Add(p);
            }
        }
    }
}
