using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class NOcrCharacterAddPageModel : ObservableObject, IQueryAttributable
{
    public NOcrCharacterAddPage? Page { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<NOcrLine> LinesForeground { get; set; }

    [ObservableProperty]
    public partial NOcrLine? SelectedLineForeground { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<NOcrLine> LinesBackground { get; set; }

    [ObservableProperty]
    public partial NOcrLine? SelectedLineBackground { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<NOcrDrawModeItem> DrawModes { get; set; }

    [ObservableProperty]
    public partial NOcrDrawModeItem SelectedDrawMode { get; set; }

    [ObservableProperty]
    public partial bool IsNewLinesForegroundActive { get; set; }

    [ObservableProperty]
    public partial bool IsNewLinesBackgroundActive { get; set; }

    [ObservableProperty]
    public partial string NewText { get; set; }

    [ObservableProperty]
    public partial string ResolutionAndTopMargin { get; set; }

    [ObservableProperty]
    public partial bool IsNewTextItalic { get; set; }

    [ObservableProperty]
    public partial bool SubmitOnFirstLetter { get; set; }

    [ObservableProperty]
    public partial ImageSource? SentenceImageSource { get; set; }

    [ObservableProperty]
    public partial ImageSource? ItemImageSource { get; set; }

    [ObservableProperty]
    public partial bool CanShrink { get; set; }

    [ObservableProperty]
    public partial bool CanExpand { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<int> NoOfLinesToAutoDrawList { get; set; }

    [ObservableProperty]
    public partial int SelectedNoOfLinesToAutoDraw { get; set; }

    private List<ImageSplitterItem2> _letters;
    private ImageSplitterItem2 _splitItem;
    private SKBitmap _sentenceBitmap;
    public NOcrChar NOcrChar { get; private set; }
    public NOcrDrawingCanvasView NOcrDrawingCanvas { get; set; }
    public Entry EntryNewText { get; set; }

    private List<OcrSubtitleItem> _ocrSubtitleItems;
    private int _startFromNumber;
    private int _maxWrongPixels;
    private NOcrDb _nOcrDb;

    public NOcrCharacterAddPageModel()
    {
        Title = string.Empty;
        NewText = string.Empty;
        ResolutionAndTopMargin = string.Empty;
        LinesForeground = new ObservableCollection<NOcrLine>();
        LinesBackground = new ObservableCollection<NOcrLine>();
        IsNewLinesForegroundActive = true;
        IsNewLinesBackgroundActive = false;
        IsNewTextItalic = false;
        SubmitOnFirstLetter = false;
        _letters = new List<ImageSplitterItem2>();
        _splitItem = new ImageSplitterItem2(0, 0, new NikseBitmap2(1, 1));
        EntryNewText = new Entry();
        DrawModes = new ObservableCollection<NOcrDrawModeItem>(NOcrDrawModeItem.Items);
        SelectedDrawMode = DrawModes[0];

        const int maxLines = 500;
        NoOfLinesToAutoDrawList = new ObservableCollection<int>();
        for (var i = 0; i <= maxLines; i++)
        {
            NoOfLinesToAutoDrawList.Add(i);
        }

        SelectedNoOfLinesToAutoDraw = 100;
        NOcrChar = new NOcrChar();
        NOcrDrawingCanvas = new NOcrDrawingCanvasView();
        _ocrSubtitleItems = new List<OcrSubtitleItem>();
        _nOcrDb = new NOcrDb(string.Empty);
        _sentenceBitmap = new SKBitmap(1, 1);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Item"] is ImageSplitterItem2 item)
        {
            InitSplitItem(item);
        }

        if (query["Bitmap"] is SKBitmap bitmap)
        {
            _sentenceBitmap = bitmap;
            InitSentenceBitmap();
        }

        if (query["Letters"] is List<ImageSplitterItem2> letters)
        {
            _letters = letters;
        }

        if (query["nOcrDb"] is NOcrDb nOcrDb)
        {
            _nOcrDb = nOcrDb;
        }

        if (query["OcrSubtitleItems"] is List<OcrSubtitleItem> ocrSubtitleItems)
        {
            _ocrSubtitleItems = ocrSubtitleItems;
        }

        if (query["StartFromNumber"] is int startFromNumber)
        {
            _startFromNumber = startFromNumber;
        }

        if (query["MaxWrongPixels"] is int maxWrongPixels)
        {
            _maxWrongPixels = maxWrongPixels;
        }

        if (query["ItalicOn"] is bool italicOn)
        {
            IsNewTextItalic = italicOn;
        }

        SetTitle();

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadSettings();
                EntryNewText.Focus();
            });
            return false;
        });
    }

    private void InitSentenceBitmap()
    {
        if (_splitItem.NikseBitmap != null)
        {
            var bmp = DrawActiveRectangle(_sentenceBitmap,
                new SKRect(_splitItem.X, _splitItem.Y,
                    _splitItem.X + _splitItem.NikseBitmap.Width, _splitItem.Y + _splitItem.NikseBitmap.Height));
            SentenceImageSource = bmp.ToImageSource();
        }
        else
        {
            SentenceImageSource = null;
        }
    }

    private void InitSplitItem(ImageSplitterItem2 item)
    {
        _splitItem = item;
        if (_splitItem.NikseBitmap != null)
        {
            ItemImageSource = _splitItem.NikseBitmap.GetBitmap().ToImageSource();

            NOcrChar = new NOcrChar
            {
                Width = _splitItem.NikseBitmap.Width,
                Height = _splitItem.NikseBitmap.Height,
                MarginTop = _splitItem.Top,
            };

            NOcrDrawingCanvas.BackgroundImage = _splitItem.NikseBitmap.GetBitmap();
            NOcrDrawingCanvas.ZoomFactor = 4;
            AutoGuessLines();

            ResolutionAndTopMargin = $"{NOcrChar.Width}x{NOcrChar.Height}, top margin: {NOcrChar.MarginTop}";
        }
        else
        {
            ItemImageSource = null;
        }
    }

    private void SetTitle()
    {
        Title = $"Add nOCR character for line  {_startFromNumber}, character {_letters.IndexOf(_splitItem) + 1} of {_letters.Count} using database \"{Path.GetFileNameWithoutExtension(_nOcrDb.FileName)}\"";
    }

    public static SKBitmap DrawActiveRectangle(
        SKBitmap originalBitmap,
        SKRect borderRect,
        int borderThickness = 1,
        SKColor? borderColor = null)
    {
        var color = borderColor ?? SKColors.Red;
        using var surface = SKSurface.Create(new SKImageInfo(originalBitmap.Width, originalBitmap.Height));
        var canvas = surface.Canvas;
        canvas.DrawBitmap(originalBitmap, new SKPoint(0, 0));

        using var paint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = borderThickness
        };
        canvas.DrawRect(borderRect, paint);

        return SKBitmap.FromImage(surface.Snapshot());
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

    [RelayCommand]
    private void AutoGuessLines()
    {
        NOcrChar.LinesForeground.Clear();
        NOcrChar.LinesBackground.Clear();
        NOcrChar.GenerateLineSegments(SelectedNoOfLinesToAutoDraw, false, NOcrChar, _splitItem.NikseBitmap!);
        ShowOcrPoints();
    }

    [RelayCommand]
    private void Shrink()
    {

    }

    [RelayCommand]
    private void Expand()
    {

    }

    [RelayCommand]
    private async Task UseOnce()
    {
        NOcrChar.Text = NewText;
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterAddPage) },
            { "NOcrChar", NOcrChar },
            { "OcrSubtitleItems", _ocrSubtitleItems },
            { "StartFromNumber", _startFromNumber },
            { "ItalicOn", IsNewTextItalic },
            { "UseOnce", true },
            { "LetterIndex", _letters.IndexOf(_splitItem) },
            { "Abort", false },
            { "Skip", false },
        });
    }

    [RelayCommand]
    private async Task Skip()
    {
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterAddPage) },
            { "NOcrChar", NOcrChar },
            { "OcrSubtitleItems", _ocrSubtitleItems },
            { "StartFromNumber", _startFromNumber },
            { "ItalicOn", IsNewTextItalic },
            { "Skip", true },
            { "UseOnce", false },
            { "LetterIndex", _letters.IndexOf(_splitItem) },
            { "Abort", false },
        });
    }

    [RelayCommand]
    private async Task Abort()
    {
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterAddPage) },
            { "NOcrChar", NOcrChar },
            { "OcrSubtitleItems", _ocrSubtitleItems },
            { "StartFromNumber", _startFromNumber },
            { "ItalicOn", IsNewTextItalic },
            { "Abort", true },
            { "Skip", true },
            { "UseOnce", false },
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        NOcrChar.Text = NewText;
        NOcrChar.Italic = IsNewTextItalic;
        _nOcrDb.Add(NOcrChar);
        _nOcrDb.Save();

        var idx = _letters.IndexOf(_splitItem) + 1;
        if (idx < _letters.Count)
        {
            for (var i = 0; i < _letters.Count; i++)
            {
                var item = _letters[i];
                var match = _nOcrDb.GetMatch(item.NikseBitmap!, _letters, item, item.Top, true, _maxWrongPixels);
                if (match == null && string.IsNullOrEmpty(item.SpecialCharacter))
                {
                    InitSplitItem(item);
                    InitSentenceBitmap();
                    SetTitle();
                    NewText = string.Empty;
                    EntryNewText.Focus();
                    return;
                }
            }
        }

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterAddPage) },
            { "OcrSubtitleItems", _ocrSubtitleItems },
            { "StartFromNumber", _startFromNumber },
            { "ItalicOn", IsNewTextItalic },
            { "Skip", false },
            { "UseOnce", false },
            { "Abort", false },
        });
    }

    [RelayCommand]
    private void ClearLines()
    {
        NOcrChar.LinesForeground.Clear();
        NOcrChar.LinesBackground.Clear();
        ShowOcrPoints();
    }

    public void OnDisappearing()
    {
        SaveSettings();
    }

    private void LoadSettings()
    {
    }

    private void SaveSettings()
    {
        Se.SaveSettings();
    }

    private void ShowOcrPoints()
    {
        NOcrDrawingCanvas.MissPaths.Clear();
        NOcrDrawingCanvas.HitPaths.Clear();

        NOcrDrawingCanvas.MissPaths.AddRange(NOcrChar.LinesBackground);
        NOcrDrawingCanvas.HitPaths.AddRange(NOcrChar.LinesForeground);

        NOcrDrawingCanvas.InvalidateSurface();
    }

    public void CheckBoxIsNewTextItalic_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            if (checkBox.IsChecked)
            {
                EntryNewText.FontAttributes = FontAttributes.Italic;
            }
            else
            {
                EntryNewText.FontAttributes = FontAttributes.None;
            }
        }
    }

    public void OnAppearing()
    {
        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(250), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                EntryNewText.Focus();
                EntryNewText.Focus();
                EntryNewText.Focus();
            });
            return false;
        });

    }

    public void PickerDrawMode_SelectedIndexChanged(object? sender, EventArgs e)
    {
        NOcrDrawingCanvas.NewLinesAreHits = SelectedDrawMode == NOcrDrawModeItem.ForegroundItem;
    }
}
