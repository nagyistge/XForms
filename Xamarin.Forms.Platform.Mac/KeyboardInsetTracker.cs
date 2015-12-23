using CoreGraphics;
using System;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal class KeyboardInsetTracker : IDisposable
  {
    private readonly NSView targetView;
    private readonly Func<UIWindow> fetchWindow;
    private readonly Action<UIEdgeInsets> setInsetAction;
    private readonly Action<CGPoint> setContentOffset;
    private CGRect lastKeyboardRect;
    private bool disposed;

    public KeyboardInsetTracker(NSView targetView, Func<UIWindow> fetchWindow, Action<UIEdgeInsets> setInsetAction)
      : this(targetView, fetchWindow, setInsetAction, (Action<CGPoint>) null)
    {
    }

    public KeyboardInsetTracker(NSView targetView, Func<UIWindow> fetchWindow, Action<UIEdgeInsets> setInsetAction, Action<CGPoint> setContentOffset)
    {
      this.setContentOffset = setContentOffset;
      this.targetView = targetView;
      this.fetchWindow = fetchWindow;
      this.setInsetAction = setInsetAction;
      KeyboardObserver.KeyboardWillShow += new EventHandler<UIKeyboardEventArgs>(this.OnKeyboardShown);
      KeyboardObserver.KeyboardWillHide += new EventHandler<UIKeyboardEventArgs>(this.OnKeyboardHidden);
    }

    internal void UpdateInsets()
    {
      if (this.lastKeyboardRect.IsEmpty || this.fetchWindow() == null)
        return;
      NSView firstResponder = UIViewExtensions.FindFirstResponder(this.targetView);
      CGRect frame = this.targetView.Frame;
      CGSize size = frame.Size;
      // ISSUE: reference to a compiler-generated method
      CGRect cgRect = CGRect.Intersect(this.targetView.Superview.ConvertRectFromView(this.lastKeyboardRect, (NSView) null), this.targetView.Frame);
      this.setInsetAction(new UIEdgeInsets((nfloat) 0, (nfloat) 0, cgRect.Height, (nfloat) 0));
      if (!(firstResponder is UITextView) || this.setContentOffset == null)
        return;
      nfloat nfloat = size.Height - cgRect.Height;
      NSView uiView = firstResponder;
      frame = uiView.Frame;
      CGPoint location = frame.Location;
      NSView superview = this.targetView.Superview;
      // ISSUE: reference to a compiler-generated method
      nfloat y1 = uiView.ConvertPointToView(location, superview).Y;
      frame = firstResponder.Frame;
      nfloat height = frame.Height;
      nfloat y2 = y1 + height - nfloat;
      if (!(y2 > (nfloat) 0))
        return;
      this.setContentOffset(new CGPoint((nfloat) 0, y2));
    }

    private void OnKeyboardShown(object sender, UIKeyboardEventArgs args)
    {
      this.lastKeyboardRect = args.FrameEnd;
      this.UpdateInsets();
    }

    private void OnKeyboardHidden(object sender, UIKeyboardEventArgs args)
    {
      this.setInsetAction(new UIEdgeInsets((nfloat) 0, (nfloat) 0, (nfloat) 0, (nfloat) 0));
      this.lastKeyboardRect = CGRect.Empty;
    }

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.disposed = true;
      KeyboardObserver.KeyboardWillShow -= new EventHandler<UIKeyboardEventArgs>(this.OnKeyboardShown);
      KeyboardObserver.KeyboardWillHide -= new EventHandler<UIKeyboardEventArgs>(this.OnKeyboardHidden);
    }
  }
}
