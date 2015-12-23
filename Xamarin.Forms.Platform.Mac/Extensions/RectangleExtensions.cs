using CoreGraphics;
using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class RectangleExtensions
  {
    public static Rectangle ToRectangle(this CGRect rect)
    {
      return new Rectangle((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);
    }

    public static CGRect ToRectangleF(this Rectangle rect)
    {
      return new CGRect((nfloat) rect.X, (nfloat) rect.Y, (nfloat) rect.Width, (nfloat) rect.Height);
    }
  }
}
