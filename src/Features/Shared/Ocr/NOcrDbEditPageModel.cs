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

    [ObservableProperty] private string _title;
    [ObservableProperty] private string _numberOfElements;
    [ObservableProperty] private ObservableCollection<string> _characterList;
    [ObservableProperty] private string? _selectedCharacter;
    [ObservableProperty] private ObservableCollection<NOcrChar> _nOcrCharList;
    [ObservableProperty] private NOcrChar? _selectedNOcrChar;
    [ObservableProperty] private ImageSource? _currentImageSource;
    [ObservableProperty] private string _currentImageResolution;
    [ObservableProperty] private string _currentText;
    [ObservableProperty] private bool _currentItalic;
    [ObservableProperty] private string _statusText;
    [ObservableProperty] private ObservableCollection<int> _noOfLinesToAutoDrawList;
    [ObservableProperty] private int _selectedNoOfLinesToAutoDraw;
    [ObservableProperty] private bool _isNewMatch;
    [ObservableProperty] private bool _isAddBetterMatchVisible;

    private NOcrChar _newMatch;
    private NOcrDb _nOcrDb;
    private bool _closing;

    public NOcrDbEditPageModel()
    {
        _title = string.Empty;
        _numberOfElements = string.Empty;
        _characterList = new ObservableCollection<string>();
        _nOcrCharList = new ObservableCollection<NOcrChar>();
        _currentImageResolution = string.Empty;
        NOcrDrawingCanvas = new NOcrDrawingCanvasView();
        LabelStatusText = new Label();
        _nOcrDb = new NOcrDb(string.Empty);
        _closing = false;
        _statusText = string.Empty;
        _isAddBetterMatchVisible = true;
        _newMatch = new NOcrChar(string.Empty);
        _currentText = string.Empty;

        const int maxLines = 500;
        _noOfLinesToAutoDrawList = new ObservableCollection<int>();
        for (var i = 0; i <= maxLines; i++)
        {
            _noOfLinesToAutoDrawList.Add(i);
        }

        _selectedNoOfLinesToAutoDraw = 100;
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
