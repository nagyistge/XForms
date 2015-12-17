using UIKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal class HeaderWrapperView : UIView
  {
    public override void LayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.LayoutSubviews();
      foreach (UIView uiView in this.Subviews)
        uiView.Frame = this.Bounds;
    }
  }
}
