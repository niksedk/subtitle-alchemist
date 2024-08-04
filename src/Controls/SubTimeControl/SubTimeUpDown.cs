namespace SubtitleAlchemist.Controls.SubTimeControl;

public partial class SubTimeUpDown : ContentView
{
    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(
        nameof(ErrorColor), typeof(Color), typeof(SubTimeUpDown));

    public static readonly BindableProperty DisplayTextProperty = BindableProperty.Create(
        nameof(DisplayText), typeof(string), typeof(SubTimeUpDown));

    public static readonly BindableProperty TimeProperty =
        BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(SubTimeUpDown), TimeSpan.Zero,
            propertyChanged: OnTimeChanged);

    /// <summary>
    /// The Color used to display the time code when an error occurs.
    /// </summary>
    public Color ErrorColor
    {
        get => (Color)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
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

    private Label _timeLabel;
    private ImageButton _upButton;
    private ImageButton _downButton;

    public SubTimeUpDown()
    {
        var fileName = System.Reflection.Assembly.GetExecutingAssembly()?.Location;
        var applicationPath = string.IsNullOrEmpty(fileName) ? string.Empty : Path.GetDirectoryName(fileName) ?? string.Empty;
        var imagePath = Path.Combine(applicationPath, "Resources", "Images", "Buttons");
        
        _timeLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
         _timeLabel.SetBinding(Label.TextProperty, new Binding(nameof(DisplayText), source: this));

        _upButton = new ImageButton
        {
            Source = ImageSource.FromFile(Path.Combine(imagePath, "Up.png")),
            WidthRequest = 16,
            HeightRequest = 16,
        };
        _upButton.Clicked += OnUpButtonClicked;

        _downButton = new ImageButton
        {
            Source = ImageSource.FromFile(Path.Combine(imagePath, "Down.png")),
            WidthRequest = 8,
            HeightRequest = 8,
        };
        _downButton.Clicked += OnDownButtonClicked;

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                _downButton,
                _timeLabel,
                _upButton
            }
        };

        Content = stackLayout;

        UpdateDisplayText();
    }

    static void OnTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SubTimeUpDown)bindable;
        control.UpdateDisplayText();
    }

    void OnUpButtonClicked(object sender, EventArgs e)
    {
        Time = Time.Add(TimeSpan.FromMinutes(1));
    }

    void OnDownButtonClicked(object sender, EventArgs e)
    {
        if (Time > TimeSpan.Zero)
        {
            Time = Time.Subtract(TimeSpan.FromMinutes(1));
        }
    }

    void UpdateDisplayText()
    {
        DisplayText = Time.ToString(@"hh\:mm");
    }
}