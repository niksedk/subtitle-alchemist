using System.Globalization;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Converters;

public class TimeCodeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeCode timeCode)
        {
            return timeCode.ToDisplayString();
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string timeString)
        {
            return new TimeCode(TimeCode.ParseHHMMSSToMilliseconds(timeString));
        }

        return TimeSpan.Zero;
    }
}