namespace SubtitleAlchemist.Features.Main;

internal static class InitMenuBar
{
    internal static void CreateMenuBar(MainPage page, MainViewModel viewModel)
    {
        page.MenuBarItems.Add(MakeFileMenu(viewModel));
        page.MenuBarItems.Add(MakeEditMenu(viewModel));
        page.MenuBarItems.Add(MakeToolsMenu(viewModel));
        page.MenuBarItems.Add(MakeSpellCheckMenu(viewModel));
        page.MenuBarItems.Add(MakeVideoMenu(viewModel));
        page.MenuBarItems.Add(MakeSynchronizationMenu(viewModel));
        page.MenuBarItems.Add(MakeOptionsMenu(viewModel));
        page.MenuBarItems.Add(MakeTranslateMenu(viewModel));
        page.MenuBarItems.Add(MakeHelpMenu(viewModel));
    }

    private static MenuBarItem MakeFileMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "File" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "New",
            Command = vm.SubtitleNewCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Open",
            Command = vm.SubtitleOpenCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Reopen",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Save",
            Command = vm.SubtitleSaveCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Save as...",
            Command = vm.SubtitleSaveAsCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Restore auto-backup...",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Export",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Exit",
            Command = new Command(() => Application.Current?.Quit())
        });

        return menu;
    }

    private static MenuBarItem MakeEditMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Edit" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Undo",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Redo",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Show history",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Find",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Find next",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Replace",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Multiple replace",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Go to subtitle number...",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Invert selection",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Select all",
        });

        return menu;
    }

    private static MenuBarItem MakeToolsMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Tools" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Adjust durations",
            Command = vm.AdjustDurationsShowCommand,
        });

        return menu;
    }

    private static MenuBarItem MakeSpellCheckMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Spell check" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Spell check...",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Get dictionaries",
        });
        menu.Add(new MenuFlyoutSeparator());

        return menu;
    }

    private static MenuBarItem MakeVideoMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Video" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Open video file...",
            Command = vm.VideoOpenCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Open video from URL...",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Close video file",
            Command = vm.VideoCloseCommand,
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Audio to text (Whisper)...",
            Command = vm.VideoAudioToTextWhisperCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Text to speech and add to video...",
        });

        return menu;
    }

    private static MenuBarItem MakeSynchronizationMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Synchronization" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Adjust all times (show earlier/later)",
        });

        return menu;
    }

    private static MenuBarItem MakeTranslateMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Auto-translate" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Auto-translate",
            Command = vm.AutoTranslateShowCommand,
        });

        return menu;
    }

    private static MenuBarItem MakeOptionsMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Options" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Settings",
            Command = vm.ShowOptionsSettingsCommand,
        });

        return menu;
    }

    private static MenuBarItem MakeHelpMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Help" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Check for updates...",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Help",
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "About...",
            Command = vm.ShowAboutCommand,
        });

        return menu;
    }
}