using SkiaSharp.Views.Maui;

namespace SubtitleAlchemist.Controls.ColorPickerControl;

public class ColorPickerView : ContentView
{
    public event EventHandler<ColorPickedEventArgs>? ValueChanged;

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(ColorPickerView));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    private List<Color> _recentColors;
    private Slider _sliderRed;
    private Slider _sliderGreen;
    private Slider _sliderBlue;

    private BoxView _currentColorBox;
    private Entry _currentColorText;

    public ColorPickerView()
    {
        _recentColors = new List<Color>
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Yellow,
            Colors.Purple,
            Colors.Orange,
            Colors.Pink,
            Colors.Brown,
        };

        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        var colorPickerBox = new ColorPickerBox
        {
            WidthRequest = 200,
            HeightRequest = 200,
        };
        colorPickerBox.ColorPicked += (sender, args) =>
        {
            _currentColorBox.Color = args.Color;
            _currentColorText.Text = args.Color.ToHex();
            _sliderRed.Value = args.Color.Red * 255;
            _sliderGreen.Value = args.Color.Green * 255;
            _sliderBlue.Value = args.Color.Blue * 255;
            ValueChanged?.Invoke(this, args);
        };
        grid.Add(colorPickerBox, 0, 0);
        grid.Add(MakeEntryAndRecentGrid(), 1, 0);

        var sliderGrid = MakeSliderGrid();
        grid.Add(sliderGrid, 0, 1);
        Grid.SetColumnSpan(sliderGrid, 2);

        Content = grid;
    }

    private IView MakeEntryAndRecentGrid()
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        _currentColorBox = new BoxView
        {
            Color = Colors.Yellow,
            WidthRequest = 150,
            HeightRequest = 50,
        };
        grid.Add(_currentColorBox, 0, 0);

        _currentColorText = new Entry
        {
            Text = "#000000",
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            HorizontalTextAlignment = TextAlignment.Center,
            WidthRequest = 150,
        };
        grid.Add(_currentColorText, 0, 1);

        var row = 2;
        foreach (var chunk in _recentColors.Chunk(4))
        {
            var recentColorsStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
            };
            foreach (var color in chunk)
            {
                var recentColorBox = new BoxView
                {
                    Color = color,
                    WidthRequest = 25,
                    HeightRequest = 25,
                };
                recentColorBox.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        _currentColorBox.Color = color;
                        _currentColorText.Text = color.ToHex();
                        _sliderRed.Value = color.Red * 255;
                        _sliderGreen.Value = color.Green * 255;
                        _sliderBlue.Value = color.Blue * 255;
                        ValueChanged?.Invoke(this, new ColorPickedEventArgs(color.ToSKColor(), color));
                    })
                });
                recentColorsStack.Children.Add(recentColorBox);
            }

            grid.Add(recentColorsStack, 0, row);
            row++;
        }

        return grid;
    }

    private Grid MakeSliderGrid()
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        };

        var labelRed = new Label
        {
            Text = "Red",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
        };
        _sliderRed = new Slider
        {
            Maximum = 255,
            Minimum = 0,
            Value = 0,
            WidthRequest = 200,
        };
        var labelRedValue = new Label
        {
            Text = "50",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
        };
        _sliderRed.ValueChanged += (sender, args) =>
        {
            labelRedValue.Text = ((int)Math.Round(args.NewValue)).ToString();
            SetColorFromSliders();
        };
        grid.Add(labelRed, 0, 0);
        grid.Add(_sliderRed, 1, 0);
        grid.Add(labelRedValue, 2, 0);


        var labelGreen = new Label
        {
            Text = "Green",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
        };
        _sliderGreen = new Slider
        {
            Maximum = 255,
            Minimum = 0,
            Value = 0,
        };
        var labelGreenValue = new Label
        {
            Text = "50",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
            WidthRequest = 200,
        };
        _sliderGreen.ValueChanged += (sender, args) =>
        {
            labelGreenValue.Text = ((int)Math.Round(args.NewValue)).ToString();
            SetColorFromSliders();
        };
        grid.Add(labelGreen, 0, 1);
        grid.Add(_sliderGreen, 1, 1);
        grid.Add(labelGreenValue, 2, 1);


        var labelBlue = new Label
        {
            Text = "Blue",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
        };
        _sliderBlue = new Slider
        {
            Maximum = 255,
            Minimum = 0,
            Value = 0,
            WidthRequest = 200,
        };
        var labelBlueValue = new Label
        {
            Text = "50",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = (Color)Application.Current.Resources["TextColor"],
        };
        _sliderBlue.ValueChanged += (sender, args) =>
        {
            labelBlueValue.Text = ((int)Math.Round(args.NewValue)).ToString();
            SetColorFromSliders();
        };
        grid.Add(labelBlue, 0, 2);
        grid.Add(_sliderBlue, 1, 2);
        grid.Add(labelBlueValue, 2, 2);

        return grid;
    }

    private void SetColorFromSliders()
    {
        var color = Color.FromRgb(
            (int)Math.Round(_sliderRed.Value),
            (int)Math.Round(_sliderGreen.Value),
            (int)Math.Round(_sliderBlue.Value));

        _currentColorBox.Color = color;
        _currentColorText.Text = color.ToHex();

        ValueChanged?.Invoke(this, new ColorPickedEventArgs(color.ToSKColor(), color));
    }
}