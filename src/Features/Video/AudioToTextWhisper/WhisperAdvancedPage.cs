﻿using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Features.Video.AudioToTextWhisper.Engines;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public class WhisperAdvancedPage : ContentPage
{
    private readonly WhisperAdvancedPageModel? _vm;
    public WhisperAdvancedPage(WhisperAdvancedPageModel vm)
    {
        vm.Page = this;
        _vm = vm;
        this.BindDynamicTheme();

        vm.WhisperEngines.Add(WhisperEngineCpp.StaticName, MakeEnginePage(new WhisperEngineCpp(), vm.EditorCppHelpText, vm.ScrollViewCppHelpText));
        vm.WhisperEngines.Add(WhisperEnginePurfviewFasterWhisperXxl.StaticName, MakeEnginePage(new WhisperEnginePurfviewFasterWhisperXxl(), vm.EditorPurfviewXxlHelpText, vm.ScrollViewPurfviewXxlHelpText));
        vm.WhisperEngines.Add(WhisperEngineOpenAi.StaticName, MakeEnginePage(new WhisperEngineOpenAi(), vm.EditorOpenAiHelpText, vm.ScrollViewOpenAiHelpText));
        vm.WhisperEngines.Add(WhisperEngineConstMe.StaticName, MakeEnginePage(new WhisperEngineConstMe(), vm.EditorConstMeHelpText, vm.ScrollViewConstMeHelpText));

        vm.WhisperHelpText.Add(WhisperEngineCpp.StaticName, vm.EditorCppHelpText);
        vm.WhisperHelpText.Add(WhisperEnginePurfviewFasterWhisperXxl.StaticName, vm.EditorPurfviewXxlHelpText);
        vm.WhisperHelpText.Add(WhisperEngineOpenAi.StaticName, vm.EditorOpenAiHelpText);
        vm.WhisperHelpText.Add(WhisperEngineConstMe.StaticName, vm.EditorConstMeHelpText);

        vm.WhisperScrollViews.Add(WhisperEngineCpp.StaticName, vm.ScrollViewCppHelpText);
        vm.WhisperScrollViews.Add(WhisperEnginePurfviewFasterWhisperXxl.StaticName, vm.ScrollViewPurfviewXxlHelpText);
        vm.WhisperScrollViews.Add(WhisperEngineOpenAi.StaticName, vm.ScrollViewOpenAiHelpText);
        vm.WhisperScrollViews.Add(WhisperEngineConstMe.StaticName, vm.ScrollViewConstMeHelpText);

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

        }.BindDynamicTheme();

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // title
                new RowDefinition { Height = GridLength.Auto }, // parameters
                new RowDefinition { Height = GridLength.Star }, // left menu and engine page
                new RowDefinition { Height = GridLength.Auto }, // buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
            },
            Padding = new Thickness(10),
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var title = new Label
        {
            Text = "Advanced Whisper Settings",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(title, 0, 0);
        Grid.SetColumnSpan(title, 2);


        var parameterGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // title
                new RowDefinition { Height = GridLength.Auto }, // parameters + button
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Padding = new Thickness(0),
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var parameterLabel = new Label
        {
            Text = "Parameters",
            Margin = 0,
        }.BindDynamicTheme();
        parameterGrid.Add(parameterLabel, 0, 0);
        parameterGrid.SetColumnSpan(parameterLabel, 2);

        var parameterEntry = new Entry
        {
            Placeholder = "Enter command line parameters",
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 0, 0, 10),
        }.Bind(nameof(vm.CurrentParameters));
        parameterGrid.Add(parameterEntry, 0, 1);

        var parameterButton = new Button
        {
            Text = "...",
            Margin = new Thickness(5),
            Command = vm.ShowHistoryCommand,
        }.BindDynamicTheme();
        parameterGrid.Add(parameterButton, 1, 1);
    
        grid.Add(parameterGrid, 0, 1);
        Grid.SetColumnSpan(parameterGrid, 2);


        vm.LeftMenu = new VerticalStackLayout
        {
            Children =
            {
                MakeLeftMenuItem(vm, WhisperEngineCpp.StaticName),
                MakeLeftMenuItem(vm, WhisperEnginePurfviewFasterWhisperXxl.StaticName),
                MakeLeftMenuItem(vm, WhisperEngineOpenAi.StaticName),
                MakeLeftMenuItem(vm, WhisperEngineConstMe.StaticName),
            },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 220,
        }.BindDynamicTheme();

        grid.Add(vm.LeftMenu, 0, 2);
        grid.Add(vm.EnginePage, 1, 2);


        var OkButton = new Button
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
                OkButton,
                cancelButton,
            }
        };

        grid.Add(buttonBar, 0, 3);
        grid.SetColumnSpan(buttonBar, 2);

        Content = grid;
    }

    private static IView MakeLeftMenuItem(WhisperAdvancedPageModel vm, string engineName)
    {
        var label = new Label
        {
            Margin = 5,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            FontSize = 15,
            Text = engineName,
            ClassId = engineName,
        }.BindDynamicTheme();

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (sender, e) => await vm.LeftMenuTapped(engineName);
        label.GestureRecognizers.Add(tapGesture);

        return label;
    }

    private View MakeEnginePage(IWhisperEngine engine, Editor editor, ScrollView scrollView)
    {
        var grid = new Grid
        {
            Padding = new Thickness(10),
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = engine.Name,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        editor.Text = "...";
        editor.VerticalOptions = LayoutOptions.Start;
        editor.HorizontalOptions = LayoutOptions.Start;
        editor.FontFamily = "RobotoMono";
        editor.IsReadOnly = true;
        editor.BindDynamicTheme();

        scrollView.VerticalOptions = LayoutOptions.Start;
        scrollView.HorizontalOptions = LayoutOptions.Start;
        scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Always;
        scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
        scrollView.Content = editor;
        scrollView.BindDynamicTheme();
        grid.Add(scrollView, 0, 1);

        if (engine.Name == WhisperEnginePurfviewFasterWhisperXxl.StaticName)
        {
            var buttonBar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                Children =
                {
                    new Button
                    {
                        Text = "Standard",
                        Command = _vm?.SetStandardParameterCommand,
                        Margin = new Thickness(5),
                    }.BindDynamicTheme(),
                    new Button
                    {
                        Text = "Standard Asia",
                        Command = _vm?.SetStandardAsiaParameterCommand,
                        Margin = new Thickness(5),
                    }.BindDynamicTheme(),
                    new Button
                    {
                        Text = "Sentence",
                        Command = _vm?.SetSentenceParameterCommand,
                        Margin = new Thickness(5),
                    }.BindDynamicTheme(),
                    new Button
                    {
                        Text = "Single words",
                        Command = _vm?.SetSingleWordsParameterCommand,
                        Margin = new Thickness(5),
                    }.BindDynamicTheme(),
                    new Button
                    {
                        Text = "Highlight word",
                        Command = _vm?.SetHighlightWordParameterCommand,
                        Margin = new Thickness(5),
                    }.BindDynamicTheme(),
                }
            };

            grid.Add(buttonBar, 0, 2);
        }

        return grid;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _vm?.OnSizeAllocated(width, height);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_vm == null)
        {
            return;
        }

        await _vm.LeftMenuTapped(_vm.DefaultWhisperEngineName);
    }
}
