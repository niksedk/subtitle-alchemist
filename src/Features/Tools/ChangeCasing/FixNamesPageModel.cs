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

    public FixNamesPage? Page { get; set; }

    private Subtitle _subtitle;

    private NameList? _nameList;
    private List<string> _nameListInclMulti;
    private string _language;
    private const string ExpectedEndChars = " ,.!?:;…')]<-\"\r\n";
    private readonly HashSet<string> _usedNames;

    public FixNamesPageModel()
    {
        _names = new ObservableCollection<FixNameItem>();
        _hits = new ObservableCollection<FixNameHitItem>();

        _nameListInclMulti = new List<string>();
        _language = "en_US";
        _subtitle = new Subtitle();
        _usedNames = new HashSet<string>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Subtitle"] is Subtitle subtitle)
        {
            _subtitle = new Subtitle(subtitle, false);
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
                _nameListInclMulti = _nameList.GetAllNames(); // Will contains both one word names and multi names

                FindAllNames();

                Hits.Add(new FixNameHitItem("Name", 1, "Before", "After"));
            });
            return false;
        });
    }

    private void FindAllNames()
    {
        var text = HtmlUtil.RemoveHtmlTags(_subtitle.GetAllTexts());
        var textToLower = text.ToLowerInvariant();
        Names.Clear();
        foreach (var name in _nameListInclMulti)
        {
            var startIndex = textToLower.IndexOf(name.ToLowerInvariant(), StringComparison.Ordinal);
            if (startIndex >= 0)
            {
                while (startIndex >= 0 && startIndex < text.Length &&
                       textToLower.Substring(startIndex).Contains(name.ToLowerInvariant()) && name.Length > 1 && name != name.ToLowerInvariant())
                {
                    var startOk = startIndex == 0 || "([ --'>\r\n¿¡\"”“„".Contains(text[startIndex - 1]);
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
                                var isDont = _language.StartsWith("en", StringComparison.OrdinalIgnoreCase) && text.Substring(startIndex).StartsWith("don't", StringComparison.InvariantCultureIgnoreCase);
                                if (!isDont)
                                {
                                    _usedNames.Add(name);
                                    Names.Add(new FixNameItem(name, true));
                                    break; // break while
                                }
                            }
                        }
                    }

                    startIndex = textToLower.IndexOf(name.ToLowerInvariant(), startIndex + 2, StringComparison.Ordinal);
                }
            }
        }

        //TODO: groupBoxNames.Text = string.Format(LanguageSettings.Current.ChangeCasingNames.NamesFoundInSubtitleX, listViewNames.Items.Count);
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

        await Shell.Current.GoToAsync("../..", new Dictionary<string, object>
        {
            { "Page", nameof(FixNamesPage) },
            { "Subtitle", subtitle },
            { "NoOfLinesChanged", 1 },
            { "Status", "status" },
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
}
