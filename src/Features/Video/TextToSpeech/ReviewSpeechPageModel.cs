using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Translate;
using SubtitleAlchemist.Controls.PickerControl;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Features.Translate;
using SubtitleAlchemist.Features.Video.TextToSpeech.Engines;
using SubtitleAlchemist.Features.Video.TextToSpeech.Voices;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public partial class ReviewSpeechPageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<TranslateRow> _lines;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private ObservableCollection<IAutoTranslator> _autoTranslators;

    [ObservableProperty]
    private IAutoTranslator _selectedAutoTranslator;

    private bool _translationInProgress;
    private bool _abort;
    private bool _singleLineMode;
    private CancellationTokenSource _cancellationTokenSource = new();
    private int _translationProgressIndex;
    private List<string> _apiUrls = new();
    private List<string> _apiModels = new();
    private bool _onlyCurrentLine;

    public ReviewSpeechPage? Page { get; set; }

    private readonly IPopupService _popupService;

    public ReviewSpeechPageModel(IPopupService popupService)
    {
        Paragraphs = new ObservableCollection<DisplayParagraph>();

        _popupService = popupService;

        Lines = new ObservableCollection<TranslateRow>();

        AutoTranslators = new ObservableCollection<IAutoTranslator>
        {
            new GoogleTranslateV1(),
            new GoogleTranslateV2(),
            new MicrosoftTranslator(),
            new DeepLTranslate(),
            new LibreTranslate(),
            new MyMemoryApi(),
            new ChatGptTranslate(),
            new LmStudioTranslate(),
            new OllamaTranslate(),
            new AnthropicTranslate(),
            new GroqTranslate(),
            new OpenRouterTranslate(),
            new GeminiTranslate(),
            new PapagoTranslate(),
            new NoLanguageLeftBehindServe(),
            new NoLanguageLeftBehindApi(),
        };
        SelectedAutoTranslator = AutoTranslators[0];
    }

    private Encoding _encoding = Encoding.UTF8;
    private Subtitle _sourceSubtitle = new();
    private Subtitle _targetSubtitle = new();

    [ObservableProperty]
    private ObservableCollection<TranslationPair> _sourceLanguages = new();

    [ObservableProperty]
    private ObservableCollection<TranslationPair> _targetLanguages = new();

    [ObservableProperty]
    private TranslationPair? _sourceLanguage;

    [ObservableProperty]
    private TranslationPair? _targetLanguage;

    [ObservableProperty]
    private ObservableCollection<string> _formalities = new();

    [ObservableProperty]
    private string? _selectedFormality;


    public Picker SourceLanguagePicker { get; set; } = new();
    public Picker TargetLanguagePicker { get; set; } = new();

    public ProgressBar ProgressBar { get; set; } = new();

    public Picker EnginePicker { get; set; } = new();

    public Label TitleLabel { get; set; } = new();

    public Label LabelApiKey { get; set; } = new();
    public Entry EntryApiKey { get; set; } = new();

    public Label LabelApiUrl { get; set; } = new();
    public Entry EntryApiUrl { get; set; } = new();
    public Button ButtonApiUrl { get; set; } = new();

    public Label LabelFormality { get; set; } = new();
    public Picker PickerFormality { get; set; } = new();

    public Label LabelModel { get; set; } = new();
    public Entry EntryModel { get; set; } = new();
    public Button ButtonModel { get; set; } = new();
    public CollectionView CollectionView { get; set; } = new();
    public Button ButtonTranslate { get; set; } = new();
    public Button ButtonOk { get; set; } = new();
    public Button ButtonCancel { get; set; } = new();

    private Voice _voice;
    private ITtsEngine _engine;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["StepResult"] is TtsStepResult[] stepResult)
        {
            Paragraphs =new ObservableCollection<DisplayParagraph>(stepResult.Select(p => new DisplayParagraph(p.Paragraph)).ToList());;

            Lines.Clear();
            foreach (var p in Paragraphs)
            {
                Lines.Add(new TranslateRow
                {
                    Number = p.Number,
                    StartTime = p.Start,
                    OriginalText = p.Text,
                    TranslatedText = string.Empty,
                });
            }
        }

        if (query["Voice"] is Voice voice)
        {
            _voice = voice;
        }

        if (query["Engine"] is ITtsEngine engine)
        {
            _engine = engine;
        }
    }

    [RelayCommand]
    public async Task Translate()
    {
    }

    private void SaveSettings(Type engineType)
    {
        var apiKey = EntryApiKey.Text ?? string.Empty;
        var apiUrl = EntryApiUrl.Text ?? string.Empty;
        var apiModel = EntryModel.Text ?? string.Empty;

        if (engineType == typeof(GoogleTranslateV2))
        {
            Configuration.Settings.Tools.GoogleApiV2Key = apiKey.Trim();
        }

        if (engineType == typeof(MicrosoftTranslator))
        {
            Configuration.Settings.Tools.MicrosoftTranslatorApiKey = apiKey.Trim();
        }

        if (engineType == typeof(DeepLTranslate))
        {
            Configuration.Settings.Tools.AutoTranslateDeepLUrl = apiUrl.Trim();
            Configuration.Settings.Tools.AutoTranslateDeepLApiKey = apiKey.Trim();
        }

        if (engineType == typeof(LibreTranslate))
        {
            Configuration.Settings.Tools.AutoTranslateLibreUrl = apiUrl.Trim();
            Configuration.Settings.Tools.AutoTranslateLibreApiKey = apiKey.Trim();
        }

        if (engineType == typeof(MyMemoryApi))
        {
            Configuration.Settings.Tools.AutoTranslateMyMemoryApiKey = apiKey.Trim();
        }

        if (engineType == typeof(ChatGptTranslate))
        {
            Configuration.Settings.Tools.ChatGptApiKey = apiKey.Trim();
            Configuration.Settings.Tools.ChatGptUrl = apiUrl.Trim();
            Configuration.Settings.Tools.ChatGptModel = apiModel.Trim();
        }

        if (engineType == typeof(LmStudioTranslate))
        {
            Configuration.Settings.Tools.LmStudioApiUrl = apiUrl.Trim();
            Configuration.Settings.Tools.LmStudioModel = apiModel.Trim();
        }

        if (engineType == typeof(OllamaTranslate))
        {
            Configuration.Settings.Tools.OllamaApiUrl = apiUrl.Trim();
            Configuration.Settings.Tools.OllamaModel = apiModel.Trim();
        }

        if (engineType == typeof(AnthropicTranslate))
        {
            Configuration.Settings.Tools.AnthropicApiKey = apiKey.Trim();
            Configuration.Settings.Tools.AnthropicApiModel = apiModel.Trim();
        }

        if (engineType == typeof(GroqTranslate))
        {
            Configuration.Settings.Tools.GroqApiKey = apiKey.Trim();
            Configuration.Settings.Tools.GroqModel = apiModel.Trim();
        }

        if (engineType == typeof(OpenRouterTranslate))
        {
            Configuration.Settings.Tools.OpenRouterApiKey = apiKey.Trim();
            Configuration.Settings.Tools.OpenRouterModel = apiModel.Trim();
        }

        if (engineType == typeof(GeminiTranslate))
        {
            Configuration.Settings.Tools.GeminiProApiKey = apiKey.Trim();
        }

        if (engineType == typeof(PapagoTranslate))
        {
            Configuration.Settings.Tools.AutoTranslatePapagoApiKeyId = apiUrl.Trim();
            Configuration.Settings.Tools.AutoTranslatePapagoApiKey = apiKey.Trim();
        }

        Configuration.Settings.Tools.AutoTranslateLastName = SelectedAutoTranslator.Name;
        Se.Settings.Tools.AutoTranslateLastName = SelectedAutoTranslator.Name;

        Se.SaveSettings();
    }

    public void EngineSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is Picker { SelectedItem: IAutoTranslator translator })
        {
            SelectedAutoTranslator = translator;
            SetAutoTranslatorEngine(translator);
            SetupLanguageSettings(translator);
        }
    }

    private void SetAutoTranslatorEngine(IAutoTranslator translator)
    {
        TitleLabel.Text = translator.Name;

        EntryApiKey.IsVisible = false;
        EntryApiKey.Text = string.Empty;
        LabelApiKey.IsVisible = false;
        EntryApiUrl.IsVisible = false;
        EntryApiUrl.Text = string.Empty;
        LabelApiUrl.IsVisible = false;
        ButtonApiUrl.IsVisible = false;
        LabelFormality.IsVisible = false;
        PickerFormality.IsVisible = false;
        LabelModel.IsVisible = false;
        EntryModel.IsVisible = false;
        ButtonModel.IsVisible = false;
        EntryModel.Text = string.Empty;
        LabelApiUrl.Text = "API url";
        LabelApiKey.Text = "API key";

        _apiUrls.Clear();
        _apiModels.Clear();

        var engineType = translator.GetType();

        if (engineType == typeof(GoogleTranslateV1))
        {
            return;
        }

        if (engineType == typeof(GoogleTranslateV2))
        {
            EntryApiKey.Text = Configuration.Settings.Tools.GoogleApiV2Key;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;
            return;
        }

        if (engineType == typeof(MicrosoftTranslator))
        {
            EntryApiKey.Text = Configuration.Settings.Tools.MicrosoftTranslatorApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;
            return;
        }

        if (engineType == typeof(DeepLTranslate))
        {
            LabelFormality.IsVisible = true;
            PickerFormality.IsVisible = true;

            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.AutoTranslateDeepLUrl,
                Configuration.Settings.Tools.AutoTranslateDeepLUrl.Contains("api-free.deepl.com") ? "https://api.deepl.com/" : "https://api-free.deepl.com/",
            });

            EntryApiKey.Text = Configuration.Settings.Tools.AutoTranslateDeepLApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;

            SelectFormality();

            return;
        }

        if (engineType == typeof(NoLanguageLeftBehindServe))
        {
            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.AutoTranslateNllbServeUrl,
                "http://127.0.0.1:6060/",
                "http://192.168.8.127:6060/",
            });

            return;
        }

        if (engineType == typeof(NoLanguageLeftBehindApi))
        {
            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.AutoTranslateNllbApiUrl,
                "http://localhost:7860/api/v2/",
                "https://winstxnhdw-nllb-api.hf.space/api/v2/",
            });

            return;
        }

        if (engineType == typeof(LibreTranslate))
        {
            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.AutoTranslateLibreUrl,
                "http://localhost:5000/",
                "https://libretranslate.com/",
                "https://translate.argosopentech.com/",
                "https://translate.terraprint.co/",
            });

            EntryApiKey.Text = Configuration.Settings.Tools.AutoTranslateLibreApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;

            return;
        }

        if (engineType == typeof(PapagoTranslate))
        {
            LabelApiUrl.Text = "Client ID";
            EntryApiUrl.Text = Configuration.Settings.Tools.AutoTranslatePapagoApiKeyId;
            EntryApiUrl.IsVisible = true;
            LabelApiUrl.IsVisible = true;

            LabelApiKey.Text = "Client secret";
            EntryApiKey.Text = Configuration.Settings.Tools.AutoTranslatePapagoApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;

            return;
        }


        if (engineType == typeof(MyMemoryApi))
        {
            EntryApiKey.Text = Configuration.Settings.Tools.AutoTranslateMyMemoryApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;
            return;
        }

        if (engineType == typeof(ChatGptTranslate))
        {
            Configuration.Settings.Tools.ChatGptUrl ??= "https://api.openai.com/v1/chat/completions";

            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.ChatGptUrl.TrimEnd('/'),
                Configuration.Settings.Tools.ChatGptUrl.StartsWith("http://localhost:1234/v1/chat/completions", StringComparison.OrdinalIgnoreCase) ? "https://api.openai.com/v1/chat/completions" : "http://localhost:1234/v1/chat/completions"
            });

            LabelModel.IsVisible = true;
            EntryModel.IsVisible = true;
            ButtonModel.IsVisible = true;
            _apiModels = ChatGptTranslate.Models.ToList();

            if (string.IsNullOrWhiteSpace(Configuration.Settings.Tools.ChatGptModel))
            {
                Configuration.Settings.Tools.ChatGptModel = ChatGptTranslate.Models[0];
            }

            EntryModel.Text = Configuration.Settings.Tools.ChatGptModel;

            EntryApiKey.Text = Configuration.Settings.Tools.ChatGptApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;
            return;
        }

        if (engineType == typeof(LmStudioTranslate))
        {
            if (string.IsNullOrEmpty(Configuration.Settings.Tools.LmStudioApiUrl))
            {
                Configuration.Settings.Tools.LmStudioApiUrl = "http://localhost:1234/v1/chat/completions";
            }

            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.LmStudioApiUrl.TrimEnd('/'),
            });

            return;
        }

        if (engineType == typeof(OllamaTranslate))
        {
            if (Configuration.Settings.Tools.OllamaApiUrl == null)
            {
                Configuration.Settings.Tools.OllamaApiUrl = "http://localhost:11434/api/generate";
            }

            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.OllamaApiUrl.TrimEnd('/'),
            });

            _apiModels = Configuration.Settings.Tools.OllamaModels.Split(',').ToList();
            EntryModel.IsVisible = true;
            LabelModel.IsVisible = true;
            ButtonModel.IsVisible = true;
            EntryModel.Text = Configuration.Settings.Tools.OllamaModel;

            //comboBoxFormality.ContextMenuStrip = contextMenuStripOlamaModels;

            return;
        }

        if (engineType == typeof(AnthropicTranslate))
        {
            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.AnthropicApiUrl,
            });

            EntryApiKey.Text = Configuration.Settings.Tools.AnthropicApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;

            _apiModels = AnthropicTranslate.Models.ToList();
            EntryModel.IsVisible = true;
            LabelModel.IsVisible = true;
            ButtonModel.IsVisible = true;
            EntryModel.Text = Configuration.Settings.Tools.AnthropicApiModel;

            return;
        }

        if (engineType == typeof(GroqTranslate))
        {
            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.GroqUrl,
            });

            EntryApiKey.Text = Configuration.Settings.Tools.GroqApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;

            _apiModels = GroqTranslate.Models.ToList();
            EntryModel.IsVisible = true;
            LabelModel.IsVisible = true;
            ButtonModel.IsVisible = true;
            EntryModel.Text = string.IsNullOrEmpty(Configuration.Settings.Tools.GroqModel) ? _apiModels[0] : Configuration.Settings.Tools.GroqModel;
            EntryModel.IsVisible = true;

            return;
        }


        if (engineType == typeof(OpenRouterTranslate))
        {
            FillUrls(new List<string>
            {
                Configuration.Settings.Tools.OpenRouterUrl,
            });

            EntryApiKey.Text = Configuration.Settings.Tools.OpenRouterApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;

            _apiModels = OpenRouterTranslate.Models.ToList();
            EntryModel.IsVisible = true;
            LabelModel.IsVisible = true;
            ButtonModel.IsVisible = true;
            EntryModel.Text = string.IsNullOrEmpty(Configuration.Settings.Tools.OpenRouterModel) ? _apiModels[0] : Configuration.Settings.Tools.OpenRouterModel;
            EntryModel.IsVisible = true;

            return;
        }

        if (engineType == typeof(GeminiTranslate))
        {
            EntryApiKey.Text = Configuration.Settings.Tools.GeminiProApiKey;
            LabelApiKey.IsVisible = true;
            EntryApiKey.IsVisible = true;
            return;
        }

        throw new Exception($"Engine {translator.Name} not handled!");
    }

    private void FillUrls(List<string> urls)
    {
        EntryApiUrl.Text = urls.Count > 0 ? urls[0] : string.Empty;
        _apiUrls = urls;
        EntryApiUrl.IsVisible = true;
        LabelApiUrl.IsVisible = true;
        ButtonApiUrl.IsVisible = urls.Count > 0;
    }

    private void SelectFormality()
    {
        LabelFormality.IsVisible = true;
        PickerFormality.IsVisible = true;

        foreach (var formality in Formalities)
        {
            if (formality == Configuration.Settings.Tools.AutoTranslateDeepLFormality)
            {
                SelectedFormality = formality;
                return;
            }
        }

        if (Formalities.Count > 0)
        {
            SelectedFormality = Formalities[0];
        }
    }

    private void SetupLanguageSettings(IAutoTranslator autoTranslator)
    {
        var sourceLanguages = autoTranslator.GetSupportedSourceLanguages();
        SourceLanguages.Clear();
        foreach (var sourceLanguage in sourceLanguages)
        {
            SourceLanguages.Add(sourceLanguage);
        }
        var sourceLanguageIsoCode = EvaluateDefaultSourceLanguageCode(_encoding, _sourceSubtitle, sourceLanguages);
        SourceLanguage = SelectLanguageCode(SourceLanguagePicker, sourceLanguageIsoCode, sourceLanguages);

        var targetLanguages = autoTranslator.GetSupportedTargetLanguages();
        TargetLanguages.Clear();
        foreach (var targetLanguage in targetLanguages)
        {
            TargetLanguages.Add(targetLanguage);
        }
        var targetLanguageIsoCode = EvaluateDefaultTargetLanguageCode(sourceLanguageIsoCode);
        TargetLanguage = SelectLanguageCode(TargetLanguagePicker, targetLanguageIsoCode, targetLanguages);
    }

    public static TranslationPair? SelectLanguageCode(Picker comboBox, string languageIsoCode, List<TranslationPair> translationPairs)
    {
        var i = 0;
        var threeLetterLanguageCode = Iso639Dash2LanguageCode.GetThreeLetterCodeFromTwoLetterCode(languageIsoCode);

        foreach (var comboBoxItem in comboBox.Items)
        {
            var item = translationPairs.FirstOrDefault(p => p.Name.Equals(comboBoxItem, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(item.TwoLetterIsoLanguageName) && item.TwoLetterIsoLanguageName == languageIsoCode)
            {
                return item;
            }

            if (item.Code.Contains('-'))
            {
                var arr = item.Code.ToLowerInvariant().Split('-');
                if (arr[0].Length == 2 && arr[0] == languageIsoCode)
                {
                    return item;
                }

                if (arr[0].Length == 3 && arr[0] == languageIsoCode)
                {
                    return item;
                }

                if (arr[1].Length == 2 && arr[1] == languageIsoCode)
                {
                    return item;
                }

                if (arr[1].Length == 3 && arr[1] == languageIsoCode)
                {
                    return item;
                }
            }

            if (languageIsoCode.Length == 2 && item.Code == languageIsoCode)
            {
                comboBox.SelectedIndex = i;
                return item;
            }

            if (!string.IsNullOrEmpty(threeLetterLanguageCode) && item.Code.StartsWith(threeLetterLanguageCode) || item.Code == languageIsoCode)
            {
                comboBox.SelectedIndex = i;
                return item;
            }

            i++;
        }

        return translationPairs.FirstOrDefault();
    }

    public static string EvaluateDefaultSourceLanguageCode(Encoding encoding, Subtitle subtitle, List<TranslationPair> sourceLanguages)
    {
        var defaultSourceLanguageCode = LanguageAutoDetect.AutoDetectGoogleLanguage(encoding); // Guess language via encoding
        if (string.IsNullOrEmpty(defaultSourceLanguageCode))
        {
            defaultSourceLanguageCode = LanguageAutoDetect.AutoDetectGoogleLanguage(subtitle); // Guess language based on subtitle contents
        }

        if (!string.IsNullOrEmpty(Configuration.Settings.Tools.GoogleTranslateLastSourceLanguage) &&
            Configuration.Settings.Tools.GoogleTranslateLastTargetLanguage.StartsWith(defaultSourceLanguageCode) &&
            sourceLanguages.Any(p => p.Code == Configuration.Settings.Tools.GoogleTranslateLastSourceLanguage))
        {
            return Configuration.Settings.Tools.GoogleTranslateLastSourceLanguage;
        }

        return defaultSourceLanguageCode;
    }

    public static string EvaluateDefaultTargetLanguageCode(string defaultSourceLanguage)
    {
        var installedLanguages = new List<string>(); // Get installed languages

#if WINDOWS
        foreach (var x in Windows.Globalization.ApplicationLanguages.ManifestLanguages)
        {
            if (x is string s)
            {
                if (s.Contains('-'))
                {
                    installedLanguages.Add(s.Split('-')[0]);
                }
                else
                {
                    installedLanguages.Add(s);
                }
            }
        }
#endif

        var uiCultureTargetLanguage = Configuration.Settings.Tools.GoogleTranslateLastTargetLanguage;
        if (uiCultureTargetLanguage == defaultSourceLanguage)
        {
            foreach (var s in Utilities.GetDictionaryLanguages())
            {
                var temp = s.Replace("[", string.Empty).Replace("]", string.Empty);
                if (temp.Length > 4)
                {
                    temp = temp.Substring(temp.Length - 5, 2).ToLowerInvariant();
                    if (temp != defaultSourceLanguage && installedLanguages.Any(p => p.Contains(temp)))
                    {
                        uiCultureTargetLanguage = temp;
                        break;
                    }
                }
            }
        }

        if (uiCultureTargetLanguage == defaultSourceLanguage)
        {
            foreach (var language in installedLanguages)
            {
                if (language != defaultSourceLanguage)
                {
                    uiCultureTargetLanguage = language;
                    break;
                }
            }
        }

        if (uiCultureTargetLanguage == defaultSourceLanguage)
        {
            var name = CultureInfo.CurrentCulture.Name;
            if (name.Length > 2)
            {
                name = name.Remove(0, name.Length - 2);
            }
            var iso = IsoCountryCodes.ThreeToTwoLetterLookup.FirstOrDefault(p => p.Value == name);
            if (!iso.Equals(default(KeyValuePair<string, string>)))
            {
                var iso639 = Iso639Dash2LanguageCode.GetTwoLetterCodeFromThreeLetterCode(iso.Key);
                if (!string.IsNullOrEmpty(iso639))
                {
                    uiCultureTargetLanguage = iso639;
                }
            }
        }

        // Set target language to something different than source language
        if (uiCultureTargetLanguage == defaultSourceLanguage && defaultSourceLanguage == "en")
        {
            uiCultureTargetLanguage = "es";
        }
        else if (uiCultureTargetLanguage == defaultSourceLanguage)
        {
            uiCultureTargetLanguage = "en";
        }

        return uiCultureTargetLanguage;
    }

    public void MouseEnteredPoweredBy()
    {
        TitleLabel.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void MouseExitedPoweredBy()
    {
        TitleLabel.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public void MouseClickedPoweredBy(object? sender, TappedEventArgs e)
    {
        UiUtil.OpenUrl(SelectedAutoTranslator.Url);
    }

    [RelayCommand]
    public async Task Cancel()
    {
        _abort = true;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Ok()
    {
        SaveSettings(SelectedAutoTranslator.GetType());

        var anyLinesTranslated = Enumerable.Any<TranslateRow>(Lines, p => !string.IsNullOrWhiteSpace(p.TranslatedText));

        if (anyLinesTranslated)
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
            {
                { "Page", nameof(TranslatePage) },
                { "Encoding", Encoding.UTF8 },
                { "TranslatedRows", Enumerable.ToList<TranslateRow>(Lines) },
            });
        }
        else
        {
            await Cancel();
        }
    }

    public void CollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_translationInProgress)
        {
            return;
        }

        if (e.PreviousSelection.FirstOrDefault() is TranslateRow rowOld)
        {
            rowOld.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
        }

        if (e.CurrentSelection.FirstOrDefault() is TranslateRow row)
        {
            row.BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
        }
    }

    public void TargetLanguagePickerSelectedIndexChanged(object? sender, EventArgs e)
    {
        var translator = SelectedAutoTranslator;

        if (TargetLanguage == null)
        {
            return;
        }

        var target = TargetLanguage;
        if (translator.Name == DeepLTranslate.StaticName)
        {
            if (target.HasFormality is null or false)
            {
                LabelFormality.IsEnabled = false;
                PickerFormality.IsEnabled = false;
                return;
            }

            LabelFormality.IsEnabled = true;
            PickerFormality.IsEnabled = true;

            if (target.TwoLetterIsoLanguageName == "ja" && Formalities.Count != 3)
            {
                Formalities.Clear();
                Formalities.Add("default");
                PickerFormality.Items.Add("more");
                Formalities.Add("less");

                SelectFormality();
            }
            else if (Formalities.Count != 3)
            {
                Formalities.Clear();
                Formalities.Add("default");
                Formalities.Add("more");
                Formalities.Add("less");
                Formalities.Add("prefer_more");
                Formalities.Add("prefer_less");

                SelectFormality();
            }
        }

        if (Formalities.Count > 0 && SelectedFormality == null)
        {
            SelectedFormality = Formalities[0];
        }
    }

    [RelayCommand]
    public async Task PickApiUrl()
    {
        var result = await _popupService.ShowPopupAsync<PickerPopupModel>(
        onPresenting: vm =>
        {
            vm.SetItems(_apiUrls, EntryApiUrl.Text);
            vm.SetSelectedItem(EntryApiUrl.Text);
        },
        CancellationToken.None);

        if (result is string s)
        {
            EntryApiUrl.Text = s;
        }
    }

    [RelayCommand]
    public async Task PickModel()
    {
        var result = await _popupService.ShowPopupAsync<PickerPopupModel>(
        onPresenting: vm =>
        {
            vm.SetItems(_apiModels, EntryModel.Text);
            vm.SetSelectedItem(EntryModel.Text);
        },
        CancellationToken.None);

        if (result is string s)
        {
            EntryModel.Text = s;
        }
    }

    [RelayCommand]
    public async Task TranslateFromCurrentLine()
    {
        await Translate();
    }

    [RelayCommand]
    public async Task TranslateCurrentLineOnly()
    {
        if (_onlyCurrentLine)
        {
            return;
        }

        _onlyCurrentLine = true;
        await Translate();
    }

    [RelayCommand]
    public async Task ShowAdvancedSettings()
    {
        var translator = SelectedAutoTranslator as IAutoTranslator;

        await _popupService.ShowPopupAsync<TranslateAdvancedSettingsPopupModel>(
            onPresenting: vm =>
            {
                vm.AutoTranslator = translator;
            },
            CancellationToken.None);
    }
}