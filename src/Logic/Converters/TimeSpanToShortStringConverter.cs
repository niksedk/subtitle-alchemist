using System.Globalization;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Converters;

public class TimeSpanToShortStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return new TimeCode(timeSpan).ToShortDisplayString();
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string timeString)
        {
            return new TimeCode(TimeCode.ParseHHMMSSToMilliseconds(timeString)).TimeSpan;
        }

        return TimeSpan.Zero;
    }
}
