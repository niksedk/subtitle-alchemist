using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Main;

public interface IMainShortcutActions
{
    void Initialize(IShortcutManager shortcutManager, MainPageModel pageModel);
}