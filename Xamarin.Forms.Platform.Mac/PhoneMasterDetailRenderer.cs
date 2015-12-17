using CoreGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class PhoneMasterDetailRenderer : UIViewController, IVisualElementRenderer, IDisposable, IRegisterable
  {
    private VisualElementTracker tracker;
    private EventTracker events;
    private UIViewController masterController;
    private UIViewController detailController;
    private UIView clickOffView;
    private UIPanGestureRecognizer panGesture;
    private UIGestureRecognizer tapGesture;
    private bool disposed;
    private bool presented;

    public VisualElement Element { get; private set; }

    private bool Presented
    {
      get
      {
        return this.presented;
      }
      set
      {
        if (this.presented == value)
          return;
        this.presented = value;
        this.LayoutChildren(true);
        if (value)
          this.AddClickOffView();
        else
          this.RemoveClickOffView();
        this.Element.SetValueFromRenderer(MasterDetailPage.IsPresentedProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public UIView NativeView
    {
      get
      {
        return this.View;
      }
    }

    public UIViewController ViewController
    {
      get
      {
        return (UIViewController) this;
      }
    }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    public PhoneMasterDetailRenderer()
    {
      if (Forms.IsiOS7OrNewer)
        return;
      this.WantsFullScreenLayout = true;
    }

    public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
    {
      if (!((MasterDetailPage) this.Element).ShouldShowSplitMode && this.presented)
        this.Presented = false;
      // ISSUE: reference to a compiler-generated method
      base.WillRotate(toInterfaceOrientation, duration);
    }

    public void SetElement(VisualElement element)
    {
      VisualElement element1 = this.Element;
      this.Element = element;
      this.Element.add_SizeChanged(new EventHandler(this.PageOnSizeChanged));
      this.masterController = (UIViewController) new PhoneMasterDetailRenderer.ChildViewController();
      this.detailController = (UIViewController) new PhoneMasterDetailRenderer.ChildViewController();
      this.clickOffView = new UIView();
      this.clickOffView.BackgroundColor = ColorExtensions.ToUIColor(new Color(0.0, 0.0, 0.0, 0.0));
      this.Presented = ((MasterDetailPage) this.Element).IsPresented;
      this.OnElementChanged(new VisualElementChangedEventArgs(element1, element));
      if (element == null)
        return;
      Forms.SendViewInitialized(element, this.NativeView);
    }

    public void SetElementSize(Size size)
    {
      this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
      {
        this.Element.remove_SizeChanged(new EventHandler(this.PageOnSizeChanged));
        this.Element.remove_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
        if (this.tracker != null)
        {
          this.tracker.Dispose();
          this.tracker = (VisualElementTracker) null;
        }
        if (this.events != null)
        {
          this.events.Dispose();
          this.events = (EventTracker) null;
        }
        if (this.tapGesture != null)
        {
          if (this.clickOffView != null && Enumerable.Contains<UIGestureRecognizer>((IEnumerable<UIGestureRecognizer>) this.clickOffView.GestureRecognizers, (UIGestureRecognizer) this.panGesture))
          {
            ArrayExtensions.Remove<UIGestureRecognizer>(this.clickOffView.GestureRecognizers, this.tapGesture);
            this.clickOffView.Dispose();
          }
          this.tapGesture.Dispose();
        }
        if (this.panGesture != null)
        {
          if (this.View != null && Enumerable.Contains<UIGestureRecognizer>((IEnumerable<UIGestureRecognizer>) this.View.GestureRecognizers, (UIGestureRecognizer) this.panGesture))
            ArrayExtensions.Remove<UIGestureRecognizer>(this.View.GestureRecognizers, (UIGestureRecognizer) this.panGesture);
          this.panGesture.Dispose();
        }
        this.EmptyContainers();
        this.disposed = true;
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    private void PageOnSizeChanged(object sender, EventArgs eventArgs)
    {
      this.LayoutChildren(false);
    }

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, -1.0, -1.0);
    }

    private void AddClickOffView()
    {
      this.View.Add(this.clickOffView);
      this.clickOffView.Frame = this.detailController.View.Frame;
    }

    private void RemoveClickOffView()
    {
      // ISSUE: reference to a compiler-generated method
      this.clickOffView.RemoveFromSuperview();
    }

    public override void ViewDidLoad()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLoad();
      this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
      this.events = new EventTracker((IVisualElementRenderer) this);
      this.events.LoadEvents(this.View);
      this.Element.add_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
      this.tapGesture = (UIGestureRecognizer) new UITapGestureRecognizer((Action) (() =>
      {
        if (!this.Presented)
          return;
        this.Presented = false;
      }));
      // ISSUE: reference to a compiler-generated method
      this.clickOffView.AddGestureRecognizer(this.tapGesture);
      this.PackContainers();
      this.UpdateMasterDetailContainers();
      this.UpdateBackground();
      this.UpdatePanGesture();
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
      if (e.PropertyName == "Master" || e.PropertyName == "Detail")
        this.UpdateMasterDetailContainers();
      else if (e.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName)
        this.Presented = ((MasterDetailPage) this.Element).IsPresented;
      else if (e.PropertyName == MasterDetailPage.IsGestureEnabledProperty.PropertyName)
        this.UpdatePanGesture();
      else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
      {
        this.UpdateBackground();
      }
      else
      {
        if (!(e.PropertyName == Page.BackgroundImageProperty.PropertyName))
          return;
        this.UpdateBackground();
      }
    }

    public override void ViewDidLayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLayoutSubviews();
      this.LayoutChildren(false);
    }

    private void PackContainers()
    {
      this.detailController.View.BackgroundColor = new UIColor((nfloat) 1, (nfloat) 1, (nfloat) 1, (nfloat) 1);
      // ISSUE: reference to a compiler-generated method
      this.View.AddSubview(this.masterController.View);
      // ISSUE: reference to a compiler-generated method
      this.View.AddSubview(this.detailController.View);
      // ISSUE: reference to a compiler-generated method
      this.AddChildViewController(this.masterController);
      // ISSUE: reference to a compiler-generated method
      this.AddChildViewController(this.detailController);
    }

    private void EmptyContainers()
    {
      foreach (UIView uiView in Enumerable.Concat<UIView>((IEnumerable<UIView>) this.detailController.View.Subviews, (IEnumerable<UIView>) this.masterController.View.Subviews))
      {
        // ISSUE: reference to a compiler-generated method
        uiView.RemoveFromSuperview();
      }
      foreach (UIViewController uiViewController in Enumerable.Concat<UIViewController>((IEnumerable<UIViewController>) this.detailController.ChildViewControllers, (IEnumerable<UIViewController>) this.masterController.ChildViewControllers))
      {
        // ISSUE: reference to a compiler-generated method
        uiViewController.RemoveFromParentViewController();
      }
    }

    private void UpdateMasterDetailContainers()
    {
      ((MasterDetailPage) this.Element).Master.remove_PropertyChanged(new PropertyChangedEventHandler(this.HandleMasterPropertyChanged));
      this.EmptyContainers();
      if (Platform.GetRenderer((VisualElement) ((MasterDetailPage) this.Element).Master) == null)
        Platform.SetRenderer((VisualElement) ((MasterDetailPage) this.Element).Master, Platform.CreateRenderer((VisualElement) ((MasterDetailPage) this.Element).Master));
      if (Platform.GetRenderer((VisualElement) ((MasterDetailPage) this.Element).Detail) == null)
        Platform.SetRenderer((VisualElement) ((MasterDetailPage) this.Element).Detail, Platform.CreateRenderer((VisualElement) ((MasterDetailPage) this.Element).Detail));
      IVisualElementRenderer renderer1 = Platform.GetRenderer((VisualElement) ((MasterDetailPage) this.Element).Master);
      IVisualElementRenderer renderer2 = Platform.GetRenderer((VisualElement) ((MasterDetailPage) this.Element).Detail);
      ((MasterDetailPage) this.Element).Master.add_PropertyChanged(new PropertyChangedEventHandler(this.HandleMasterPropertyChanged));
      // ISSUE: reference to a compiler-generated method
      this.masterController.View.AddSubview(renderer1.NativeView);
      // ISSUE: reference to a compiler-generated method
      this.masterController.AddChildViewController(renderer1.ViewController);
      // ISSUE: reference to a compiler-generated method
      this.detailController.View.AddSubview(renderer2.NativeView);
      // ISSUE: reference to a compiler-generated method
      this.detailController.AddChildViewController(renderer2.ViewController);
    }

    private void HandleMasterPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == Page.IconProperty.PropertyName) && !(e.PropertyName == Page.TitleProperty.PropertyName))
        return;
      MessagingCenter.Send<IVisualElementRenderer>((IVisualElementRenderer) this, "Xamarin.UpdateToolbarButtons");
    }

    private void LayoutChildren(bool animated)
    {
      CGRect cgRect1 = RectangleExtensions.ToRectangleF(this.Element.Bounds);
      CGRect cgRect2 = cgRect1;
      cgRect2.Width = (nfloat) ((int) (Math.Min((double) cgRect2.Width, (double) cgRect2.Height) * 0.8));
      this.masterController.View.Frame = cgRect2;
      CGRect cgRect3 = cgRect1;
      if (this.Presented)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        CGRect& local = @cgRect3;
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        (^local).X = (^local).X + cgRect2.Width;
      }
      if (animated)
      {
        UIView.BeginAnimations("Flyout");
        this.detailController.View.Frame = cgRect3;
        // ISSUE: reference to a compiler-generated method
        UIView.SetAnimationCurve(UIViewAnimationCurve.EaseOut);
        // ISSUE: reference to a compiler-generated method
        UIView.SetAnimationDuration(250.0);
        // ISSUE: reference to a compiler-generated method
        UIView.CommitAnimations();
      }
      else
        this.detailController.View.Frame = cgRect3;
      ((MasterDetailPage) this.Element).MasterBounds = new Rectangle(0.0, 0.0, (double) cgRect2.Width, (double) cgRect2.Height);
      ((MasterDetailPage) this.Element).DetailBounds = new Rectangle(0.0, 0.0, (double) cgRect1.Width, (double) cgRect1.Height);
      if (!this.Presented)
        return;
      this.clickOffView.Frame = this.detailController.View.Frame;
    }

    private void UpdateBackground()
    {
      if (!string.IsNullOrEmpty(((Page) this.Element).BackgroundImage))
      {
        // ISSUE: reference to a compiler-generated method
        // ISSUE: reference to a compiler-generated method
        this.View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle(((Page) this.Element).BackgroundImage));
      }
      else if (this.Element.BackgroundColor == Color.Default)
        this.View.BackgroundColor = UIColor.White;
      else
        this.View.BackgroundColor = ColorExtensions.ToUIColor(this.Element.BackgroundColor);
    }

    private void UpdatePanGesture()
    {
      if (!((MasterDetailPage) this.Element).IsGestureEnabled)
      {
        if (this.panGesture == null)
          return;
        // ISSUE: reference to a compiler-generated method
        this.View.RemoveGestureRecognizer((UIGestureRecognizer) this.panGesture);
      }
      else
      {
        if (this.panGesture != null)
          return;
        UITouchEventArgs uiTouchEventArgs = (UITouchEventArgs) ((g, t) => !(t.View is UISlider));
        CGPoint center = new CGPoint();
        this.panGesture = new UIPanGestureRecognizer((Action<UIPanGestureRecognizer>) (g =>
        {
          long num1 = (long) (g.State - 1L);
          long num2 = 2;
          if ((ulong) num1 > (ulong) num2)
            return;
          switch ((uint) num1)
          {
            case 0:
              UIPanGestureRecognizer gestureRecognizer1 = g;
              UIView view1 = gestureRecognizer1.View;
              // ISSUE: reference to a compiler-generated method
              center = gestureRecognizer1.LocationInView(view1);
              break;
            case 1:
              UIPanGestureRecognizer gestureRecognizer2 = g;
              UIView view2 = gestureRecognizer2.View;
              // ISSUE: reference to a compiler-generated method
              nfloat nfloat = gestureRecognizer2.LocationInView(view2).X - center.X;
              UIView view3 = this.detailController.View;
              CGRect frame1 = view3.Frame;
              frame1.X = !this.Presented ? (nfloat) Math.Min((double) this.masterController.View.Frame.Width, Math.Max(0.0, (double) nfloat)) : (nfloat) Math.Max(0.0, (double) this.masterController.View.Frame.Width + Math.Min(0.0, (double) nfloat));
              CGRect cgRect = frame1;
              view3.Frame = cgRect;
              break;
            case 2:
              CGRect frame2 = this.detailController.View.Frame;
              CGRect frame3 = this.masterController.View.Frame;
              if (this.Presented)
              {
                if ((double) frame2.X < (double) frame3.Width * 0.75)
                {
                  this.Presented = false;
                  break;
                }
                this.LayoutChildren(true);
                break;
              }
              if ((double) frame2.X > (double) frame3.Width * 0.25)
              {
                this.Presented = true;
                break;
              }
              this.LayoutChildren(true);
              break;
          }
        }));
        this.panGesture.ShouldReceiveTouch = uiTouchEventArgs;
        this.panGesture.MaximumNumberOfTouches = (nuint) (byte) 2;
        // ISSUE: reference to a compiler-generated method
        this.View.AddGestureRecognizer((UIGestureRecognizer) this.panGesture);
      }
    }

    public override void ViewDidAppear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidAppear(animated);
      ((Page) this.Element).SendAppearing();
    }

    public override void ViewDidDisappear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidDisappear(animated);
      ((Page) this.Element).SendDisappearing();
    }

    private class ChildViewController : UIViewController
    {
      public override void ViewDidLayoutSubviews()
      {
        foreach (UIViewController uiViewController in this.ChildViewControllers)
          uiViewController.View.Frame = this.View.Bounds;
      }
    }
  }
}
