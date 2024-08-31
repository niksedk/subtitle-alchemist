using Nikse.SubtitleEdit.Core.Common;
using System.Diagnostics;

namespace SubtitleAlchemist.Logic;

internal static class UiUtil
{
    public static void OpenFolderFromFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        string? folderName = Path.GetDirectoryName(fileName);
        if (Configuration.IsRunningOnWindows)
        {
            var argument = @"/select, " + fileName;
#pragma warning disable CA1416 // Validate platform compatibility
            Process.Start("explorer.exe", argument);
#pragma warning restore CA1416 // Validate platform compatibility
        }
        else if (!string.IsNullOrEmpty(folderName)) 
        {
            OpenFolder(folderName);
        }
    }

    public static void OpenFolder(string folder)
    {
        OpenItem(folder, "folder");
    }

    public static void OpenUrl(string url)
    {
        Launcher.OpenAsync(url);
    }

    public static async Task OpenUrlAsync(string url)
    {
        await Launcher.OpenAsync(url);
    }

    public static void OpenFile(string file)
    {
        OpenItem(file, "file");
    }

    public static void OpenItem(string item, string type)
    {
        try
        {
            if (Configuration.IsRunningOnWindows || Configuration.IsRunningOnMac)
            {
                var startInfo = new ProcessStartInfo(item)
                {
                    UseShellExecute = true
                };

#pragma warning disable CA1416 // Validate platform compatibility
                Process.Start(startInfo);
#pragma warning restore CA1416 // Validate platform compatibility
            }
            else if (Configuration.IsRunningOnLinux)
            {
                var process = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = { FileName = "xdg-open", Arguments = item }
                };

#pragma warning disable CA1416 // Validate platform compatibility
                var _ = process.Start();
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Cannot open {type}: {item}{Environment.NewLine}{Environment.NewLine}{exception.Source}: {exception.Message}");
        }
    }

    public static VideoInfo GetVideoInfo(string waveFileName)
    {
        return new VideoInfo
        {
            TotalMilliseconds = 10000,
            Width = 1280,
            Height = 720,
        };
    }
}