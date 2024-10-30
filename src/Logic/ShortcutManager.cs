﻿using SharpHook;
using SharpHook.Native;
using SubtitleAlchemist.Logic.Config;

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

    public void RegisterShortcut(SeShortCut shortcut, Action action)
    {
        _shortcuts.Add(new ShortCut(shortcut.Keys, null, action));
    }

    public Action? CheckShortcuts(object? control)
    {
        var input = new ShortCut(_activeKeys.Select(p => p.ToString().Remove(0, 2)).ToList(), control, () => { });
        var inputWithNormalizedModifiers = NormalizeModifiers(input);

        if (_activeKeys.Count < 2)
        {
            return null; //TODO: remove
        }

        foreach (var shortcut in _shortcuts)
        {
            if (input.HashCode == shortcut.HashCode || inputWithNormalizedModifiers.HashCode == shortcut.HashCode)
            {
                return shortcut.Action;
            }
        }

        return null;
    }

    private static ShortCut NormalizeModifiers(ShortCut input)
    {
        var keys = new List<string>();
        foreach (var key in input.Keys)
        {
            if (key is "LeftControl" or "RightControl")
            {
                keys.Add("Control");
            }
            else if (key is "LeftShift" or "RightShift")
            {
                keys.Add("Shift");
            }
            else if (key is "LeftAlt" or "RightAlt")
            {
                keys.Add("Alt");
            }
            else
            {
                keys.Add(key);
            }
        }

        return new ShortCut(keys, input.Control, input.Action);
    }

}
