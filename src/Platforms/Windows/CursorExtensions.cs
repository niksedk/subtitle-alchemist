using MauiCursor;
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Reflection;
using Windows.UI.Core;

// ReSharper disable once CheckNamespace
namespace SubtitleAlchemist; // Do not change this line as this namespace is required for platform code

public static partial class CursorExtensions
{
	public static void SetCustomCursor(this VisualElement visualElement, CursorIcon cursor, IMauiContext? mauiContext)
	{
		ArgumentNullException.ThrowIfNull(mauiContext);
		var view = (UIElement)visualElement.ToPlatform(mauiContext);
        view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(GetCursor(cursor), 1)));

        //view.PointerEntered += ViewOnPointerEntered;
        //view.PointerExited += ViewOnPointerExited;

  //      MainThread.BeginInvokeOnMainThread(() =>
  //      {
  //          view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(GetCursor(cursor), 1)));
		//	view.InvalidateArrange();
  //      });


  //      void ViewOnPointerExited(object sender, PointerRoutedEventArgs e)
		//{
		//	view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(GetCursor(CursorIcon.Arrow), 1)));
		//}

  //      void ViewOnPointerEntered(object sender, PointerRoutedEventArgs e)
  //      {
  //          view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(GetCursor(cursor), 1)));
  //      }
	}

    private static void ChangeCursor(this UIElement uiElement, InputCursor cursor)
	{
		var type = typeof(UIElement);
		type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { cursor });
	}

    private static CoreCursorType GetCursor(CursorIcon cursor)
	{
		return cursor switch
		{
			CursorIcon.Hand => CoreCursorType.Hand,
			CursorIcon.IBeam => CoreCursorType.IBeam,
			CursorIcon.Cross => CoreCursorType.Cross,
			CursorIcon.Arrow => CoreCursorType.Arrow,
			CursorIcon.SizeAll => CoreCursorType.SizeAll,
            CursorIcon.Wait => CoreCursorType.Wait,
            CursorIcon.ResizeLeftRight => CoreCursorType.SizeWestEast,
			_ => CoreCursorType.Arrow,
		};
	}
}