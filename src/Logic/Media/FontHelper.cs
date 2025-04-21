using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Drawing.Text;

namespace SubtitleAlchemist.Logic.Media;

public static class FontHelper
{
    public static List<string> GetSystemFonts()
    {
        return DeviceInfo.Platform.ToString() switch
        {
            nameof(DevicePlatform.Android) => GetAndroidFonts(),
            nameof(DevicePlatform.iOS) => GetIOSFonts(),
            nameof(DevicePlatform.WinUI) => GetWindowsFonts(),
            nameof(DevicePlatform.MacCatalyst) => GetMacFonts(),
            _ => new List<string>() { "Platform not supported" }
        };
    }

    private static List<string> GetAndroidFonts()
    {
        var fontList = new List<string>();
#if ANDROID
        var platform = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        var assetManager = platform.Assets;

        try
        {
            var fonts = assetManager.List("fonts");
            if (fonts != null)
            {
                fontList.AddRange(fonts);
            }

            // Also get system fonts
            using var typeface = Android.Graphics.Typeface.Default;
            fontList.Add(typeface.ToString());

            // For more comprehensive list, could use FontsContract API
            // but requires higher API level
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Android fonts: {ex.Message}");
        }
#endif

        return fontList;
    }

    private static List<string> GetIOSFonts()
    {
        var fontList = new List<string>();

        try
        {
#if IOS
            var familyNames = UIKit.UIFont.FamilyNames;
            foreach (var familyName in familyNames)
            {
                fontList.Add(familyName);

                // If you also need the specific font names within each family:
                var fontNames = UIKit.UIFont.FontNamesForFamilyName(familyName);
                foreach (var fontName in fontNames)
                {
                    fontList.Add($"  {fontName}");
                }
            }
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting iOS fonts: {ex.Message}");
        }

        return fontList;
    }

    private static List<string> GetWindowsFonts()
    {
        var fontList = new List<string>();

        try
        {
#if WINDOWS
            var fonts = new InstalledFontCollection();
            return fonts.Families.Select(f => f.Name).ToList();
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Windows fonts: {ex.Message}");
        }

        return fontList;
    }

    private static List<string> GetMacFonts()
    {
        var fontList = new List<string>();

        try
        {
#if MACCATALYST
            // For MacCatalyst, similar to iOS approach
            var familyNames = UIKit.UIFont.FamilyNames;
            foreach (var familyName in familyNames)
            {
                fontList.Add(familyName);
            }
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Mac fonts: {ex.Message}");
        }

        return fontList;
    }
}
