using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class PageRenderer : UIViewController, IVisualElementRenderer, IDisposable, IRegisterable
  {
    private VisualElementPackager packager;
    private VisualElementTracker tracker;
    private EventTracker events;
    private bool disposed;
    private bool appeared;

    public VisualElement Element { get; private set; }

    public UIView NativeView
    {
      get
      {
        if (!this.disposed)
          return this.View;
        return (UIView) null;
      }
    }

    public UIViewController ViewController
    {
      get
      {
        if (!this.disposed)
          return (UIViewController) this;
        return (UIViewController) null;
      }
    }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    public PageRenderer()
    {
      if (Forms.IsiOS7OrNewer)
        return;
      this.WantsFullScreenLayout = true;
    }

    public void SetElement(VisualElement element)
    {
      VisualElement element1 = this.Element;
      this.Element = element;
      this.UpdateTitle();
      this.OnElementChanged(new VisualElementChangedEventArgs(element1, element));
      if (element == null)
        return;
      Forms.SendViewInitialized(element, this.NativeView);
    }

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, -1.0, -1.0);
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

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
      {
        this.Element.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnHandlePropertyChanged));
        Platform.SetRenderer(this.Element, (IVisualElementRenderer) null);
        if (this.appeared)
          ((Page) this.Element).SendDisappearing();
        this.appeared = false;
        if (this.events != null)
        {
          this.events.Dispose();
          this.events = (EventTracker) null;
        }
        if (this.packager != null)
        {
          this.packager.Dispose();
          this.packager = (VisualElementPackager) null;
        }
        if (this.tracker != null)
        {
          this.tracker.Dispose();
          this.tracker = (VisualElementTracker) null;
        }
        this.Element = (VisualElement) null;
        this.disposed = true;
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    private IEnumerable<UIView> ViewAndSuperviewsOfView(UIView view)
    {
      for (; view != null; view = view.Superview)
        yield return view;
    }

    public override void ViewDidLoad()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLoad();
      // ISSUE: reference to a compiler-generated method
      UITapGestureRecognizer gestureRecognizer1 = new UITapGestureRecognizer((Action<UITapGestureRecognizer>) (a => UIView_UITextField.EndEditing(this.View, true)));
      gestureRecognizer1.ShouldRecognizeSimultaneously = (UIGesturesProbe) ((recognizer, gestureRecognizer) => true);
      gestureRecognizer1.ShouldReceiveTouch = new UITouchEventArgs(this.OnShouldReceiveTouch);
      UITapGestureRecognizer gestureRecognizer2 = gestureRecognizer1;
      int num1;
      bool flag = (num1 = 0) != 0;
      gestureRecognizer2.DelaysTouchesEnded = num1 != 0;
      int num2 = flag ? 1 : 0;
      gestureRecognizer2.DelaysTouchesBegan = num2 != 0;
      // ISSUE: reference to a compiler-generated method
      this.View.AddGestureRecognizer((UIGestureRecognizer) gestureRecognizer1);
      this.UpdateBackground();
      this.packager = new VisualElementPackager((IVisualElementRenderer) this);
      this.packager.Load();
      this.Element.add_PropertyChanged(new PropertyChangedEventHandler(this.OnHandlePropertyChanged));
      this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
      this.events = new EventTracker((IVisualElementRenderer) this);
      this.events.LoadEvents(this.View);
      Forms.SendViewInitialized(this.Element, this.View);
    }

    private bool OnShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
    {
      IEnumerable<UIView> source = this.ViewAndSuperviewsOfView(touch.View);
      Func<UIView, bool> func = (Func<UIView, bool>) (v =>
      {
        if (!(v is UITableView) && !(v is UITableViewCell))
          return v.CanBecomeFirstResponder;
        return true;
      });
      Func<UIView, bool> predicate;
      return !Enumerable.Any<UIView>(source, predicate);
    }

    private void OnHandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
        this.UpdateBackground();
      else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
      {
        this.UpdateBackground();
      }
      else
      {
        if (!(e.PropertyName == Page.TitleProperty.PropertyName))
          return;
        this.UpdateTitle();
      }
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

    private void UpdateTitle()
    {
      if (string.IsNullOrWhiteSpace(((Page) this.Element).Title))
        return;
      this.Title = ((Page) this.Element).Title;
    }

    public override void ViewDidAppear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidAppear(animated);
      if (this.appeared || this.disposed)
        return;
      this.appeared = true;
      ((Page) this.Element).SendAppearing();
    }

    public override void ViewDidDisappear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidDisappear(animated);
      if (!this.appeared || this.disposed)
        return;
      this.appeared = false;
      ((Page) this.Element).SendDisappearing();
    }

    public override void ViewWillDisappear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewWillDisappear(animated);
      if (this.View.Window == null)
        return;
      // ISSUE: reference to a compiler-generated method
      UIView_UITextField.EndEditing((UIView) this.View.Window, true);
    }
  }
}
