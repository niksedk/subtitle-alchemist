using System.Globalization;
using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Converters;
public class TimeSpanToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return new TimeCode(timeSpan).ToDisplayString();
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
