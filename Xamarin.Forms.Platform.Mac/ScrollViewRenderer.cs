using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ScrollViewRenderer : UIScrollView, IVisualElementRenderer, IDisposable, IRegisterable
  {
    private VisualElementPackager packager;
    private VisualElementTracker tracker;
    private EventTracker events;
    private KeyboardInsetTracker insetTracker;
    private ScrollToRequestedEventArgs requestedScroll;
    private CGRect previousFrame;

    public VisualElement Element { get; private set; }

    protected IScrollViewController Controller
    {
      get
      {
        return (IScrollViewController) this.Element;
      }
    }

    private ScrollView scrollView
    {
      get
      {
        return this.Element as ScrollView;
      }
    }

    public UIView NativeView
    {
      get
      {
        return (UIView) this;
      }
    }

    public UIViewController ViewController
    {
      get
      {
        return (UIViewController) null;
      }
    }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    public ScrollViewRenderer()
      : base(CGRect.Empty)
    {
      this.ScrollAnimationEnded += new EventHandler(this.HandleScrollAnimationEnded);
      this.Scrolled += new EventHandler(this.HandleScrolled);
    }

    private void HandleScrolled(object sender, EventArgs e)
    {
      this.UpdateScrollPosition();
    }

    private void HandleScrollAnimationEnded(object sender, EventArgs e)
    {
      this.Controller.SendScrollFinished();
    }

    private void UpdateScrollPosition()
    {
      if (this.scrollView == null)
        return;
      this.Controller.SetScrolledPosition((double) this.ContentOffset.X, (double) this.ContentOffset.Y);
    }

    private void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
    {
      if (this.Superview == null)
      {
        this.requestedScroll = e;
      }
      else
      {
        if (e.Mode == ScrollToMode.Position)
        {
          // ISSUE: reference to a compiler-generated method
          this.SetContentOffset(new CGPoint((nfloat) e.ScrollX, (nfloat) e.ScrollY), e.ShouldAnimate);
        }
        else
        {
          Point positionForElement = this.Controller.GetScrollPositionForElement(e.Element as VisualElement, e.Position);
          if (this.scrollView.Orientation == ScrollOrientation.Horizontal)
          {
            // ISSUE: reference to a compiler-generated method
            this.SetContentOffset(new CGPoint((nfloat) positionForElement.X, this.ContentOffset.Y), e.ShouldAnimate);
          }
          else
          {
            // ISSUE: reference to a compiler-generated method
            this.SetContentOffset(new CGPoint(this.ContentOffset.X, (nfloat) positionForElement.Y), e.ShouldAnimate);
          }
        }
        if (e.ShouldAnimate)
          return;
        this.Controller.SendScrollFinished();
      }
    }

    public override void LayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.LayoutSubviews();
      if (this.requestedScroll != null && this.Superview != null)
      {
        ScrollToRequestedEventArgs e = this.requestedScroll;
        this.requestedScroll = (ScrollToRequestedEventArgs) null;
        this.OnScrollToRequested((object) this, e);
      }
      if (!(this.previousFrame != this.Frame))
        return;
      this.previousFrame = this.Frame;
      KeyboardInsetTracker keyboardInsetTracker = this.insetTracker;
      if (keyboardInsetTracker == null)
        return;
      keyboardInsetTracker.UpdateInsets();
    }

    public void SetElement(VisualElement element)
    {
      this.requestedScroll = (ScrollToRequestedEventArgs) null;
      VisualElement element1 = this.Element;
      this.Element = element;
      if (element1 != null)
      {
        element1.remove_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
        ((IScrollViewController) element1).remove_ScrollToRequested(new EventHandler<ScrollToRequestedEventArgs>(this.OnScrollToRequested));
      }
      if (element == null)
        return;
      element.add_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
      ((IScrollViewController) element).add_ScrollToRequested(new EventHandler<ScrollToRequestedEventArgs>(this.OnScrollToRequested));
      if (this.packager == null)
      {
        this.DelaysContentTouches = true;
        this.packager = new VisualElementPackager((IVisualElementRenderer) this);
        this.packager.Load();
        this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
        this.tracker.NativeControlUpdated += new EventHandler(this.OnNativeControlUpdated);
        this.events = new EventTracker((IVisualElementRenderer) this);
        this.events.LoadEvents((UIView) this);
        this.insetTracker = new KeyboardInsetTracker((UIView) this, (Func<UIWindow>) (() => this.Window), (Action<UIEdgeInsets>) (insets => this.ContentInset = this.ScrollIndicatorInsets = insets), (Action<CGPoint>) (point =>
        {
          CGPoint contentOffset = this.ContentOffset;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          CGPoint& local = @contentOffset;
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          (^local).Y = (^local).Y + point.Y;
          // ISSUE: reference to a compiler-generated method
          this.SetContentOffset(contentOffset, true);
        }));
      }
      this.UpdateContentSize();
      this.UpdateBackgroundColor();
      this.OnElementChanged(new VisualElementChangedEventArgs(element1, element));
      if (element == null)
        return;
      Forms.SendViewInitialized(element, (UIView) this);
    }

    private void OnNativeControlUpdated(object sender, EventArgs eventArgs)
    {
      this.ContentSize = this.Bounds.Size;
      this.UpdateContentSize();
    }

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, 44.0, 44.0);
    }

    public void SetElementSize(Size size)
    {
      this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
    }

    protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
    {
      // ISSUE: reference to a compiler-generated field
      EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, e);
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == ScrollView.ContentSizeProperty.PropertyName)
      {
        this.UpdateContentSize();
      }
      else
      {
        if (!(e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName))
          return;
        this.UpdateBackgroundColor();
      }
    }

    private void UpdateBackgroundColor()
    {
      this.BackgroundColor = ColorExtensions.ToUIColor(this.Element.BackgroundColor, Color.Transparent);
    }

    private void UpdateContentSize()
    {
      CGSize cgSize = SizeExtensions.ToSizeF(((ScrollView) this.Element).ContentSize);
      if (cgSize.IsEmpty)
        return;
      this.ContentSize = cgSize;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.packager == null)
          return;
        this.SetElement((VisualElement) null);
        this.packager.Dispose();
        this.packager = (VisualElementPackager) null;
        this.tracker.NativeControlUpdated -= new EventHandler(this.OnNativeControlUpdated);
        this.tracker.Dispose();
        this.tracker = (VisualElementTracker) null;
        this.events.Dispose();
        this.events = (EventTracker) null;
        this.insetTracker.Dispose();
        this.insetTracker = (KeyboardInsetTracker) null;
        this.ScrollAnimationEnded -= new EventHandler(this.HandleScrollAnimationEnded);
        this.Scrolled -= new EventHandler(this.HandleScrolled);
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }
  }
}
