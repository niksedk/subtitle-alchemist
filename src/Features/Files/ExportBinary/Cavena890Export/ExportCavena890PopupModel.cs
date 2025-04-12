using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Files.ExportBinary.Cavena890Export
{
    public partial class ExportCavena890PopupModel : ObservableObject
    {
        [ObservableProperty] public partial string TranslatedTitle { get; set; } = string.Empty;
        [ObservableProperty] public partial string OriginalTitle { get; set; } = string.Empty;
        [ObservableProperty] public partial string Translator { get; set; }  = string.Empty;
        [ObservableProperty] public partial string Comment { get; set; } = string.Empty;
        [ObservableProperty] public partial string? SelectedLanguage { get; set; }
        [ObservableProperty] public partial ObservableCollection<string> Languages { get; set; }
        [ObservableProperty] public partial TimeSpan StartTime { get; set; }

        public ExportCavena890Popup? Popup { get; set; }

        private readonly IFileHelper _fileHelper;

        public ExportCavena890PopupModel(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
            Languages = new ObservableCollection<string>()
            {
                "Arabic",
                "Danish",
                "Chinese Simplified",
                "Chinese Traditional",
                "English",
                "Hebrew",
                "Russian",
                "Romanian",
            };

            SelectedLanguage = "English";

            StartTime = new TimeSpan(10, 0, 0);
        }

        [RelayCommand]
        private void Ok()
        {
            if (SelectedLanguage is null)
            {
                return;
            }

            Configuration.Settings.SubtitleSettings.Cavena890StartOfMessage = new TimeCode(StartTime).ToHHMMSSFF();
            Configuration.Settings.SubtitleSettings.CurrentCavena89Title = TranslatedTitle;
            Configuration.Settings.SubtitleSettings.CurrentCavena890riginalTitle = OriginalTitle;
            Configuration.Settings.SubtitleSettings.CurrentCavena890Translator = Translator;
            Configuration.Settings.SubtitleSettings.CurrentCavena89Comment = Comment;

            switch (SelectedLanguage)
            {
                case "Arabic":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdArabic;
                    break;
                case "Danish":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdDanish;
                    break;
                case "Chinese Simplified":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdChineseSimplified;
                    break;
                case "Chinese Traditional":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdChineseTraditional;
                    break;
                case "Hebrew":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdHebrew;
                    break;
                case "Russian":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdRussian;
                    break;
                case "Romanian":
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdRomanian;
                    break;
                default:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdEnglish;
                    break;
            }

            Popup?.Close("OK");
        }

        [RelayCommand]
        private void Cancel()
        {
            Popup?.Close();
        }

        [RelayCommand]
        private async Task Import()
        {
            var format = new Cavena890();
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

            var s = System.Text.Encoding.ASCII.GetString(buffer, 40, 28);
            TranslatedTitle = s.Replace("\0", string.Empty);

            s = System.Text.Encoding.ASCII.GetString(buffer, 218, 28);
            OriginalTitle = s.Replace("\0", string.Empty);

            s = System.Text.Encoding.ASCII.GetString(buffer, 68, 28);
            Translator = s.Replace("\0", string.Empty);

            s = System.Text.Encoding.ASCII.GetString(buffer, 148, 24);
            Comment = s.Replace("\0", string.Empty);

            s = System.Text.Encoding.ASCII.GetString(buffer, 256, 11);
            StartTime = new TimeCode(TimeCode.ParseHHMMSSFFToMilliseconds(s)).TimeSpan;

            switch (buffer[146])
            {
                case Cavena890.LanguageIdHebrew:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdHebrew;
                    SelectedLanguage = "Hebrew";
                    break;
                case Cavena890.LanguageIdRussian:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdRussian;
                    SelectedLanguage = "Russian";
                    break;
                case Cavena890.LanguageIdChineseSimplified:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdChineseSimplified;
                    SelectedLanguage = "Chinese Simplified";
                    break;
                case Cavena890.LanguageIdChineseTraditional:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdChineseSimplified;
                    SelectedLanguage = "Chinese Traditional";
                    break;
                case Cavena890.LanguageIdDanish:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdDanish;
                    SelectedLanguage = "Danish";
                    break;
                case Cavena890.LanguageIdRomanian:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdRomanian;
                    SelectedLanguage = "Romanian";
                    break;
                default:
                    Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdEnglish;
                    SelectedLanguage = "English";
                    break;
            }
        }

        public void SetValues(Subtitle subtitle)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TranslatedTitle = Configuration.Settings.SubtitleSettings.CurrentCavena89Title;
                    OriginalTitle = Configuration.Settings.SubtitleSettings.CurrentCavena890riginalTitle;
                    Translator = Configuration.Settings.SubtitleSettings.CurrentCavena890Translator;
                    Comment = Configuration.Settings.SubtitleSettings.CurrentCavena89Comment;
                    if (string.IsNullOrWhiteSpace(Comment))
                    {
                        Comment = "Made with Subtitle Edit";
                    }

                    var language = LanguageAutoDetect.AutoDetectGoogleLanguage(subtitle);
                    switch (language)
                    {
                        case "he":
                            Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdHebrew;
                            SelectedLanguage = "Hebrew";
                            break;
                        case "ru":
                            Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdRussian;
                            SelectedLanguage = "Russian";
                            break;
                        case "ro":
                            Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdRomanian;
                            SelectedLanguage = "Romanian";
                            break;
                        case "zh":
                            Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdChineseSimplified;
                            SelectedLanguage = "Chinese Simplified";
                            break;
                        case "da":
                            Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdDanish;
                            SelectedLanguage = "Danish";
                            break;
                        default:
                            Configuration.Settings.SubtitleSettings.CurrentCavena89LanguageId = Cavena890.LanguageIdEnglish;
                            SelectedLanguage = "English";
                            break;
                    }
                });
                return false;
            });
        }
    }
}
