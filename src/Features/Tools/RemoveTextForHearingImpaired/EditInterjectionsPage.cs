using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Constants;

namespace SubtitleAlchemist.Features.Tools.RemoveTextForHearingImpaired;

public class EditInterjectionsPage : ContentPage
{
    private const int LeftMenuWidth = 400;

    public EditInterjectionsPage(EditInterjectionsPageModel vm)
    {
        this.BindDynamicTheme();
        Padding = new Thickness(10);
        vm.Page = this;
        BindingContext = vm;

        Resources.Add(ThemeHelper.GetGridSelectionStyle());

        var grid = new Grid
        {
            Padding = new Thickness(20),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Title
                new RowDefinition { Height = GridLength.Auto }, // Title 2
                new RowDefinition { Height = GridLength.Star }, // Options + fixes
                new RowDefinition { Height = GridLength.Auto }, // Buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            }
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "Interjections",
        }.AsTitle();

        var row = 0;
        grid.Add(titleLabel, 0, row++);
        Grid.SetColumnSpan(titleLabel, 2);

        var labelTitleEdit = new Label
        {
            Text = "Interjections"
        }.BindDynamicTheme();
        grid.Add(labelTitleEdit, 0, row);


        var labelSkipEdit = new Label
        {
            Text = "Skip if source starts with"
        }.BindDynamicTheme();
        grid.Add(labelTitleEdit, 1, row++);

        var editorInterjections = new Editor
        {
            WidthRequest = 250,
            HeightRequest = 500,
        }.BindDynamicTheme();
        grid.Add(editorInterjections, 0, row);


        var editorSkipList = new Editor
        {
            WidthRequest = 250,
            HeightRequest = 500,
        }.BindDynamicTheme();
        grid.Add(editorSkipList, 1, row++);


        var buttonOk = new Button
        {
            Text = "OK",
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        }.BindDynamicTheme();

        grid.Add(buttonBar, 0, row);
        Grid.SetColumnSpan(buttonBar, 2);

        Content = grid;
    }
}
