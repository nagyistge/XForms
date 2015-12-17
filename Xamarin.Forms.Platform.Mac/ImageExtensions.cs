using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class ImageExtensions
  {
    public static UIViewContentMode ToUIViewContentMode(this Aspect aspect)
    {
      switch (aspect)
      {
        case Aspect.AspectFill:
          return UIViewContentMode.ScaleAspectFill;
        case Aspect.Fill:
          return UIViewContentMode.ScaleToFill;
        default:
          return UIViewContentMode.ScaleAspectFit;
      }
    }
  }
}
