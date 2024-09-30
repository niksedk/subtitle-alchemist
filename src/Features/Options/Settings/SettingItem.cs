namespace SubtitleAlchemist.Features.Options.Settings;

public class SettingItem
{
    public string Text { get; set; }
    public int TextWidth { get; set; }
    public string Hint { get; set; }
    public SettingItemType Type { get; set; }
    public View WholeView { get; set; }
    public SettingsPage.SectionName? SectionName { get; set; }

    public SettingItem(View wholeView, string text, int textWidth, string hint)
    {
        Text = text;
        TextWidth = textWidth;
        Hint = hint;
        Type = SettingItemType.Setting;
        WholeView = wholeView;
    }

    public SettingItem(string text, int textWidth, string hint, View view)
    {
        Text = text;
        TextWidth = textWidth;
        Hint = hint;
        Type = SettingItemType.Setting;

        WholeView = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            Children =
            {
                new Label
                {
                    Text = text,
                    WidthRequest = textWidth,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                },
                view,
            },
        };
    }

    public SettingItem(string text, SettingsPage.SectionName sectionName)
    {
        Text = text;
        TextWidth = 0;
        Hint = string.Empty;
        Type = SettingItemType.Category;

        var label = new Label
        {
            Text = text,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        };
        label.FontSize = 20;
        label.FontAttributes = FontAttributes.Bold;

        WholeView = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { label }
        };
        WholeView.Margin = new Thickness(0, 25, 0, 0);

        SectionName = sectionName;
    }

    public SettingItem(string text)
    {
        Text = text;
        TextWidth = 0;
        Hint = string.Empty;
        Type = SettingItemType.SubCategory;

        var label = new Label
        {
            Text = text,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
        };

        label.FontSize = 18;
        label.FontAttributes = FontAttributes.Bold;

        WholeView = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { label }
        };
        WholeView.Margin = new Thickness(0, 15, 0,0);
    }
}

public enum SettingItemType
{
    Category,
    SubCategory,
    Setting,
}
