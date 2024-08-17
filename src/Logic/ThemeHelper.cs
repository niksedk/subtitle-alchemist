using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Logic
{
    public static class ThemeHelper
    {
        public static void SetDynamicTheme(Switch control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Switch.ThumbColorProperty, ThemeNames.TextColor);
            control.SetDynamicResource(Switch.OnColorProperty, ThemeNames.BorderColor);
        }

        public static void SetDynamicTheme(Label control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Label.TextColorProperty, ThemeNames.TextColor);
        }

        public static void SetDynamicTheme(Grid control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
        }

        public static void SetDynamicTheme(Picker control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Picker.TextColorProperty, ThemeNames.TextColor);
            control.SetDynamicResource(Picker.TitleColorProperty, ThemeNames.TextColor);
        }

        public static void SetDynamicTheme(Page control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
        }

        public static void SetDynamicTheme(Border control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Border.StrokeProperty, ThemeNames.BorderColor);
        }

        public static void SetDynamicTheme(Entry control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Entry.TextColorProperty, ThemeNames.TextColor);
        }
    }
}
