using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public class WhisperAdvancedPage : ContentPage
{
    public WhisperAdvancedPage(WhisperAdvancedModel vm)
    {
        vm.Page = this;
        this.BindDynamicTheme();

        vm.WhisperEngines.Add(WhisperEngineCpp.StaticName, MakeCppPage(vm));
        vm.WhisperEngines.Add(WhisperEnginePurfviewFasterWhisper.StaticName, MakePurfviewPage(vm));
        vm.WhisperEngines.Add(WhisperEnginePurfviewFasterWhisperXxl.StaticName, MakePurfviewXxlPage(vm));
        vm.WhisperEngines.Add(WhisperEngineOpenAi.StaticName, MakeOpenAiPage(vm));
        vm.WhisperEngines.Add(WhisperEngineConstMe.StaticName, MakeConstMePage(vm));

        vm.WhisperHelpLabels.Add(WhisperEngineCpp.StaticName, vm.LabelCppHelpText);
        vm.WhisperHelpLabels.Add(WhisperEnginePurfviewFasterWhisper.StaticName, vm.LabelPurfviewHelpText);
        vm.WhisperHelpLabels.Add(WhisperEnginePurfviewFasterWhisperXxl.StaticName, vm.LabelPurfviewXxlHelpText);
        vm.WhisperHelpLabels.Add(WhisperEngineOpenAi.StaticName, vm.LabelOpenAiHelpText);
        vm.WhisperHelpLabels.Add(WhisperEngineConstMe.StaticName, vm.LabelConstMeHelpText);

        BindingContext = vm;

        vm.EnginePage = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 600,
            HeightRequest = 600,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(2),
            },

        }.BindDynamicTheme();

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto },
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
            VerticalOptions = LayoutOptions.Fill,
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

        var title = new Label
        {
            Text = "Advanced Whisper Settings",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(title, 0, 0);
        Grid.SetColumnSpan(title, 2);

        var parameterLine = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
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
                    HorizontalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(0,0,0, 10),
                }.Bind(nameof(vm.CurrentParameters))
            }
        };
        grid.Add(parameterLine, 0, 1);
        Grid.SetColumnSpan(parameterLine, 2);

        vm.LeftMenu = new VerticalStackLayout
        {
            Children =
            {
                MakeLeftMenuItem(vm, WhisperEngineCpp.StaticName),
                MakeLeftMenuItem(vm, WhisperEnginePurfviewFasterWhisper.StaticName),
                MakeLeftMenuItem(vm, WhisperEnginePurfviewFasterWhisperXxl.StaticName),
                MakeLeftMenuItem(vm, WhisperEngineOpenAi.StaticName),
                MakeLeftMenuItem(vm, WhisperEngineConstMe.StaticName),
            },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();

        grid.Add(vm.LeftMenu, 0, 2);
        grid.Add(vm.EnginePage, 1, 2);


        var transcribeButton = new Button
        {
            Text = "OK",
           VerticalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(5),
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            VerticalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
            Margin = new Thickness(5),
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                transcribeButton,
                cancelButton,
            }
        };

        grid.Add(buttonBar, 0, 3);
        grid.SetColumnSpan(buttonBar, 2);


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

    private static IView MakeLeftMenuItem(WhisperAdvancedModel vm, string engineName)
    {
        var label = new Label
        {
            Margin = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 17,
            Text = engineName,
            ClassId = engineName,
        }.BindDynamicTheme();

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (sender, e) => await vm.LeftMenuTapped(engineName);
        label.GestureRecognizers.Add(tapGesture);

        return label;
    }

    private static View MakeCppPage(WhisperAdvancedModel vm)
    {
        var engine = new WhisperEngineCpp();

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
            Text = engine.Name,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        vm.LabelCppHelpText = new Editor
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            IsReadOnly = true,
        }.BindDynamicTheme();
        grid.Add(vm.LabelCppHelpText, 0, 1);

        return grid;
    }

    private static View MakeConstMePage(WhisperAdvancedModel vm)
    {
        var engine = new WhisperEngineConstMe();

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
            Text = engine.Name,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        vm.LabelConstMeHelpText = new Editor
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            IsReadOnly = true,
        }.BindDynamicTheme();
        grid.Add(vm.LabelConstMeHelpText, 0, 1);

        return grid;
    }

    private static View MakeOpenAiPage(WhisperAdvancedModel vm)
    {
        var engine = new WhisperEngineOpenAi();

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
            Text = engine.Name,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        vm.LabelOpenAiHelpText = new Editor
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Center,
            IsReadOnly = true,
        }.BindDynamicTheme();
        grid.Add(vm.LabelOpenAiHelpText, 0, 1);

        return grid;
    }

    private static View MakePurfviewPage(WhisperAdvancedModel vm)
    {
        var engine = new WhisperEngineOpenAi();

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
            Text = engine.Name,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        vm.LabelPurfviewHelpText = new Editor
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Center,
            IsReadOnly = true,
        }.BindDynamicTheme();
        grid.Add(vm.LabelPurfviewHelpText, 0, 1);

        return grid;
    }

    private static View MakePurfviewXxlPage(WhisperAdvancedModel vm)
    {
        var engine = new WhisperEnginePurfviewFasterWhisperXxl();

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
            Text = engine.Name,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        vm.LabelPurfviewXxlHelpText = new Editor
        {
            Text = "...",
            VerticalOptions = LayoutOptions.Center,
            IsReadOnly = true,
        }.BindDynamicTheme();
        grid.Add(vm.LabelPurfviewXxlHelpText, 0, 1);

        return grid;
    }
}
