using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Files;

public partial class RestoreAutoBackupPageModel : ObservableObject
{
    [ObservableProperty]
    private bool _isOkButtonEnabled;

    [ObservableProperty]
    private DisplayFile? _selectedFile;

    [ObservableProperty]
    private ObservableCollection<DisplayFile> _files = new();

    public RestoreAutoBackupPage? Page { get; set; }
    public Label LabelOpenFolder { get; set; } = new();

    private readonly IAutoBackup _autoBackup;

    public RestoreAutoBackupPageModel(IAutoBackup autoBackup)
    {
        _autoBackup = autoBackup;
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

            Files.Add(new DisplayFile(fileName, displayDate, Utilities.FormatBytesToDisplayFileSize(fileInfo.Length)));
        }

        Files = new ObservableCollection<DisplayFile>(Files.OrderByDescending(f => f.DateAndTime));

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Files.Count > 0)
                {
                    SelectedFile = Files[0];
                }
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
        IsOkButtonEnabled = e.CurrentSelection.FirstOrDefault() is DisplayFile;
    }
}
