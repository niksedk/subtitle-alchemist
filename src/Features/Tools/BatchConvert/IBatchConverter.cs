namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public interface IBatchConverter
{
    void Initialize(BatchConvertConfig config); 
    Task Convert(BatchConvertItem item);
}