using SharpHook;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Logic;

public interface IShortcutManager
{
    void OnKeyPressed(object? sender, KeyboardHookEventArgs e);
    void OnKeyReleased(object? sender, KeyboardHookEventArgs e);
    void RegisterShortcut(SeShortCut shortcut, Action action);
    void RegisterShortcut(SeShortCut shortcut, Action action, string control);
    Action? CheckShortcuts(string? control);
    void ClearShortcuts();
    bool IsControlDown { get; }
    bool IsAltDown { get; }
    bool IsShiftDown { get; }
}