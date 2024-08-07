using CommunityToolkit.Mvvm.Input;
using static SubtitleAlchemist.Features.Options.Settings.SettingsPage;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class SettingsViewModel
{
    public string Theme { get; set; }
    public Dictionary<PageNames, View> Pages { get; set; }
    public Border Page { get; set; }

    public SettingsViewModel()
    {
        Pages = new Dictionary<PageNames, View>();
        Page = new Border();
        Theme = "Dark";
    }

    public Action Tapped(PageNames pageName)
    {
        return async() =>
        {
            if (Page.Content != null)
            {
                await Page.Content.FadeTo(0, 200);
            }

            Page.Content = Pages[pageName];

            await Page.Content.FadeTo(0, 0);
            await Page.Content.FadeTo(1, 200);
        };
    }
}