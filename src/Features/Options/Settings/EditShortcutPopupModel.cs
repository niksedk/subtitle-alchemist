using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleAlchemist.Features.Options.Settings;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Shared.PickSubtitleLine;

public partial class EditShortcutPopupModel : ObservableObject
{
    public EditShortcutPopup? Popup { get; set; }

    [ObservableProperty]
    private bool _modifierCtrl;

    [ObservableProperty]
    private bool _modifierAlt;

    [ObservableProperty]
    private bool _modifierShift;

    [ObservableProperty]
    private ObservableCollection<string> _keys;

    [ObservableProperty]
    private string _key1;

    [ObservableProperty]
    private string _key2;

    [ObservableProperty]
    private string _key3;

    [ObservableProperty]
    private string _title;

    private ShortcutDisplay _shortcut;

    public EditShortcutPopupModel()
    {
        _title = string.Empty;
        _shortcut = new ShortcutDisplay(ShortcutArea.General, string.Empty, new ShortcutType(ShortcutAction.GeneralMergeSelectedLines, () => { }, new List<string>()));
        _keys = new ObservableCollection<string>(GetShortcutKeys());
        _key1 = string.Empty;
        _key2 = string.Empty;
        _key3 = string.Empty;
    }

    private List<string> GetShortcutKeys()
    {
        var result = new List<string>();

        var keyCodeType = typeof(SharpHook.Native.KeyCode);
        var keyNames = Enum.GetNames(keyCodeType).Where(p => !IsObsolete(keyCodeType, p)).Select(p => p.Remove(0, 2)).ToList();

        foreach (var keyName in keyNames)
        {
            if (keyName != "Undefined")
            {
                result.Add(keyName);
            }
        }

        var orderedResult = result.OrderBy(p => p).ToList();
        orderedResult.Insert(0, "- None -");

        return orderedResult;
    }

    public static bool IsObsolete(Type type, string name)
    {
        var fi = type.GetField(name);
        var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        return attributes != null && attributes.Length > 0;
    }

    [RelayCommand]
    private void Ok()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Popup?.Close(_shortcut);
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
                ModifierCtrl = keys.Contains(SharpHook.Native.KeyCode.VcLeftControl.ToString()) &&
                               keys.Contains(SharpHook.Native.KeyCode.VcRightControl.ToString());
                ModifierAlt = keys.Contains(SharpHook.Native.KeyCode.VcLeftAlt.ToString()) &&
                              keys.Contains(SharpHook.Native.KeyCode.VcRightAlt.ToString());
                ModifierShift = keys.Contains(SharpHook.Native.KeyCode.VcLeftShift.ToString()) &&
                                keys.Contains(SharpHook.Native.KeyCode.VcRightShift.ToString());

                Key1 = keys.First();
                Key2 = keys.First();
                Key3 = keys.First();

                foreach (var key in keys)
                {
                    if (ModifierCtrl && key.Contains("Control"))
                    {
                        continue;
                    }

                    if (ModifierAlt && key.Contains("Alt"))
                    {
                        continue;
                    }

                    if (ModifierShift && key.Contains("Shift"))
                    {
                        continue;
                    }

                    if (Key1 == keys.First())
                    {
                        Key1 = key;
                    }
                    else if (Key2 == keys.First())
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