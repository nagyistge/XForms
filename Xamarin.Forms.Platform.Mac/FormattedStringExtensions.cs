using Foundation;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class FormattedStringExtensions
  {
    internal static NSAttributedString ToAttributed(this Span span, Element owner, Color defaultForegroundColor)
    {
      if (span == null)
        return (NSAttributedString) null;
      UIFont font = !FontExtensions.IsDefault(span) ? FontExtensions.ToUIFont((IFontElement) span) : FontExtensions.ToUIFont((IFontElement) owner);
      Color color = span.ForegroundColor;
      if (color.IsDefault)
        color = defaultForegroundColor;
      if (color.IsDefault)
        color = Color.Black;
      return new NSAttributedString(span.Text, font, ColorExtensions.ToUIColor(color), ColorExtensions.ToUIColor(span.BackgroundColor), (UIColor) null, (NSParagraphStyle) null, NSLigatureType.Default, 0.0f, NSUnderlineStyle.None, (NSShadow) null, 0.0f, NSUnderlineStyle.None);
    }

    internal static NSAttributedString ToAttributed(this FormattedString formattedString, Element owner, Color defaultForegroundColor)
    {
      if (formattedString == null)
        return (NSAttributedString) null;
      NSMutableAttributedString attributedString = new NSMutableAttributedString();
      foreach (Span span in (IEnumerable<Span>) formattedString.get_Spans())
      {
        if (span.Text != null)
        {
          // ISSUE: reference to a compiler-generated method
          attributedString.Append(FormattedStringExtensions.ToAttributed(span, owner, defaultForegroundColor));
        }
      }
      return (NSAttributedString) attributedString;
    }

    public static NSAttributedString ToAttributed(this Span span, Font defaultFont, Color defaultForegroundColor)
    {
      if (span == null)
        return (NSAttributedString) null;
      Font self = span.Font != Font.Default ? span.Font : defaultFont;
      Color color = span.ForegroundColor;
      if (color.IsDefault)
        color = defaultForegroundColor;
      if (color.IsDefault)
        color = Color.Black;
      return new NSAttributedString(span.Text, self == Font.Default ? (UIFont) null : FontExtensions.ToUIFont(self), ColorExtensions.ToUIColor(color), ColorExtensions.ToUIColor(span.BackgroundColor), (UIColor) null, (NSParagraphStyle) null, NSLigatureType.Default, 0.0f, NSUnderlineStyle.None, (NSShadow) null, 0.0f, NSUnderlineStyle.None);
    }

    public static NSAttributedString ToAttributed(this FormattedString formattedString, Font defaultFont, Color defaultForegroundColor)
    {
      if (formattedString == null)
        return (NSAttributedString) null;
      NSMutableAttributedString attributedString = new NSMutableAttributedString();
      foreach (Span span in (IEnumerable<Span>) formattedString.get_Spans())
      {
        if (span.Text != null)
        {
          // ISSUE: reference to a compiler-generated method
          attributedString.Append(FormattedStringExtensions.ToAttributed(span, defaultFont, defaultForegroundColor));
        }
      }
      return (NSAttributedString) attributedString;
    }
  }
}
