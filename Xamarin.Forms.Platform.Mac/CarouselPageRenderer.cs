using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class CarouselPageRenderer : UIViewController, IVisualElementRenderer, IDisposable, IRegisterable
  {
    private UIScrollView scrollView;
    private Dictionary<Page, UIView> containerMap;
    private VisualElementTracker tracker;
    private EventTracker events;
    private bool disposed;
    private bool appeared;
    private bool ignoreNativeScrolling;

    public VisualElement Element { get; private set; }

    protected int SelectedIndex
    {
      get
      {
        return (int) (this.scrollView.ContentOffset.X / this.scrollView.Frame.Width);
      }
      set
      {
        this.ScrollToPage(value, true);
      }
    }

    protected CarouselPage Carousel
    {
      get
      {
        return (CarouselPage) this.Element;
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

    public CarouselPageRenderer()
    {
      if (Forms.IsiOS7OrNewer)
        return;
      this.WantsFullScreenLayout = true;
    }

    public void SetElement(VisualElement element)
    {
      VisualElement element1 = this.Element;
      this.Element = element;
      this.containerMap = new Dictionary<Page, UIView>();
      this.OnElementChanged(new VisualElementChangedEventArgs(element1, element));
      if (element == null)
        return;
      Forms.SendViewInitialized(element, this.NativeView);
    }

    public void SetElementSize(Size size)
    {
      this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
    }

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, -1.0, -1.0);
    }

    public override void ViewDidLoad()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLoad();
      this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
      this.events = new EventTracker((IVisualElementRenderer) this);
      this.events.LoadEvents(this.View);
      this.scrollView = new UIScrollView()
      {
        ShowsHorizontalScrollIndicator = false
      };
      this.scrollView.DecelerationEnded += new EventHandler(this.OnDecelerationEnded);
      this.View.Add((UIView) this.scrollView);
      for (int index = 0; index < this.Element.get_LogicalChildren().Count; ++index)
      {
        ContentPage page = this.Element.get_LogicalChildren()[index] as ContentPage;
        if (page != null)
          this.InsertPage(page, index);
      }
      this.PositionChildren();
      this.Carousel.add_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
      this.Carousel.add_PagesChanged(new NotifyCollectionChangedEventHandler(this.OnPagesChanged));
    }

    protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
    {
      // ISSUE: reference to a compiler-generated field
      EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, e);
    }

    private void OnDecelerationEnded(object sender, EventArgs eventArgs)
    {
      if (this.ignoreNativeScrolling || this.SelectedIndex >= this.Element.get_LogicalChildren().Count)
        return;
      this.Carousel.CurrentPage = (ContentPage) this.Element.get_LogicalChildren()[this.SelectedIndex];
    }

    private void Clear()
    {
      foreach (KeyValuePair<Page, UIView> keyValuePair in this.containerMap)
      {
        // ISSUE: reference to a compiler-generated method
        keyValuePair.Value.RemoveFromSuperview();
        IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) keyValuePair.Key);
        if (renderer != null)
        {
          // ISSUE: reference to a compiler-generated method
          renderer.ViewController.RemoveFromParentViewController();
          // ISSUE: reference to a compiler-generated method
          renderer.NativeView.RemoveFromSuperview();
          Platform.SetRenderer((VisualElement) keyValuePair.Key, (IVisualElementRenderer) null);
        }
      }
      this.containerMap.Clear();
    }

    private void Reset()
    {
      this.Clear();
      for (int index = 0; index < this.Element.get_LogicalChildren().Count; ++index)
      {
        ContentPage page = this.Element.get_LogicalChildren()[index] as ContentPage;
        if (page != null)
          this.InsertPage(page, index);
      }
    }

    private void InsertPage(ContentPage page, int index)
    {
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
      if (renderer == null)
      {
        renderer = Platform.CreateRenderer((VisualElement) page);
        Platform.SetRenderer((VisualElement) page, renderer);
      }
      UIView view = (UIView) new CarouselPageRenderer.PageContainer((VisualElement) page);
      // ISSUE: reference to a compiler-generated method
      view.AddSubview(renderer.NativeView);
      this.containerMap[(Page) page] = view;
      // ISSUE: reference to a compiler-generated method
      this.AddChildViewController(renderer.ViewController);
      // ISSUE: reference to a compiler-generated method
      this.scrollView.InsertSubview(view, (nint) index);
      if ((index != 0 || this.SelectedIndex != 0) && index >= this.SelectedIndex)
        return;
      this.ScrollToPage(this.SelectedIndex + 1, false);
    }

    private void RemovePage(ContentPage page, int index)
    {
      // ISSUE: reference to a compiler-generated method
      this.containerMap[(Page) page].RemoveFromSuperview();
      this.containerMap.Remove((Page) page);
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
      if (renderer == null)
        return;
      // ISSUE: reference to a compiler-generated method
      renderer.ViewController.RemoveFromParentViewController();
      // ISSUE: reference to a compiler-generated method
      renderer.NativeView.RemoveFromSuperview();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "CurrentPage"))
        return;
      this.UpdateCurrentPage(true);
    }

    private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.ignoreNativeScrolling = true;
      int num1 = (int) NotifyCollectionChangedEventArgsExtensions.Apply(e, (Action<object, int, bool>) ((o, i, c) => this.InsertPage((ContentPage) o, i)), (Action<object, int>) ((o, i) => this.RemovePage((ContentPage) o, i)), new Action(this.Reset));
      this.PositionChildren();
      this.ignoreNativeScrolling = false;
      int num2 = 4;
      if (num1 != num2)
        return;
      int index = this.Carousel.CurrentPage != null ? MultiPage<ContentPage>.GetIndex(this.Carousel.CurrentPage) : 0;
      if (index < 0)
        index = 0;
      this.ScrollToPage(index, true);
    }

    public override void ViewDidLayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLayoutSubviews();
      this.View.Frame = this.View.Superview.Bounds;
      this.scrollView.Frame = this.View.Bounds;
      this.PositionChildren();
      this.UpdateCurrentPage(false);
    }

    public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
    {
      this.ignoreNativeScrolling = true;
    }

    public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
    {
      this.ignoreNativeScrolling = false;
    }

    private void UpdateCurrentPage(bool animated = true)
    {
      ContentPage currentPage = this.Carousel.CurrentPage;
      if (currentPage == null)
        return;
      this.ScrollToPage(MultiPage<ContentPage>.GetIndex(currentPage), animated);
    }

    private void ScrollToPage(int index, bool animated = true)
    {
      if (this.scrollView.ContentOffset.X == (nfloat) index * this.scrollView.Frame.Width)
        return;
      // ISSUE: reference to a compiler-generated method
      this.scrollView.SetContentOffset(new CGPoint((nfloat) index * this.scrollView.Frame.Width, (nfloat) 0), animated);
    }

    private void PositionChildren()
    {
      nfloat x = (nfloat) 0;
      CGRect bounds = this.View.Bounds;
      foreach (Page index in (IEnumerable<ContentPage>) ((MultiPage<ContentPage>) this.Element).get_Children())
      {
        this.containerMap[index].Frame = new CGRect(x, bounds.Y, bounds.Width, bounds.Height);
        x += bounds.Width;
      }
      this.scrollView.PagingEnabled = true;
      this.scrollView.ContentSize = new CGSize(bounds.Width * (nfloat) ((MultiPage<ContentPage>) this.Element).get_Children().Count, bounds.Height);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
      {
        this.Carousel.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
        Platform.SetRenderer(this.Element, (IVisualElementRenderer) null);
        this.Clear();
        if (this.scrollView != null)
        {
          this.scrollView.DecelerationEnded -= new EventHandler(this.OnDecelerationEnded);
          // ISSUE: reference to a compiler-generated method
          this.scrollView.RemoveFromSuperview();
          this.scrollView = (UIScrollView) null;
        }
        if (this.appeared)
        {
          this.appeared = false;
          this.Carousel.SendDisappearing();
        }
        if (this.events != null)
        {
          this.events.Dispose();
          this.events = (EventTracker) null;
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

    public override void ViewDidAppear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidAppear(animated);
      if (this.appeared || this.disposed)
        return;
      this.appeared = true;
      this.Carousel.SendAppearing();
    }

    public override void ViewDidDisappear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidDisappear(animated);
      if (!this.appeared || this.disposed)
        return;
      this.appeared = false;
      this.Carousel.SendDisappearing();
    }

    private class PageContainer : UIView
    {
      public VisualElement Element { get; private set; }

      public PageContainer(VisualElement element)
      {
        this.Element = element;
      }

      public override void LayoutSubviews()
      {
        // ISSUE: reference to a compiler-generated method
        base.LayoutSubviews();
        if (this.Subviews.Length == 0)
          return;
        this.Subviews[0].Frame = new CGRect((nfloat) 0, (nfloat) 0, (nfloat) ((float) this.Element.Width), (nfloat) ((float) this.Element.Height));
      }
    }
  }
}
