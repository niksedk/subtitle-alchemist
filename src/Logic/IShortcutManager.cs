using SharpHook;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Logic;

public interface IShortcutManager
{
    void OnKeyPressed(object? sender, KeyboardHookEventArgs e);
    void OnKeyReleased(object? sender, KeyboardHookEventArgs e);
    void RegisterShortcut(SeShortCut shortcut, Action action);
    Action? CheckShortcuts(object? control);
}