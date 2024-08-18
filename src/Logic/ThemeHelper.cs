using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Controls.UpDownControl;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Logic
{
    public static class ThemeHelper
    {
        public static CollectionView BindDynamicTheme(this CollectionView control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }
        public static Switch BindDynamicTheme(this Switch control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Switch.ThumbColorProperty, ThemeNames.TextColor);
            control.SetDynamicResource(Switch.OnColorProperty, ThemeNames.BorderColor);
            return control;
        }

        public static Label BindDynamicTheme(this Label control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Label.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Grid BindDynamicTheme(this Grid control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static Picker BindDynamicTheme(this Picker control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Picker.TextColorProperty, ThemeNames.TextColor);
            control.SetDynamicResource(Picker.TitleColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Page BindDynamicTheme(this Page control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static MediaElement BindDynamicTheme(this MediaElement control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static Border BindDynamicTheme(this Border control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Border.StrokeProperty, ThemeNames.BorderColor);
            return control;
        }

        public static Entry BindDynamicTheme(this Entry control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Entry.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Button BindDynamicTheme(this Button control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.SecondaryBackgroundColor);
            control.SetDynamicResource(Button.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static ImageButton BindDynamicTheme(this ImageButton control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static StackLayout BindDynamicTheme(this StackLayout control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static ColorPickerView BindDynamicTheme(this ColorPickerView control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(ColorPickerView.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static UpDownView BindDynamicTheme(this UpDownView control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(UpDownView.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static SubTimeUpDown BindDynamicTheme(this SubTimeUpDown control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }


        public static Editor BindDynamicTheme(this Editor control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Editor.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Frame BindDynamicTheme(this Frame control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Frame.BorderColorProperty, ThemeNames.BorderColor);
            return control;
        }

        public static Popup BindDynamicTheme(this Popup control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }
    }
}
