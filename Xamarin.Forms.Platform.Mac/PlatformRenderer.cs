using UIKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal class PlatformRenderer : UIViewController
  {
    public Platform Platform { get; set; }

    internal PlatformRenderer(Platform platform)
    {
      this.Platform = platform;
    }

    public override void ViewDidLayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLayoutSubviews();
      this.Platform.LayoutSubviews();
    }

    public override void ViewDidAppear(bool animated)
    {
      this.Platform.DidAppear();
      // ISSUE: reference to a compiler-generated method
      base.ViewDidAppear(animated);
    }

    public override void ViewWillAppear(bool animated)
    {
      this.View.BackgroundColor = UIColor.White;
      this.Platform.WillAppear();
      // ISSUE: reference to a compiler-generated method
      base.ViewWillAppear(animated);
    }

    public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
    {
      return UIInterfaceOrientationMask.All;
    }
  }
}
