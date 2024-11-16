using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic.Constants;
using SubtitleAlchemist.Logic.Media;
using System.Collections.ObjectModel;
using Nikse.SubtitleEdit.Core.Common;
using System.Text;
using SubtitleAlchemist.Logic.Config;
using System.Globalization;

namespace SubtitleAlchemist.Features.Files.ExportBinary.EbuExport
{
    public partial class ExportEbuPageModel : ObservableObject, IQueryAttributable
    {

        [ObservableProperty]
        private ObservableCollection<CodePageNumberItem> _codePageNumbers;

        [ObservableProperty]
        private CodePageNumberItem? _selectedCodePageNumber;

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
        private LanguageItem? _selectedLanguageCode;

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
        private ObservableCollection<int> _revisionNumbers;

        [ObservableProperty]
        private int _selectedRevisionNumber;

        [ObservableProperty]
        private ObservableCollection<int> _maximumCharactersPerRowList;

        [ObservableProperty]
        private int _selectedMaximumCharactersPerRow;

        [ObservableProperty]
        private ObservableCollection<int> _maximumRowsList;

        [ObservableProperty]
        private int _selectedMaximumRows;

        [ObservableProperty]
        private ObservableCollection<int> _discSequenceNumberList;

        [ObservableProperty]
        private int _selectedDiscSequenceNumber;

        [ObservableProperty]
        private ObservableCollection<int> _totalNumberOfDiscsList;

        [ObservableProperty]
        private int _selectedTotalNumberOfDiscs;

        [ObservableProperty]
        private ObservableCollection<string> _justificationCodes;

        [ObservableProperty]
        private string _selectedJustificationCode = string.Empty;

        [ObservableProperty]
        private ObservableCollection<int> _marginTopList;

        [ObservableProperty]
        private int _selectedMarginTop;

        [ObservableProperty]
        private ObservableCollection<int> _marginBottomList;

        [ObservableProperty]
        private int _selectedMarginBottom;

        [ObservableProperty]
        private ObservableCollection<int> _newLineRowsList;

        [ObservableProperty]
        private int _selectedNewLineRows;

        [ObservableProperty]
        private bool _teletextBox;

        [ObservableProperty]
        private bool _teletextDoubleHeight;

        [ObservableProperty]
        private string _errorLogTitle = "Errors";

        [ObservableProperty]
        private string _errorLog;

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

        private Subtitle _subtitle = new Subtitle();
        private bool _useSubtitleFileName = false;
        private Ebu.EbuGeneralSubtitleInformation _header = new Ebu.EbuGeneralSubtitleInformation();

        private readonly IFileHelper _fileHelper;

        public ExportEbuPageModel(IFileHelper fileHelper)
        {
            _errorLog = string.Empty;
            _fileHelper = fileHelper;
            GeneralBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor];
            TextAndTimingBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];
            ErrorsBackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor];

            _codePageNumbers = new ObservableCollection<CodePageNumberItem>(CodePageNumberItem.GetCodePageNumberItems());
            _selectedCodePageNumber = _codePageNumbers[0];

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

            _revisionNumbers = new ObservableCollection<int>(Enumerable.Range(0, 100).ToList());

            _maximumCharactersPerRowList = new ObservableCollection<int>(Enumerable.Range(0, 100).ToList());

            _maximumRowsList = new ObservableCollection<int>(Enumerable.Range(0, 100).ToList());

            _discSequenceNumberList = new ObservableCollection<int>(Enumerable.Range(0, 10).ToList());

            _totalNumberOfDiscsList = new ObservableCollection<int>(Enumerable.Range(0, 10).ToList());

            _marginTopList = new ObservableCollection<int>(Enumerable.Range(0, 51).ToList());

            _marginBottomList = new ObservableCollection<int>(Enumerable.Range(0, 51).ToList());

            _newLineRowsList = new ObservableCollection<int>(Enumerable.Range(0, 11).ToList());
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

            FillHeaderFromFile(subtitleFileName);
        }

        [RelayCommand]
        private async Task Ok()
        {
            _header.CodePageNumber = SelectedCodePageNumber == null ? "865" : SelectedCodePageNumber.CodePage;

            _header.DiskFormatCode = RemoveAfterParenthesisAndTrim(SelectedDiskFormatCode);

            if (double.TryParse(SelectedFrameRate.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, "."), out var d) && d is > 20 and < 200)
            {
                _header.FrameRateFromSaveDialog = d;
            }

            var firstLetter = SelectedDisplayStandardCode[0].ToString();
            _header.DisplayStandardCode = int.TryParse(firstLetter, out _) ? firstLetter : " ";

            _header.CharacterCodeTableNumber = "0" + CharacterTables.IndexOf(SelectedCharacterTable);
            _header.LanguageCode = SelectedLanguageCode != null ? SelectedLanguageCode.Code : string.Empty;
            if (_header.LanguageCode.Length != 2)
            {
                _header.LanguageCode = "0A";
            }

            _header.OriginalProgrammeTitle = OriginalProgramTitle.PadRight(32, ' ');
            _header.OriginalEpisodeTitle = OriginalEpisodeTitle.PadRight(32, ' ');
            _header.TranslatedProgrammeTitle = TranslatedProgramTitle.PadRight(32, ' ');
            _header.TranslatedEpisodeTitle = TranslatedEpisodeTitle.PadRight(32, ' ');
            _header.TranslatorsName = TranslatorName.PadRight(32, ' ');
            _header.SubtitleListReferenceCode = SubtitleListReferenceCode.PadRight(16, ' ');
            _header.CountryOfOrigin = CountryOfOrigin;
            if (_header.CountryOfOrigin.Length != 3)
            {
                _header.CountryOfOrigin = "USA";
            }

            _header.TimeCodeStatus = TimeCodeStatusList.IndexOf(SelectedTimeCodeStatus).ToString(CultureInfo.InvariantCulture);
            _header.TimeCodeStartOfProgramme = new TimeCode(StartOfProgramme).ToHHMMSSFF().RemoveChar(':');

            _header.RevisionNumber = SelectedRevisionNumber.ToString("00");
            _header.MaximumNumberOfDisplayableCharactersInAnyTextRow = SelectedMaximumCharactersPerRow.ToString("00");
            _header.MaximumNumberOfDisplayableRows = SelectedMaximumRows.ToString("00");
            _header.DiskSequenceNumber = SelectedDiscSequenceNumber.ToString(CultureInfo.InvariantCulture);
            _header.TotalNumberOfDisks = SelectedTotalNumberOfDiscs.ToString(CultureInfo.InvariantCulture);

            //TODO: JustificationCode = (byte)JustificationCodes.IndexOf(SelectedJustificationCode);
            Configuration.Settings.SubtitleSettings.EbuStlMarginTop = SelectedMarginTop;
            Configuration.Settings.SubtitleSettings.EbuStlMarginBottom = SelectedMarginBottom;
            Configuration.Settings.SubtitleSettings.EbuStlNewLineRows = SelectedNewLineRows;
            Configuration.Settings.SubtitleSettings.EbuStlTeletextUseBox = TeletextBox;
            Configuration.Settings.SubtitleSettings.EbuStlTeletextUseDoubleHeight = TeletextDoubleHeight;

            _subtitle.Header = _header.ToString();

            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
            {
                { "Page", nameof(ExportEbuPage) },
                { "Subtitle", _subtitle },
                { "UseSubtitleFileName", _useSubtitleFileName },
            });
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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query["Subtitle"] is Subtitle subtitle)
            {
                Initialize(subtitle);
            }

            if (query["UseSubtitleFileName"] is bool useSubtitleFileName)
            {
                _useSubtitleFileName = useSubtitleFileName;
            }
        }

        private void Initialize(Subtitle? subtitle)
        {
            _subtitle = subtitle ?? new Subtitle();

            Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectedDiskFormatCode = DiskFormatCodes[2];
                    SelectedFrameRate = FrameRates[2];
                    SelectedDisplayStandardCode = DisplayStandardCodes[0];
                    SelectedCharacterTable = CharacterTables[0];
                    SelectedLanguageCode = LanguageCodes.FirstOrDefault(p => p.Language == "English");
                    SelectedTimeCodeStatus = TimeCodeStatusList[1];
                    SelectedJustificationCode = JustificationCodes[1];
                    SelectedRevisionNumber = 1;
                    SelectedMaximumCharactersPerRow = 40;
                    SelectedMaximumRows = 23;
                    SelectedDiscSequenceNumber = 1;
                    SelectedTotalNumberOfDiscs = 1;
                    SelectedMarginTop = 0;
                    SelectedMarginBottom = 2;
                    SelectedNewLineRows = 2;

                    CheckErrors(_subtitle);
                });
                return false;
            });
        }

        private void FillHeaderFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var ebu = new Ebu();
                var temp = new Subtitle();
                ebu.LoadSubtitle(temp, null, fileName);
                FillFromHeader(ebu.Header);
                if (ebu.JustificationCodes.Count > 2 && ebu.JustificationCodes[1] == ebu.JustificationCodes[2])
                {
                    if (ebu.JustificationCodes[1] >= 0 && ebu.JustificationCodes[1] < JustificationCodes.Count)
                    {
                        SelectedJustificationCode = JustificationCodes[ebu.JustificationCodes[1]];
                    }
                }
            }
        }

        private void FillFromHeader(Ebu.EbuGeneralSubtitleInformation header)
        {
            SelectedCodePageNumber = CodePageNumbers.FirstOrDefault(p=>p.CodePage == header.CodePageNumber);

            SelectedDiskFormatCode = DiskFormatCodes.First(p => p.Contains(header.DiskFormatCode, StringComparison.OrdinalIgnoreCase));

            if (header.FrameRateFromSaveDialog is > 20 and < 200)
            {
                SelectedFrameRate = header.FrameRateFromSaveDialog.ToString(CultureInfo.CurrentCulture);
            }

            SelectedDisplayStandardCode = DisplayStandardCodes.First(p => p.StartsWith(header.DisplayStandardCode, StringComparison.InvariantCulture));

            if (int.TryParse(header.CharacterCodeTableNumber, out var tableNumber))
            {
                SelectedCharacterTable = CharacterTables[tableNumber];
            }

            SelectedLanguageCode = LanguageCodes.FirstOrDefault(p => p.Code == header.LanguageCode);
            OriginalProgramTitle = header.OriginalProgrammeTitle.TrimEnd();
            OriginalEpisodeTitle = header.OriginalEpisodeTitle.TrimEnd();
            TranslatedProgramTitle = header.TranslatedProgrammeTitle.TrimEnd();
            TranslatedEpisodeTitle = header.TranslatedEpisodeTitle.TrimEnd();
            TranslatorName = header.TranslatorsName.TrimEnd();
            SubtitleListReferenceCode = header.SubtitleListReferenceCode.TrimEnd();
            CountryOfOrigin = header.CountryOfOrigin;

            SelectedTimeCodeStatus = TimeCodeStatusList.Last();
            if (header.TimeCodeStatus == "0")
            {
                SelectedTimeCodeStatus = TimeCodeStatusList.First();
            }

            try
            {
                // HHMMSSFF
                var hh = int.Parse(header.TimeCodeStartOfProgramme.Substring(0, 2));
                var mm = int.Parse(header.TimeCodeStartOfProgramme.Substring(2, 2));
                var ss = int.Parse(header.TimeCodeStartOfProgramme.Substring(4, 2));
                var ff = int.Parse(header.TimeCodeStartOfProgramme.Substring(6, 2));
                StartOfProgramme = new TimeCode(hh, mm, ss, SubtitleFormat.FramesToMillisecondsMax999(ff)).TimeSpan;
            }
            catch (Exception)
            {
                StartOfProgramme = new TimeSpan(0);
            }

            if (int.TryParse(header.RevisionNumber, out var number))
            {
                SelectedRevisionNumber = number;
            }
            else
            {
                SelectedRevisionNumber = 1;
            }

            if (int.TryParse(header.MaximumNumberOfDisplayableCharactersInAnyTextRow, out number))
            {
                SelectedMaximumCharactersPerRow = number;
            }

            SelectedMaximumRows = 23;
            if (int.TryParse(header.MaximumNumberOfDisplayableRows, out number))
            {
                SelectedMaximumRows = number;
            }

            if (int.TryParse(header.DiskSequenceNumber, out number))
            {
                SelectedDiscSequenceNumber = number;
            }
            else
            {
                SelectedDiscSequenceNumber = 1;
            }

            if (int.TryParse(header.TotalNumberOfDisks, out number))
            {
                SelectedTotalNumberOfDiscs = number;
            }
            else
            {
                SelectedTotalNumberOfDiscs = 1;
            }
        }

        private string RemoveAfterParenthesisAndTrim(string input)
        {
            var index = input.IndexOf("(");
            return index >= 0 ? input.Substring(0, index).TrimEnd() : input;
        }

        private void CheckErrors(Subtitle subtitle)
        {
            if (subtitle.Paragraphs.Count == 0)
            {
                return;
            }

            var sb = new StringBuilder();
            var errorCount = 0;
            var i = 1;
            var isTeletext = SelectedDiskFormatCode.Contains("teletext", StringComparison.OrdinalIgnoreCase);
            foreach (var p in subtitle.Paragraphs)
            {
                var arr = p.Text.SplitToLines();
                for (var index = 0; index < arr.Count; index++)
                {
                    var line = arr[index];
                    var s = HtmlUtil.RemoveHtmlTags(line, true);
                    if (s.Length > SelectedMaximumCharactersPerRow)
                    {
                        sb.AppendLine(string.Format(Se.Language.EbuSaveOptions.MaxLengthError, i, SelectedMaximumCharactersPerRow, s.Length - SelectedMaximumCharactersPerRow, s));
                        errorCount++;
                    }

                    if (isTeletext)
                    {
                        // See https://kb.fab-online.com/0040-fabsubtitler-editor/00010-linelengthineditor/

                        // 36 characters for double height colored tex
                        if (arr.Count == 2 && s.Length > 36 && arr[index].Contains("<font ", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Line {i}-{index + 1}: 36 (not {s.Length}) should be maximum characters for double height colored text");
                            errorCount++;
                        }

                        // 37 characters for double height white text
                        else if (arr.Count == 2 && s.Length > 37 && !p.Text.Contains("<font ", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.AppendLine($"Line {i}-{index + 1}: 37 (not {s.Length}) should be maximum characters for double height white text");
                            errorCount++;
                        }

                        // 38 characters for single height white text
                        else if (arr.Count == 1 && s.Length > 38)
                        {
                            sb.AppendLine($"Line {i}: 38 (not {s.Length}) should be maximum characters for single height white text");
                            errorCount++;
                        }
                    }
                }

                i++;
            }

            ErrorLog = sb.ToString();
            ErrorLogTitle = string.Format(Se.Language.EbuSaveOptions.ErrorsX, errorCount);
        }
    }
}
