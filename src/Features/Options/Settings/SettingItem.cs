using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic.Constants;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class SettingItem : ObservableObject
{
    public string Text { get; set; }
    public int TextWidth { get; set; }
    public string Hint { get; set; }
    public SettingItemType Type { get; set; }

    [ObservableProperty]
    public partial View WholeView { get; set; }
    public Label? Label { get; set; }
    public SettingsPage.SectionName? SectionName { get; set; }

    public SettingItem()
    {
        Text = string.Empty;
        Hint = string.Empty;
        WholeView = new Label();
    }

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
            Label = new Label
            {
                Text = Text, 
                VerticalOptions = LayoutOptions.Center, 
                Margin = new Thickness(10, 0, 0, 0),
            };
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

    public static SettingItem MakeFooter(SettingsPage.SectionName sectionName)
    {
        var settingsItem = new SettingItem();

        var border = new Border
        {
            StrokeThickness = 0,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.SecondaryBackgroundColor],
            Margin = new Thickness(0, 0, 0, 10),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(0, 0, 10, 10),
            },
            Content = new BoxView
            {
                MinimumWidthRequest = 50,
                MinimumHeightRequest = 25,
                BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.SecondaryBackgroundColor],
            },
        };

        var contentView = new ContentView
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Content = border,
        };

        settingsItem.SectionName = sectionName;
        settingsItem.WholeView = contentView;
        settingsItem.Hint = string.Empty;
        settingsItem.TextWidth = 0;
        settingsItem.Type = SettingItemType.Footer;
        settingsItem.Label = null;

        return settingsItem;
    }

    public SettingItem(string text, SettingsPage.SectionName sectionName)
    {
        Text = text;
        TextWidth = 0;
        Hint = string.Empty;
        Type = SettingItemType.Category;
        SectionName = sectionName;

        var border = new Border
        {
            StrokeThickness = 0,
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.SecondaryBackgroundColor],
            Margin = new Thickness(0, 10, 0, 0),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(10, 10, 0, 0),
            },
            Content = new Label
            {
                Text = text,
                FontSize = 21,
                Margin = new Thickness(10, 20, 0, 0),
                FontAttributes = FontAttributes.Bold,
            },
        };

        var contentView = new ContentView
        {
            BackgroundColor = (Color)Application.Current!.Resources[ThemeNames.BackgroundColor],
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 30),
            Content = border,
        };

        WholeView = contentView;
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
            Margin = new Thickness(10, 20, 0, 0),
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