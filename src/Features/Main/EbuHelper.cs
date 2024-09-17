using Nikse.SubtitleEdit.Core.Common;
using static Nikse.SubtitleEdit.Core.SubtitleFormats.Ebu;

namespace SubtitleAlchemist.Features.Main;

internal class EbuHelper : IEbuUiHelper
{
    public void Initialize(EbuGeneralSubtitleInformation header, byte justificationCode, string fileName, Subtitle subtitle)
    {
        header = new EbuGeneralSubtitleInformation();
    }

    public bool ShowDialogOk()
    {
        return true;
    }

    public byte JustificationCode { get; set; } = 2; //TODO: default value?
}
