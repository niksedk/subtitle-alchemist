using CommunityToolkit.Mvvm.ComponentModel;
using Nikse.SubtitleEdit.Core.Interfaces;

namespace SubtitleAlchemist.Features.SpellCheck;

public partial class SpellCheckerPageModel : ObservableObject, IQueryAttributable
{
    public SpellCheckerPage? Page { get; set; }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        throw new NotImplementedException();
    }
}
