using AppKit;
using MauiCursor;
using Microsoft.Maui.Platform;
using UIKit;

// ReSharper disable once CheckNamespace
namespace SubtitleAlchemist; // Do not change this line as this namespace is required for platform code

public static class CursorExtensions
{
    public static void SetCustomCursor(this VisualElement visualElement, CursorIcon cursor, IMauiContext? mauiContext)
    {
        GetNSCursor(cursor).Set();
    }

    static NSCursor GetNSCursor(CursorIcon cursor)
    {
        return cursor switch
        {
            CursorIcon.Hand => NSCursor.OpenHandCursor,
            CursorIcon.IBeam => NSCursor.IBeamCursor,
            CursorIcon.Cross => NSCursor.CrosshairCursor,
            CursorIcon.Arrow => NSCursor.ArrowCursor,
            CursorIcon.SizeAll => NSCursor.ResizeUpCursor,
            CursorIcon.Wait => NSCursor.OperationNotAllowedCursor,
            CursorIcon.ResizeLeftRight => NSCursor.ResizeLeftRightCursor,
            _ => NSCursor.ArrowCursor,
        };
    }
}