using SharpHook;
using SharpHook.Native;

namespace SubtitleAlchemist.Logic;

public interface IShortcutManager
{
    void OnKeyPressed(object? sender, KeyboardHookEventArgs e);
    void OnKeyReleased(object? sender, KeyboardHookEventArgs e);
    void RegisterShortcut(List<KeyCode> keys, object control, Action action);
    Action? CheckShortcuts(object? control);
}