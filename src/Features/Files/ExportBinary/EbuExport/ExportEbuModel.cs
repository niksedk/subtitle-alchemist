using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Files.ExportBinary.EbuExport
{
    public partial class ExportEbuModel : ObservableObject
    {
        [ObservableProperty]
        private string _codePageNumber = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _diskFormatCodes;

        [ObservableProperty]
        private string _selectedDiskFormatCode = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _frameRates;

        [ObservableProperty]
        private string _selectedFrameRate = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _displayStandardCodes;

        [ObservableProperty]
        private string _selectedDisplayStandardCode = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _characterTables;

        [ObservableProperty]
        private string _selectedCharacterTable = string.Empty;

        [ObservableProperty]
        private ObservableCollection<LanguageItem> _languageCodes;

        [ObservableProperty]
        private string _selectedLanguageCode = string.Empty;

        [ObservableProperty]
        private string _originalProgramTitle = string.Empty;

        [ObservableProperty]
        private string _originalEpisodeTitle = string.Empty;

        [ObservableProperty]
        private string _translatedProgramTitle = string.Empty;

        [ObservableProperty]
        private string _translatedEpisodeTitle = string.Empty;

        [ObservableProperty]
        private string _translatorName = string.Empty;

        [ObservableProperty]
        private string _subtitleListReferenceCode = string.Empty;

        [ObservableProperty]
        private string _countryOfOrigin = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _timeCodeStatusList;

        [ObservableProperty]
        private string _selectedTimeCodeStatus = string.Empty;

        [ObservableProperty]
        private TimeSpan _startOfProgramme;

        [ObservableProperty]
        private int _revisionNumber;

        [ObservableProperty]
        private int _maximumCharactersPerRow;

        [ObservableProperty]
        private int _maximumRows;

        [ObservableProperty]
        private int _discSequenceNumber;

        [ObservableProperty]
        private int _totalNumberOfDiscs;


        [ObservableProperty]
        private ObservableCollection<string> _justificationCodes;

        [ObservableProperty]
        private string _selectedJustificationCode = string.Empty;

        [ObservableProperty]
        private int _marginTop;

        [ObservableProperty]
        private int _marginBottom;

        [ObservableProperty]
        private int _newLineRows;

        [ObservableProperty]
        private bool _teletextBox;

        [ObservableProperty]
        private bool _teletextDoubleHeight;

        [ObservableProperty]
        private Color _generalBackgroundColor;

        [ObservableProperty]
        private Color _textAndTimingBackgroundColor;

        [ObservableProperty]
        private Color _errorsBackgroundColor;

        public ExportEbuPage? Page { get; set; }
        public Border GeneralView { get; set; } = new Border();
        public Border TextAndTimingView { get; set; } = new Border();
        public Border ErrorsView { get; set; } = new Border();

        private readonly IFileHelper _fileHelper;

        public ExportEbuModel(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
            GeneralBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
            TextAndTimingBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
            ErrorsBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];

            _diskFormatCodes = new ObservableCollection<string>
            {
                "STL23.01 (non-standard)",
                "STL24.01 (non-standard)",
                "STL25.01",
                "STL29.01 (non-standard)",
                "STL30.01",
            };

            _frameRates = new ObservableCollection<string>
            {
                "23.976",
                "24",
                "25",
                "29.97",
                "30",
            };

            _displayStandardCodes = new ObservableCollection<string>
            {
               "0 Open subtitling",
               "1 Level-1 teletext",
               "2 Level-2 teletext",
               "Undefined",
            };

            _characterTables = new ObservableCollection<string>
            {
                "Latin",
                "Latin/Cyrillic",
                "Latin/Arabic",
                "Latin/Greek",
                "Latin/Hebrew",
            };

            _languageCodes = new ObservableCollection<LanguageItem>
            {
                new("00", ""),
                new("01", "Albanian"),
                new("02", "Breton"),
                new("03", "Catalan"),
                new("04", "Croatian"),
                new("05", "Welsh"),
                new("06", "Czech"),
                new("07", "Danish"),
                new("08", "German"),
                new("09", "English"),
                new("0A", "Spanish"),
                new("0B", "Esperanto"),
                new("0C", "Estonian"),
                new("0D", "Basque"),
                new("0E", "Faroese"),
                new("0F", "French"),
                new("10", "Frisian"),
                new("11", "Irish"),
                new("12", "Gaelic"),
                new("13", "Galician"),
                new("14", "Icelandic"),
                new("15", "Italian"),
                new("16", "Lappish"),
                new("17", "Latin"),
                new("18", "Latvian"),
                new("19", "Luxembourgi"),
                new("1A", "Lithuanian"),
                new("1B", "Hungarian"),
                new("1C", "Maltese"),
                new("1D", "Dutch"),
                new("1E", "Norwegian"),
                new("1F", "Occitan"),
                new("20", "Polish"),
                new("21", "Portuguese"),
                new("22", "Romanian"),
                new("23", "Romansh"),
                new("24", "Serbian"),
                new("25", "Slovak"),
                new("26", "Slovenian"),
                new("27", "Finnish"),
                new("28", "Swedish"),
                new("29", "Turkish"),
                new("2A", "Flemish"),
                new("2B", "Wallon"),
                new("7F", "Amharic"),
                new("7E", "Arabic"),
                new("7D", "Armenian"),
                new("7C", "Assamese"),
                new("7B", "Azerbaijani"),
                new("7A", "Bambora"),
                new("79", "Bielorussian"),
                new("78", "Bengali"),
                new("77", "Bulgarian"),
                new("76", "Burmese"),
                new("75", "Chinese"),
                new("74", "Churash"),
                new("73", "Dari"),
                new("72", "Fulani"),
                new("71", "Georgian"),
                new("70", "Greek"),
                new("6F", "Gujurati"),
                new("6E", "Gurani"),
                new("6D", "Hausa"),
                new("6C", "Hebrew"),
                new("6B", "Hindi"),
                new("6A", "Indonesian"),
                new("69", "Japanese"),
                new("68", "Kannada"),
                new("67", "Kazakh"),
                new("66", "Khmer"),
                new("65", "Korean"),
                new("64", "Laotian"),
                new("63", "Macedonian"),
                new("62", "Malagasay"),
                new("61", "Malaysian"),
                new("60", "Moldavian"),
                new("5F", "Marathi"),
                new("5E", "Ndebele"),
                new("5D", "Nepali"),
                new("5C", "Oriya"),
                new("5B", "Papamiento"),
                new("5A", "Persian"),
                new("59", "Punjabi"),
                new("58", "Pushtu"),
                new("57", "Quechua"),
                new("56", "Russian"),
                new("55", "Ruthenian"),
                new("54", "Serbocroat"),
                new("53", "Shona"),
                new("52", "Sinhalese"),
                new("51", "Somali"),
                new("50", "Sranan Tongo"),
                new("4F", "Swahili"),
                new("4E", "Tadzhik"),
                new("4D", "Tamil"),
                new("4C", "Tatar"),
                new("4B", "Telugu"),
                new("4A", "Thai"),
                new("49", "Ukrainian"),
                new("48", "Urdu"),
                new("47", "Uzbek"),
                new("46", "Vietnamese"),
                new("45", "Zulu"),
            };

            _timeCodeStatusList = new ObservableCollection<string>
            {
                "Not intended for use",
                "Intended for use",
            };

            _justificationCodes = new ObservableCollection<string>
            {
                "Unchanged presentation",
                "Left-justified text",
                "Centered text",
                "Right-justified text",
            };
        }

        [RelayCommand]
        private async Task Import()
        {
            var format = new Ebu();
            var subtitleFileName = await _fileHelper.PickAndShowSubtitleFile($"Open {format.Name} file", format);
            if (string.IsNullOrEmpty(subtitleFileName))
            {
                return;
            }

            var buffer = await File.ReadAllBytesAsync(subtitleFileName);
            if (buffer.Length <= 270)
            {
                return;
            }

            //TODO: Implement import
        }

        [RelayCommand]
        private async Task Ok()
        {

        }

        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void GeneralTapped(object? sender, TappedEventArgs e)
        {
            GeneralBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
            TextAndTimingBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
            ErrorsBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];

            GeneralView.Content!.Opacity = 0;
            GeneralView.IsVisible = true;
            TextAndTimingView.IsVisible = false;
            ErrorsView.IsVisible = false;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await GeneralView.Content!.FadeTo(1, 200);
            });
        }

        public void TextAndTimingTapped(object? sender, TappedEventArgs e)
        {
            GeneralBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
            TextAndTimingBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
            ErrorsBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];

            GeneralView.IsVisible = false;
            TextAndTimingView.Content!.Opacity = 0;
            TextAndTimingView.IsVisible = true;
            ErrorsView.IsVisible = false;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await TextAndTimingView.Content!.FadeTo(1, 200);
            });
        }

        public void ErrorsTapped(object? sender, TappedEventArgs e)
        {
            GeneralBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
            TextAndTimingBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
            ErrorsBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];

            GeneralView.IsVisible = false;
            TextAndTimingView.IsVisible = false;
            ErrorsView.Content!.Opacity = 0;
            ErrorsView.IsVisible = true;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await ErrorsView.Content!.FadeTo(1, 200);
            });
        }
    }
}
