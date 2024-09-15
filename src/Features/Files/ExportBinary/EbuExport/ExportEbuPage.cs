using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Files.ExportBinary.EbuExport;

public class ExportEbuPage : ContentPage
{
    public ExportEbuPage(ExportEbuModel vm)
    {
        BindingContext = vm;

        var pageGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // top bar
                new RowDefinition { Height = GridLength.Star }, // main content
                new RowDefinition { Height = GridLength.Auto }, // bottom bar with buttons
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();


        var topBar = MakeTopBar(vm);
        pageGrid.Add(topBar, 0);

        vm.GeneralView = MakeGeneralGrid(vm);
        pageGrid.Add(vm.GeneralView, 0, 1);
        pageGrid.SetColumnSpan(vm.GeneralView, 3);

        vm.TextAndTimingView = MakeTextAndTimingView(vm);
        vm.TextAndTimingView.IsVisible = false;
        pageGrid.Add(vm.TextAndTimingView, 0, 1);
        pageGrid.SetColumnSpan(vm.TextAndTimingView, 3);

        vm.ErrorsView = MakeErrorsView(vm);
        vm.ErrorsView.IsVisible = false;
        pageGrid.Add(vm.ErrorsView, 0, 1);
        pageGrid.SetColumnSpan(vm.ErrorsView, 3);

        var buttonOk = new Button
        {
            Text = "Ok",
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 10, 10),
            Command = vm.OkCommand,
        }.BindDynamicTheme();

        var buttonCancel = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 0, 10),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var okCancelBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        }.BindDynamicTheme();

        pageGrid.Add(okCancelBar, 0, 2);

        Content = pageGrid;

        this.BindDynamicTheme();

        vm.Page = this;
    }

    private static Grid MakeTopBar(ExportEbuModel vm)
    {
        var topBar = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star }, // General 
                new ColumnDefinition { Width = GridLength.Star }, // Text and timing 
                new ColumnDefinition { Width = GridLength.Star }, // Errors
            },
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelGeneral = new Label
        {
            Text = "General",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            Padding = new Thickness(10),
        }.BindDynamicThemeTextColorOnly();
        labelGeneral.SetBinding(Label.BackgroundColorProperty, nameof(vm.GeneralBackgroundColor));
        var tapGestureRecognizerGeneral = new TapGestureRecognizer();
        tapGestureRecognizerGeneral.Tapped += vm.GeneralTapped;
        labelGeneral.GestureRecognizers.Add(tapGestureRecognizerGeneral);
        topBar.Add(labelGeneral, 0);

        var labelTextAndTiming = new Label
        {
            Text = "Text and timing",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            Padding = new Thickness(10),
        }.BindDynamicThemeTextColorOnly();
        labelTextAndTiming.SetBinding(Label.BackgroundColorProperty, nameof(vm.TextAndTimingBackgroundColor));
        var tapGestureRecognizerTextAndTiming = new TapGestureRecognizer();
        tapGestureRecognizerTextAndTiming.Tapped += vm.TextAndTimingTapped;
        labelTextAndTiming.GestureRecognizers.Add(tapGestureRecognizerTextAndTiming);
        topBar.Add(labelTextAndTiming, 1);

        var labelErrors = new Label
        {
            Text = "Errors",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            Padding = new Thickness(10),
        }.BindDynamicThemeTextColorOnly();
        labelErrors.SetBinding(Label.BackgroundColorProperty, nameof(vm.ErrorsBackgroundColor));
        var tapGestureRecognizerErrors = new TapGestureRecognizer();
        tapGestureRecognizerErrors.Tapped += vm.ErrorsTapped;
        labelErrors.GestureRecognizers.Add(tapGestureRecognizerErrors);
        topBar.Add(labelErrors, 2);

        return topBar;
    }

    private static Border MakeGeneralGrid(ExportEbuModel vm)
    {
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
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
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
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelCodePageNumber = new Label
        {
            Text = "Code Page Number",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelCodePageNumber, 0);

        var entryCodePageNumber = new Entry
        {
            Placeholder = "Code Page Number",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryCodePageNumber, 1);
        entryCodePageNumber.SetBinding(Entry.TextProperty, nameof(vm.CodePageNumber));

        var labelDiskFormatCode = new Label
        {
            Text = "Disk Format Code",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelDiskFormatCode, 0, 1);

        var pickerDiskFormatCode = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(pickerDiskFormatCode, 1, 1);
        pickerDiskFormatCode.SetBinding(Picker.ItemsSourceProperty, nameof(vm.DiskFormatCodes));

        var labelFrameRate = new Label
        {
            Text = "Frame Rate",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelFrameRate, 0, 2);

        var pickerFrameRate = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(pickerFrameRate, 1, 2);
        pickerFrameRate.SetBinding(Picker.ItemsSourceProperty, nameof(vm.FrameRates));

        var labelDisplayStandardCode = new Label
        {
            Text = "Display Standard Code",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelDisplayStandardCode, 0, 3);

        var pickerDisplayStandardCode = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(pickerDisplayStandardCode, 1, 3);
        pickerDisplayStandardCode.SetBinding(Picker.ItemsSourceProperty, nameof(vm.DisplayStandardCodes));

        var labelCharacterTable = new Label
        {
            Text = "Character Table",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelCharacterTable, 0, 4);

        var pickerCharacterTable = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(pickerCharacterTable, 1, 4);
        pickerCharacterTable.SetBinding(Picker.ItemsSourceProperty, nameof(vm.CharacterTables));

        var labelLanguageCode = new Label
        {
            Text = "Language Code",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelLanguageCode, 0, 5);

        var pickerLanguageCode = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(pickerLanguageCode, 1, 5);
        pickerLanguageCode.SetBinding(Picker.ItemsSourceProperty, nameof(vm.LanguageCodes));

        var labelOriginalProgramTitle = new Label
        {
            Text = "Original Program Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelOriginalProgramTitle, 0, 6);

        var entryOriginalProgramTitle = new Entry
        {
            Placeholder = "Original Program Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryOriginalProgramTitle, 1, 6);
        entryOriginalProgramTitle.SetBinding(Entry.TextProperty, nameof(vm.OriginalProgramTitle));

        var labelOriginalEpisodeTitle = new Label
        {
            Text = "Original Episode Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelOriginalEpisodeTitle, 0, 7);

        var entryOriginalEpisodeTitle = new Entry
        {
            Placeholder = "Original Episode Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryOriginalEpisodeTitle, 1, 7);
        entryOriginalEpisodeTitle.SetBinding(Entry.TextProperty, nameof(vm.OriginalEpisodeTitle));

        var labelTranslatedProgramTitle = new Label
        {
            Text = "Translated Program Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelTranslatedProgramTitle, 0, 8);

        var entryTranslatedProgramTitle = new Entry
        {
            Placeholder = "Translated Program Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryTranslatedProgramTitle, 1, 8);
        entryTranslatedProgramTitle.SetBinding(Entry.TextProperty, nameof(vm.TranslatedProgramTitle));

        var labelTranslatedEpisodeTitle = new Label
        {
            Text = "Translated Episode Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelTranslatedEpisodeTitle, 0, 9);

        var entryTranslatedEpisodeTitle = new Entry
        {
            Placeholder = "Translated Episode Title",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryTranslatedEpisodeTitle, 1, 9);
        entryTranslatedEpisodeTitle.SetBinding(Entry.TextProperty, nameof(vm.TranslatedEpisodeTitle));

        var labelTranslatorName = new Label
        {
            Text = "Translator Name",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelTranslatorName, 0, 10);

        var entryTranslatorName = new Entry
        {
            Placeholder = "Translator Name",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryTranslatorName, 1, 10);
        entryTranslatorName.SetBinding(Entry.TextProperty, nameof(vm.TranslatorName));


        // label/input column 2
        var rowNo = 0;

        var labelSubtitleListReferenceCode = new Label
        {
            Text = "Subtitle List Reference Code",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelSubtitleListReferenceCode, 2, rowNo);

        var entrySubtitleListReferenceCode = new Entry
        {
            Placeholder = "Subtitle List Reference Code",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entrySubtitleListReferenceCode, 3, rowNo);
        entrySubtitleListReferenceCode.SetBinding(Entry.TextProperty, nameof(vm.SubtitleListReferenceCode));

        rowNo++;
        var labelCountryOfOrigin = new Label
        {
            Text = "Country Of Origin",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelCountryOfOrigin, 2, rowNo);

        var entryCountryOfOrigin = new Entry
        {
            Placeholder = "Country Of Origin",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryCountryOfOrigin, 3, rowNo);
        entryCountryOfOrigin.SetBinding(Entry.TextProperty, nameof(vm.CountryOfOrigin));

        rowNo++;
        var labelTimeCodeStatus = new Label
        {
            Text = "Time Code Status",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelTimeCodeStatus, 2, rowNo);

        var pickerTimeCodeStatus = new Picker
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(pickerTimeCodeStatus, 3, rowNo);
        pickerTimeCodeStatus.SetBinding(Picker.ItemsSourceProperty, nameof(vm.TimeCodeStatusList));

        rowNo++;
        var labelStartOfProgramme = new Label
        {
            Text = "Start Of Programme",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelStartOfProgramme, 2, rowNo);

        var subTimeUpDownStartOfProgramme = new SubTimeUpDown
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(subTimeUpDownStartOfProgramme, 3, rowNo);
        subTimeUpDownStartOfProgramme.SetBinding(SubTimeUpDown.TimeProperty, nameof(vm.StartOfProgramme));

        rowNo++;
        rowNo++;
        var labelRevisionNumber = new Label
        {
            Text = "Revision Number",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelRevisionNumber, 2, rowNo);

        var entryRevisionNumber = new Entry
        {
            Placeholder = "Revision Number",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryRevisionNumber, 3, rowNo);
        entryRevisionNumber.SetBinding(Entry.TextProperty, nameof(vm.RevisionNumber));

        rowNo++;
        var labelMaximumCharactersPerRow = new Label
        {
            Text = "Maximum Characters Per Row",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelMaximumCharactersPerRow, 2, rowNo);

        var entryMaximumCharactersPerRow = new Entry
        {
            Placeholder = "Maximum Characters Per Row",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryMaximumCharactersPerRow, 3, rowNo);
        entryMaximumCharactersPerRow.SetBinding(Entry.TextProperty, nameof(vm.MaximumCharactersPerRow));

        rowNo++;
        var labelMaximumRows = new Label
        {
            Text = "Maximum Rows",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelMaximumRows, 2, rowNo);

        var entryMaximumRows = new Entry
        {
            Placeholder = "Maximum Rows",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryMaximumRows, 3, rowNo);
        entryMaximumRows.SetBinding(Entry.TextProperty, nameof(vm.MaximumRows));

        rowNo++;
        var labelDiscSequenceNumber = new Label
        {
            Text = "Disc Sequence Number",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelDiscSequenceNumber, 2, rowNo);

        var entryDiscSequenceNumber = new Entry
        {
            Placeholder = "Disc Sequence Number",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryDiscSequenceNumber, 3, rowNo);
        entryDiscSequenceNumber.SetBinding(Entry.TextProperty, nameof(vm.DiscSequenceNumber));

        rowNo++;
        var labelTotalNumberOfDiscs = new Label
        {
            Text = "Total Number Of Discs",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(30, 0, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelTotalNumberOfDiscs, 2, rowNo);

        var entryTotalNumberOfDiscs = new Entry
        {
            Placeholder = "Total Number Of Discs",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryTotalNumberOfDiscs, 3, rowNo);
        entryTotalNumberOfDiscs.SetBinding(Entry.TextProperty, nameof(vm.TotalNumberOfDiscs));


        // import, column 3

        var buttonImport = new Button
        {
            Text = "Import",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(25, 0, 0, 0),
            Command = vm.ImportCommand,
        }.BindDynamicTheme();
        grid.Add(buttonImport, 5);


        var scrollView = new ScrollView
        {
            Content = grid,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

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
            Content = scrollView,
        }.BindDynamicTheme();

        return windowBorder;
    }

    private static Border MakeTextAndTimingView(ExportEbuModel vm)
    {
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
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelJustification = new Label
        {
            Text = "Justification",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelJustification, 0);

        var pickerJustification = new Picker
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        pickerJustification.SetBinding(Picker.ItemsSourceProperty, nameof(vm.JustificationCodes));
        pickerJustification.SetBinding(Picker.SelectedItemProperty, nameof(vm.SelectedJustificationCode));
        grid.Add(pickerJustification, 1);

        var labelVerticalPosition = new Label
        {
            Text = "Vertical Position",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(0, 50, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelVerticalPosition, 0, 1);
        grid.SetColumnSpan(labelVerticalPosition, 2);

        var labelMarginTop = new Label
        {
            Text = "Margin Top (for top aligned subtitles)",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelMarginTop, 0, 2);

        var entryMarginTop = new Entry
        {
            Placeholder = "Margin Top",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(entryMarginTop, 1, 2);

        var labelTeletext = new Label
        {
            Text = "Teletext",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(0, 50, 0, 0),
        }.BindDynamicTheme();
        grid.Add(labelTeletext, 0, 3);

        var labelUseBox = new Label
        {
            Text = "Use Box",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelUseBox, 0, 4);

        var switchUseBox = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(switchUseBox, 1, 4);

        var labelDoubleHeight = new Label
        {
            Text = "Double Height",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelDoubleHeight, 0, 5);

        var switchDoubleHeight = new Switch
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(switchDoubleHeight, 1, 5);


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

        return windowBorder;
    }

    private static Border MakeErrorsView(ExportEbuModel vm)
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 5,
            ColumnSpacing = 5,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

        var labelErrors = new Label
        {
            Text = "Errors",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelErrors, 0);

        var editorErrors = new Editor
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            IsReadOnly = true,
            FontFamily = "RobotoMono",
        }.BindDynamicTheme();
        grid.Add(editorErrors, 0, 1);

        var scrollView = new ScrollView
        {
            Content = grid,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        }.BindDynamicTheme();

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
            Content = scrollView,
        }.BindDynamicTheme();

        return windowBorder;
    }
}
