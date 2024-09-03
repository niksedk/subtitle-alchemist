using System.Runtime.InteropServices;

namespace SubtitleAlchemist; // Do not change this line as this namespace is required for platform code

/// <summary>
/// Windows 7+ taskbar list - http://msdn.microsoft.com/en-us/library/windows/desktop/dd391692%28v=vs.85%29.aspx
/// </summary>
public partial class TaskbarList
{
    private static readonly Lazy<bool> SupportedLazy = new Lazy<bool>(() => Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 1));
    private static readonly Lazy<ITaskbarList3> TaskbarListLazy = new Lazy<ITaskbarList3>(() => (ITaskbarList3)new CLSID_TaskbarList());

    public static bool Supported => SupportedLazy.Value;
    internal static ITaskbarList3 Taskbar => TaskbarListLazy.Value;

    public partial void SetProgressState(IntPtr hwnd, bool on)
    {
        if (Supported && hwnd != IntPtr.Zero)
        {
            var state = on ? TaskbarButtonProgressFlags.Normal : TaskbarButtonProgressFlags.NoProgress;
            Taskbar.SetProgressState(hwnd, state);
        }
    }

    public partial void SetProgressValue(IntPtr hwnd, double value, double max)
    {
        if (Supported && hwnd != IntPtr.Zero)
        {
            Taskbar.SetProgressValue(hwnd, (ulong)value, (ulong)max);
        }
    }

    private static int _timerBlinkCount;

    private System.Timers.Timer _timerBlink = new();
    public partial void StartBlink(IntPtr hwnd, int maxBlinkCount)
    {
        _timerBlinkCount = 0;
        _timerBlink?.Dispose();
        _timerBlink = new System.Timers.Timer { Interval = 500 };
        _timerBlink.Elapsed += (o, args) =>
        {
            _timerBlink.Stop();
            SetProgressValue(hwnd, _timerBlinkCount % 2 == 0 ? 0 : 100, 100);
            if (_timerBlinkCount > maxBlinkCount * 2)
            {
                SetProgressValue(hwnd, 0, 100);
                return;
            }

            _timerBlinkCount++;
            _timerBlink.Start();
        };
        _timerBlink.Start();
    }

    [ClassInterface(ClassInterfaceType.None), ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
    private class CLSID_TaskbarList
    {
    }
}

/// <summary>Extends ITaskbarList2 by exposing methods that support the unified launching and switching
/// taskbar button functionality added in Windows 7. This functionality includes thumbnail representations
/// and switch targets based on individual tabs in a tabbed application, thumbnail toolbars, notification
/// and status overlays, and progress indicators.</summary>
[ComImport, Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ITaskbarList3
{
    /// <summary>Displays or updates a progress bar hosted in a taskbar button to show
    /// the specific percentage completed of the full operation.</summary>
    /// <param name="hWnd">The handle of the window whose associated taskbar button is being used as
    /// a progress indicator.</param>
    /// <param name="ullCompleted">An application-defined value that indicates the proportion of the
    /// operation that has been completed at the time the method is called.</param>
    /// <param name="ullTotal">An application-defined value that specifies the value ullCompleted will
    /// have when the operation is complete.</param>
    void SetProgressValue(IntPtr hWnd, ulong ullCompleted, ulong ullTotal);

    /// <summary>Sets the type and state of the progress indicator displayed on a taskbar button.</summary>
    /// <param name="hWnd">The handle of the window in which the progress of an operation is being
    /// shown. This window’s associated taskbar button will display the progress bar.</param>
    /// <param name="tbpFlags">Flags that control the current state of the progress button. Specify
    /// only one of the following flags; all states are mutually exclusive of all others.</param>
    void SetProgressState(IntPtr hWnd, TaskbarButtonProgressFlags tbpFlags);
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
public enum TaskbarButtonProgressFlags
{
    NoProgress = 0,
    Indeterminate = 0x1,
    Normal = 0x2,
    Error = 0x4,
    Paused = 0x8
}
