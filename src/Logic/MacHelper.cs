using System.Diagnostics;

namespace SubtitleAlchemist.Logic;

public class MacHelper
{
    public static bool MakeExecutable(string filePath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/chmod",
                Arguments = $"+x \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine("chmod failed:");
            Console.WriteLine(error);
            return false;
        }

        return true;
    }
}