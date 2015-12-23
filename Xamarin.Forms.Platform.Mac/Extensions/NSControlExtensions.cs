using System;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	public static class NSControlExtensions
	{
		public static Color GetBackgroundColor(this NSControl self)
		{
			if (self == null)
				return Color.Transparent;
			
			if (self.WantsLayer)
				NSColor.FromCGColor( self.Layer.BackgroundColor ).ToColor ();
			return Color.Transparent;
		}

		public static void SetBackgroundColor(this NSControl self, Color color)
		{
			if (self == null)
				return;
			
			if (color == Color.Transparent)
				self.WantsLayer = false;
			else
			{
				self.WantsLayer = true;
				self.Layer.BackgroundColor = color.ToCGColor ();
			}
		}

		public static void SetBackgroundColor(this NSView self, NSColor color)
		{
			if (color == NSColor.Clear)
				self.WantsLayer = false;
			else
			{
				self.WantsLayer = true;
				self.Layer.BackgroundColor =  color.CGColor;
			}
		}

	}
}

