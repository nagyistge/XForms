using Foundation;
using System.Collections.Generic;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public static class FormattedStringExtensions
	{
		internal static NSAttributedString ToAttributed (this Span span, Element owner, Color defaultForegroundColor)
		{
			if (span == null)
				return (NSAttributedString)null;
			NSFont font = !FontExtensions.IsDefault (span) ? FontExtensions.ToUIFont ((IFontElement)span) : FontExtensions.ToUIFont ((IFontElement)owner);
			Color color = span.ForegroundColor;
			if (color.IsDefault)
				color = defaultForegroundColor;
			if (color.IsDefault)
				color = Color.Black;

			return new NSAttributedString (
				span.Text, 
				font, 
				ColorExtensions.ToUIColor (color), 
				ColorExtensions.ToUIColor (span.BackgroundColor), 
				null, 	// NSColor strokeColor = null, 
				null, 	// NSColor underlineColor = null, 
				null, 	// NSColor strikethroughColor = null, 
				NSUnderlineStyle.None, 	// UnderlineStyle
				NSUnderlineStyle.None, 	// Strikethrough Style
				null, 	// NSParagraphStyle paragraphStyle = null, 
				0, 		//float strokeWidth = 0, 
				null, 	// NSShadow shadow = null, 
				null, 	// NSUrl link = null, 
				false, 	// bool superscript = false, 
				null, 	// NSTextAttachment attachment = null, 
				NSLigatureType.Default, 
				0, 		// float baselineOffset = 0, 
				0, 		// float kerningAdjustment = 0, 
				0, 		// float obliqueness = 0, 
				0, 		// float expansion = 0, 
				null, 	// NSCursor cursor = null, 
				null, 	// string toolTip = null, 
				0, 		// int characterShape = 0, 
				null, 	// NSGlyphInfo glyphInfo = null, 
				null, 	// NSArray writingDirection = null, 
				false, 	// bool markedClauseSegment = false, 
				NSTextLayoutOrientation.Horizontal, 
				null, 	// NSTextAlternatives textAlternatives = null, 
				NSSpellingState.None);
		}

		internal static NSAttributedString ToAttributed (this FormattedString formattedString, Element owner, Color defaultForegroundColor)
		{
			if (formattedString == null)
				return (NSAttributedString)null;
			NSMutableAttributedString attributedString = new NSMutableAttributedString ();
			foreach (Span span in formattedString.Spans)
			{
				if (span.Text != null)
				{
					attributedString.Append (FormattedStringExtensions.ToAttributed (span, owner, defaultForegroundColor));
				}
			}
			return (NSAttributedString)attributedString;
		}

		public static NSAttributedString ToAttributed (this Span span, Font defaultFont, Color defaultForegroundColor)
		{
			if (span == null)
				return (NSAttributedString)null;
			Color color = span.ForegroundColor;
			if (color.IsDefault)
				color = defaultForegroundColor;
			if (color.IsDefault)
				color = Color.Black;

			Font self = span.Font != Font.Default ? span.Font : defaultFont;
			var font = self == Font.Default ? (NSFont)null : FontExtensions.ToUIFont (self);

			return new NSAttributedString (
				span.Text, 
				font, 
				ColorExtensions.ToUIColor (color), 
				ColorExtensions.ToUIColor (span.BackgroundColor), 
				null, 	// NSColor strokeColor = null, 
				null, 	// NSColor underlineColor = null, 
				null, 	// NSColor strikethroughColor = null, 
				NSUnderlineStyle.None, 	// UnderlineStyle
				NSUnderlineStyle.None, 	// Strikethrough Style
				null, 	// NSParagraphStyle paragraphStyle = null, 
				0, 		// float strokeWidth = 0, 
				null, 	// NSShadow shadow = null, 
				null, 	// NSUrl link = null, 
				false, 	// bool superscript = false, 
				null, 	// NSTextAttachment attachment = null, 
				NSLigatureType.Default, 
				0, 		// float baselineOffset = 0, 
				0, 		// float kerningAdjustment = 0, 
				0, 		// float obliqueness = 0, 
				0, 		// float expansion = 0, 
				null, 		// NSCursor cursor = null, 
				null, 	// string toolTip = null, 
				0, 		// int characterShape = 0, 
				null, 	// NSGlyphInfo glyphInfo = null, 
				null, 	// NSArray writingDirection = null, 
				false, 	// bool markedClauseSegment = false, 
				NSTextLayoutOrientation.Horizontal, 
				null, 	// NSTextAlternatives textAlternatives = null, 
				NSSpellingState.None);

		}

		public static NSAttributedString ToAttributed (this FormattedString formattedString, Font defaultFont, Color defaultForegroundColor)
		{
			if (formattedString == null)
				return (NSAttributedString)null;
			NSMutableAttributedString attributedString = new NSMutableAttributedString ();
			foreach (Span span in formattedString.Spans)
			{
				if (span.Text != null)
				{
					attributedString.Append (FormattedStringExtensions.ToAttributed (span, defaultFont, defaultForegroundColor));
				}
			}
			return (NSAttributedString)attributedString;
		}
	}
}
