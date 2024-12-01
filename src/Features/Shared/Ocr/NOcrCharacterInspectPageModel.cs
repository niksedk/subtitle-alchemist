using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic.Media;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class NOcrCharacterInspectPageModel : ObservableObject, IQueryAttributable
{
    public class LetterItem
    {
        public ImageSplitterItem2 Item { get; set; }
        public ImageSource? ImageSource { get; set; }
        public string? Text { get; set; }
        public NOcrChar? NOcrChar { get; set; }

        public LetterItem(ImageSplitterItem2 item, ImageSource? imageSource, string text, NOcrChar? nOcrChar)
        {
            Item = item;
            ImageSource = imageSource;
            Text = text;
            NOcrChar = nOcrChar;
        }
    }

    public NOcrCharacterInspectPage? Page { get; set; }
    public NOcrDrawingCanvasView NOcrDrawingCanvas { get; set; }
    public Label LabelStatusText { get; set; } = new();


    [ObservableProperty] private ObservableCollection<LetterItem> _letterItems;
    [ObservableProperty] private LetterItem? _selectedLetterItem;
    [ObservableProperty] private ImageSource? _currentImageSource;
    [ObservableProperty] private string _currentImageResolution;
    [ObservableProperty] private ImageSource? _matchImageSource;
    [ObservableProperty] private string _matchText;
    [ObservableProperty] private bool _matchIsItalic;
    [ObservableProperty] private string _statusText;

    private ImageSplitterItem2 _splitItem;
    private List<NOcrChar> _nOcrChars;
    private NOcrDb _nOcrDb;
    private bool _closing;

    public NOcrCharacterInspectPageModel()
    {
        _letterItems = new ObservableCollection<LetterItem>();
        _matchText = string.Empty;
        _splitItem = new ImageSplitterItem2(string.Empty);
        _nOcrChars = new List<NOcrChar>();
        _currentImageResolution = string.Empty;
        NOcrDrawingCanvas = new NOcrDrawingCanvasView();
        LabelStatusText = new Label();
        _nOcrDb = new NOcrDb(string.Empty);
        _closing = false;
        _statusText = string.Empty;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Letters"] is List<LetterItem> letters)
        {
            LetterItems = new ObservableCollection<LetterItem>(letters);
        }

        if (query["Matches"] is List<NOcrChar> matches)
        {
            _nOcrChars = new List<NOcrChar>();  
        }

        if (query["NOcrDb"] is NOcrDb nOcrDb)
        {
            _nOcrDb = nOcrDb;
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectedLetterItem = LetterItems.FirstOrDefault();
            });
            return false;
        });
    }

    [RelayCommand]
    private void DeleteMatch()
    {
        if (SelectedLetterItem is { NOcrChar: { } } item)
        {
            _nOcrDb?.Remove(item.NOcrChar);
            SelectedLetterItem.NOcrChar = null;
            SelectedLetterItemChanged();
            ShowStatus("Match deleted");
        }
    }

    [RelayCommand]
    private void UpdateMatch()
    {
        if (SelectedLetterItem is { NOcrChar: { } } item)
        {
            item.NOcrChar.Text = MatchText;
            item.NOcrChar.Italic = MatchIsItalic;
            ShowStatus("Match updated");
        }
    }

    [RelayCommand]
    private void AddBetterMatch()
    {
    }

    private void ShowStatus(string statusText)
    {
        LabelStatusText.Opacity = 0;
        StatusText = statusText;
        LabelStatusText.FadeTo(1, 200);

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(6_000), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_closing)
                {
                    return;
                }

                if (StatusText == statusText)
                {
                    LabelStatusText.FadeTo(0, 200);
                }
            });
            return false;
        });
    }


    [RelayCommand]
    private async Task Ok()
    {
        _closing = true;
        _nOcrDb.Save();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        _closing = true;
        _nOcrDb.LoadOcrCharacters();
        await Shell.Current.GoToAsync("..");
    }

    public void OnDisappearing()
    {
    }

    public void SelectedLetterItemChanged(object? sender, SelectionChangedEventArgs e)
    {
        SelectedLetterItemChanged();
    }

    private void SelectedLetterItemChanged()
    {
        CurrentImageResolution = string.Empty;

        if (SelectedLetterItem is { } letterItem)
        {
            if (letterItem.Item.NikseBitmap is { } bitmap)
            {
                CurrentImageSource = bitmap.GetBitmap().ToImageSource();
                CurrentImageResolution = $"{bitmap.Width}x{bitmap.Height}";

                NOcrDrawingCanvas.BackgroundImage = bitmap.GetBitmap();
                NOcrDrawingCanvas.ZoomFactor = 4.0f;
            }
            else
            {
                CurrentImageSource = null;
                CurrentImageResolution = string.Empty;
            }

            if (letterItem.NOcrChar is { } match)
            {
                MatchText = match.Text;
                MatchIsItalic = match.Italic;

                NOcrDrawingCanvas.HitPaths = match.LinesForeground;
                NOcrDrawingCanvas.MissPaths = match.LinesBackground;
                NOcrDrawingCanvas.InvalidateSurface();
            }
            else
            {
                MatchText = string.Empty;
                MatchIsItalic = false;
                NOcrDrawingCanvas.HitPaths = new List<NOcrLine>();
                NOcrDrawingCanvas.MissPaths = new List<NOcrLine>();
                NOcrDrawingCanvas.InvalidateSurface();
            }
        }
    }
}
