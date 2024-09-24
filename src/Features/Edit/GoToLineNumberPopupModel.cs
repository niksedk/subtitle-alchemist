using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Features.Edit
{
    public partial class GoToLineNumberPopupModel : ObservableObject
    {
        public GoToLineNumberPopup? Popup { get; set; }
        public Entry EntryLineNumber { get; set; } = new Entry();

        [ObservableProperty]
        private int _lineNumber;

        public GoToLineNumberPopupModel()
        {
        }

        [RelayCommand]
        private void Ok()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(LineNumber);
            });
        }

        [RelayCommand]
        private void Cancel()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close();
            });
        }

        public void Initialize(Subtitle updatedSubtitle)
        {
            Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(async() =>
                {
                    if (Clipboard.HasText)
                    {
                        var clipboardText = await Clipboard.GetTextAsync();
                        if (int.TryParse(clipboardText, out var lineNumber))
                        {
                            LineNumber = lineNumber;
                            return;
                        }
                    }

                    LineNumber = updatedSubtitle.Paragraphs.Count;

                    EntryLineNumber.Focus();
                    EntryLineNumber.CursorPosition = 0;
                    EntryLineNumber.SelectionLength = EntryLineNumber.Text.Length;
                });

                return false;
            });
        }
    }
}
