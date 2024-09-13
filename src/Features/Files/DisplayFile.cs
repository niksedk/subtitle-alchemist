using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace SubtitleAlchemist.Features.Files;

public partial class DisplayFile : ObservableObject
{
    [ObservableProperty]
    private string _dateAndTime;

    [ObservableProperty]
    private string _fileName;

    [ObservableProperty]
    private string _extension;

    [ObservableProperty] 
    private string _size;

    public string FullFileName { get; set; }

    public DisplayFile(string fileName, string dateAndTime, string size)
    {
        FullFileName = fileName;

        var displayName = Path.GetFileNameWithoutExtension(fileName);
        if (displayName.Length > 20)
        {
            displayName = displayName.Remove(0, 20);
        }

        _dateAndTime = dateAndTime;
        _fileName = displayName;
        _extension = Path.GetExtension(fileName);
        _size = size;
    }
}
