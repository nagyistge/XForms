using CoreGraphics;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class ModalWrapper : UIViewController
  {
    private IVisualElementRenderer modal;

    internal ModalWrapper(IVisualElementRenderer modal)
    {
      this.modal = modal;
      this.View.BackgroundColor = UIColor.White;
      // ISSUE: reference to a compiler-generated method
      this.View.AddSubview(modal.ViewController.View);
      // ISSUE: reference to a compiler-generated method
      this.AddChildViewController(modal.ViewController);
      // ISSUE: reference to a compiler-generated method
      modal.ViewController.DidMoveToParentViewController((UIViewController) this);
    }

    public override void ViewDidLayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLayoutSubviews();
      if (this.modal == null)
        return;
      IVisualElementRenderer visualElementRenderer = this.modal;
      CGRect bounds = this.View.Bounds;
      double width = (double) bounds.Width;
      bounds = this.View.Bounds;
      double height = (double) bounds.Height;
      Size size = new Size(width, height);
      visualElementRenderer.SetElementSize(size);
    }

    public override void ViewWillAppear(bool animated)
    {
      this.View.BackgroundColor = UIColor.White;
      // ISSUE: reference to a compiler-generated method
      base.ViewWillAppear(animated);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.modal = (IVisualElementRenderer) null;
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
    {
      return UIInterfaceOrientationMask.All;
    }
  }
}
