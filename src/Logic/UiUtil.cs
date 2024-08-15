using Nikse.SubtitleEdit.Core.Common;
using System.Diagnostics;

namespace SubtitleAlchemist.Logic;

internal static class UiUtil
{
    public static void OpenFolderFromFileName(string fileName)
    {
        string folderName = Path.GetDirectoryName(fileName);
        if (Configuration.IsRunningOnWindows)
        {
            var argument = @"/select, " + fileName;
            Process.Start("explorer.exe", argument);
        }
        else
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
        OpenItem(url, "url");
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

                Process.Start(startInfo);
            }
            else if (Configuration.IsRunningOnLinux)
            {
                var process = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = { FileName = "xdg-open", Arguments = item }
                };
                process.Start();
            }
        }
        catch (Exception exception)
        {
           // MessageBox.Show($"Cannot open {type}: {item}{Environment.NewLine}{Environment.NewLine}{exception.Source}: {exception.Message}", "Error opening URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}