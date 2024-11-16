using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public class FixNamesPage : ContentPage
{
    public FixNamesPage(FixNamesPageModel vm)
    {
        BindingContext = vm;

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = GridLength.Auto }, // Title
                new() { Height = GridLength.Star }, // Names
                new() { Height = GridLength.Auto }, // Buttons
                new() { Height = GridLength.Auto }, // Extra names label
                new() { Height = GridLength.Auto }, // Extra names entry
                new() { Height = GridLength.Auto }, // Hits title
                new() { Height = GridLength.Star }, // Hits
                new() { Height = GridLength.Auto }, // Buttons
                },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = GridLength.Star },
            },
            Margin = new Thickness(20,20,20,20)
        };

        var row = 0;
        var labelTitle = new Label
        {
            Text = "Fix Names",
            HorizontalOptions = LayoutOptions.Start,
        }.AsTitle();
        grid.Add(labelTitle, 0, row);

        row++;
        grid.Add(MakeNamesView(vm));

        row++;
        var buttonSelectAll = new Button
        {
            Text = "Select All",
            Command = vm.NamesSelectAllCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonInvertSelection = new Button
        {
            Text = "Invert selection",
            Command = vm.NamesInvertSelectionCommand,
        }.BindDynamicTheme();

        var stackNamesButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                buttonSelectAll,
                buttonInvertSelection,
            }
        };
        grid.Add(stackNamesButtons, 0, row);

        row++;
        var labelExtraNames = new Label
        {
            Text = "Extra names",
            HorizontalOptions = LayoutOptions.Start,
        }.BindDynamicTheme();
        grid.Add(labelExtraNames, 0, row);

        row++;
        var entryExtraNames = new Entry
        {
            Text = "",
            Placeholder = "Enter extra names separated by comma",
        }.BindDynamicTheme();
        grid.Add(entryExtraNames, 0, row);

        row++;
        var labelHits = new Label
        {
            Text = "Hits",
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelHits, 0, row);

        row++;
        grid.Add(MakeHitsView(vm));

        // OK and Cancel buttons
        row++;
        var buttonOk = new Button
        {
            Text = "Ok",
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                buttonOk,
                buttonCancel,
            }
        };
        grid.Add(stackButtons, 0, row);

        Content = grid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private static Grid MakeNamesView(FixNamesPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = GridLength.Auto }, // Title
                new() { Height = GridLength.Auto }, // Names
                new() { Height = GridLength.Auto }, // Buttons
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = GridLength.Star },
            }
        };

        return grid;
    }

    private static Grid MakeHitsView(FixNamesPageModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = GridLength.Auto }, // Title
                new() { Height = GridLength.Auto }, // Names
                new() { Height = GridLength.Auto }, // Buttons
            },
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new() { Width = GridLength.Star },
            }
        };

        return grid;
    }
}
