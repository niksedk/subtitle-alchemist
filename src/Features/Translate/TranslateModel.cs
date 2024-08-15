using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Main;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Translate;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Translate;

public partial class TranslateModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<ExcelRow> _lines;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private ObservableCollection<IAutoTranslator> _autoTranslators;

    [ObservableProperty]
    private IAutoTranslator _selectedAutoTranslator;

    public TranslateModel()
    {
        Paragraphs = new ObservableCollection<DisplayParagraph>();

        Lines = new ObservableCollection<ExcelRow>();

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
    private Subtitle _subtitle = new();

    public Picker FromLanguagePicker { get; set; } = new();
    public Picker ToLanguagePicker { get; set; } = new();
    public ProgressBar ProgressBar { get; set; } = new();
    public Picker EnginePicker { get; set; } = new();
    public Label TitleLabel { get; set; } = new();
    public ICommand PoweredByMouseEnterCommand { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Paragraphs"] is ObservableCollection<DisplayParagraph> paragraphs)
        {
            Paragraphs = paragraphs;

            Lines.Clear();
            foreach (var p in Paragraphs)
            {
                Lines.Add(new ExcelRow
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
            _subtitle = subtitle;
        }
    }

    [RelayCommand]
    public void Translate()
    {
        ProgressBar.IsVisible = true;
        ProgressBar.IsEnabled = true;
        ProgressBar.Progress = 0.5;
    }

    public void EngineSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is Picker { SelectedItem: IAutoTranslator translator })
        {
            TitleLabel.Text = $"Powered by {translator.Name}";

            SetAutoTranslatorEngine();
            SetupLanguageSettings(translator);
        }
    }

    private void SetAutoTranslatorEngine()
    {

    }

    private void SetupLanguageSettings(IAutoTranslator autoTranslator)
    {
        var fromLanguages = autoTranslator.GetSupportedSourceLanguages();
        FromLanguagePicker.Items.Clear();
        foreach (var language in fromLanguages)
        {
            FromLanguagePicker.Items.Add(ToProperCase(language.Name));
        }
        var sourceLanguageIsoCode = EvaluateDefaultSourceLanguageCode(_encoding, _subtitle, fromLanguages);
        SelectLanguageCode(FromLanguagePicker, sourceLanguageIsoCode, fromLanguages);

        var toLanguages = autoTranslator.GetSupportedTargetLanguages();
        ToLanguagePicker.Items.Clear();
        foreach (var language in toLanguages)
        {
            ToLanguagePicker.Items.Add(ToProperCase(language.Name));
        }
        var targetLanguageIsoCode = EvaluateDefaultTargetLanguageCode(sourceLanguageIsoCode);
        SelectLanguageCode(ToLanguagePicker, targetLanguageIsoCode, toLanguages);
    }

    static string ToProperCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
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
        TitleLabel.TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor];
        TitleLabel.TextDecorations = TextDecorations.None;
    }


    public void MouseClickedPoweredBy(object? sender, TappedEventArgs e)
    {
        UiUtil.OpenUrl(SelectedAutoTranslator.Url);
    }
}