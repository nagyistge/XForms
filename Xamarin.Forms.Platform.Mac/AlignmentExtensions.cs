using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal static class AlignmentExtensions
  {
    internal static UITextAlignment ToNativeTextAlignment(this TextAlignment alignment)
    {
      if (alignment == TextAlignment.Center)
        return UITextAlignment.Center;
      return alignment == TextAlignment.End ? UITextAlignment.Right : UITextAlignment.Left;
    }
  }
}
