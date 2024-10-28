namespace SubtitleAlchemist.Features.Options.Settings;

public class SettingItem
{
    public string Text { get; set; }
    public int TextWidth { get; set; }
    public string Hint { get; set; }
    public SettingItemType Type { get; set; }
    public View WholeView { get; set; }
    public Label? Label { get; set; }
    public SettingsPage.SectionName? SectionName { get; set; }

    public SettingItem(View wholeView, string text, int textWidth, string hint)
    {
        Text = text;
        TextWidth = textWidth;
        Hint = hint;
        Type = SettingItemType.Setting;
        WholeView = wholeView;

        SetLabel(text, textWidth);
    }

    private void SetLabel(string text, int textWidth)
    {
        if (!string.IsNullOrEmpty(text))
        {
            Label = new Label { Text = Text, VerticalOptions = LayoutOptions.Center };
            if (textWidth > 0)
            {
                Label.WidthRequest = textWidth;
            }
        }
    }

    public SettingItem(string text, int textWidth, string hint, View view)
    {
        Text = text;
        TextWidth = textWidth;
        Hint = hint;
        Type = SettingItemType.Setting;
        WholeView = view;
        SetLabel(text, textWidth);
    }

    public SettingItem(string hint, View view)
    {
        Text = string.Empty;
        TextWidth = 0;
        Hint = hint;
        Type = SettingItemType.Setting;
        WholeView = view;
    }

    public SettingItem(string text, SettingsPage.SectionName sectionName)
    {
        Text = text;
        TextWidth = 0;
        Hint = string.Empty;
        Type = SettingItemType.Category;
        SectionName = sectionName;

        WholeView = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            Text = text,
            FontSize = 21,
            Margin = new Thickness(0, 30, 0, 0),
            FontAttributes = FontAttributes.Bold,
        };
    }

    public SettingItem(string text)
    {
        Text = text;
        TextWidth = 0;
        Hint = string.Empty;
        Type = SettingItemType.SubCategory;

        WholeView = new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            Text = text,
            FontSize = 18,
            Margin = new Thickness(0, 20, 0, 0),
            FontAttributes = FontAttributes.Bold,
        };
    }

    public void Hide()
    {
        WholeView.IsVisible = false;
        if (Label != null)
        {
            Label.IsVisible = false;
        }
    }

    public void Show()
    {
        WholeView.IsVisible = true;
        if (Label != null)
        {
            Label.IsVisible = true;
        }
    }
}