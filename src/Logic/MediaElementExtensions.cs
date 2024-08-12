using CommunityToolkit.Maui.Views;
using System.Reflection;

namespace SubtitleAlchemist.Logic;

public static class MediaElementExtensions
{
    public static void SetTimerInterval(this MediaElement mediaElement, int milliseconds)
    {
        var mediaElementType = typeof(MediaElement);
        var timerField = mediaElementType.GetField("timer", BindingFlags.NonPublic | BindingFlags.Instance);

        if (timerField == null)
        {
            return;
        }

        var dispatcherTimerInstance = timerField.GetValue(mediaElement);
        if (dispatcherTimerInstance == null)
        {
            return;
        }

        var dispatcherTimerType = dispatcherTimerInstance.GetType();
        var intervalProperty = dispatcherTimerType.GetProperty("Interval");
        if (intervalProperty != null && intervalProperty.CanWrite)
        {
            intervalProperty.SetValue(dispatcherTimerInstance, TimeSpan.FromMilliseconds(milliseconds));
        }
    }
}