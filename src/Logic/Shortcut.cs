using SharpHook.Native;

namespace SubtitleAlchemist.Logic;

public class ShortCut
{
    public List<KeyCode> Keys { get; set; }
    public object? Control { get; set; }
    public Action Action { get; set; }
    public int HashCode { get; set; }

    public ShortCut(List<KeyCode> keys, object? control, Action action)
    {
        Keys = keys;
        Control = control;
        Action = action;
        HashCode = keys.Aggregate(0, (hash, keyCode) => hash ^ keyCode.GetHashCode());
        if (control != null)
        {
            HashCode ^= control.GetHashCode();
        }
    }
}