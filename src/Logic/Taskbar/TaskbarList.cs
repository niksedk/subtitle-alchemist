namespace SubtitleAlchemist; // Do not change this line as this namespace is required for platform code

public partial class TaskbarList
{
    public partial void SetProgressState(IntPtr hwnd, bool on);

    public partial void SetProgressValue(IntPtr hwnd, double value, double max);

    public partial void StartBlink(IntPtr hwnd, int maxBlinkCount);
}
