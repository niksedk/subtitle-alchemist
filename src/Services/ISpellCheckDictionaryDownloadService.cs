namespace SubtitleAlchemist.Services;

public interface ISpellCheckDictionaryDownloadService
{
    Task DownloadDictionary(Stream stream, string url, IProgress<float>? progress, CancellationToken cancellationToken);
}