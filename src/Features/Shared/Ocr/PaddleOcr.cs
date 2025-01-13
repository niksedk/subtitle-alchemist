using System.Diagnostics;
using System.Text;
using SkiaSharp;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public class PaddleOcr
{
    public string Error { get; set; }

    public PaddleOcr()
    {
        Error = string.Empty;
    }

    public async Task<string> Ocr(SKBitmap bitmap, string language, CancellationToken cancellationToken)
    {
        var tempImage = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
        await File.WriteAllBytesAsync(tempImage, bitmap.ToPngArray(), cancellationToken);
        var parameters = $"paddleocr --image_dir \"{tempImage}\" --use_angle_cls true --lang {language} --show_log false";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "paddleocr",
                Arguments = parameters,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            },
        };

        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.RedirectStandardOutput = true;
        process.OutputDataReceived += OutputHandler;

#pragma warning disable CA1416 // Validate platform compatibility
        process.Start();
#pragma warning restore CA1416 // Validate platform compatibility;

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            Error = await process.StandardError.ReadToEndAsync(cancellationToken);
            return string.Empty;
        }

        File.Delete(tempImage);
        return string.Empty;
    }

    private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (string.IsNullOrWhiteSpace(outLine.Data))
        {
            return;
        }
    }

    public List<OcrLanguage2> ocrLanguage()
    {
        return new List<OcrLanguage2>
        {
            new OcrLanguage2("abq", "Abkhazian"),
            new OcrLanguage2("ady", "Adyghe"),
            new OcrLanguage2("af", "Afrikaans"),
            new OcrLanguage2("sq", "Albanian"),
            new OcrLanguage2("ang", "Angika"),
            new OcrLanguage2("ar", "Arabic"),
            new OcrLanguage2("ava", "Avar"),
            new OcrLanguage2("az", "Azerbaijani"),
            new OcrLanguage2("be", "Belarusian"),
            new OcrLanguage2("bho", "Bhojpuri"),
            new OcrLanguage2("bh", "Bihari"),
            new OcrLanguage2("bs", "Bosnian"),
            new OcrLanguage2("bg", "Bulgarian"),
            new OcrLanguage2("ch", "Chinese and english"),
            new OcrLanguage2("ch_tra", "Chinese traditional"),
            new OcrLanguage2("hr", "Croatian"),
            new OcrLanguage2("cs", "Czech"),
            new OcrLanguage2("da", "Danish"),
            new OcrLanguage2("dar", "Dargwa"),
            new OcrLanguage2("nl", "Dutch"),
            new OcrLanguage2("en", "English"),
            new OcrLanguage2("et", "Estonian"),
            new OcrLanguage2("fr", "French"),
            new OcrLanguage2("german", "German"),
            new OcrLanguage2("gom", "Goan Konkani"),
            new OcrLanguage2("hi", "Hindi"),
            new OcrLanguage2("hu", "Hungarian"),
            new OcrLanguage2("is", "Icelandic"),
            new OcrLanguage2("id", "Indonesian"),
            new OcrLanguage2("inh", "Ingush"),
            new OcrLanguage2("ga", "Irish"),
            new OcrLanguage2("it", "Italian"),
            new OcrLanguage2("japan", "Japan"),
            new OcrLanguage2("kbd", "Kabardian"),
            new OcrLanguage2("korean", "Korean"),
            new OcrLanguage2("ku", "Kurdish"),
            new OcrLanguage2("lbe", "Lak"),
            new OcrLanguage2("lv", "Latvian"),
            new OcrLanguage2("lez", "Lezghian"),
            new OcrLanguage2("lt", "Lithuanian"),
            new OcrLanguage2("mah", "Magahi"),
            new OcrLanguage2("mai", "Maithili"),
            new OcrLanguage2("ms", "Malay"),
            new OcrLanguage2("mt", "Maltese"),
            new OcrLanguage2("mi", "Maori"),
            new OcrLanguage2("mr", "Marathi"),
            new OcrLanguage2("mn", "Mongolian"),
            new OcrLanguage2("sck", "Nagpur"),
            new OcrLanguage2("ne", "Nepali"),
            new OcrLanguage2("new", "Newari"),
            new OcrLanguage2("no", "Norwegian"),
            new OcrLanguage2("oc", "Occitan"),
            new OcrLanguage2("fa", "Persian"),
            new OcrLanguage2("pl", "Polish"),
            new OcrLanguage2("pt", "Portuguese"),
            new OcrLanguage2("ro", "Romanian"),
            new OcrLanguage2("ru", "Russia"),
            new OcrLanguage2("sa", "Saudi Arabia"),
            new OcrLanguage2("rs_cyrillic", "Serbian(cyrillic)"),
            new OcrLanguage2("rs_latin", "Serbian(latin)"),
            new OcrLanguage2("sk", "Slovak"),
            new OcrLanguage2("sl", "Slovenian"),
            new OcrLanguage2("es", "Spanish"),
            new OcrLanguage2("sw", "Swahili"),
            new OcrLanguage2("sv", "Swedish"),
            new OcrLanguage2("tab", "Tabassaran"),
            new OcrLanguage2("tl", "Tagalog"),
            new OcrLanguage2("ta", "Tamil"),
            new OcrLanguage2("te", "Telugu"),
            new OcrLanguage2("tr", "Turkish"),
            new OcrLanguage2("uk", "Ukranian"),
            new OcrLanguage2("ur", "Urdu"),
            new OcrLanguage2("ug", "Uyghur"),
            new OcrLanguage2("uz", "Uzbek"),
            new OcrLanguage2("vi", "Vietnamese"),
            new OcrLanguage2("cy", "Welsh"),
        };
    }
}
