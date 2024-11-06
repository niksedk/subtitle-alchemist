using CommunityToolkit.Maui.Markup;
using SubtitleAlchemist.Features.Options.Settings;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Main;

public class MainShortcutActions : IMainShortcutActions
{
    private MainViewModel _vm;
    private MainPage _page;

    public void Initialize(IShortcutManager shortcutManager, MainViewModel viewModel, MainPage mainPage)
    {
        _vm = viewModel;
        _page = mainPage;

        shortcutManager.ClearShortcuts();
        foreach (var shortcut in Se.Settings.Shortcuts)
        {
            var action = GetShortcutAction(shortcut.ActionName);
            if (action != null)
            {
                shortcutManager.RegisterShortcut(shortcut, action);
            }
        }
    }

    private Action? GetShortcutAction(ShortcutAction shortcutActionName)
    {
        switch (shortcutActionName)
        {
            case ShortcutAction.GeneralChooseLayout: return ChooseLayout;
            case ShortcutAction.GeneralGoToLineNumber: return GoToLineNumber;
            case ShortcutAction.GeneralGoToPrevSubtitle: return SubtitleListUp;
            case ShortcutAction.GeneralGoToNextSubtitle: return SubtitleListDown;
            case ShortcutAction.ListSelectAll: return SubtitleListSelectAll;
            case ShortcutAction.ListSelectFirst: return SubtitleListSelectFirst;
            case ShortcutAction.ListSelectLast: return SubtitleListSelectLast;
            case ShortcutAction.GeneralMergeSelectedLines: return MergeSelectedLines;
        }

        return null;
    }

    private void MergeSelectedLines()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _vm.MergeSelectedLinesCommand.Execute(_vm);
        });
    }

    private void GoToLineNumber()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _vm.ShowGoToLineNumber();
        });
    }

    private void SubtitleListUp()
    {
        var idx = _vm.GetFirstSelectedIndex();
        if (idx <= 0)
        {
            return;
        }

        _vm.SelectParagraph(idx - 1);
    }

    private void ChooseLayout()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _vm.ShowLayoutPicker();
        });
    }

    private void SubtitleListDown()
    {
        var idx = _vm.GetFirstSelectedIndex();
        if (idx < 0 || idx >= _vm.Paragraphs.Count - 1)
        {
            return;
        }

        _vm.SelectParagraph(idx + 1);
    }

    private void SubtitleListSelectAll()
    {
        foreach (var displayParagraph in _vm.Paragraphs)
        {
            displayParagraph.IsSelected = true;
        }
    }

    private void SubtitleListSelectFirst()
    {
        _vm.SelectParagraph(_vm.Paragraphs.FirstOrDefault());
    }

    private void SubtitleListSelectLast()
    {
        _vm.SelectParagraph(_vm.Paragraphs.LastOrDefault());
    }
}