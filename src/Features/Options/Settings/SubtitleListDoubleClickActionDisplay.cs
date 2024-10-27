using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Options.Settings;
public class SubtitleListDoubleClickActionDisplay
{
    public string Name { get; set; }
    public SubtitleListDoubleClickActionType Action { get; set; }

    public SubtitleListDoubleClickActionDisplay(string name, SubtitleListDoubleClickActionType action)
    {
        Name = name;
        Action = action;
    }

    public override string ToString()
    {
        return Name;
    }

    public static List<SubtitleListDoubleClickActionDisplay> GetSubtitleListDoubleClickActions()
    {
        var l = Se.Language.Settings;
        return new List<SubtitleListDoubleClickActionDisplay>
        {
            new(l.SubtitleListActionNothing, SubtitleListDoubleClickActionType.Nothing),
            new(l.SubtitleListActionVideoGoToPositionAndPause, SubtitleListDoubleClickActionType.VideoGoToPositionAndPause),
            new(l.SubtitleListActionVideoGoToPositionAndPlay, SubtitleListDoubleClickActionType.VideoGoToPositionAndPlay),
            new(l.SubtitleListActionVideoGoToPositionAndPlayCurrentAndPause, SubtitleListDoubleClickActionType.VideoGoToPositionAndPlayCurrentAndPause),
            new(l.SubtitleListActionEditText, SubtitleListDoubleClickActionType.EditText),
            new(l.SubtitleListActionVideoGoToPositionMinus1SecAndPause, SubtitleListDoubleClickActionType.VideoGoToPositionMinus1SecAndPause),
            new(l.SubtitleListActionVideoGoToPositionMinusHalfSecAndPause, SubtitleListDoubleClickActionType.VideoGoToPositionMinusHalfSecAndPause),
            new(l.SubtitleListActionVideoGoToPositionMinus1SecAndPlay, SubtitleListDoubleClickActionType.VideoGoToPositionMinus1SecAndPlay),
            new(l.SubtitleListActionEditTextAndPause, SubtitleListDoubleClickActionType.EditTextAndPause),
        };
    }
}

public enum SubtitleListDoubleClickActionType
{
    Nothing,
    VideoGoToPositionAndPause,
    VideoGoToPositionAndPlay,
    VideoGoToPositionAndPlayCurrentAndPause,
    EditText,
    VideoGoToPositionMinus1SecAndPause,
    VideoGoToPositionMinusHalfSecAndPause,
    VideoGoToPositionMinus1SecAndPlay,
    EditTextAndPause,
}