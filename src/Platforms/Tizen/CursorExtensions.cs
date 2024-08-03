using MauiCursor;

// ReSharper disable once CheckNamespace
namespace SubtitleAlchemist;

public static class CursorExtensions // Do not change this line as this namespace is required for platform code
{
	public static void SetCustomCursor(this VisualElement visualElement, CursorIcon cursor, IMauiContext? mauiContext)
	{
		ArgumentNullException.ThrowIfNull(mauiContext);
	}
}