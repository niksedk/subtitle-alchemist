using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace SubtitleAlchemist.Features.Files;

public partial class DisplayFile : ObservableObject
{
    [ObservableProperty] public partial string DateAndTime { get; set; }
    [ObservableProperty] public partial string FileName { get; set; }
    [ObservableProperty] public partial string Extension { get; set; }
    [ObservableProperty] public partial string Size { get; set; }

    public string FullPath { get; set; }

    public DisplayFile(string fileName, string dateAndTime, string size)
    {
        FullPath = fileName;

        var displayName = Path.GetFileNameWithoutExtension(fileName);
        if (displayName.Length > 20)
        {
            displayName = displayName.Remove(0, 20);
        }

        DateAndTime = dateAndTime;
        FileName = displayName;
        Extension = Path.GetExtension(fileName);
        Size = size;
    }
}
