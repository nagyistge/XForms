using CoreGraphics;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class PointExtensions
  {
    public static Point ToPoint(this CGPoint point)
    {
      return new Point((double) point.X, (double) point.Y);
    }
  }
}
