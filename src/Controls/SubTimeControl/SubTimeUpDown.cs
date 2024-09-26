using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.UpDownControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Controls.SubTimeControl;

public class SubTimeUpDown : ContentView
{
    public bool UseShortFormat { get; set; }

    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(
        nameof(ErrorColor), typeof(Color), typeof(SubTimeUpDown));

    public static readonly BindableProperty DisplayTextProperty = BindableProperty.Create(
        nameof(DisplayText), typeof(string), typeof(SubTimeUpDown));

    public static readonly BindableProperty TimeProperty =
        BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(SubTimeUpDown), TimeSpan.Zero,
            propertyChanged: OnTimeChanged);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(Time), typeof(Color), typeof(SubTimeUpDown), Colors.Black,
            propertyChanged: OnTextColorChanged);

    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    /// <summary>
    /// The Color used to display the time code when an error occurs.
    /// </summary>
    public Color ErrorColor
    {
        get => (Color)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
    }

    /// <summary>
    /// The Color used to display the time code and up/down arrows.
    /// </summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// A string containing the time code in display text format.
    /// </summary>
    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    private readonly Label _timeLabel;
    private readonly UpDownView _upDown;

    public SubTimeUpDown()
    {
        _timeLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        }.BindDynamicTheme();
        _timeLabel.SetBinding(Label.TextProperty, new Binding(nameof(DisplayText), source: this));

        _upDown = new UpDownView
        {
        }.BindDynamicTheme();
        _upDown.ValueChanged += OnUpDownValueChanged;

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                _timeLabel,
                _upDown,
            }
        };

        var border = new Border
        {
            Padding = new Thickness(4, 1, 1, 0),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(2)
            },
            Content = stackLayout,
        }.BindDynamicTheme();

        Content = border;

        UpdateDisplayText();
    }

    private void OnUpDownValueChanged(object? sender, ValueChangedEventArgs e)
    {
        Time = TimeSpan.FromMilliseconds(e.NewValue);
        UpdateDisplayText();
        ValueChanged?.Invoke(this, e);
    }

    private static void OnTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is TimeSpan timeSpan && bindable is SubTimeUpDown control)
        {
            control._upDown.Value = (float)timeSpan.TotalMilliseconds;
            control.UpdateDisplayText();
        }
    }

    private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is Color color && bindable is SubTimeUpDown control)
        {
            control._upDown.TextColor = color;
            control._upDown.TextColor = color;
            control._timeLabel.TextColor = color;
        }
    }

    public static string ToDisplayText(TimeSpan time, bool useShortFormat)
    {
        var newDisplayText = time.ToString(@"hh\.mm\.ss\,fff");

        if (useShortFormat)
        {
            newDisplayText = newDisplayText.TrimStart('0', ':', '.');
            if (newDisplayText.Length == 0)
            {
                newDisplayText = "0";
            }
        }

        var prefix = time.TotalMilliseconds < 0 ? "-" : string.Empty;
        return prefix + newDisplayText;
    }

    private void UpdateDisplayText()
    {
        DisplayText = ToDisplayText(Time, UseShortFormat);
    }
}