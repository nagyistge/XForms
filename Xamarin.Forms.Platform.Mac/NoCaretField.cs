using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal class NoCaretField : UITextField
  {
    public NoCaretField()
      : base(new CGRect())
    {
    }

    public override CGRect GetCaretRectForPosition(UITextPosition position)
    {
      return new CGRect();
    }
  }
}
