using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class TabbedRenderer : UITabBarController, IVisualElementRenderer, IDisposable, IRegisterable
  {
    private bool loaded;
    private Xamarin.Forms.Size queuedSize;

    public VisualElement Element { get; private set; }

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

    public override UIViewController SelectedViewController
    {
      get
      {
        return base.SelectedViewController;
      }
      set
      {
        base.SelectedViewController = value;
        this.UpdateCurrentPage();
      }
    }

    protected TabbedPage Tabbed
    {
      get
      {
        return (TabbedPage) this.Element;
      }
    }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Tabbed.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
        this.Tabbed.remove_PagesChanged(new NotifyCollectionChangedEventHandler(this.OnPagesChanged));
        this.FinishedCustomizingViewControllers -= new EventHandler<UITabBarCustomizeChangeEventArgs>(this.HandleFinishedCustomizingViewControllers);
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    public void SetElement(VisualElement element)
    {
      VisualElement element1 = this.Element;
      this.Element = element;
      this.FinishedCustomizingViewControllers += new EventHandler<UITabBarCustomizeChangeEventArgs>(this.HandleFinishedCustomizingViewControllers);
      this.Tabbed.add_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
      this.Tabbed.add_PagesChanged(new NotifyCollectionChangedEventHandler(this.OnPagesChanged));
      this.OnElementChanged(new VisualElementChangedEventArgs(element1, element));
      this.OnPagesChanged((object) null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      if (element != null)
        Forms.SendViewInitialized(element, this.NativeView);
      this.CustomizableViewControllers = (UIViewController[]) null;
    }

    private void HandleFinishedCustomizingViewControllers(object sender, UITabBarCustomizeChangeEventArgs e)
    {
      if (!e.Changed)
        return;
      this.UpdateChildrenOrderIndex(e.ViewControllers);
    }

    private void UpdateChildrenOrderIndex(UIViewController[] viewControllers)
    {
      for (int index = 0; index < viewControllers.Length; ++index)
      {
        int result = -1;
        if (int.TryParse(viewControllers[index].TabBarItem.Tag.ToString(), out result))
          MultiPage<Page>.SetIndex((Page) this.Tabbed.get_InternalChildren()[result], index);
      }
    }

    public void SetElementSize(Xamarin.Forms.Size size)
    {
      if (this.loaded)
        this.Element.Layout(new Xamarin.Forms.Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
      else
        this.queuedSize = size;
    }

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, -1.0, -1.0);
    }

    private void UpdateCurrentPage()
    {
      ((MultiPage<Page>) this.Element).CurrentPage = !(this.SelectedIndex >= (nint) 0) || !(this.SelectedIndex < (nint) this.Tabbed.get_InternalChildren().Count) ? (Page) null : this.Tabbed.GetPageByIndex((int) this.SelectedIndex);
    }

    public override void ViewDidLoad()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLoad();
      if (Forms.IsiOS7OrNewer)
        return;
      this.WantsFullScreenLayout = false;
    }

    public override void ViewDidLayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLayoutSubviews();
      if (this.Element == null)
        return;
      if (!this.Element.Bounds.IsEmpty)
        this.View.Frame = (CGRect) new RectangleF((float) this.Element.X, (float) this.Element.Y, (float) this.Element.Width, (float) this.Element.Height);
      CGRect frame1 = this.View.Frame;
      CGRect frame2 = this.TabBar.Frame;
      ((Page) this.Element).ContainerArea = new Xamarin.Forms.Rectangle(0.0, 0.0, (double) frame1.Width, (double) (frame1.Height - frame2.Height));
      if (!this.queuedSize.IsZero)
      {
        this.Element.Layout(new Xamarin.Forms.Rectangle(this.Element.X, this.Element.Y, this.queuedSize.Width, this.queuedSize.Height));
        this.queuedSize = Xamarin.Forms.Size.Zero;
      }
      this.loaded = true;
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

    public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
    {
      // ISSUE: reference to a compiler-generated method
      base.DidRotate(fromInterfaceOrientation);
      // ISSUE: reference to a compiler-generated method
      this.View.SetNeedsLayout();
    }

    protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
    {
      // ISSUE: reference to a compiler-generated field
      EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, e);
    }

    private UIViewController GetViewController(Page page)
    {
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
      if (renderer == null)
        return (UIViewController) null;
      return renderer.ViewController;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "CurrentPage"))
        return;
      Page currentPage = this.Tabbed.CurrentPage;
      if (currentPage == null)
        return;
      UIViewController viewController = this.GetViewController(currentPage);
      if (viewController == null)
        return;
      this.SelectedViewController = viewController;
    }

    private void SetupPage(Page page, int index)
    {
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
      if (renderer == null)
      {
        renderer = Platform.CreateRenderer((VisualElement) page);
        Platform.SetRenderer((VisualElement) page, renderer);
      }
      page.add_PropertyChanged(new PropertyChangedEventHandler(this.OnPagePropertyChanged));
      UIImage image = (UIImage) null;
      if (page.Icon != null)
        image = new UIImage((string) page.Icon);
      renderer.ViewController.TabBarItem = new UITabBarItem(page.Title, image, (nint) 0);
      if (image != null)
        image.Dispose();
      renderer.ViewController.TabBarItem.Tag = (nint) index;
    }

    private void SetControllers()
    {
      List<UIViewController> list = new List<UIViewController>();
      for (int index = 0; index < this.Element.get_LogicalChildren().Count; ++index)
      {
        VisualElement bindable = this.Element.get_LogicalChildren()[index] as VisualElement;
        if (bindable != null && Platform.GetRenderer(bindable) != null)
          list.Add(Platform.GetRenderer(bindable).ViewController);
      }
      this.ViewControllers = list.ToArray();
    }

    private void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Page.TitleProperty.PropertyName)
      {
        Page page = (Page) sender;
        IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
        if (renderer == null || renderer.ViewController.TabBarItem == null)
          return;
        renderer.ViewController.TabBarItem.Title = page.Title;
      }
      else
      {
        if (!(e.PropertyName == Page.IconProperty.PropertyName))
          return;
        Page page = (Page) sender;
        IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
        if (renderer == null || renderer.ViewController.TabBarItem == null)
          return;
        UIImage image = (UIImage) null;
        if (!string.IsNullOrEmpty((string) page.Icon))
          image = new UIImage((string) page.Icon);
        renderer.ViewController.TabBarItem = new UITabBarItem(page.Title, image, (nint) 0);
        if (image == null)
          return;
        image.Dispose();
      }
    }

    private void TeardownPage(Page page, int index)
    {
      page.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnPagePropertyChanged));
      Platform.SetRenderer((VisualElement) page, (IVisualElementRenderer) null);
    }

    private void Reset()
    {
      int num = 0;
      foreach (Page page in (IEnumerable<Page>) this.Tabbed.get_Children())
        this.SetupPage(page, num++);
    }

    private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      int num = (int) NotifyCollectionChangedEventArgsExtensions.Apply(e, (Action<object, int, bool>) ((o, i, c) => this.SetupPage((Page) o, i)), (Action<object, int>) ((o, i) => this.TeardownPage((Page) o, i)), new Action(this.Reset));
      this.SetControllers();
      UIViewController uiViewController = (UIViewController) null;
      if (this.Tabbed.CurrentPage != null)
        uiViewController = this.GetViewController(this.Tabbed.CurrentPage);
      if (uiViewController == null || uiViewController == base.SelectedViewController)
        return;
      base.SelectedViewController = uiViewController;
    }
  }
}
