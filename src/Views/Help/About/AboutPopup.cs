using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;

namespace SubtitleAlchemist.Views.Help.About;

public class AboutPopup : Popup
{
    public AboutPopup(AboutModel model)
    {
        BindingContext = model;

        Content = new ContentView
        {
            BackgroundColor = Colors.DarkGray,
            Content = new StackLayout
            {
                Margin = 1,
                Padding = 10,
                BackgroundColor = Colors.Black,
                Children =
                {
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


