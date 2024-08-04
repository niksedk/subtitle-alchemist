using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;

namespace SubtitleAlchemist.Features.Help.About;

public class AboutPopup : Popup
{
    public AboutPopup(AboutModel model)
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
                Margin = 1,
                Padding = 10,
                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"],
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.End,
                        Children =
                        {                             
                            new ImageButton
                            {
                                Command = model.CloseCommand,
                            }
                            .Width(30)
                            .Height(30)
                            .Margin(10)
                            .Source (ImageSource.FromFile(Path.Combine(imagePath, "Close.png"))),
                        }
                    },

                    new Label()
                        .Text("About Subtitle Alchemist ALPHA 1")
                        .TextColor((Color)Application.Current.Resources["TextColor"])
                        .FontSize(30)
                        .Bold()
                        .Margin(20),

                    new Label()
                        .Text("Subtitle Alchemist is Free Software under the GNU Public License." + Environment.NewLine + "You may distribute, modify and use it freely.")
                        .TextColor((Color)Application.Current.Resources["TextColor"])
                        .FontSize(20)
                        .Margin(10),

                    new Label()
                        .Text("C# source code is available on GitHub: ")
                        .TextColor((Color)Application.Current.Resources["TextColor"])
                        .FontSize(15)
                        .Margin(10),

                    new ImageButton()
                        .Margin(10),
                }
            },
        };


        model.Popup = this;
    }
}


