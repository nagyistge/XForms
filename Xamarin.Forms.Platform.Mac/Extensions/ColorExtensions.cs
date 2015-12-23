using CoreGraphics;
using System;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public static class ColorExtensions
	{
		internal static readonly NSColor Black = NSColor.Black;
		internal static readonly NSColor SeventyPercentGrey = NSColor.FromRgba ((nfloat)0.7f, (nfloat)0.7f, (nfloat)0.7f, (nfloat)1);

		public static CGColor ToCGColor (this Color color)
		{
			return new CGColor ((nfloat)((float)color.R), (nfloat)((float)color.G), (nfloat)((float)color.B), (nfloat)((float)color.A));
		}

		public static NSColor ToUIColor (this Color color)
		{
			return NSColor.FromDeviceRgba((nfloat)((float)color.R), (nfloat)((float)color.G), (nfloat)((float)color.B), (nfloat)((float)color.A));
		}

		public static NSColor ToUIColor (this Color color, Color defaultColor)
		{
			if (color.IsDefault)
				return ColorExtensions.ToUIColor (defaultColor);
			return ColorExtensions.ToUIColor (color);
		}

		public static NSColor ToUIColor (this Color color, NSColor defaultColor)
		{
			if (color.IsDefault)
				return defaultColor;
			return ColorExtensions.ToUIColor (color);
		}

		public static Color ToColor (this NSColor color)
		{
			nfloat red, green, blue, alpha;

			color.GetRgba (out red, out green, out blue, out alpha);
			return new Color ((double)red, (double)green, (double)blue, (double)alpha);
		}
	}
}
