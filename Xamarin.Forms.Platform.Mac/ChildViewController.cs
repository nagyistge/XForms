using UIKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal class ChildViewController : UIViewController
  {
    public override void ViewDidLayoutSubviews()
    {
      foreach (UIViewController uiViewController in this.ChildViewControllers)
        uiViewController.View.Frame = this.View.Bounds;
    }
  }
}
