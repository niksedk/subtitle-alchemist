namespace SubtitleAlchemist.Services;

public interface IFfmpegDownloadService
{
    Task<byte[]> DownloadFfmpeg();
}