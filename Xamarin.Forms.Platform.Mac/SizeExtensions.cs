using CoreGraphics;
using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class SizeExtensions
  {
    public static CGSize ToSizeF(this Size size)
    {
      return new CGSize((nfloat) ((float) size.Width), (nfloat) ((float) size.Height));
    }
  }
}
