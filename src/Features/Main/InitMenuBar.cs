using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Main;

internal static class InitMenuBar
{
    internal static void CreateMenuBar(MainPage page, MainViewModel vm)
    {
        page.MenuBarItems.Clear();
        page.MenuBarItems.Add(MakeFileMenu(vm));
        page.MenuBarItems.Add(MakeEditMenu(vm));
        page.MenuBarItems.Add(MakeToolsMenu(vm));
        page.MenuBarItems.Add(MakeSpellCheckMenu(vm));
        page.MenuBarItems.Add(MakeVideoMenu(vm));
        page.MenuBarItems.Add(MakeSynchronizationMenu(vm));
        page.MenuBarItems.Add(MakeOptionsMenu(vm));
        page.MenuBarItems.Add(MakeTranslateMenu(vm));
        page.MenuBarItems.Add(MakeHelpMenu(vm));
    }

    private static MenuBarItem MakeFileMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "File" };

        vm.MenuFlyoutItemReopen = new MenuFlyoutSubItem
        {
            Text = "Reopen",
        };

        var recentFilesCount = 0;
        foreach (var recentFile in Se.Settings.File.RecentFiles)
        {
            if (string.IsNullOrEmpty(recentFile.SubtitleFileName))
            {
                continue;
            }

            recentFilesCount++;
            var reopenItem = new MenuFlyoutItem
            {
                Text = recentFile.SubtitleFileName,
                Command = new Command(() => vm.ReopenSubtitle(recentFile.SubtitleFileName))
            };
            vm.MenuFlyoutItemReopen.Add(reopenItem);
        }
        

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

        if (recentFilesCount > 0)
        {
            menu.Add(vm.MenuFlyoutItemReopen);

            vm.MenuFlyoutItemReopen.Add(new MenuFlyoutSeparator());
            vm.MenuFlyoutItemReopen.Add(new MenuFlyoutItem
            {
                Text = "Clear",
                Command = vm.RecentFilesClearCommand,
            });
        }

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
            Command = vm.ShowRestoreAutoBackupCommand,
        });
        menu.Add(new MenuFlyoutSeparator());

        var exportMenuItem = new MenuFlyoutSubItem
        {
            Text = "Export",
        };
        //TODO: wait for libse 4.0.9
        //exportMenuItem.Add(new MenuFlyoutItem
        //{
        //    Text = new CapMakerPlus().Name,
        //    Command = vm.ExportCapMakerPlusCommand,
        //});
        exportMenuItem.Add(new MenuFlyoutItem
        {
            Text = Cavena890.NameOfFormat,
            Command = vm.ExportCavena890Command,
        });
        exportMenuItem.Add(new MenuFlyoutItem
        {
            Text = Pac.NameOfFormat,
            Command = vm.ExportPacCommand,
        });
        exportMenuItem.Add(new MenuFlyoutItem
        {
            Text = new PacUnicode().Name,
            Command = vm.ExportPacUnicodeCommand,
        });
        exportMenuItem.Add(new MenuFlyoutItem
        {
            Text = Ebu.NameOfFormat,
            Command = vm.ExportEbuStlCommand,
        });

        menu.Add(exportMenuItem);
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
            Command = vm.UndoCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Redo",
            Command = vm.RedoCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Show history",
            Command = vm.HistoryShowCommand,
        });
        menu.Add(new MenuFlyoutSeparator());

        var find = new MenuFlyoutItem
        {
            Text = "Find",
            Command = vm.FindShowCommand,
        };
        find.KeyboardAccelerators.Add(new KeyboardAccelerator
        {
            Modifiers = KeyboardAcceleratorModifiers.Ctrl,
            Key = "F"
        });
        menu.Add(find);

        var findNext = new MenuFlyoutItem
        {
            Text = "Find next",
        };
        findNext.KeyboardAccelerators.Add(new KeyboardAccelerator
        {
            Key = "F3"
        });
        menu.Add(findNext);

        var replace = new MenuFlyoutItem
        {
            Text = "Replace",
            Command= vm.ReplaceShowCommand,
        };
        find.KeyboardAccelerators.Add(new KeyboardAccelerator
        {
            Modifiers = KeyboardAcceleratorModifiers.Ctrl,
            Key = "H"
        });
        menu.Add(replace);

        menu.Add(new MenuFlyoutItem
        {
            Text = "Multiple replace",
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Go to subtitle number...",
            Command = vm.ShowGoToLineNumberCommand,
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
            Text = "Adjust durations...",
            Command = vm.AdjustDurationsShowCommand,
        });

        menu.Add(new MenuFlyoutItem
        {
            Text = "Fix common errors...",
            Command = vm.FixCommonErrorsShowCommand,
        });

        return menu;
    }

    private static MenuBarItem MakeSpellCheckMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Spell check" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Spell check...",
            Command = vm.SpellCheckShowCommand,
        });
        menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "Get dictionaries...",
            Command = vm.SpellCheckGetDictionariesShowCommand,
        });

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
            Command = vm.OpenVideoFromUrlShowCommand,
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
            Text = "Generate video with burned-in subtitles",
            Command = vm.VideoBurnInCommand,
        });
        menu.Add(new MenuFlyoutItem
        {
            Text = "Text to speech and add to video...",
            Command = vm.VideoSpeechToTextCommand,
        });

        return menu;
    }

    private static MenuBarItem MakeSynchronizationMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Synchronization" };

        menu.Add(new MenuFlyoutItem
        {
            Text = "Adjust all times (show earlier/later)",
            Command = vm.AdjustAllTimesShowCommand,
        });

        menu.Add(new MenuFlyoutItem
        {
            Text = "Change frame rate",
            Command = vm.ChangeFrameRateShowCommand,
        });

        menu.Add(new MenuFlyoutItem
        {
            Text = "Change speed (percent)",
            Command = vm.ChangeSpeedShowCommand,
        });

        return menu;
    }

    private static MenuBarItem MakeTranslateMenu(MainViewModel vm)
    {
        var menu = new MenuBarItem { Text = "Translate" };

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

        //menu.Add(new MenuFlyoutItem
        //{
        //    Text = "Check for updates...",
        //});
        //menu.Add(new MenuFlyoutSeparator());
        //menu.Add(new MenuFlyoutItem
        //{
        //    Text = "Help",
        //});
        //menu.Add(new MenuFlyoutSeparator());
        menu.Add(new MenuFlyoutItem
        {
            Text = "About...",
            Command = vm.ShowAboutCommand,
        });

        return menu;
    }
}