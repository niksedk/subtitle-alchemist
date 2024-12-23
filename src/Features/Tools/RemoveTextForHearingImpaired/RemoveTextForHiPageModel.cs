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
    [ObservableProperty] private bool _isRemoveCurlyBracketsOn;
    [ObservableProperty] private bool _isRemoveParenthesesOn;
    [ObservableProperty] private bool _isRemoveCustomOn;
    [ObservableProperty] private string _customStart;
    [ObservableProperty] private string _customEnd;
    [ObservableProperty] private bool _isOnlySeparateLine;

    [ObservableProperty] private bool _isRemoveTextBeforeColonOn;
    [ObservableProperty] private bool _isRemoveTextBeforeColonUppercaseOn;
    [ObservableProperty] private bool _isRemoveTextBeforeColonSeparateLineOn;

    [ObservableProperty] private bool _isRemoveTextUppercaseLineOn;

    [ObservableProperty] private bool _isRemoveTextContainsOn;
    [ObservableProperty] private string _textContains;

    [ObservableProperty] private bool _isRemoveOnlyMusicSymbolsOn;

    [ObservableProperty] private bool _isRemoveInterjectionsOn;
    [ObservableProperty] private bool _isInterjectionsSeparateLineOn;

    [ObservableProperty] private DisplayFile? _selectedFile;

    [ObservableProperty] private ObservableCollection<LanguageItem> _languages;
    [ObservableProperty] private LanguageItem? _selectedLanguage;

    [ObservableProperty] private ObservableCollection<RemoveItem> _fixes;

    public RemoveTextForHiPage? Page { get; set; }

    private Subtitle _subtitle;
    private RemoveTextForHI? _removeTextForHiLib;
    private readonly System.Timers.Timer _timer;

    public RemoveTextForHiPageModel()
    {
        _customStart = "?";
        _customEnd = "?";
        _textContains = string.Empty;
        _languages = new ObservableCollection<LanguageItem>();
        _fixes = new ObservableCollection<RemoveItem>();
        _subtitle = new Subtitle();

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

        foreach (var fix in Fixes)
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

        var newFixes = new List<RemoveItem>();
        for (var index = 0; index < _subtitle.Paragraphs.Count; index++)
        {
            var p = _subtitle.Paragraphs[index];
            _removeTextForHiLib.WarningIndex = index - 1;
            //if (_edited.Contains(p))
            //{
            //    count++;
            //    var old = _editedOld.First(x => x.Id == p.Id);
            //    AddToListView(old, p.Text);
            //    _fixes.Add(old, p.Text);
            //}
            //else
            //{
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
            }
            //}
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
}
