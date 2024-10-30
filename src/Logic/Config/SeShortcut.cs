using SubtitleAlchemist.Features.Options.Settings;
using System.Text.Json.Serialization;

namespace SubtitleAlchemist.Logic.Config;

public class SeShortCut
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ShortcutAction ActionName { get; set; }

    public List<string> Keys { get; set; }

    public SeShortCut()
    {
        Keys = new List<string>();
    }

    public SeShortCut(ShortcutAction action, List<string> keys)
    {
        ActionName = action;
        Keys = keys;
    }
}
