using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Translate;
using SubtitleAlchemist.Features.Main;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace SubtitleAlchemist.Features.Translate;

public partial class TranslateModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<TranslateRow> _lines;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private ObservableCollection<IAutoTranslator> _autoTranslators;

    [ObservableProperty]
    private IAutoTranslator _selectedAutoTranslator;

    private bool _translationInProgress = false;
    private bool _abort = false;
    private bool _singleLineMode = false;
    private CancellationTokenSource _cancellationTokenSource = new();
    private int _translationProgressIndex = 0;

    public TranslatePage? TranslatePage { get; set; }

    public TranslateModel()
    {
        Paragraphs = new ObservableCollection<DisplayParagraph>();

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
           // new OpenRouterTranslate(),
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


    public Picker SourceLanguagePicker { get; set; } = new();
    public Picker TargetLanguagePicker { get; set; } = new();

    public ProgressBar ProgressBar { get; set; } = new();

    public Picker EnginePicker { get; set; } = new();

    public Label TitleLabel { get; set; } = new();

    public Label LabelApiKey { get; set; } = new();
    public Entry EntryApiKey { get; set; } = new();

    public Label LabelApiUrl { get; set; } = new();
    public Entry EntryApiUrl { get; set; } = new();

    public Label LabelFormality { get; set; } = new();
    public Picker PickerFormality { get; set; } = new();

    public Label LabelModel { get; set; } = new();
    public Entry EntryModel { get; set; } = new();
    public CollectionView CollectionView { get; set; } = new();
    public Button ButtonTranslate { get; set; } = new();
    public Button ButtonOk { get; set; } = new();
    public Button ButtonCancel { get; set; } = new();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Paragraphs"] is ObservableCollection<DisplayParagraph> paragraphs)
        {
            Paragraphs = paragraphs;

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

        if (query["Encoding"] is Encoding encoding)
        {
            _encoding = encoding;
        }

        if (query["Subtitle"] is Subtitle subtitle)
        {
            _sourceSubtitle = subtitle;

            _targetSubtitle = new Subtitle(subtitle, false);
            foreach (var p in _targetSubtitle.Paragraphs)
            {
                p.Text = string.Empty;
            }
        }
    }

    [RelayCommand]
    public async Task Translate()
    {
        if (TranslatePage == null)
        {
            return;
        }

        if (_translationInProgress)
        {
            _translationInProgress = false;
            _abort = true;
            await _cancellationTokenSource.CancelAsync();
            return;
        }

        ProgressBar.IsVisible = true;
        ProgressBar.IsEnabled = true;
        ProgressBar.Progress = 0;
        _translationInProgress = true;
        _cancellationTokenSource = new CancellationTokenSource();

        var translator = SelectedAutoTranslator;
        var engineType = translator.GetType();

        if (translator.Name == DeepLTranslate.StaticName && string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            await TranslatePage.DisplayAlert("API key required", string.Format("{0} requires an API key.", translator.Name), "OK");
            _translationInProgress = false;
            _singleLineMode = false;
            return;
        }

        if (translator.Name == DeepLTranslate.StaticName && string.IsNullOrWhiteSpace(EntryApiUrl.Text))
        {
            await TranslatePage.DisplayAlert("URL key required", string.Format("{0} requires an URL.", translator.Name), "OK");
            _translationInProgress = false;
            _singleLineMode = false;
            return;
        }

        SaveSettings(engineType);

        ButtonOk.IsEnabled = false;
        ButtonCancel.IsEnabled = false;
        ButtonTranslate.Text  = "Cancel";

        translator.Initialize();

        var sourceLanguage = translator.GetSupportedSourceLanguages()
            .FirstOrDefault(p => p.Name.Equals(SourceLanguagePicker.SelectedItem.ToString(), StringComparison.InvariantCultureIgnoreCase));

        var targetLanguage = translator.GetSupportedTargetLanguages()
            .FirstOrDefault(p => p.Name.Equals(TargetLanguagePicker.SelectedItem.ToString(), StringComparison.InvariantCultureIgnoreCase));

        if (sourceLanguage == null || targetLanguage == null)
        {
            return;
        }

        Configuration.Settings.Tools.GoogleTranslateLastSourceLanguage = sourceLanguage.TwoLetterIsoLanguageName;
        Configuration.Settings.Tools.GoogleTranslateLastTargetLanguage = targetLanguage.TwoLetterIsoLanguageName;


        // do translate
        await Task.Run(async () =>
        {
            await DoTranslate(sourceLanguage, targetLanguage, translator);
        });

        ButtonOk.IsEnabled = true;
        ButtonCancel.IsEnabled = true;
        ButtonTranslate.Text = "Translate";
        _translationInProgress = false;
        _abort = false;
        ProgressBar.IsVisible = false;
        ButtonOk.Focus();
    }

    private async Task DoTranslate(TranslationPair sourceLanguage, TranslationPair targetLanguage, IAutoTranslator translator)
    {
        var start = 0;
        if (CollectionView.SelectedItem is TranslateRow selectedItem)
        {
            start = Lines.IndexOf(selectedItem);
        }

        var forceSingleLineMode = Configuration.Settings.Tools.AutoTranslateStrategy == TranslateStrategy.TranslateEachLineSeparately.ToString() ||
                                  translator.Name == NoLanguageLeftBehindApi.StaticName ||  // NLLB seems to miss some text...
                                  translator.Name == NoLanguageLeftBehindServe.StaticName ||
                                  _singleLineMode;

        var index = start;
        var linesTranslated = 0;
        var errorCount = 0;
        while (index < Lines.Count)
        {
            if (_abort)
            {
                break;
            }

            var linesMergedAndTranslated = await MergeAndSplitHelper.MergeAndTranslateIfPossible(_sourceSubtitle,
                _targetSubtitle, sourceLanguage, targetLanguage, index, translator, forceSingleLineMode,
                _cancellationTokenSource.Token);

            if (linesMergedAndTranslated > 0)
            {
                for (var j = index; j < index + linesMergedAndTranslated; j++)
                {
                    if (j < Lines.Count && j < _targetSubtitle.Paragraphs.Count)
                    {
                        Lines[j].TranslatedText = _targetSubtitle.Paragraphs[j].Text;
                    }
                }

                index += linesMergedAndTranslated;

                var index1 = index;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProgressBar.Progress = (double)index1 / Lines.Count;
                });

                linesTranslated += linesMergedAndTranslated;
                _translationProgressIndex = index - 1;
                errorCount = 0;
                continue;
            }

            errorCount++;
            forceSingleLineMode = true;

            if (errorCount > 1)
            {
                break;
            }
        }
    }

    private void SaveSettings(Type engineType)
    {
        if (engineType == typeof(MicrosoftTranslator) && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.MicrosoftTranslatorApiKey = EntryApiKey.Text.Trim();
        }

        if (engineType == typeof(DeepLTranslate) && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.AutoTranslateDeepLUrl = EntryApiUrl.Text.Trim();
            Configuration.Settings.Tools.AutoTranslateDeepLApiKey = EntryApiKey.Text.Trim();
        }

        if (engineType == typeof(LibreTranslate) && EntryApiKey.IsVisible && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.AutoTranslateLibreApiKey = EntryApiKey.Text.Trim();
        }

        if (engineType == typeof(MyMemoryApi) && EntryApiKey.IsVisible && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.AutoTranslateMyMemoryApiKey = EntryApiKey.Text.Trim();
        }

        if (engineType == typeof(ChatGptTranslate))
        {
            Configuration.Settings.Tools.ChatGptApiKey = EntryApiKey.Text.Trim();
            Configuration.Settings.Tools.ChatGptUrl = EntryApiUrl.Text.Trim();
            //Configuration.Settings.Tools.ChatGptModel = EntryFormality.Text.Trim();
        }

        if (engineType == typeof(LmStudioTranslate))
        {
            Configuration.Settings.Tools.LmStudioApiUrl = EntryApiUrl.Text.Trim();
            //Configuration.Settings.Tools.LmStudioModel = comboBoxFormality.Text.Trim();
        }

        if (engineType == typeof(OllamaTranslate))
        {
            Configuration.Settings.Tools.OllamaApiUrl = EntryApiUrl.Text.Trim();
            //Configuration.Settings.Tools.OllamaModel = comboBoxFormality.Text.Trim();
        }

        if (engineType == typeof(AnthropicTranslate) && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.AnthropicApiKey = EntryApiKey.Text.Trim();
            //Configuration.Settings.Tools.AnthropicApiModel = comboBoxFormality.Text.Trim();
        }

        if (engineType == typeof(GroqTranslate) && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.GroqApiKey = EntryApiKey.Text.Trim();
            //Configuration.Settings.Tools.GroqModel = comboBoxFormality.Text.Trim();
        }

        //if (engineType == typeof(OpenRouterTranslate) && !string.IsNullOrWhiteSpace(nikseTextBoxApiKey.Text))
        //{
        //    Configuration.Settings.Tools.OpenRouterApiKey = nikseTextBoxApiKey.Text.Trim();
        //    Configuration.Settings.Tools.OpenRouterModel = comboBoxFormality.Text.Trim();
        //}

        if (engineType == typeof(GeminiTranslate) && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.GeminiProApiKey = EntryApiKey.Text.Trim();
        }

        if (engineType == typeof(PapagoTranslate) && !string.IsNullOrWhiteSpace(EntryApiKey.Text))
        {
            Configuration.Settings.Tools.AutoTranslatePapagoApiKeyId = EntryApiUrl.Text.Trim();
            Configuration.Settings.Tools.AutoTranslatePapagoApiKey = EntryApiKey.Text.Trim();
        }
    }

    public void EngineSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is Picker { SelectedItem: IAutoTranslator translator })
        {
            SetAutoTranslatorEngine(translator);
            SetupLanguageSettings(translator);
        }
    }

    private void SetAutoTranslatorEngine(IAutoTranslator translator)
    {
        TitleLabel.Text = $"Powered by {translator.Name}";

        EntryApiKey.IsVisible = false;
        EntryApiKey.Text = string.Empty;
        LabelApiKey.IsVisible = false;
        EntryApiUrl.IsVisible = false;
        EntryApiUrl.Text = string.Empty;
        LabelApiUrl.IsVisible = false;
        PickerFormality.IsVisible = false;
        LabelFormality.IsVisible = false;
        EntryModel.IsVisible = false;
        EntryModel.Text = string.Empty;
        LabelModel.IsVisible = false;

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

            return;
        }

        if (engineType == typeof(PapagoTranslate))
        {
            EntryApiUrl.Text = Configuration.Settings.Tools.AutoTranslatePapagoApiKeyId;
            EntryApiUrl.IsVisible = true;
            LabelApiUrl.IsVisible = true;
            LabelApiUrl.Text = "Client ID";

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
            if (Configuration.Settings.Tools.ChatGptUrl == null)
            {
                Configuration.Settings.Tools.ChatGptUrl = "https://api.openai.com/v1/chat/completions";
            }

            FillUrls(new List<string>
                {
                    Configuration.Settings.Tools.ChatGptUrl.TrimEnd('/'),
                    Configuration.Settings.Tools.ChatGptUrl.StartsWith("http://localhost:1234/v1/chat/completions", StringComparison.OrdinalIgnoreCase) ? "https://api.openai.com/v1/chat/completions" : "http://localhost:1234/v1/chat/completions"
                });

            //LabelFormality.Text = "Model";
            //LabelFormality.IsVisible = true;

            //comboBoxFormality.Items.Clear();
            //comboBoxFormality.Enabled = true;
            //comboBoxFormality.Left = labelFormality.Right + 3;
            //comboBoxFormality.IsVisible = true;
            //comboBoxFormality.Items.AddRange(ChatGptTranslate.Models);
            //comboBoxFormality.Text = Configuration.Settings.Tools.ChatGptModel;

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

            var models = Configuration.Settings.Tools.OllamaModels.Split(',').ToList();

            LabelModel.IsVisible = true;

            //comboBoxFormality.DropDownStyle = ComboBoxStyle.DropDown;
            //comboBoxFormality.Items.Clear();
            //comboBoxFormality.Enabled = true;
            //comboBoxFormality.Left = labelFormality.Right + 3;
            //comboBoxFormality.IsVisible = true;
            //foreach (var model in models)
            //{
            //    comboBoxFormality.Items.Add(model);
            //}
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

            LabelModel.Text = "Model";
            LabelModel.IsVisible = true;
            //comboBoxFormality.IsVisible = true;
            //comboBoxFormality.DropDownStyle = ComboBoxStyle.DropDown;
            //comboBoxFormality.Items.Clear();
            //comboBoxFormality.Items.AddRange(AnthropicTranslate.Models);
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

            LabelModel.Text = "Model";
            LabelModel.IsVisible = true;
            //comboBoxFormality.Left = labelFormality.Right + 3;
            //comboBoxFormality.IsVisible = true;
            //comboBoxFormality.DropDownStyle = ComboBoxStyle.DropDown;
            //comboBoxFormality.Items.Clear();
            //comboBoxFormality.Items.AddRange(GroqTranslate.Models);
            EntryModel.Text = Configuration.Settings.Tools.GroqModel;
            EntryModel.IsVisible = true;

            return;
        }


        //if (engineType == typeof(OpenRouterTranslate))
        //{
        //    FillUrls(new List<string>
        //        {
        //            Configuration.Settings.Tools.OpenRouterUrl,
        //        });

        //    labelApiKey.Left = nikseComboBoxUrl.Right + 12;
        //    nikseTextBoxApiKey.Text = Configuration.Settings.Tools.OpenRouterApiKey;
        //    nikseTextBoxApiKey.Left = labelApiKey.Right + 3;
        //    labelApiKey.IsVisible = true;
        //    nikseTextBoxApiKey.IsVisible = true;

        //    labelFormality.Text = LanguageSettings.Current.AudioToText.Model;
        //    labelFormality.IsVisible = true;
        //    comboBoxFormality.Left = labelFormality.Right + 3;
        //    comboBoxFormality.IsVisible = true;
        //    comboBoxFormality.DropDownStyle = ComboBoxStyle.DropDown;
        //    comboBoxFormality.Items.Clear();
        //    comboBoxFormality.Items.AddRange(OpenRouterTranslate.Models);
        //    comboBoxFormality.Text = Configuration.Settings.Tools.OpenRouterModel;

        //    return;
        //}


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
        //nikseComboBoxUrl.Items.Clear();
        //foreach (var url in list.Distinct())
        //{
        //    if (!string.IsNullOrEmpty(url))
        //    {
        //        nikseComboBoxUrl.Items.Add(url.TrimEnd('/') + "/");
        //    }
        //}

        LabelApiUrl.Text = "Url";
        //nikseComboBoxUrl.Left = labelUrl.Right + 3;
        //if (nikseComboBoxUrl.Items.Count > 0)
        //{
        //    nikseComboBoxUrl.SelectedIndex = 0;
        //}

        EntryApiUrl.IsVisible = true;
        LabelApiUrl.IsVisible = true;
    }

    private void SelectFormality()
    {
        //comboBoxFormality.SelectedIndex = 0;
        //for (var i = 0; i < comboBoxFormality.Items.Count; i++)
        //{
        //    if (comboBoxFormality.Items[i].ToString() == Configuration.Settings.Tools.AutoTranslateDeepLFormality)
        //    {
        //        comboBoxFormality.SelectedIndex = i;
        //        break;
        //    }
        //}
    }


    private void SetupLanguageSettings(IAutoTranslator autoTranslator)
    {
        var sourceLanguages = autoTranslator.GetSupportedSourceLanguages();
        SourceLanguagePicker.Items.Clear();
        foreach (var language in sourceLanguages)
        {
            SourceLanguagePicker.Items.Add(ToProperCase(language.Name));
        }
        var sourceLanguageIsoCode = EvaluateDefaultSourceLanguageCode(_encoding, _sourceSubtitle, sourceLanguages);
        SelectLanguageCode(SourceLanguagePicker, sourceLanguageIsoCode, sourceLanguages);

        var targetLanguages = autoTranslator.GetSupportedTargetLanguages();
        TargetLanguagePicker.Items.Clear();
        foreach (var language in targetLanguages)
        {
            TargetLanguagePicker.Items.Add(ToProperCase(language.Name));
        }
        var targetLanguageIsoCode = EvaluateDefaultTargetLanguageCode(sourceLanguageIsoCode);
        SelectLanguageCode(TargetLanguagePicker, targetLanguageIsoCode, targetLanguages);
    }

    private static string ToProperCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }

    public static void SelectLanguageCode(Picker comboBox, string languageIsoCode, List<TranslationPair> translationPairs)
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
                comboBox.SelectedIndex = i;
                return;
            }

            if (item.Code.Contains('-'))
            {
                var arr = item.Code.ToLowerInvariant().Split('-');
                if (arr[0].Length == 2 && arr[0] == languageIsoCode)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }

                if (arr[0].Length == 3 && arr[0] == languageIsoCode)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }

                if (arr[1].Length == 2 && arr[1] == languageIsoCode)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }

                if (arr[1].Length == 3 && arr[1] == languageIsoCode)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            if (languageIsoCode.Length == 2 && item.Code == languageIsoCode)
            {
                comboBox.SelectedIndex = i;
                return;
            }

            if (!string.IsNullOrEmpty(threeLetterLanguageCode) && item.Code.StartsWith(threeLetterLanguageCode) || item.Code == languageIsoCode)
            {
                comboBox.SelectedIndex = i;
                return;
            }

            i++;
        }

        if (comboBox.SelectedIndex < 0 && comboBox.Items.Count > 0)
        {
            comboBox.SelectedIndex = 0;
        }
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
        TitleLabel.TextColor = Colors.BlueViolet;
        TitleLabel.TextDecorations = TextDecorations.Underline;
    }

    public void MouseExitedPoweredBy()
    {
        TitleLabel.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
        TitleLabel.TextDecorations = TextDecorations.None;
    }

    public void MouseClickedPoweredBy(object? sender, TappedEventArgs e)
    {
        UiUtil.OpenUrl(SelectedAutoTranslator.Url);
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Ok()
    {
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(GetType) },
            { "Encoding", Encoding.UTF8 },
            { "TranslatedRows", Lines },
        });
    }

    public void CollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        //e.Handled = true;
    }
}