using System.Globalization;
using Nikse.SubtitleEdit.Core.AutoTranslate;

namespace SubtitleAlchemist.Logic.Converters;

public class AutoTranslatorToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IAutoTranslator autoTranslator)
        {
            return autoTranslator.Name;
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
