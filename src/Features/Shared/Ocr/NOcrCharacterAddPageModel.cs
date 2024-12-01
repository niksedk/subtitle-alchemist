using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class NOcrCharacterAddPageModel : ObservableObject, IQueryAttributable
{
    public NOcrCharacterAddPage? Page { get; set; }

    [ObservableProperty] private ObservableCollection<NOcrLine> _linesForeground;
    [ObservableProperty] private NOcrLine? _selectedLineForeground;
    [ObservableProperty] private ObservableCollection<NOcrLine> _linesBackground;
    [ObservableProperty] private NOcrLine? _selectedLineBackground;
    [ObservableProperty] private bool _isNewLinesForegroundActive;
    [ObservableProperty] private bool _isNewLinesBackgroundActive;
    [ObservableProperty] private string _newText;
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
        _linesForeground = new ObservableCollection<NOcrLine>();
        _linesBackground = new ObservableCollection<NOcrLine>();
        _isNewLinesForegroundActive = true;
        _isNewLinesBackgroundActive = false;
        _isNewTextItalic = false;
        _submitOnFirstLetter = false;
        _letters = new List<ImageSplitterItem2>();
        _splitItem = new ImageSplitterItem2(0, 0, new NikseBitmap2(1, 1));
        EntryNewText = new Entry();

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
        GenerateLineSegments(SelectedNoOfLinesToAutoDraw, false, NOcrChar, _splitItem.NikseBitmap!);
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
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrCharacterAddPage) },
            { "NOcrChar", NOcrChar },
            { "OcrSubtitleItems", _ocrSubtitleItems },
            { "StartFromNumber", _startFromNumber },
            { "ItalicOn", IsNewTextItalic },
            { "UseOnce", true },
            { "Abort", false },
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

    public static void GenerateLineSegments(int maxNumberOfLines, bool veryPrecise, NOcrChar nOcrChar, NikseBitmap2 bitmap)
    {
        const int giveUpCount = 15_000;
        var r = new Random();
        var count = 0;
        var hits = 0;
        var tempVeryPrecise = veryPrecise;
        var verticalLineX = 2;
        var horizontalLineY = 2;
        while (hits < maxNumberOfLines && count < giveUpCount)
        {
            var start = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
            var end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));

            if (hits < 5 && count < 200 && nOcrChar.Width > 4 && nOcrChar.Height > 4) // vertical lines
            {
                start = new OcrPoint(0, 0);
                end = new OcrPoint(0, 0);
                for (; verticalLineX < nOcrChar.Width - 3; verticalLineX += 1)
                {
                    start = new OcrPoint(verticalLineX, 2);
                    end = new OcrPoint(verticalLineX, nOcrChar.Height - 3);

                    if (IsMatchPointForeGround(new NOcrLine(start, end), true, bitmap, nOcrChar))
                    {
                        verticalLineX++;
                        break;
                    }
                }
            }
            else if (hits < 10 && count < 400 && nOcrChar.Width > 4 && nOcrChar.Height > 4) // horizontal lines
            {
                start = new OcrPoint(0, 0);
                end = new OcrPoint(0, 0);
                for (; horizontalLineY < nOcrChar.Height - 3; horizontalLineY += 1)
                {
                    start = new OcrPoint(2, horizontalLineY);
                    end = new OcrPoint(nOcrChar.Width - 3, horizontalLineY);

                    if (IsMatchPointForeGround(new NOcrLine(start, end), true, bitmap, nOcrChar))
                    {
                        horizontalLineY++;
                        break;
                    }
                }
            }
            else if (hits < 20 && count < 2000) // a few large lines
            {
                for (var k = 0; k < 500; k++)
                {
                    if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) > nOcrChar.Height / 2)
                    {
                        break;
                    }

                    end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                }
            }
            else if (hits < 30 && count < 3000) // some medium lines
            {
                for (var k = 0; k < 500; k++)
                {
                    if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) < 15)
                    {
                        break;
                    }

                    end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                }
            }
            else // and a lot of small lines
            {
                for (var k = 0; k < 500; k++)
                {
                    if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) < 15)
                    {
                        break;
                    }

                    end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                }
            }

            var op = new NOcrLine(start, end);
            var ok = true;
            foreach (var existingOp in nOcrChar.LinesForeground)
            {
                if (existingOp.Start.X == op.Start.X && existingOp.Start.Y == op.Start.Y &&
                    existingOp.End.X == op.End.X && existingOp.End.Y == op.End.Y)
                {
                    ok = false;
                }
            }

            if (end.X == start.X && end.Y == start.Y)
            {
                ok = false;
            }

            if (ok && IsMatchPointForeGround(op, !tempVeryPrecise, bitmap, nOcrChar))
            {
                nOcrChar.LinesForeground.Add(op);
                hits++;
            }
            count++;
            if (count > giveUpCount - 100 && !tempVeryPrecise)
            {
                tempVeryPrecise = true;
            }
        }

        count = 0;
        hits = 0;
        horizontalLineY = 2;
        tempVeryPrecise = veryPrecise;
        while (hits < maxNumberOfLines && count < giveUpCount)
        {
            var start = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
            var end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));

            if (hits < 5 && count < 400 && nOcrChar.Width > 4 && nOcrChar.Height > 4) // horizontal lines
            {
                for (; horizontalLineY < nOcrChar.Height - 3; horizontalLineY += 1)
                {
                    start = new OcrPoint(2, horizontalLineY);
                    end = new OcrPoint(nOcrChar.Width - 2, horizontalLineY);

                    if (IsMatchPointBackGround(new NOcrLine(start, end), true, bitmap, nOcrChar))
                    {
                        horizontalLineY++;
                        break;
                    }
                }
            }
            if (hits < 10 && count < 1000) // a few large lines
            {
                for (var k = 0; k < 500; k++)
                {
                    if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) > nOcrChar.Height / 2)
                    {
                        break;
                    }
                    else
                    {
                        end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                    }
                }
            }
            else if (hits < 30 && count < 2000) // some medium lines
            {
                for (var k = 0; k < 500; k++)
                {
                    if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) < 15)
                    {
                        break;
                    }

                    end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                }
            }
            else // and a lot of small lines
            {
                for (var k = 0; k < 500; k++)
                {
                    if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) < 5)
                    {
                        break;
                    }
                    else
                    {
                        end = new OcrPoint(r.Next(nOcrChar.Width), r.Next(nOcrChar.Height));
                    }
                }
            }

            var op = new NOcrLine(start, end);
            var ok = true;
            foreach (var existingOp in nOcrChar.LinesBackground)
            {
                if (existingOp.Start.X == op.Start.X && existingOp.Start.Y == op.Start.Y &&
                    existingOp.End.X == op.End.X && existingOp.End.Y == op.End.Y)
                {
                    ok = false;
                }
            }
            if (ok && IsMatchPointBackGround(op, !tempVeryPrecise, bitmap, nOcrChar))
            {
                nOcrChar.LinesBackground.Add(op);
                hits++;
            }
            count++;

            if (count > giveUpCount - 100 && !tempVeryPrecise)
            {
                tempVeryPrecise = true;
            }
        }

        RemoveDuplicates(nOcrChar.LinesForeground);
        RemoveDuplicates(nOcrChar.LinesBackground);
    }

    private static bool IsMatchPointForeGround(NOcrLine op, bool loose, NikseBitmap2 nbmp, NOcrChar nOcrChar)
    {
        if (Math.Abs(op.Start.X - op.End.X) < 2 && Math.Abs(op.End.Y - op.Start.Y) < 2)
        {
            return false;
        }

        foreach (var point in op.ScaledGetPoints(nOcrChar, nbmp.Width, nbmp.Height))
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
            {
                var c = nbmp.GetPixel(point.X, point.Y);
                if (c.Alpha > 150)
                {
                }
                else
                {
                    return false;
                }

                if (loose)
                {
                    if (nbmp.Width > 10 && point.X + 1 < nbmp.Width)
                    {
                        c = nbmp.GetPixel(point.X + 1, point.Y);
                        if (c.Alpha > 150)
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (nbmp.Width > 10 && point.X >= 1)
                    {
                        c = nbmp.GetPixel(point.X - 1, point.Y);
                        if (c.Alpha > 150)
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (nbmp.Height > 10 && point.Y + 1 < nbmp.Height)
                    {
                        c = nbmp.GetPixel(point.X, point.Y + 1);
                        if (c.Alpha > 150)
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (nbmp.Height > 10 && point.Y >= 1)
                    {
                        c = nbmp.GetPixel(point.X, point.Y - 1);
                        if (c.Alpha > 150)
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    private static bool IsMatchPointBackGround(NOcrLine op, bool loose, NikseBitmap2 nbmp, NOcrChar nOcrChar)
    {
        foreach (var point in op.ScaledGetPoints(nOcrChar, nbmp.Width, nbmp.Height))
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < nbmp.Width && point.Y < nbmp.Height)
            {
                var c = nbmp.GetPixel(point.X, point.Y);
                if (c.Alpha > 150)
                {
                    return false;
                }

                if (nbmp.Width > 10 && point.X + 1 < nbmp.Width)
                {
                    c = nbmp.GetPixel(point.X + 1, point.Y);
                    if (c.Alpha > 150)
                    {
                        return false;
                    }
                }

                if (loose)
                {
                    if (nbmp.Width > 10 && point.X >= 1)
                    {
                        c = nbmp.GetPixel(point.X - 1, point.Y);
                        if (c.Alpha > 150)
                        {
                            return false;
                        }
                    }

                    if (nbmp.Height > 10 && point.Y + 1 < nbmp.Height)
                    {
                        c = nbmp.GetPixel(point.X, point.Y + 1);
                        if (c.Alpha > 150)
                        {
                            return false;
                        }
                    }

                    if (nbmp.Height > 10 && point.Y >= 1)
                    {
                        c = nbmp.GetPixel(point.X, point.Y - 1);
                        if (c.Alpha > 150)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private static void RemoveDuplicates(List<NOcrLine> lines)
    {
        var indicesToDelete = new List<int>();
        for (var index = 0; index < lines.Count; index++)
        {
            var outerPoint = lines[index];
            for (var innerIndex = 0; innerIndex < lines.Count; innerIndex++)
            {
                var innerPoint = lines[innerIndex];
                if (innerPoint != outerPoint)
                {
                    if (innerPoint.Start.X == innerPoint.End.X && outerPoint.Start.X == outerPoint.End.X && innerPoint.Start.X == outerPoint.Start.X)
                    {
                        // same y
                        if (Math.Max(innerPoint.Start.Y, innerPoint.End.Y) <= Math.Max(outerPoint.Start.Y, outerPoint.End.Y) &&
                            Math.Min(innerPoint.Start.Y, innerPoint.End.Y) >= Math.Min(outerPoint.Start.Y, outerPoint.End.Y))
                        {
                            if (!indicesToDelete.Contains(innerIndex))
                            {
                                indicesToDelete.Add(innerIndex);
                            }
                        }
                    }
                    else if (innerPoint.Start.Y == innerPoint.End.Y && outerPoint.Start.Y == outerPoint.End.Y && innerPoint.Start.Y == outerPoint.Start.Y)
                    {
                        // same x
                        if (Math.Max(innerPoint.Start.X, innerPoint.End.X) <= Math.Max(outerPoint.Start.X, outerPoint.End.X) &&
                            Math.Min(innerPoint.Start.X, innerPoint.End.X) >= Math.Min(outerPoint.Start.X, outerPoint.End.X))
                        {
                            if (!indicesToDelete.Contains(innerIndex))
                            {
                                indicesToDelete.Add(innerIndex);
                            }
                        }
                    }
                }
            }
        }

        foreach (var i in indicesToDelete.OrderByDescending(p => p))
        {
            lines.RemoveAt(i);
        }
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
}
