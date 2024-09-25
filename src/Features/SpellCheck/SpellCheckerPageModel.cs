using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Features.SpellCheck;

public partial class SpellCheckerPageModel : ObservableObject, IQueryAttributable
{
    public SpellCheckerPage? Page { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
    }
}
