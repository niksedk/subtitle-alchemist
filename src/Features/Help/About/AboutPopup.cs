using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Help.About;

public sealed class AboutPopup : Popup
{
    public AboutPopup(AboutPopupModel vm)
    {
        BindingContext = vm;
        vm.Popup = this;
        this.BindDynamicTheme();

        CanBeDismissedByTappingOutsideOfPopup = true;

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
                new() { Height = new GridLength(1, GridUnitType.Auto) },
            },
            Padding = new Thickness(25),
        }.BindDynamicTheme();

        var labelTitle = new Label()
            .Text("About Subtitle Alchemist ALPHA 1")
            .BindDynamicTheme()
            .FontSize(20)
            .Bold()
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme();
        grid.Add(labelTitle, 0);

        var labelContent = new Label()
            .Text("Subtitle Alchemist is Free Software under the MIT License." + Environment.NewLine +
                  "You may distribute, modify and use it freely." + Environment.NewLine +
                  Environment.NewLine +
                  "Subtitle Alchemist is a prototype/research project exploring the feasibility" + Environment.NewLine +
                  "of porting Subtitle Edit from Windows Forms to .NET MAUI" + Environment.NewLine +
                  "for cross-platform (Mac and Windows 11) compatibility.")
            .BindDynamicTheme()
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme();
        grid.Add(labelContent, 0, 1);

        var labelStatusHeader = new Label()
            .Text("Status")
            .BindDynamicTheme()
            .Bold()
            .FontSize(17)
            .Margin(new Thickness(0, 0, 0, 2)).BindDynamicTheme();
        grid.Add(labelStatusHeader, 0, 2);

        var labelStatusContent = new Label()
            .Text("This is an ALPHA release. It is not feature complete and may contain bugs." + Environment.NewLine +
                  "Please report any issues on GitHub.")
            .BindDynamicTheme()
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme();
        grid.Add(labelStatusContent, 0, 3);

        var labelGoalsHeader = new Label()
            .Text("Goals")
            .BindDynamicTheme()
            .Bold()
            .FontSize(17)
            .Margin(new Thickness(0, 0, 0, 2)).BindDynamicTheme();
        grid.Add(labelGoalsHeader, 0, 4);

        var labelGoalsContent = new Label()
            .Text("The goal of this project is to provide a free, open-source, cross-platform" + Environment.NewLine +
                  "subtitle editor for Mac and Windows 11 users.")
            .BindDynamicTheme()
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme();
        grid.Add(labelGoalsContent, 0, 5);

        var labelSupportHeader = new Label()
            .Text("Support")
            .BindDynamicTheme()
            .Bold()
            .FontSize(17)
            .Margin(new Thickness(0, 0, 0, 2)).BindDynamicTheme();
        grid.Add(labelSupportHeader, 0, 6);
        var labelSupportContent = new Label()
            .Text("Please consider supporting the development of Subtitle Alchemist by donating." + Environment.NewLine +
                  "Your support will help keep the project alive and free.")
            .BindDynamicTheme()
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme();
        grid.Add(labelSupportContent, 0, 7);

        var labelSourceCodeLink = new Label()
            .Text("C# source code is available on GitHub")
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme().WithLinkLabel(vm.OpenSourceLinkCommand);
        grid.Add(labelSourceCodeLink, 0, 8);

        var labelDonateLink = new Label()
            .Text("Donate to support the development of Subtitle Alchemist")
            .BindDynamicTheme()
            .Margin(new Thickness(0, 0, 0, 15)).BindDynamicTheme().WithLinkLabel(vm.OpenDonateLinkCommand);
        grid.Add(labelDonateLink, 0, 9);

        var buttonClose = new Button()
            .Text("Close")
            .Margin(new Thickness(0, 15, 0, 15)).BindDynamicTheme();
        buttonClose.Command = vm.CloseCommand;
        grid.Add(buttonClose, 0, 10);

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
    }
}
