using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Help.About;

public class AboutPopup : Popup
{
    public AboutPopup(AboutModel model)
    {
        BindingContext = model;

        CanBeDismissedByTappingOutsideOfPopup = false;

        Content = new ContentView
        {
            BackgroundColor = Colors.DarkGray,
            Content = new StackLayout
            {
                Margin = 1,
                Padding = 10,
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
                            .Source("btn_close.png"),
                        }
                    },

                    new Label()
                        .Text("About Subtitle Alchemist ALPHA 1")
                        .BindDynamicTheme()
                        .FontSize(30)
                        .Bold()
                        .Margin(20),

                    new Label()
                        .Text("Subtitle Alchemist is Free Software under the GNU Public License." + Environment.NewLine + "You may distribute, modify and use it freely.")
                        .BindDynamicTheme()
                        .FontSize(20)
                        .Margin(10),

                    new Label()
                        .Text("C# source code is available on GitHub: ")
                        .BindDynamicTheme()
                        .FontSize(15)
                        .Margin(10),

                    new ImageButton()
                        .Margin(10),
                }
            }.BindDynamicTheme(),
        };


        model.Popup = this;
    }
}


