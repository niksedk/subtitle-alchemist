using CommunityToolkit.Maui.Views;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public class ColorPickerPopup : Popup
{
    public ColorPickerPopup(ColorPickerPopupModel model)
    {
        BindingContext = model;

        var fileName = System.Reflection.Assembly.GetExecutingAssembly()?.Location;
        var applicationPath = string.IsNullOrEmpty(fileName) ? string.Empty : Path.GetDirectoryName(fileName) ?? string.Empty;
        var imagePath = Path.Combine(applicationPath, "Resources", "Images", "Buttons");

        Content = new ContentView
        {
            BackgroundColor = Colors.DarkGray,
            Content = new StackLayout
            {
                
            },
        };


        model.Popup = this;
    }
}


