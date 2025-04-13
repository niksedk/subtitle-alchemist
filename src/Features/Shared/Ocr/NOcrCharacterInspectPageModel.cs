using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic;
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
    public Label LabelStatusText { get; set; }


    [ObservableProperty]
    public partial ObservableCollection<LetterItem> LetterItems { get; set; }

    [ObservableProperty]
    public partial LetterItem? SelectedLetterItem { get; set; }

    [ObservableProperty]
    public partial ImageSource? CurrentImageSource { get; set; }

    [ObservableProperty]
    public partial string CurrentImageResolution { get; set; }

    [ObservableProperty]
    public partial string MatchText { get; set; }

    [ObservableProperty]
    public partial bool MatchIsItalic { get; set; }

    [ObservableProperty]
    public partial string MatchInfo { get; set; }

    [ObservableProperty]
    public partial string StatusText { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> NoOfLinesToAutoDrawList { get; set; }

    [ObservableProperty]
    public partial int SelectedNoOfLinesToAutoDraw { get; set; }

    [ObservableProperty]
    public partial bool IsNewMatch { get; set; }

    [ObservableProperty]
    public partial bool IsAddBetterMatchVisible { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<NOcrDrawModeItem> DrawModes { get; set; }

    [ObservableProperty]
    public partial NOcrDrawModeItem SelectedDrawMode { get; set; }

    private NOcrChar _newMatch;
    private ImageSplitterItem2 _splitItem;
    private List<NOcrChar> _nOcrChars;
    private NOcrDb _nOcrDb;
    private bool _closing;

    public NOcrCharacterInspectPageModel()
    {
        LetterItems = new ObservableCollection<LetterItem>();
        MatchText = string.Empty;
        _splitItem = new ImageSplitterItem2(string.Empty);
        _nOcrChars = new List<NOcrChar>();
        CurrentImageResolution = string.Empty;
        NOcrDrawingCanvas = new NOcrDrawingCanvasView();
        LabelStatusText = new Label();
        _nOcrDb = new NOcrDb(string.Empty);
        _closing = false;
        StatusText = string.Empty;
        IsAddBetterMatchVisible = true;
        _newMatch = new NOcrChar(string.Empty);
        MatchInfo = string.Empty;
        DrawModes = new ObservableCollection<NOcrDrawModeItem>(NOcrDrawModeItem.Items);
        SelectedDrawMode = NOcrDrawModeItem.ForegroundItem;

        const int maxLines = 500;
        NoOfLinesToAutoDrawList = new ObservableCollection<int>();
        for (var i = 0; i <= maxLines; i++)
        {
            NoOfLinesToAutoDrawList.Add(i);
        }

        SelectedNoOfLinesToAutoDraw = 100;
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

        if (query["nOcrDb"] is NOcrDb nOcrDb)
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
        if (SelectedLetterItem is { } item)
        {
            if (IsNewMatch)
            {
                item.NOcrChar = new NOcrChar(_newMatch);
                item.NOcrChar.Text = MatchText;
                item.NOcrChar.Italic = MatchIsItalic;
                _nOcrDb.Add(item.NOcrChar);
                ShowStatus("Match added");
                IsNewMatch = false;
                IsAddBetterMatchVisible = true;
            }
            else if (item.NOcrChar != null)
            {
                item.Text = MatchText;
                item.NOcrChar.Text = MatchText;
                item.NOcrChar.Italic = MatchIsItalic;
                ShowStatus("Match updated");
            }
        }
    }

    [RelayCommand]
    private void AddBetterMatch()
    {
        if (SelectedLetterItem is { NOcrChar: { } } letterItem)
        {
            _newMatch = new NOcrChar(letterItem.NOcrChar);
            DrawAutoAll();
            IsNewMatch = true;
            IsAddBetterMatchVisible = false;
        }
        else if (SelectedLetterItem is { } letterItem2 && letterItem2.Item.NikseBitmap != null)
        {
            _newMatch = new NOcrChar(string.Empty)
            {
                Width = letterItem2.Item.NikseBitmap.Width,
                Height = letterItem2.Item.NikseBitmap.Height,
                MarginTop = letterItem2.Item.Top
            };
            DrawAutoAll();
            IsNewMatch = true;
            IsAddBetterMatchVisible = false;
        }
    }

    [RelayCommand]
    private void DrawClearAll()
    {
        _newMatch.LinesForeground.Clear();
        _newMatch.LinesBackground.Clear();
        ShowOcrPoints(_newMatch);
    }

    [RelayCommand]
    private void DrawClearBackground()
    {
        _newMatch.LinesBackground.Clear();
        ShowOcrPoints(_newMatch);
    }

    [RelayCommand]
    private void DrawClearForeground()
    {
        _newMatch.LinesForeground.Clear();
        ShowOcrPoints(_newMatch);
    }

    [RelayCommand]
    private void DrawAutoAll()
    {
        if (SelectedLetterItem is { } letterItem)
        {
            _newMatch.LinesBackground.Clear();
            _newMatch.LinesForeground.Clear();

            NOcrChar.GenerateLineSegments(SelectedNoOfLinesToAutoDraw, false, _newMatch, letterItem.Item.NikseBitmap!);
        }

        ShowOcrPoints(_newMatch);
    }

    [RelayCommand]
    private void DrawAutoBackground()
    {
        var oldForeground = new List<NOcrLine>(_newMatch.LinesForeground);
        _newMatch.LinesBackground.Clear();
        if (SelectedLetterItem is { } letterItem)
        {
            NOcrChar.GenerateLineSegments(SelectedNoOfLinesToAutoDraw, false, _newMatch, letterItem.Item.NikseBitmap!);
        }
        _newMatch.LinesForeground.Clear();
        _newMatch.LinesForeground.AddRange(oldForeground);

        ShowOcrPoints(_newMatch);
    }

    [RelayCommand]
    private void DrawAutoForeground()
    {
        var oldBackground = new List<NOcrLine>(_newMatch.LinesBackground);
        _newMatch.LinesForeground.Clear();
        if (SelectedLetterItem is { } letterItem)
        {
            NOcrChar.GenerateLineSegments(SelectedNoOfLinesToAutoDraw, false, _newMatch, letterItem.Item.NikseBitmap!);
        }
        _newMatch.LinesBackground.Clear();
        _newMatch.LinesBackground.AddRange(oldBackground);

        ShowOcrPoints(_newMatch);
    }

    private void ShowOcrPoints(NOcrChar nOcrChar)
    {
        NOcrDrawingCanvas.MissPaths.Clear();
        NOcrDrawingCanvas.HitPaths.Clear();

        NOcrDrawingCanvas.MissPaths.AddRange(nOcrChar.LinesBackground);
        NOcrDrawingCanvas.HitPaths.AddRange(nOcrChar.LinesForeground);

        NOcrDrawingCanvas.InvalidateSurface();
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
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterInspectPage) },
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        _closing = true;
        _nOcrDb.LoadOcrCharacters();
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterInspectPage) },
        });
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
        IsAddBetterMatchVisible = true;
        IsNewMatch = false;
        CurrentImageResolution = string.Empty;
        CurrentImageSource = null;
        NOcrDrawingCanvas.BackgroundImage = new SKBitmap(1, 1);

        if (SelectedLetterItem is { } letterItem)
        {
            if (letterItem.Item.NikseBitmap is { } bitmap)
            {
                CurrentImageSource = bitmap.GetBitmap().ToImageSource();
                CurrentImageResolution = $"{bitmap.Width}x{bitmap.Height}, margin top: {letterItem.Item.Top}";

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
                if (match.Width == 0 && match.Height == 0)
                {
                    MatchInfo = match.Text == " " ? "Space" : string.Empty;
                }
                else
                {
                    MatchInfo = $"{match.Width}x{match.Height}, margin top: {match.MarginTop}";
                }

                NOcrDrawingCanvas.HitPaths = match.LinesForeground;
                NOcrDrawingCanvas.MissPaths = match.LinesBackground;
                NOcrDrawingCanvas.InvalidateSurface();
            }
            else
            {
                MatchText = string.Empty;
                MatchIsItalic = false;
                MatchInfo = string.Empty;
                NOcrDrawingCanvas.HitPaths = new List<NOcrLine>();
                NOcrDrawingCanvas.MissPaths = new List<NOcrLine>();
                NOcrDrawingCanvas.InvalidateSurface();
            }
        }
    }

    [RelayCommand]
    private void ZoomIn()
    {
        if (NOcrDrawingCanvas.ZoomFactor < 10)
        {
            NOcrDrawingCanvas.ZoomFactor++;
        }
    }

    [RelayCommand]
    private void ZoomOut()
    {
        if (NOcrDrawingCanvas.ZoomFactor > 1)
        {
            NOcrDrawingCanvas.ZoomFactor--;
        }
    }

    public void PickerDrawMode_SelectedIndexChanged(object? sender, EventArgs e)
    {
        NOcrDrawingCanvas.NewLinesAreHits = SelectedDrawMode == NOcrDrawModeItem.ForegroundItem;
    }
}
