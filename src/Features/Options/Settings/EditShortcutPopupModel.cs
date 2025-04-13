using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;

public partial class EditShortcutPopupModel : ObservableObject
{
    public EditShortcutPopup? Popup { get; set; }

    [ObservableProperty]
    public partial bool ModifierCtrl { get; set; }

    [ObservableProperty]
    public partial bool ModifierAlt { get; set; }

    [ObservableProperty]
    public partial bool ModifierShift { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> Keys { get; set; }

    [ObservableProperty]
    public partial string Key1 { get; set; }

    [ObservableProperty]
    public partial string Key2 { get; set; }

    [ObservableProperty]
    public partial string Key3 { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    private readonly ShortcutDisplay _shortcut;
    private readonly string _none; 
    private const string KeyUndefined = "Undefined"; 
    private const string KeyControl = "Control"; 
    private const string KeyShift = "Shift"; 
    private const string KeyAlt = "Alt"; 

    public EditShortcutPopupModel()
    {
        Title = string.Empty;
        _none = $"- {Se.Language.General.None} -";
        _shortcut = new ShortcutDisplay(ShortcutArea.General, string.Empty, ShortcutAction.GeneralMergeSelectedLines);
        Keys = new ObservableCollection<string>(GetShortcutKeys());
        Key1 = string.Empty;
        Key2 = string.Empty;
        Key3 = string.Empty;
    }

    private List<string> GetShortcutKeys()
    {
        var result = new List<string>();

        var keyCodeType = typeof(SharpHook.Native.KeyCode);
        var keyNames = Enum.GetNames(keyCodeType).Where(p => !IsObsolete(keyCodeType, p)).Select(p => p.Remove(0, 2)).ToList();

        foreach (var keyName in keyNames)
        {
            if (keyName != KeyUndefined && keyName.Trim().Length > 0)
            {
                result.Add(keyName);
            }
        }

        var orderedResult = result.OrderBy(p => p).ToList();
        orderedResult.Insert(0, _none);

        return orderedResult;
    }

    public static bool IsObsolete(Type type, string name)
    {
        var fi = type.GetField(name);
        if (fi == null)
        {
            return false;
        }

        var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return attributes is { Length: > 0 };
    }

    [RelayCommand]
    private void Ok()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var keys = new List<string>();
            if (ModifierCtrl)
            {
                keys.Add(KeyControl);
            }

            if (ModifierAlt)
            {
                keys.Add(KeyAlt);
            }

            if (ModifierShift)
            {
                keys.Add(KeyShift);
            }

            if (!string.IsNullOrEmpty(Key1) && Key1 != _none)
            {
                keys.Add(Key1);
            }

            if (!string.IsNullOrEmpty(Key2) && Key2 != _none)
            {
                keys.Add(Key2);
            }

            if (!string.IsNullOrEmpty(Key3) && Key3 != _none)
            {
                keys.Add(Key3);
            }

            var shortCut = new ShortcutDisplay(_shortcut.Area, _shortcut.Name, new ShortcutType(_shortcut.Type.ActionName, keys));

            Popup?.Close(shortCut);
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close();
        });
    }

    public void Initialize(string title, ShortcutDisplay shortcut)
    {
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var keys = shortcut.Type.Keys ?? new List<string>();
                ModifierCtrl = keys.Contains(KeyControl);
                ModifierAlt = keys.Contains(KeyAlt);
                ModifierShift = keys.Contains(KeyShift);

                Key1 = Keys.First();
                Key2 = Keys.First();
                Key3 = Keys.First();

                foreach (var key in keys)
                {
                    if (ModifierCtrl && key.Contains(KeyControl))
                    {
                        continue;
                    }

                    if (ModifierAlt && key.Contains(KeyAlt))
                    {
                        continue;
                    }

                    if (ModifierShift && key.Contains(KeyShift))
                    {
                        continue;
                    }

                    if (Key1 == _none)
                    {
                        Key1 = key;
                    }
                    else if (Key2 == _none)
                    {
                        Key2 = key;
                    }
                    else if (Key3 == keys.First())
                    {
                        Key3 = key;
                    }
                }

                Title = title;
            });

            return false;
        });
    }
}