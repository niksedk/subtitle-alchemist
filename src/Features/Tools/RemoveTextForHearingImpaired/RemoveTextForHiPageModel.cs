using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Files;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public partial class RemoveTextForHiPageModel : ObservableObject
{
    [ObservableProperty] private bool _isRemoveBracketsOn;
    [ObservableProperty] private bool _isRemoveCurlyBracketsOn;
    [ObservableProperty] private bool _isRemoveParenthesesOn;
    [ObservableProperty] private bool _isRemoveCustomOn;
    [ObservableProperty] private string _customStart;
    [ObservableProperty] private string _customEnd;
    [ObservableProperty] private bool _isOnlySeparateLine;

    [ObservableProperty] private bool _isRemoveTextBeforeColonOn;
    [ObservableProperty] private bool _isRemoveTextBeforeColonUppercaseOn;
    [ObservableProperty] private bool _isRemoveTextBeforeColonSeparateLineOn;

    [ObservableProperty] private bool _isRemoveTextUppercaseLineOn;

    [ObservableProperty] private bool _isRemoveTextContainsOn;
    [ObservableProperty] private string _textContains;

    [ObservableProperty] private bool _isRemoveOnlyMusicSymbolsOn;

    [ObservableProperty] private bool _isRemoveInterjectionsOn;
    [ObservableProperty] private bool _isInterjectionsSeparateLineOn;

    [ObservableProperty] private DisplayFile? _selectedFile;

    [ObservableProperty] private ObservableCollection<string> _languages;
    [ObservableProperty] private string _selectedLanguage;

    [ObservableProperty] private ObservableCollection<RemoveItem> _fixes;


    public RemoveTextForHiPage? Page { get; set; }
    public Label LabelOpenFolder { get; set; } = new();

    private readonly IAutoBackup _autoBackup;

    public RemoveTextForHiPageModel(IAutoBackup autoBackup)
    {
        _autoBackup = autoBackup;
        _customStart = "?";
        _customEnd = "?";
        _textContains = string.Empty;
        _languages = new ObservableCollection<string> { "English" };
        _fixes = new ObservableCollection<RemoveItem>();
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task Ok()
    {
        if (SelectedFile is not { } file || Page == null)
        {
            return;
        }

        var answer = await Page.DisplayAlert(
            "Restore auto-backup file?",
            $"Do you want to restore \"{file.FileName}\" from {file.DateAndTime}?",
            "Yes",
            "No");

        if (!answer)
        {
            return;
        }

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(RestoreAutoBackupPage) },
            { "SubtitleFileName", file.FullPath },
        });
    }

    public void Initialize()
    {
        foreach (var fileName in _autoBackup.GetAutoBackupFiles())
        {
            var fileInfo = new FileInfo(fileName);

            var path = Path.GetFileName(fileName);
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            var displayDate = path[..19].Replace('_', ' ');
            displayDate = displayDate.Remove(13, 1).Insert(13, ":");
            displayDate = displayDate.Remove(16, 1).Insert(16, ":");

//            Files.Add(new DisplayFile(fileName, displayDate, Utilities.FormatBytesToDisplayFileSize(fileInfo.Length)));
        }

  //      Files = new ObservableCollection<DisplayFile>(Files.OrderByDescending(f => f.DateAndTime));

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //if (Files.Count > 0)
                //{
                //    SelectedFile = Files[0];
                //}
            });
            return false;
        });
    }

    public void OpenContainingFolderTapped()
    {
        if (SelectedFile is not { } file)
        {
            return;
        }

        UiUtil.OpenFolderFromFileName(file.FullPath);
    }

    public void OpenContainingFolderPointerEntered(object? sender, PointerEventArgs e)
    {
        LabelOpenFolder.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
    }

    public void OpenContainingFolderPointerExited(object? sender, PointerEventArgs e)
    {
        LabelOpenFolder.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
    }

    public void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        //IsOkButtonEnabled = e.CurrentSelection.FirstOrDefault() is DisplayFile;
    }
}
