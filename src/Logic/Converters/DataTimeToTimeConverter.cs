using System.Globalization;

namespace SubtitleAlchemist.Logic.Converters;
public class DataTimeToTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string timeString)
        {
            var arr = timeString.Split(':');
            if (arr.Length == 3)
            {
                if (int.TryParse(arr[0], out var hours) &&
                    int.TryParse(arr[1], out var minutes) &&
                    int.TryParse(arr[2], out var seconds))
                {
                    var now = DateTime.Now;
                    return new DateTime( now.Year, now.Month, now.Day, hours, minutes, seconds);
                }
            }
        }

        return DateTime.Now;
    }
}
