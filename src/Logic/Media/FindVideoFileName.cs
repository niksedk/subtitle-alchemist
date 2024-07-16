using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Logic.Media
{
    public static class FindVideoFileName
    {
        public static bool TryFindVideoFileName(string inputFileName, out string videoFileName)
        {
            videoFileName = string.Empty;

            if (string.IsNullOrEmpty(inputFileName))
            {
                return false;
            }

            foreach (var extension in Utilities.VideoFileExtensions.Concat(Utilities.AudioFileExtensions))
            {
                var fileName = inputFileName + extension;
                if (File.Exists(fileName))
                {
                    videoFileName = fileName;
                    return true;
                }
            }

            var index = inputFileName.LastIndexOf('.');
            if (index > 0 && TryFindVideoFileName(inputFileName.Remove(index), out videoFileName))
            {
                return true;
            }

            index = inputFileName.LastIndexOf('_');
            if (index > 0 && TryFindVideoFileName(inputFileName.Remove(index), out videoFileName))
            {
                return true;
            }

            return false;
        }
    }
}
