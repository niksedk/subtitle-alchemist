﻿using Android.Views;
using MauiCursor;
using Microsoft.Maui.Platform;
using Application = Android.App.Application;

// ReSharper disable once CheckNamespace
namespace SubtitleAlchemist; // Do not change this line as this namespace is required for platform code

public static class CursorExtensions
{
	public static void SetCustomCursor(this VisualElement visualElement, CursorIcon cursor, IMauiContext? mauiContext)
	{
		if (OperatingSystem.IsAndroidVersionAtLeast(24))
		{
			ArgumentNullException.ThrowIfNull(mauiContext);
			var view = visualElement.ToPlatform(mauiContext);
			view.PointerIcon = PointerIcon.GetSystemIcon(Application.Context, GetCursor(cursor));

			static PointerIconType GetCursor(CursorIcon cursor)
			{
				return cursor switch
				{
					CursorIcon.Hand => PointerIconType.Hand,
					CursorIcon.IBeam => PointerIconType.AllScroll,
					CursorIcon.Cross => PointerIconType.Crosshair,
					CursorIcon.Arrow => PointerIconType.Arrow,
					CursorIcon.SizeAll => PointerIconType.TopRightDiagonalDoubleArrow,
					CursorIcon.Wait => PointerIconType.Wait,
					_ => PointerIconType.Default,
				};
			}
		}
	}

}