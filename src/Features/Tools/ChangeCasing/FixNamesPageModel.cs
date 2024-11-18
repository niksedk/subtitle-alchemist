using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Dictionaries;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Dictionaries;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public partial class FixNamesPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<FixNameItem> _names;

    [ObservableProperty]
    private ObservableCollection<FixNameHitItem> _hits;

    [ObservableProperty]
    private string _namesCount;

    [ObservableProperty]
    private string _hitCount;

    [ObservableProperty]
    private string _extraNames;

    public FixNamesPage? Page { get; set; }

    private Subtitle _subtitle;
    private Subtitle _subtitleBefore;
    private NameList? _nameList;
    private List<string> _nameListInclMulti;
    private string _language;
    private const string ExpectedEndChars = " ,.!?:;�')]<-\"\r\n";
    private readonly HashSet<string> _usedNames;
    private bool _dirty;
    private readonly System.Timers.Timer _previewTimer;
    private bool _loading;
    private readonly object _lock = new object();

    public FixNamesPageModel()
    {
        _names = new ObservableCollection<FixNameItem>();
        _hits = new ObservableCollection<FixNameHitItem>();

        _loading = true;
        _namesCount = string.Empty;
        _hitCount = string.Empty;
        _nameListInclMulti = new List<string>();
        _language = "en_US";
        _subtitle = new Subtitle();
        _usedNames = new HashSet<string>();
        _extraNames = string.Empty;

        _previewTimer = new System.Timers.Timer(500);
        _previewTimer.Elapsed += (sender, args) =>
        {
            if (_dirty && !_loading)
            {
                lock (_lock)
                {
                    GeneratePreview();
                    _dirty = false;
                }
            }
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
        }

        if (query["SubtitleBefore"] is Subtitle subtitleBefore)
        {
            _subtitleBefore = new Subtitle(subtitleBefore, false);
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                _language = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);
                if (string.IsNullOrEmpty(_language))
                {
                    _language = "en_US";
                }

                await DictionaryLoader.UnpackIfNotFound();
                _nameList = new NameList(Se.DictionariesFolder, _language, Configuration.Settings.WordLists.UseOnlineNames, Configuration.Settings.WordLists.NamesUrl);

                ExtraNames = Se.Settings.Tools.ChangeCasing.ExtraNames;
                FindAllNames();
                GeneratePreview();
                _previewTimer.Start();
                _loading = false;
            });
            return false;
        });
    }

    private void FindAllNames()
    {
        var text = HtmlUtil.RemoveHtmlTags(_subtitle.GetAllTexts());
        var textToLower = text.ToLowerInvariant();

        _nameListInclMulti = _nameList!.GetAllNames(); // Will contains both one word names and multi names
        foreach (var s in ExtraNames.Split(','))
        {
            var name = s.Trim();
            if (name.Length > 1 && !_nameListInclMulti.Contains(name))
            {
                _nameListInclMulti.Add(name);
            }
        }

        _usedNames.Clear();
        var names = new List<FixNameItem>();
        foreach (var name in _nameListInclMulti)
        {
            var startIndex = textToLower.IndexOf(name.ToLowerInvariant(), StringComparison.Ordinal);
            if (startIndex >= 0)
            {
                while (startIndex >= 0 && startIndex < text.Length &&
                       textToLower.Substring(startIndex).Contains(name.ToLowerInvariant()) && name.Length > 1 && name != name.ToLowerInvariant())
                {
                    var startOk = startIndex == 0 || "([ --'>\r\n��\"���".Contains(text[startIndex - 1]);
                    if (startOk)
                    {
                        var end = startIndex + name.Length;
                        var endOk = end <= text.Length;
                        if (endOk)
                        {
                            endOk = end == text.Length || ExpectedEndChars.Contains(text[end]);
                        }

                        if (endOk && text.Substring(startIndex, name.Length) != name) // do not add names where casing already is correct
                        {
                            if (!_usedNames.Contains(name))
                            {
                                var skip = false;
                                var isChecked = true;
                                if (_language.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                                {
                                    var isDont = text.Substring(startIndex).StartsWith("don't", StringComparison.InvariantCultureIgnoreCase);
                                    if (isDont)
                                    {
                                        skip = true;
                                    }

                                    var commonNamesAndWords = new List<string>
                                    {
                                        "US",
                                        "Lane",
                                        "Bill",
                                        "Rose",
                                    };
                                    if (commonNamesAndWords.Contains(name))
                                    {
                                        isChecked = false;
                                    }
                                }

                                if (!skip)
                                {
                                    _usedNames.Add(name);
                                    names.Add(new FixNameItem(name, isChecked));
                                    break; // break while
                                }
                            }
                        }
                    }

                    startIndex = textToLower.IndexOf(name.ToLowerInvariant(), startIndex + 2, StringComparison.Ordinal);
                }
            }
        }

        Names = new ObservableCollection<FixNameItem>(names);
        NamesCount = string.Format("Names: {0:#,##0}",  Names.Count);
    }

    private void GeneratePreview()
    {
        var hits = new List<FixNameHitItem>();
        foreach (var p in _subtitle.Paragraphs)
        {
            var text = p.Text;
            foreach (var item in Names)
            {
                var name = item.Name;

                var textNoTags = HtmlUtil.RemoveHtmlTags(text, true);
                if (textNoTags != textNoTags.ToUpperInvariant())
                {
                    if (item.IsChecked && text != null && text.Contains(name, StringComparison.OrdinalIgnoreCase) && name.Length > 1 && name != name.ToLowerInvariant())
                    {
                        var st = new StrippableText(text);
                        st.FixCasing(new List<string> { name }, true, false, false, string.Empty);
                        text = st.MergedString;
                    }
                }
            }

            if (text != p.Text)
            {
                var hit = new FixNameHitItem(p.Text, p.Number, p.Text, text, true);
                hits.Add(hit);
            }
        }

        Hits = new ObservableCollection<FixNameHitItem>(hits);
        HitCount = string.Format("Hits: {0:#,##0}", Hits.Count);
    }

    [RelayCommand]
    public void NamesSelectAll()
    {
        foreach (var name in Names)
        {
            name.IsChecked = true;
        }
    }

    [RelayCommand]
    public void NamesInvertSelection()
    {
        foreach (var name in Names)
        {
            name.IsChecked = !name.IsChecked;
        }
    }


    [RelayCommand]
    private async Task Ok()
    {
        var subtitle = new Subtitle(_subtitle, false);

        foreach (var hit in Hits)
        {
            if (hit.IsEnabled)
            {
                subtitle.Paragraphs[hit.LineIndex].Text = hit.After;
            }
        }

        Se.Settings.Tools.ChangeCasing.ExtraNames = ExtraNames;

        var noOfLinesChanged = 0;
        for (var i = 0; i < _subtitle.Paragraphs.Count; i++)
        {
            if (_subtitleBefore.Paragraphs[i].Text != subtitle.Paragraphs[i].Text)
            {
                noOfLinesChanged++;
            }
        }
        var info = $"Change casing - lines changed: {noOfLinesChanged}";

        await Shell.Current.GoToAsync("../..", new Dictionary<string, object>
        {
            { "Page", nameof(FixNamesPage) },
            { "Subtitle", subtitle },
            { "NoOfLinesChanged", noOfLinesChanged },
            { "Status", info },
        });
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("../..", new Dictionary<string, object>
        {
            { "Page", nameof(FixNamesPage) },
        });
    }

    [RelayCommand]
    public void AddExtraName()
    {
        _loading = true;
        FindAllNames();
        _loading = false;
        _dirty = true;
    }

    public void OnNameToggled(object? sender, ToggledEventArgs e)
    {
        if (_loading)
        {
            return;
        }   

        _dirty = true;
    }
}
