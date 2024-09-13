using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Logic;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Files;

public partial class RestoreAutoBackupModel : ObservableObject
{
    [ObservableProperty]
    private DisplayFile? _selectedFile;

    [ObservableProperty]
    private ObservableCollection<DisplayFile> _files = new();

    public RestoreAutoBackupPage? Page { get; set; }
    public Label LabelOpenFolder { get; set; }

    private readonly IAutoBackup _autoBackup;

    public RestoreAutoBackupModel(IAutoBackup autoBackup)
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
        if (SelectedFile is not { } file)
        {
            return;
        }

        var subtitle = Subtitle.Parse(SelectedFile.FullFileName);

        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "Page", nameof(RestoreAutoBackupPage) },
            { "Subtitle", subtitle },
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

            var displayDate = path.Substring(0, 19).Replace('_', ' ');
            displayDate = displayDate.Remove(13, 1).Insert(13, ":");
            displayDate = displayDate.Remove(16, 1).Insert(16, ":");

            Files.Add(new DisplayFile(fileName, displayDate, Utilities.FormatBytesToDisplayFileSize(fileInfo.Length)));
        }

        if (Files.Count > 0)
        {
            SelectedFile = Files[0];
        }
    }

    public void OpenContainingFolderTapped()
    {
        if (SelectedFile is not { } file)
        {
            return;
        }

        UiUtil.OpenFolderFromFileName(SelectedFile.FullFileName);
    }

    public void OpenContainingFolderPointerEntered(object? sender, PointerEventArgs e)
    {
        LabelOpenFolder.TextColor = (Color)Application.Current.Resources[ThemeNames.LinkColor];
    }

    public void OpenContainingFolderPointerExited(object? sender, PointerEventArgs e)
    {
        LabelOpenFolder.TextColor = (Color)Application.Current.Resources[ThemeNames.TextColor];
    }
}
