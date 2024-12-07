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

    [ObservableProperty] private ObservableCollection<NOcrLine> _linesForeground;
    [ObservableProperty] private NOcrLine? _selectedLineForeground;
    [ObservableProperty] private ObservableCollection<NOcrLine> _linesBackground;
    [ObservableProperty] private NOcrLine? _selectedLineBackground;
    [ObservableProperty] private ObservableCollection<NOcrDrawModeItem> _drawModes;
    [ObservableProperty] private NOcrDrawModeItem _selectedDrawMode;
    [ObservableProperty] private bool _isNewLinesForegroundActive;
    [ObservableProperty] private bool _isNewLinesBackgroundActive;
    [ObservableProperty] private string _newText;
    [ObservableProperty] private string _resolutionAndTopMargin;
    [ObservableProperty] private bool _isNewTextItalic;
    [ObservableProperty] private bool _submitOnFirstLetter;
    [ObservableProperty] private ImageSource? _sentenceImageSource;
    [ObservableProperty] private ImageSource? _itemImageSource;
    [ObservableProperty] private bool _canShrink;
    [ObservableProperty] private bool _canExpand;
    [ObservableProperty] private ObservableCollection<int> _noOfLinesToAutoDrawList;
    [ObservableProperty] private int _selectedNoOfLinesToAutoDraw;

    private List<ImageSplitterItem2> _letters;
    private ImageSplitterItem2 _splitItem;
    public NOcrChar NOcrChar { get; private set; }
    public NOcrDrawingCanvasView NOcrDrawingCanvas { get; set; }
    public Entry EntryNewText { get; set; }

    private List<OcrSubtitleItem> _ocrSubtitleItems;
    private int _startFromNumber;

    public NOcrCharacterAddPageModel()
    {
        _newText = string.Empty;
        _resolutionAndTopMargin = string.Empty;
        _linesForeground = new ObservableCollection<NOcrLine>();
        _linesBackground = new ObservableCollection<NOcrLine>();
        _isNewLinesForegroundActive = true;
        _isNewLinesBackgroundActive = false;
        _isNewTextItalic = false;
        _submitOnFirstLetter = false;
        _letters = new List<ImageSplitterItem2>();
        _splitItem = new ImageSplitterItem2(0, 0, new NikseBitmap2(1, 1));
        EntryNewText = new Entry();
        _drawModes = new ObservableCollection<NOcrDrawModeItem>(NOcrDrawModeItem.Items);
        _selectedDrawMode = _drawModes[0];

        const int maxLines = 500;
        _noOfLinesToAutoDrawList = new ObservableCollection<int>();
        for (var i = 0; i <= maxLines; i++)
        {
            _noOfLinesToAutoDrawList.Add(i);
        }

        _selectedNoOfLinesToAutoDraw = 100;
        NOcrChar = new NOcrChar();
        NOcrDrawingCanvas = new NOcrDrawingCanvasView();
        _ocrSubtitleItems = new List<OcrSubtitleItem>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Item"] is ImageSplitterItem2 item)
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
        }

        if (query["Bitmap"] is SKBitmap bitmap)
        {
            if (_splitItem.NikseBitmap != null)
            {
                var bmp = DrawActiveRectangle(bitmap,
                    new SKRect(_splitItem.X, _splitItem.Y,
                        _splitItem.X + _splitItem.NikseBitmap.Width, _splitItem.Y + _splitItem.NikseBitmap.Height));
                SentenceImageSource = bmp.ToImageSource();
            }
        }

        if (query["Letters"] is List<ImageSplitterItem2> letters)
        {
            _letters = letters;
        }

        if (query["OcrSubtitleItems"] is List<OcrSubtitleItem> ocrSubtitleItems)
        {
            _ocrSubtitleItems = ocrSubtitleItems;
        }

        if (query["StartFromNumber"] is int startFromNumber)
        {
            _startFromNumber = startFromNumber;
        }

        if (query["ItalicOn"] is bool italicOn)
        {
            IsNewTextItalic = italicOn;
        }

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

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterAddPage) },
            { "NOcrChar", NOcrChar },
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
