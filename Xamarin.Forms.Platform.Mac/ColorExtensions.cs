using CoreGraphics;
using System;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class ColorExtensions
  {
    internal static readonly UIColor Black = UIColor.Black;
    internal static readonly UIColor SeventyPercentGrey = new UIColor((nfloat) 0.7f, (nfloat) 0.7f, (nfloat) 0.7f, (nfloat) 1);

    public static CGColor ToCGColor(this Color color)
    {
      return new CGColor((nfloat) ((float) color.R), (nfloat) ((float) color.G), (nfloat) ((float) color.B), (nfloat) ((float) color.A));
    }

    public static UIColor ToUIColor(this Color color)
    {
      return new UIColor((nfloat) ((float) color.R), (nfloat) ((float) color.G), (nfloat) ((float) color.B), (nfloat) ((float) color.A));
    }

    public static UIColor ToUIColor(this Color color, Color defaultColor)
    {
      if (color.IsDefault)
        return ColorExtensions.ToUIColor(defaultColor);
      return ColorExtensions.ToUIColor(color);
    }

    public static UIColor ToUIColor(this Color color, UIColor defaultColor)
    {
      if (color.IsDefault)
        return defaultColor;
      return ColorExtensions.ToUIColor(color);
    }

    public static Color ToColor(this UIColor color)
    {
      nfloat red;
      nfloat green;
      nfloat blue;
      nfloat alpha;
      color.GetRGBA(out red, out green, out blue, out alpha);
      return new Color((double) red, (double) green, (double) blue, (double) alpha);
    }
  }
}
