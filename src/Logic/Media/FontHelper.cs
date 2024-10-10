using System.Drawing.Text;

namespace SubtitleAlchemist.Logic.Media;

public static class FontHelper
{
    public static List<string> GetSystemFonts()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        var installedFontCollection = new InstalledFontCollection();
#pragma warning restore CA1416 // Validate platform compatibility

        // Get the array of FontFamily objects.
#pragma warning disable CA1416 // Validate platform compatibility
        var fontFamilies = installedFontCollection.Families;
#pragma warning restore CA1416 // Validate platform compatibility

        // The loop below creates a large string that is a comma-separated
        // list of all font family names.

        var count = fontFamilies.Length;
        var result = new List<string>(count);
        for (var j = 0; j < count; ++j)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            result.Add(fontFamilies[j].Name);
#pragma warning restore CA1416 // Validate platform compatibility
        }

        return result;
    }
}
