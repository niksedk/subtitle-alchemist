using CommunityToolkit.Maui.Markup;
using SubtitleAlchemist.Controls.SubTimeControl;
using SubtitleAlchemist.Logic;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace SubtitleAlchemist.Features.Main;

internal static class InitLayout
{
    private enum Row { Toolbar, ListViewAndVideo, WaveForm, StatusBar }
    private enum RowLayout4 { Toolbar, Video, ListView, WaveForm, StatusBar }
    private enum RowLayout5 { Toolbar, ListView, WaveForm, StatusBar }
    private enum RowLayout6 { Toolbar, ListViewAndVideo, StatusBar }
    private enum RowLayout7 { Toolbar, ListView, Video, StatusBar }
    private enum RowLayout8 { Toolbar, Video, ListView, StatusBar }
    private enum RowLayout9 { Toolbar, VideoAndWaveformAndText, ListView, StatusBar }
    private enum RowLayout10 { Toolbar, Video, WaveForm, ListView, StatusBar }
    private enum RowLayout11 { Toolbar, ListView, StatusBar }
    private enum Column { Left, Right }

    public static void MakeLayout(MainPage mainPage, MainPageModel pageModel, int layoutNumber)
    {
        mainPage.BatchBegin();

        (mainPage.Content as Grid)?.Children.Clear();
        pageModel.ListViewAndEditBox?.Children.Clear();
        mainPage.Content = null;

        if (layoutNumber == 1) // like default, but video left
        {
            MakeLayout1(mainPage, pageModel);
        }
        else if (layoutNumber == 2) // mobile, video right
        {
            MakeLayout2(mainPage, pageModel);
        }
        else if (layoutNumber == 3) // mobile, video left
        {
            MakeLayout3(mainPage, pageModel);
        }
        else if (layoutNumber == 4) // stacked vertically: video list-view waveform
        {
            MakeLayout4(mainPage, pageModel);
        }
        else if (layoutNumber == 5) // stacked vertically: list-view waveform
        {
            MakeLayout5(mainPage, pageModel);
        }
        else if (layoutNumber == 6) // mobile, video right (no waveform)
        {
            MakeLayout6(mainPage, pageModel);
        }
        else if (layoutNumber == 7) // stacked vertically: list-view video
        {
            MakeLayout7(mainPage, pageModel);
        }
        else if (layoutNumber == 8) // stacked vertically: video, list-view 
        {
            MakeLayout8(mainPage, pageModel);
        }
        else if (layoutNumber == 9) // video + waveform-and-text, list-view 
        {
            MakeLayout9(mainPage, pageModel);
        }
        else if (layoutNumber == 10) // stacked vertically: video waveform list-view 
        {
            MakeLayout10(mainPage, pageModel);
        }
        else if (layoutNumber == 11) // stacked vertically: video waveform list-view 
        {
            MakeLayout11(mainPage, pageModel);
        }
        else
        {
            MakeLayout0(mainPage, pageModel);
        }

        mainPage.BatchCommit();
    }

    private static void MakeLayout0(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Toolbar, GetToolbarHeight()),
                (Row.ListViewAndVideo, Stars(2)),
                (Row.WaveForm, Stars(1)),
                (Row.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Stars(2)),
                (Column.Right, Stars(2))
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)Row.ListViewAndVideo);

        grid.Add(pageModel.VideoPlayer, (int)Column.Right, (int)Row.ListViewAndVideo);

        grid.Add(pageModel.AudioVisualizer, (int)Column.Left, (int)Row.WaveForm);
        grid.SetColumnSpan(pageModel.AudioVisualizer, 2);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)Row.StatusBar);
        grid.SetColumnSpan(statusBar, 2);

        mainPage.Content = grid;
    }

    private static int GetToolbarHeight()
    {
#if MACCATALYST
        return 0;
#endif

        return 50;
    }

    private static void MakeLayout1(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Toolbar, GetToolbarHeight()),
                (Row.ListViewAndVideo, Stars(2)),
                (Row.WaveForm, Stars(1)),
                (Row.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Stars(2)),
                (Column.Right, Stars(2))
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        }.BindDynamicTheme();


        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)Row.ListViewAndVideo);

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Right, (int)Row.ListViewAndVideo);

        grid.Add(pageModel.AudioVisualizer, (int)Column.Left, (int)Row.WaveForm);
        grid.SetColumnSpan(pageModel.AudioVisualizer, 2);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)Row.StatusBar);
        grid.SetColumnSpan(statusBar, 2);

        mainPage.Content = grid;
    }

    private static void MakeLayout2(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Toolbar, GetToolbarHeight()),
                (Row.ListViewAndVideo, Stars(2)),
                (Row.WaveForm, Stars(1)),
                (Row.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Stars(1)),
                (Column.Right, Stars(1))
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)Row.ListViewAndVideo);

        grid.Add(pageModel.VideoPlayer, (int)Column.Right, (int)Row.ListViewAndVideo);
        grid.SetRowSpan(pageModel.VideoPlayer, 2);

        grid.Add(pageModel.AudioVisualizer, (int)Column.Left, (int)Row.WaveForm);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)Row.StatusBar);
        grid.SetColumnSpan(statusBar, 2);

        mainPage.Content = grid;
    }

    private static void MakeLayout3(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Toolbar, GetToolbarHeight()),
                (Row.ListViewAndVideo, Stars(2)),
                (Row.WaveForm, Stars(1)),
                (Row.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Stars(1)),
                (Column.Right, Stars(1))
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Right, (int)Row.ListViewAndVideo);

        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)Row.ListViewAndVideo);
        grid.SetRowSpan(pageModel.VideoPlayer, 2);



        grid.Add(pageModel.AudioVisualizer, (int)Column.Right, (int)Row.WaveForm);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)Row.StatusBar);
        grid.SetColumnSpan(statusBar, 2);

        mainPage.Content = grid;
    }

    private static void MakeLayout4(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout4.Toolbar, GetToolbarHeight()),
                (RowLayout4.Video, Stars(3)),
                (RowLayout4.ListView, Stars(3)),
                (RowLayout4.WaveForm, Stars(1)),
                (RowLayout4.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Star)
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)RowLayout4.Video);

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout4.ListView);

        grid.Add(pageModel.AudioVisualizer, (int)Column.Left, (int)RowLayout4.WaveForm);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout4.StatusBar);

        mainPage.Content = grid;
    }

    private static void MakeLayout5(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout5.Toolbar, GetToolbarHeight()),
                (RowLayout5.ListView, Stars(3)),
                (RowLayout5.WaveForm, Stars(1)),
                (RowLayout5.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Star)
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout5.ListView);

        grid.Add(pageModel.AudioVisualizer, (int)Column.Left, (int)RowLayout5.WaveForm);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout5.StatusBar);

        mainPage.Content = grid;
    }

    private static void MakeLayout6(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout6.Toolbar, GetToolbarHeight()),
                (RowLayout6.ListViewAndVideo, Star),
                (RowLayout6.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Stars(1)),
                (Column.Right, Stars(1))
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout6.ListViewAndVideo);

        grid.Add(pageModel.VideoPlayer, (int)Column.Right, (int)RowLayout6.ListViewAndVideo);
        grid.SetRowSpan(pageModel.VideoPlayer, 2);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout6.StatusBar);
        grid.SetColumnSpan(statusBar, 2);

        mainPage.Content = grid;
    }

    private static void MakeLayout7(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout7.Toolbar, GetToolbarHeight()),
                (RowLayout7.ListView, Stars(3)),
                (RowLayout7.Video, Stars(1)),
                (RowLayout7.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Star)
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout7.ListView);

        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)RowLayout7.Video);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout7.StatusBar);

        mainPage.Content = grid;
    }

    private static void MakeLayout8(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout8.Toolbar, GetToolbarHeight()),
                (RowLayout8.Video, Stars(1)),
                (RowLayout8.ListView, Stars(3)),
                (RowLayout8.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Star)
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)RowLayout8.Video);

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout8.ListView);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout8.StatusBar);

        mainPage.Content = grid;
    }

    private static void MakeLayout9(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout9.Toolbar, GetToolbarHeight()),
                (RowLayout9.VideoAndWaveformAndText, Stars(2)),
                (RowLayout9.ListView, Stars(3)),
                (RowLayout9.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Stars(1)),
                (Column.Right, Stars(1))
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)RowLayout9.VideoAndWaveformAndText);

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout9.ListView);

        grid.SetColumnSpan(pageModel.ListViewAndEditBox, 2);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout9.StatusBar);
        grid.SetColumnSpan(statusBar, 2);

        mainPage.Content = grid;
    }

    private static void MakeLayout10(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout10.Toolbar, GetToolbarHeight()),
                (RowLayout10.Video, Stars(3)),
                (RowLayout10.WaveForm, Stars(1)),
                (RowLayout10.ListView, Stars(3)),
                (RowLayout10.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Star)
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        grid.Add(pageModel.VideoPlayer, (int)Column.Left, (int)RowLayout10.Video);
        grid.Add(pageModel.AudioVisualizer, (int)Column.Left, (int)RowLayout10.WaveForm);

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout10.ListView);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout10.StatusBar);

        mainPage.Content = grid;
    }

    private static void MakeLayout11(MainPage mainPage, MainPageModel pageModel)
    {
        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (RowLayout11.Toolbar, GetToolbarHeight()),
                (RowLayout11.ListView, Star),
                (RowLayout11.StatusBar, 30)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Left, Star)
            ),

            Children =
            {
                InitToolbar.CreateToolbarBar(mainPage, pageModel).Row(Row.Toolbar).ColumnSpan(2),
            }
        };

        pageModel.ListViewAndEditBox = MakeDefaultListViewAndEditBox(pageModel);
        pageModel.ListViewAndEditBox.Add(pageModel.SubtitleListBorder, 0, 0);
        pageModel.ListViewAndEditBox.SetColumnSpan(pageModel.SubtitleListBorder, 3);
        grid.Add(pageModel.ListViewAndEditBox, (int)Column.Left, (int)RowLayout11.ListView);

        var statusBar = MakeStatusBar(pageModel);
        grid.Add(statusBar, 0, (int)RowLayout11.StatusBar);

        mainPage.Content = grid;
    }

    private static Grid MakeDefaultListViewAndEditBox(MainPageModel vm)
    {
        var startTimeUpDown = new SubTimeUpDown
        {
            DisplayText = "00:00:00,000",
            HorizontalOptions = LayoutOptions.Start,
        }
            .BindDynamicTheme()
            .Column(0)
            .Row(1);
        startTimeUpDown.BindingContext = vm;
        startTimeUpDown.ValueChanged += vm.CurrentStartChanged;
        startTimeUpDown.Bind(SubTimeUpDown.TimeProperty, nameof(vm.CurrentStart), BindingMode.TwoWay);

        var durationUpDown = new SubTimeUpDown
        {
            DisplayText = "00,000",
            UseShortFormat = true,
            HorizontalOptions = LayoutOptions.Start,
        }
            .BindDynamicTheme()
            .Column(0)
            .Row(1);
        durationUpDown.BindingContext = vm;
        durationUpDown.ValueChanged += vm.CurrentDurationChanged;
        durationUpDown.Bind(SubTimeUpDown.TimeProperty, nameof(vm.CurrentDuration), BindingMode.TwoWay);

        vm.TextBox = new Editor
        {
            Margin = new Thickness(10),
        }.BindDynamicTheme()
            .Column(1).Bind("CurrentText")
            .Row(1);
        vm.TextBox.TextChanged += vm.CurrentTextChanged;

        var leftGrid = new Grid
        {
            Margin = new Thickness(10),
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new(GridLength.Auto),
                new(GridLength.Auto),
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new() { Height = GridLength.Auto },
                new() { Height = GridLength.Auto },
            },
        };

        leftGrid.Add(new Label
        {
            Text = "Show",
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(0, 0, 5, 5),
        }.BindDynamicTheme(), 0, 0);
        leftGrid.Add(startTimeUpDown, 1, 0);
        leftGrid.Add(new Label
        {
            Text = "Duration",
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(0, 0, 5, 5),
        }.BindDynamicTheme(), 0, 1);
        leftGrid.Add(durationUpDown, 1, 1);

        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new(GridLength.Auto),
                new(GridLength.Star),
                new(100),
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new(Star),
                new(100),
            },
        };

        grid.Add(leftGrid, 0, 1);
        grid.Add(vm.TextBox, 1, 1);

        return grid;
    }

    private static StackLayout MakeStatusBar(MainPageModel vm)
    {
        vm.LabelStatusText = new Label
        {
            Padding = new Thickness(10, 2, 2, 2),
        }.BindDynamicTheme();

        return new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children =
            {
                vm.LabelStatusText
                    .Bind(Label.TextProperty, static vm => vm.StatusText,
                    static (MainPageModel vm, string text) => vm.StatusText = text),
                new Label()
                    .BindDynamicTheme()
                    .Bind(Label.TextProperty, static vm => vm.SelectedLineInfo,
                    static (MainPageModel vm, string text) => vm.SelectedLineInfo = text),
            }
        };
    }
}