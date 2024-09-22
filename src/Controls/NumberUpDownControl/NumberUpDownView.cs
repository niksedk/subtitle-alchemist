using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.UpDownControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Controls.NumberUpDownControl;

public class NumberUpDownView : ContentView
{
    public int NumberOfDecimals { get; set; }

    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(
        nameof(ErrorColor), typeof(Color), typeof(NumberUpDownView));

    public static readonly BindableProperty DisplayTextProperty = BindableProperty.Create(
        nameof(DisplayText), typeof(string), typeof(NumberUpDownView));

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(NumberUpDownView), (double)0,
            propertyChanged: OnValueChanged);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(Value), typeof(Color), typeof(NumberUpDownView), Colors.Black,
            propertyChanged: OnTextColorChanged);

    public static readonly BindableProperty StepValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(NumberUpDownView), 1,
            propertyChanged: OnStepValueChanged);

    public static readonly BindableProperty StepValueFastProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(NumberUpDownView), 1,
            propertyChanged: OnStepValueFastChanged);

    public static readonly BindableProperty MinValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(NumberUpDownView), 1,
            propertyChanged: OnMinValueChanged);

    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(NumberUpDownView), 1,
            propertyChanged: OnMaxValueChanged);

    public static readonly BindableProperty PostfixProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(NumberUpDownView), string.Empty,
            propertyChanged: OnPostFixFastChanged);

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

    public int StepValue
    {
        get => (int)GetValue(StepValueProperty);
        set => SetValue(StepValueProperty, value);
    }

    public int StepValueFast
    {
        get => (int)GetValue(StepValueFastProperty);
        set => SetValue(StepValueFastProperty, value);
    }

    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public string Postfix
    {
        get => (string)GetValue(PostfixProperty);
        set => SetValue(PostfixProperty, value);
    }

    /// <summary>
    /// A string containing the time code in display text format.
    /// </summary>
    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private readonly Label _timeLabel;
    private readonly UpDownView _upDown;

    public NumberUpDownView()
    {
        _timeLabel = new Label().BindDynamicTheme();
        _timeLabel.SetBinding(Label.TextProperty, new Binding(nameof(DisplayText), source: this));

        _upDown = new UpDownView
        {
            WidthRequest = 25,
            HeightRequest = 25

        }.BindDynamicTheme();
        _upDown.ValueChanged += OnUpDownValueChanged;

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
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
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(2)
            },
            Content = stackLayout,
        }.BindDynamicTheme();

        Content = border;

        _upDown.StepValue = StepValue;
        _upDown.StepValueFast = StepValueFast;

        UpdateDisplayText();
    }

    private void OnUpDownValueChanged(object? sender, ValueChangedEventArgs e)
    {
        Value = e.NewValue;
        UpdateDisplayText();
        ValueChanged?.Invoke(this, e);
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is double number && bindable is NumberUpDownView control)
        {
            control._upDown.Value = (float)number;
            control.UpdateDisplayText();
        }
    }

    private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is Color color && bindable is NumberUpDownView control)
        {
            control._upDown.TextColor = color;
            control._upDown.TextColor = color;
            control._timeLabel.TextColor = color;
        }
    }

    private static void OnStepValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is int stepValue && bindable is NumberUpDownView control)
        {
            control._upDown.StepValue = stepValue;
        }
    }

    private static void OnStepValueFastChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is int stepValue && bindable is NumberUpDownView control)
        {
            control._upDown.StepValueFast = stepValue;
        }
    }

    private static void OnMinValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is int minValue && bindable is NumberUpDownView control)
        {
            control._upDown.MinValue = minValue;
        }
    }

    private static void OnMaxValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is int maxValue && bindable is NumberUpDownView control)
        {
            control._upDown.MaxValue = maxValue;
        }
    }

    private static void OnPostFixFastChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NumberUpDownView control)
        {
            control.UpdateDisplayText();
        }
    }

    public string ToDisplayText(double number, int numberOfDecimals)
    {
        if (numberOfDecimals <= 0)
        {
            return number.ToString("#,###,##0") + Postfix;
        }

        var format = "#,###,##0." + new string('0', numberOfDecimals);
        return number.ToString(format) + Postfix;
    }

    private void UpdateDisplayText()
    {
        DisplayText = ToDisplayText(Value, NumberOfDecimals);
    }
}