using System.Drawing.Text;

namespace SubtitleAlchemist.Logic.Media;

public static class FontHelper
{
    public static List<string> GetSystemFonts()
    {
        var installedFontCollection = new InstalledFontCollection();

        // Get the array of FontFamily objects.
        var fontFamilies = installedFontCollection.Families;

        // The loop below creates a large string that is a comma-separated
        // list of all font family names.

        var count = fontFamilies.Length;
        var result = new List<string>(count);
        for (var j = 0; j < count; ++j)
        {
            result.Add(fontFamilies[j].Name);
        }

        return result;
    }
}


