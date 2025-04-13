using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SubtitleAlchemist.Controls.DrawingCanvasControl;
using SubtitleAlchemist.Logic.Ocr;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class NOcrDbEditPageModel : ObservableObject, IQueryAttributable
{

    public ImageSource? ImageSource { get; set; }
    public string? Text { get; set; }
    public NOcrChar? NOcrChar { get; set; }
    public NOcrDbEditPage? Page { get; set; }
    public NOcrDrawingCanvasView NOcrDrawingCanvas { get; set; }
    public Label LabelStatusText { get; set; } = new();

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string NumberOfElements { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> CharacterList { get; set; }

    [ObservableProperty]
    public partial string? SelectedCharacter { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<NOcrChar> NOcrCharList { get; set; }

    [ObservableProperty]
    public partial NOcrChar? SelectedNOcrChar { get; set; }

    [ObservableProperty]
    public partial ImageSource? CurrentImageSource { get; set; }

    [ObservableProperty]
    public partial string CurrentImageResolution { get; set; }

    [ObservableProperty]
    public partial string CurrentText { get; set; }

    [ObservableProperty]
    public partial bool CurrentItalic { get; set; }

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

    private NOcrChar _newMatch;
    private NOcrDb _nOcrDb;
    private bool _closing;

    public NOcrDbEditPageModel()
    {
        Title = string.Empty;
        NumberOfElements = string.Empty;
        CharacterList = new ObservableCollection<string>();
        NOcrCharList = new ObservableCollection<NOcrChar>();
        CurrentImageResolution = string.Empty;
        NOcrDrawingCanvas = new NOcrDrawingCanvasView();
        LabelStatusText = new Label();
        _nOcrDb = new NOcrDb(string.Empty);
        _closing = false;
        StatusText = string.Empty;
        IsAddBetterMatchVisible = true;
        _newMatch = new NOcrChar(string.Empty);
        CurrentText = string.Empty;

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
        if (query["nOcrDb"] is NOcrDb nOcrDb)
        {
            _nOcrDb = nOcrDb;

            Title = $"Edit nOCR database \"{Path.GetFileNameWithoutExtension(nOcrDb.FileName)}\"";
            UpdateNumberOfElements();

            LoadUiData();
        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
            });
            return false;
        });
    }

    private void UpdateNumberOfElements()
    {
        NumberOfElements = $"{(_nOcrDb.OcrCharacters.Count + _nOcrDb.OcrCharactersExpanded.Count):#,###} elements";
    }

    private void LoadUiData()
    {
        var letters = _nOcrDb.OcrCharacters
            .Where(c => !string.IsNullOrEmpty(c.Text))
            .Select(c => c.Text)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        CharacterList = new ObservableCollection<string>(letters);

        if (CharacterList.Count > 0)
        {
            SelectedCharacter = CharacterList[0];
        }
    }

    [RelayCommand]
    private void DeleteMatch()
    {
        if (SelectedNOcrChar is { } nOcrChar)
        {
            _nOcrDb?.Remove(nOcrChar);
            SelectedNOcrChar = null;
            SelectedLetterItemChanged();
            ShowStatus("nOCR char deleted");
            UpdateNumberOfElements();
        }
    }

    [RelayCommand]
    private void UpdateMatch()
    {
        if (SelectedNOcrChar is { } nOcrChar)
        {
            nOcrChar.Text = CurrentText;
            nOcrChar.Italic = CurrentItalic;
            ShowStatus("nOCR char updated");
        }
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
    private async Task Ok()
    {
        _closing = true;
        _nOcrDb.Save();
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrDbEditPage) },
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        _closing = true;
        _nOcrDb.LoadOcrCharacters();
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(NOcrDbEditPage) },
        });
    }

    public void OnDisappearing()
    {
        _closing = true;
    }

    private void SelectedLetterItemChanged()
    {
        IsAddBetterMatchVisible = true;
        IsNewMatch = false;
        CurrentImageResolution = string.Empty;
        CurrentImageSource = null;
        NOcrDrawingCanvas.BackgroundImage = new SKBitmap(1, 1);
    }

    public void SelectedCharacterChanged()
    {
        if (SelectedCharacter is { } character)
        {
            NOcrCharList = new ObservableCollection<NOcrChar>(_nOcrDb.OcrCharacters
                .Where(c => c.Text == character)
                .OrderByDescending(c => c.ExpandCount)
                .ThenByDescending(c => c.Italic)
                .ThenByDescending(c => c.Width));

            if (NOcrCharList.Count > 0)
            {
                SelectedNOcrChar = NOcrCharList[0];
            }
        }
        else
        {
            NOcrCharList = new ObservableCollection<NOcrChar>();
            SelectedNOcrChar = null;
        }
    }

    public void SelectedNOcrCharChanged()
    {
        if (SelectedNOcrChar is { } nOcrChar)
        {
            CurrentText = nOcrChar.Text;
            CurrentItalic = nOcrChar.Italic;
            CurrentImageResolution = $"{nOcrChar.Width}x{nOcrChar.Height}, margin top: {nOcrChar.MarginTop}";

            NOcrDrawingCanvas.ZoomFactor = 1;
            NOcrDrawingCanvas.BackgroundImage = new SKBitmap(nOcrChar.Width, nOcrChar.Height);
            NOcrDrawingCanvas.ZoomFactor = 3;
            ShowOcrPoints(nOcrChar);
        }
        else
        {
            CurrentText = string.Empty;
            CurrentItalic = false;
            CurrentImageResolution = string.Empty;

            ShowOcrPoints(new NOcrChar(string.Empty));
        }
    }
}
