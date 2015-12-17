using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal class GlobalCloseContextGestureRecognizer : UIGestureRecognizer
  {
    private List<UIButton> buttons;
    private UIScrollView scrollView;

    public GlobalCloseContextGestureRecognizer(UIScrollView scrollView, List<UIButton> buttons, Action activated)
      : base(activated)
    {
      this.scrollView = scrollView;
      this.buttons = buttons;
      this.ShouldReceiveTouch = new UITouchEventArgs(this.OnShouldReceiveTouch);
    }

    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      this.State = UIGestureRecognizerState.Began;
      // ISSUE: reference to a compiler-generated method
      base.TouchesBegan(touches, evt);
    }

    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
      this.State = UIGestureRecognizerState.Ended;
      // ISSUE: reference to a compiler-generated method
      base.TouchesMoved(touches, evt);
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      this.State = UIGestureRecognizerState.Ended;
      // ISSUE: reference to a compiler-generated method
      base.TouchesEnded(touches, evt);
    }

    protected override void Dispose(bool disposing)
    {
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
      if (!disposing)
        return;
      this.buttons = (List<UIButton>) null;
      this.scrollView = (UIScrollView) null;
    }

    private bool OnShouldReceiveTouch(UIGestureRecognizer r, UITouch t)
    {
      // ISSUE: reference to a compiler-generated method
      CGPoint point = t.LocationInView((UIView) this.scrollView);
      CGRect cgRect;
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      CGRect& local = @cgRect;
      nfloat x = (nfloat) 0;
      nfloat y = (nfloat) 0;
      CGSize contentSize = this.scrollView.ContentSize;
      nfloat width = contentSize.Width;
      contentSize = this.scrollView.ContentSize;
      nfloat height = contentSize.Height;
      // ISSUE: explicit reference operation
      ^local = new CGRect(x, y, width, height);
      return !cgRect.Contains(point);
    }
  }
}
