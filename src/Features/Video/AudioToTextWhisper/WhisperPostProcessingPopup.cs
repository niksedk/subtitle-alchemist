using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.AudioToTextWhisper;

public class WhisperPostProcessingPopup : Popup
{
    public WhisperPostProcessingPopup(WhisperPostProcessingPopupModel vm)
    {
        BindingContext = vm;

        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
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
            WidthRequest = 500,
            HeightRequest = 500,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Post-processing",
            Margin = 2,
            ZIndex = -1000,
            FontSize = 20,
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0, 0);
        Grid.SetColumnSpan(titleLabel, 2);

        var closeLine = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                new ImageButton
                {
                    Command = vm.CancelCommand,
                    WidthRequest = 30,
                    HeightRequest = 30,
                    Margin = 10,
                    Source = "btn_close.png",
                    ZIndex = 1100,
                }.BindDynamicTheme(),
            }
        };
        grid.Add(closeLine, 0, 0);
        Grid.SetColumnSpan(closeLine, 2);


        grid.Add(new Label
        {
            Text = "Merge short lines",
            Margin = 2,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme(), 0, 1);
        vm.MergeShortLinesSwitch = new Switch { VerticalOptions = LayoutOptions.Center}.BindDynamicTheme();
        grid.Add(vm.MergeShortLinesSwitch, 1,1);


        grid.Add(new Label
        {
            Text = "Break/split long lines",
            Margin = 2,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme(), 0, 2);
        vm.BreakSplitLongLinesSwitch = new Switch { VerticalOptions = LayoutOptions.Center }.BindDynamicTheme();
        grid.Add(vm.BreakSplitLongLinesSwitch, 1, 2);


        grid.Add(new Label
        {
            Text = "Fix short durations",
            Margin = 2,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme(), 0, 3);
        vm.FixShortDurationsSwitch = new Switch { VerticalOptions = LayoutOptions.Center }.BindDynamicTheme();
        grid.Add(vm.FixShortDurationsSwitch, 1, 3);


        grid.Add(new Label
        {
            Text = "Fix casing",
            Margin = 2,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme(), 0, 4);
        vm.FixCasingSwitch = new Switch{ VerticalOptions = LayoutOptions.Center }.BindDynamicTheme();
        grid.Add(vm.FixCasingSwitch, 1, 4);


        grid.Add(new Label
        {
            Text = "Add periods",
            Margin = 2,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme(), 0, 5);
        vm.AddPeriodsSwitch = new Switch{ VerticalOptions = LayoutOptions.Center }.BindDynamicTheme();
        grid.Add(vm.AddPeriodsSwitch, 1, 5);

        var buttonOk = new Button
        {
            Text = "OK",
            Command = vm.OkCommand,
            Margin = 2,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
            Margin = 2,
        }.BindDynamicTheme();

        var buttonLine = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 15,
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        }.BindDynamicTheme();

        grid.Add(buttonLine, 0, 6);
        Grid.SetColumnSpan(buttonLine, 2);


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
            BackgroundColor = Colors.Transparent,
            Content = grid,
        }.BindDynamicTheme();

        Content = windowBorder;

        this.BindDynamicTheme();

        vm.Popup = this;
    }
}
