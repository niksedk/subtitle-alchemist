using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public class WhisperAdvancedPopup : Popup
{

    public enum WhisperEngineNames
    {
        WhisperCpp,
        WhisperPurfview,
        WhisperPurfviewXxl,
    }

    private readonly WhisperAdvancedPopupModel _vm;

    public WhisperAdvancedPopup(WhisperAdvancedPopupModel vm)
    {
        _vm = vm;
        vm.Popup = this;
        this.BindDynamicTheme();

        vm.WhisperEngines.Add(WhisperEngineNames.WhisperCpp, MakeCppPage(vm));
        vm.WhisperEngines.Add(WhisperEngineNames.WhisperPurfview, MakePurfviewPage(vm));
        vm.WhisperEngines.Add(WhisperEngineNames.WhisperPurfviewXxl, MakePurfviewXxlPage(vm));

        BindingContext = vm;

        vm.EnginePage = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(2),
            },
            Content = vm.WhisperEngines[WhisperEngineNames.WhisperCpp],
        }.BindDynamicTheme();

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var closeLine = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                new ImageButton
                {
                    Command = vm.CloseCommand,
                    WidthRequest = 30,
                    HeightRequest = 30,
                    Margin = 10,
                    Source = "btn_close.png",
                }
            }
        };
        grid.Add(closeLine, 0, 0);
        Grid.SetColumnSpan(closeLine, 2);


        var parameterLine = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                new Label
                {
                    Text = "Parameters",
                    Margin = 2,
                },
                new Entry
                {
                    Placeholder = "Parameters",
                    WidthRequest = 600,
                    Margin = new Thickness(2,2,2, 15),
                }
            }
        };
        grid.Add(parameterLine, 0, 1);
        Grid.SetColumnSpan(parameterLine, 2);

        vm.LeftMenu = new VerticalStackLayout
        {
            Children =
            {
                MakeLeftMenuItem(vm, WhisperEngineNames.WhisperCpp, WhisperEngineCpp.StaticName),
                MakeLeftMenuItem(vm, WhisperEngineNames.WhisperPurfview, WhisperEnginePurfviewFasterWhisper.StaticName),
                MakeLeftMenuItem(vm, WhisperEngineNames.WhisperPurfviewXxl, WhisperEnginePurfviewFasterWhisperXxl.StaticName),
            }
        }.BindDynamicTheme();

        grid.Add(vm.LeftMenu, 0, 2);
        grid.Add(vm.EnginePage, 1, 2);

        var windowBorder = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5),
            },
            Content = grid,
        }.BindDynamicTheme();

        Content = windowBorder;
    }

    private static IView MakeLeftMenuItem(WhisperAdvancedPopupModel vm, WhisperEngineNames engineName, string text)
    {
        var label = new Label
        {
            Margin = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 17,
            Text = text,
            ClassId = engineName.ToString(),
        }.BindDynamicTheme();

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (sender, e) => await vm.LeftMenuTapped(sender, e, engineName);
        label.GestureRecognizers.Add(tapGesture);

        return label;
    }

    private static View MakeCppPage(WhisperAdvancedPopupModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            WidthRequest = 600,
            HeightRequest = 600,
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
                new ColumnDefinition { Width = GridLength.Star }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Whisper CPP",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelFavoriteSubtitleFormats = new Label
        {
            Text = "Select your favorite subtitle formats",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelFavoriteSubtitleFormats, 0, 1);

        return grid;
    }

    private static View MakePurfviewPage(WhisperAdvancedPopupModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            WidthRequest = 600,
            HeightRequest = 600,
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
                new ColumnDefinition { Width = GridLength.Star }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Purfview Faster Whisper",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelFavoriteSubtitleFormats = new Label
        {
            Text = "Select your favorite subtitle formats",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelFavoriteSubtitleFormats, 0, 1);

        return grid;
    }

    private static View MakePurfviewXxlPage(WhisperAdvancedPopupModel vm)
    {
        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            WidthRequest = 600,
            HeightRequest = 600,
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
                new ColumnDefinition { Width = GridLength.Star }
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = WhisperEnginePurfviewFasterWhisperXxl.StaticName,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelFavoriteSubtitleFormats = new Label
        {
            Text = "Select your favorite subtitle formats",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelFavoriteSubtitleFormats, 0, 1);

        return grid;
    }
}
