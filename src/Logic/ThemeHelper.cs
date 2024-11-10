using CommunityToolkit.Maui.Views;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Controls.NumberUpDownControl;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Controls.UpDownControl;
using SubtitleAlchemist.Logic.Constants;
using System.Windows.Input;
using static Microsoft.Maui.Controls.VisualStateManager;

namespace SubtitleAlchemist.Logic
{
    public static class ThemeHelper
    {
        public static double TitleFontSize { get; set; } = 24;

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

        public static Switch BindDynamicThemeTextOnly(this Switch control)
        {
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

        public static Label WithLinkLabel(this Label control)
        {
            control.TextDecorations = TextDecorations.Underline;

            var pointerGesture = new PointerGestureRecognizer();
            pointerGesture.PointerEntered += (s, e) =>
            {
                control.TextColor = (Color)Application.Current!.Resources[ThemeNames.LinkColor];
            };
            pointerGesture.PointerExited += (s, e) =>
            {
                control.TextColor = (Color)Application.Current!.Resources[ThemeNames.TextColor];
            };
            control.GestureRecognizers.Add(pointerGesture);

            return control;
        }

        public static Label WithLinkLabel(this Label control, ICommand command)
        {
            control.GestureRecognizers.Add(new TapGestureRecognizer { Command = command });
            return control.WithLinkLabel();
        }

        public static CheckBox BindDynamicTheme(this CheckBox control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(CheckBox.ColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static SearchBar BindDynamicTheme(this SearchBar control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(SearchBar.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Switch BindToggledProperty(this Switch control, string bindName)
        {
            control.SetBinding(Switch.IsToggledProperty, bindName);
            return control;
        }

        public static Label BindIsVisible(this Label control, string bindName)
        {
            control.SetBinding(VisualElement.IsVisibleProperty, bindName);
            return control;
        }

        public static Label BindText(this Label control, string bindName)
        {
            control.SetBinding(Label.TextProperty, bindName);
            return control;
        }

        public static RadioButton BindIsChecked(this RadioButton control, string bindName)
        {
            control.SetBinding(RadioButton.IsCheckedProperty, bindName);
            return control;
        }

        public static SubTimeUpDown BindTime(this SubTimeUpDown control, string bindName)
        {
            control.SetBinding(SubTimeUpDown.TimeProperty, bindName, BindingMode.TwoWay);
            return control;
        }

        public static ProgressBar BindIsVisible(this ProgressBar control, string bindName)
        {
            control.SetBinding(VisualElement.IsVisibleProperty, bindName);
            return control;
        }

        public static ProgressBar BindProgress(this ProgressBar control, string bindName)
        {
            control.SetBinding(ProgressBar.ProgressProperty, bindName);
            return control;
        }

        public static Label BindDynamicThemeTextColorOnly(this Label control)
        {
            control.SetDynamicResource(Label.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Switch BindDynamicThemeTextColorOnly(this Switch control)
        {
            //control.SetDynamicResource(Switch.property.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Grid BindDynamicTheme(this Grid control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static VerticalStackLayout BindDynamicTheme(this VerticalStackLayout control)
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

        public static ContentView BindDynamicTheme(this ContentView control)
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
            control.SetDynamicResource(SubTimeUpDown.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static NumberUpDownView BindDynamicTheme(this NumberUpDownView control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(NumberUpDownView.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static Editor BindDynamicTheme(this Editor control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.SetDynamicResource(Editor.TextColorProperty, ThemeNames.TextColor);
            return control;
        }

        public static ScrollView BindDynamicTheme(this ScrollView control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            return control;
        }

        public static Popup BindDynamicTheme(this Popup control)
        {
            control.SetDynamicResource(VisualElement.BackgroundColorProperty, ThemeNames.BackgroundColor);
            control.Color = Colors.Transparent;
            return control;
        }

        /// <summary>
        /// Get the style for a grid selection, with background color set to the "active background" color.
        /// </summary>
        /// <returns>Style for setting on Page</returns>
        public static Style GetGridSelectionStyle()
        {

            Setter backgroundColorSetter = new() { Property = VisualElement.BackgroundColorProperty, Value = (Color)Application.Current!.Resources[ThemeNames.ActiveBackgroundColor] };
            VisualState stateSelected = new() { Name = CommonStates.Selected, Setters = { backgroundColorSetter } };
            VisualState stateNormal = new() { Name = CommonStates.Normal };
            VisualStateGroup visualStateGroup = new() { Name = nameof(CommonStates), States = { stateSelected, stateNormal } };
            VisualStateGroupList visualStateGroupList = new() { visualStateGroup };
            Setter vsgSetter = new() { Property = VisualStateGroupsProperty, Value = visualStateGroupList };
            Style style = new(typeof(Grid)) { Setters = { vsgSetter } };
            return style;
        }

        public static void UpdateTheme(string themeName)
        {
            var mergedDictionaries = Application.Current!.Resources.MergedDictionaries;
            if (mergedDictionaries == null)
            {
                return;
            }

            foreach (var dictionaries in mergedDictionaries)
            {
                if (themeName == "Light")
                {
                    SetThemeDictionaryColor(dictionaries, ThemeNames.BackgroundColor, Color.FromRgb(240, 240, 240));
                    SetThemeDictionaryColor(dictionaries, ThemeNames.TextColor, Colors.Black);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.SecondaryBackgroundColor, Color.FromRgb(253, 253, 253));
                    SetThemeDictionaryColor(dictionaries, ThemeNames.BorderColor, Colors.DarkGray);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.ActiveBackgroundColor, Colors.LightGreen);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.LinkColor, Colors.DarkBlue);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.TableHeaderBackgroundColor, Color.FromRgb(253, 253, 253));
                }
                else if (themeName == "Dark")
                {
                    SetThemeDictionaryColor(dictionaries, ThemeNames.BackgroundColor, Color.FromRgb(0x1F, 0x1F, 0x1F));
                    SetThemeDictionaryColor(dictionaries, ThemeNames.TextColor, Colors.WhiteSmoke);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.SecondaryBackgroundColor, Color.FromRgb(52, 52, 52));
                    SetThemeDictionaryColor(dictionaries, ThemeNames.BorderColor, Colors.DarkGray);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.ActiveBackgroundColor, Color.FromRgb(24, 52, 75));
                    SetThemeDictionaryColor(dictionaries, ThemeNames.LinkColor, Colors.LightSkyBlue);
                    SetThemeDictionaryColor(dictionaries, ThemeNames.TableHeaderBackgroundColor, Color.FromRgb(52, 52, 52));
                }
            }
        }

        private static void SetThemeDictionaryColor(ResourceDictionary dictionaries, string name, Color color)
        {
            var found = dictionaries.TryGetValue(name, out _);
            if (found)
            {
                dictionaries[name] = color;
            }
        }
    }
}
