﻿using Nikse.SubtitleEdit.Core.SubtitleFormats;
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
        foreach (var recentFile in Se.Settings.File.RecentFiles)
        {
            if (string.IsNullOrEmpty(recentFile.SubtitleFileName))
            {
                continue;
            }

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
        menu.Add(vm.MenuFlyoutItemReopen);
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