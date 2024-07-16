using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;

namespace SubtitleAlchemist.Views.Options.DownloadFfmpeg;

public class DownloadFfmpegPopup : Popup
{
    public DownloadFfmpegPopup(DownloadFfmpegModel model)
    {
        BindingContext = model;

        Content = new StackLayout
        {
            BackgroundColor = Colors.Black,

            Children =
            {
                new Label()
                    .Text("Downloading ffmpeg...")
                    .TextColor(Colors.White)
                    .FontSize(30)
                    .Bold()
                    .Margin(10),
            }
        };

        model.Popup = this;
    }
}