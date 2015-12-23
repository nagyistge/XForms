using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public static class FontExtensions
	{
		private static Dictionary<FontExtensions.ToUIFontKey, NSFont> toUiFont = new Dictionary<FontExtensions.ToUIFontKey, NSFont> ();

		public static NSFont ToUIFont (this Font self)
		{
			float fontSize = (float)self.FontSize;
			if (self.UseNamedSize)
			{
				switch (self.NamedSize)
				{
				case NamedSize.Micro:
					fontSize = 12f;
					break;
				case NamedSize.Small:
					fontSize = 14f;
					break;
				case NamedSize.Medium:
					fontSize = 17f;
					break;
				case NamedSize.Large:
					fontSize = 22f;
					break;
				default:
					fontSize = 17f;
					break;
				}
			}
			bool isBold = ((Enum)self.FontAttributes).HasFlag ((Enum)FontAttributes.Bold);
			bool isItalic = ((Enum)self.FontAttributes).HasFlag ((Enum)FontAttributes.Italic);

			int fontWeight = 0;

			NSFontTraitMask mask = 0;
			if (isBold)		mask = mask | NSFontTraitMask.Bold;
			if (isItalic)	mask = mask | NSFontTraitMask.Italic;

			String fontFamily;
			if (string.IsNullOrWhiteSpace (self.FontFamily))
				fontFamily = "Verdana";	// Default OS X font
			else
				fontFamily = self.FontFamily;

			try
			{
				var font = NSFontManager.SharedFontManager.FontWithFamily(
					fontFamily, mask, fontWeight, fontSize );
				return font;
			}
			catch
			{
			}
			return NSFont.SystemFontOfSize ((nfloat)fontSize);
		}

		internal static NSFont ToUIFont (this Label label)
		{
			object[] values = label.GetValues (Label.FontFamilyProperty, Label.FontSizeProperty, Label.FontAttributesProperty);
			return FontExtensions.ToUIFont ((string)values [0], (float)(double)values [1], (FontAttributes)values [2]) ?? 
				NSFont.SystemFontOfSize (NSFont.LabelFontSize);
		}

		internal static NSFont ToUIFont (this IFontElement element)
		{
			return FontExtensions.ToUIFont (element.FontFamily, (float)element.FontSize, element.FontAttributes);
		}

		private static NSFont ToUIFont (string family, float size, FontAttributes attributes)
		{
			FontExtensions.ToUIFontKey key = new FontExtensions.ToUIFontKey (family, size, attributes);
			Dictionary<FontExtensions.ToUIFontKey, NSFont> dictionary1 = FontExtensions.toUiFont;
			bool lockTaken1 = false;
			try
			{
				Monitor.Enter ((object)dictionary1, ref lockTaken1);
				NSFont uiFont;
				if (FontExtensions.toUiFont.TryGetValue (key, out uiFont))
					return uiFont;
			}
			finally
			{
				if (lockTaken1)
					Monitor.Exit ((object)dictionary1);
			}
			NSFont uiFont1 = FontExtensions._ToUIFont (family, size, attributes);
			Dictionary<FontExtensions.ToUIFontKey, NSFont> dictionary2 = FontExtensions.toUiFont;
			bool lockTaken2 = false;
			try
			{
				Monitor.Enter ((object)dictionary2, ref lockTaken2);
				NSFont uiFont2;
				if (!FontExtensions.toUiFont.TryGetValue (key, out uiFont2))
					FontExtensions.toUiFont.Add (key, uiFont2 = uiFont1);
				return uiFont2;
			}
			finally
			{
				if (lockTaken2)
					Monitor.Exit ((object)dictionary2);
			}
		}

		private static NSFont _ToUIFont (string family, float size, FontAttributes attributes)
		{
			bool isBold = (uint)(attributes & FontAttributes.Bold) > 0U;
			bool isItalic = (uint)(attributes & FontAttributes.Italic) > 0U;
			if (family != null)
			{
				try
				{
					var uiFont1 = NSFont.FromFontName (family, (nfloat)size);
					if (uiFont1 != null)
						return uiFont1;
				}
				catch
				{
				}
			}
			if (isBold & isItalic)
			{
				var uiFont = NSFont.SystemFontOfSize ((nfloat)size);
				return NSFont.BoldSystemFontOfSize ((nfloat)size);
			}
			if (isBold)
				return NSFont.BoldSystemFontOfSize ((nfloat)size);
			
			//if (isItalic)
			//	return NSFont.ItalicSystemFontOfSize ((nfloat)size);

			return NSFont.SystemFontOfSize ((nfloat)size);
		}

		internal static bool IsDefault (this Span self)
		{
			if (self.FontFamily == null && self.FontSize == Device.GetNamedSize (NamedSize.Default, typeof(Label), true))
				return self.FontAttributes == FontAttributes.None;
			return false;
		}

		private struct ToUIFontKey
		{
			private string family;
			private float size;
			private FontAttributes attributes;

			internal ToUIFontKey (string family, float size, FontAttributes attributes)
			{
				this.family = family;
				this.size = size;
				this.attributes = attributes;
			}
		}
	}
}
