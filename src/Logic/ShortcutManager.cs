using SharpHook;
using SharpHook.Native;

namespace SubtitleAlchemist.Logic;

public class ShortcutManager : IShortcutManager
{
    private readonly HashSet<KeyCode> _activeKeys;
    private readonly List<ShortCut> _shortcuts;

    public ShortcutManager()
    {
        _activeKeys = new HashSet<KeyCode>();
        _shortcuts = new List<ShortCut>();
    }

    public void OnKeyPressed(object sender, KeyboardHookEventArgs e)
    {
        _activeKeys.Add(e.Data.KeyCode);
    }

    public void OnKeyReleased(object sender, KeyboardHookEventArgs e)
    {
        _activeKeys.Remove(e.Data.KeyCode);
    }

    public void RegisterShortcut(List<KeyCode> keys, object? control, Action action)
    {
        _shortcuts.Add(new ShortCut(keys, control, action));
    }

    public Action? CheckShortcuts(object? control)
    {
        var input = new ShortCut(_activeKeys.ToList(), control, () => { });

        foreach (var shortcut in _shortcuts)
        {
            if (input.HashCode == shortcut.HashCode)
            {
                return shortcut.Action;
            }
        }

        return null;
    }
}