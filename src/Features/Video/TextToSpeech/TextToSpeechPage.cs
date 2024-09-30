using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Features.Video.BurnIn;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public class TextToSpeechPage : ContentPage
{
    private readonly Grid _grid;

    public TextToSpeechPage(TextToSpeechPageModel vm)
    {
        BindingContext = vm;
        ThemeHelper.GetGridSelectionStyle();

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }, // Subtitle list
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();
        _grid = pageGrid;


        var topBar = MakeTopBar(vm);
        pageGrid.Add(topBar, 0);

      

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 15, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Margin = new Thickness(0, 25, 0, 0),
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        }.BindDynamicTheme();

        pageGrid.Add(okCancelBar, 0, 5);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    public void Initialize(Subtitle subtitle, TextToSpeechPageModel vm)
    {

    }

    private static View MakeTopBar(TextToSpeechPageModel vm)
    {
        var topBar = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star }, // Title
                new ColumnDefinition { Width = GridLength.Star }, // Adjust via
            },
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0, 0, 10),
        }.BindDynamicTheme();

        var labelTitle = new Label
        {
            Text = Se.Language.AdjustDurations.Title,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 25),
            FontSize = 18,
        }.BindDynamicThemeTextColorOnly();
        topBar.Add(labelTitle, 0);
        topBar.SetColumnSpan(labelTitle, 2);

        var labelAdjustVia = new Label
        {
            Text = Se.Language.AdjustDurations.AdjustVia,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicThemeTextColorOnly();
        topBar.Add(labelAdjustVia, 0, 1);

        var pickerAdjustVia = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerAdjustVia.SetBinding(Picker.ItemsSourceProperty, nameof(vm.AdjustViaItems));
        pickerAdjustVia.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedAdjustViaItem));
        topBar.Add(pickerAdjustVia, 1, 1);

        return topBar;
    }
}
