using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty] private ObservableCollection<LetterItem> _letterItems;
    [ObservableProperty] private LetterItem? _selectedLetterItem;
    [ObservableProperty] private ImageSource? _currentImageSource;
    [ObservableProperty] private string _currentImageResolution;
    [ObservableProperty] private ImageSource? _matchImageSource;
    [ObservableProperty] private string _matchText;
    [ObservableProperty] private bool _matchIsItalic;

    private ImageSplitterItem2 _splitItem;
    private List<NOcrChar> _nOcrChars;

    public NOcrCharacterInspectPageModel()
    {
        _letterItems = new ObservableCollection<LetterItem>();
        _matchText = string.Empty;
        _splitItem = new ImageSplitterItem2(string.Empty);
        _nOcrChars = new List<NOcrChar>();
        _currentImageResolution = string.Empty;
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

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
            });
            return false;
        });
    }

    [RelayCommand]
    private async Task Ok()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void OnDisappearing()
    {
    }

    public void SelectedLetterItemChanged(object? sender, SelectionChangedEventArgs e)
    {
        CurrentImageResolution = string.Empty;

        if (SelectedLetterItem is { } letterItem)
        {
            if (letterItem.Item.NikseBitmap is  { } bitmap)
            {
                CurrentImageSource = bitmap.GetBitmap().ToImageSource();
                CurrentImageResolution = $"{bitmap.Width}x{bitmap.Height}";
            }

            if (letterItem.NOcrChar is { } match)
            {
                MatchText = match.Text;
                MatchIsItalic = match.Italic;
                //MatchImageSource = // draw lines on original image
            }
        }
    }
}
