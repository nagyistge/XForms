using CoreGraphics;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class NavigationRenderer : UINavigationController, IVisualElementRenderer, IDisposable, IRegisterable
  {
    internal const string UpdateToolbarButtons = "Xamarin.UpdateToolbarButtons";
    private VisualElementTracker tracker;
    private UIToolbar secondaryToolbar;
    private MasterDetailPage parentMasterDetailPage;
    private UIViewController[] removeControllers;
    private bool appeared;
    private bool loaded;
    private bool ignorePopCall;
    private Size queuedSize;
    private Page current;

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

    private Page Current
    {
      get
      {
        return this.current;
      }
      set
      {
        this.current = value;
      }
    }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    public NavigationRenderer()
    {
      MessagingCenter.Subscribe<IVisualElementRenderer>((object) this, "Xamarin.UpdateToolbarButtons", (Action<M0>) (sender =>
      {
        if (!Enumerable.Any<UIViewController>((IEnumerable<UIViewController>) this.ViewControllers))
          return;
        this.UpdateLeftBarButtonItem((NavigationRenderer.ParentingViewController) Enumerable.Last<UIViewController>((IEnumerable<UIViewController>) this.ViewControllers));
      }), (M0) null);
    }

    public void SetElement(VisualElement element)
    {
      VisualElement element1 = this.Element;
      this.Element = element;
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
      if (this.loaded)
        this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
      else
        this.queuedSize = size;
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
      if (disposing)
      {
        MessagingCenter.Unsubscribe<IVisualElementRenderer>((object) this, "Xamarin.UpdateToolbarButtons");
        foreach (NSObject nsObject in this.ViewControllers)
          nsObject.Dispose();
        if (this.tracker != null)
          this.tracker.Dispose();
        // ISSUE: reference to a compiler-generated method
        this.secondaryToolbar.RemoveFromSuperview();
        this.secondaryToolbar.Dispose();
        this.secondaryToolbar = (UIToolbar) null;
        this.parentMasterDetailPage = (MasterDetailPage) null;
        this.Current = (Page) null;
        NavigationPage navigationPage = (NavigationPage) this.Element;
        PropertyChangedEventHandler changedEventHandler = new PropertyChangedEventHandler(this.HandlePropertyChanged);
        navigationPage.remove_PropertyChanged(changedEventHandler);
        EventHandler<NavigationRequestedEventArgs> eventHandler1 = new EventHandler<NavigationRequestedEventArgs>(this.OnPushRequested);
        navigationPage.remove_PushRequested(eventHandler1);
        EventHandler<NavigationRequestedEventArgs> eventHandler2 = new EventHandler<NavigationRequestedEventArgs>(this.OnPopRequested);
        navigationPage.remove_PopRequested(eventHandler2);
        EventHandler<NavigationRequestedEventArgs> eventHandler3 = new EventHandler<NavigationRequestedEventArgs>(this.OnPopToRootRequested);
        navigationPage.remove_PopToRootRequested(eventHandler3);
        EventHandler<NavigationRequestedEventArgs> eventHandler4 = new EventHandler<NavigationRequestedEventArgs>(this.OnRemovedPageRequested);
        navigationPage.remove_RemovePageRequested(eventHandler4);
        EventHandler<NavigationRequestedEventArgs> eventHandler5 = new EventHandler<NavigationRequestedEventArgs>(this.OnInsertPageBeforeRequested);
        navigationPage.remove_InsertPageBeforeRequested(eventHandler5);
        if (this.appeared)
          ((Page) this.Element).SendDisappearing();
        this.appeared = false;
        this.Element = (VisualElement) null;
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    public override void ViewDidLoad()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLoad();
      if (Forms.IsiOS7OrNewer)
        this.NavigationBar.Translucent = false;
      else
        this.WantsFullScreenLayout = false;
      NavigationRenderer.SecondaryToolbar secondaryToolbar = new NavigationRenderer.SecondaryToolbar();
      CGRect cgRect = new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 320, (nfloat) 44);
      secondaryToolbar.Frame = cgRect;
      this.secondaryToolbar = (UIToolbar) secondaryToolbar;
      this.View.Add((UIView) this.secondaryToolbar);
      this.secondaryToolbar.Hidden = true;
      this.FindParentMasterDetail();
      NavigationPage navigationPage = (NavigationPage) this.Element;
      if (navigationPage.CurrentPage == null)
        throw new InvalidOperationException("NavigationPage must have a root Page before being used. Either call PushAsync with a valid Page, or pass a Page to the constructor before usage.");
      navigationPage.add_PushRequested(new EventHandler<NavigationRequestedEventArgs>(this.OnPushRequested));
      navigationPage.add_PopRequested(new EventHandler<NavigationRequestedEventArgs>(this.OnPopRequested));
      navigationPage.add_PopToRootRequested(new EventHandler<NavigationRequestedEventArgs>(this.OnPopToRootRequested));
      navigationPage.add_RemovePageRequested(new EventHandler<NavigationRequestedEventArgs>(this.OnRemovedPageRequested));
      navigationPage.add_InsertPageBeforeRequested(new EventHandler<NavigationRequestedEventArgs>(this.OnInsertPageBeforeRequested));
      this.UpdateTint();
      this.UpdateBarBackgroundColor();
      this.UpdateBarTextColor();
      int num;
      EnumerableExtensions.ForEach<Page>((IEnumerable<M0>) Enumerable.Reverse<Page>((IEnumerable<Page>) navigationPage.get_StackCopy()), (Action<M0>) (async p => num = await this.PushPageAsync(p, false) ? 1 : 0));
      this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
      this.Element.add_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
      this.UpdateToolBarVisible();
      this.UpdateBackgroundColor();
      this.Current = navigationPage.CurrentPage;
    }

    private void OnInsertPageBeforeRequested(object sender, NavigationRequestedEventArgs e)
    {
      this.InsertPageBefore(e.Page, e.BeforePage);
    }

    private void OnRemovedPageRequested(object sender, NavigationRequestedEventArgs e)
    {
      this.RemovePage(e.Page);
    }

    private void OnPopToRootRequested(object sender, NavigationRequestedEventArgs e)
    {
      e.set_Task(this.PopToRootAsync(e.Page, e.Animated));
    }

    private void OnPopRequested(object sender, NavigationRequestedEventArgs e)
    {
      e.set_Task(this.PopViewAsync(e.Page, e.Animated));
    }

    private void OnPushRequested(object sender, NavigationRequestedEventArgs e)
    {
      e.set_Task(this.PushPageAsync(e.Page, e.Animated));
    }

    private void UpdateBackgroundColor()
    {
      this.View.BackgroundColor = ColorExtensions.ToUIColor(this.Element.BackgroundColor == Color.Default ? Color.White : this.Element.BackgroundColor);
    }

    private void FindParentMasterDetail()
    {
      IEnumerable<Page> parentPages = ViewExtensions.GetParentPages((Page) this.Element);
      MasterDetailPage masterDetailPage = Enumerable.FirstOrDefault<MasterDetailPage>(Enumerable.OfType<MasterDetailPage>((IEnumerable) parentPages));
      if (masterDetailPage == null || !Enumerable.Contains<Page>((IEnumerable<Page>) EnumerableExtensions.Append<Page>((IEnumerable<M0>) parentPages, (M0) this.Element), masterDetailPage.Detail))
        return;
      this.parentMasterDetailPage = masterDetailPage;
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == NavigationPage.TintProperty.PropertyName)
        this.UpdateTint();
      if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
        this.UpdateBarBackgroundColor();
      else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
        this.UpdateBarTextColor();
      else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
      {
        this.UpdateBackgroundColor();
      }
      else
      {
        if (!(e.PropertyName == NavigationPage.CurrentPageProperty.PropertyName))
          return;
        this.Current = ((NavigationPage) this.Element).CurrentPage;
      }
    }

    private void UpdateTint()
    {
      Color tint = ((NavigationPage) this.Element).Tint;
      if (Forms.IsiOS7OrNewer)
      {
        this.NavigationBar.BarTintColor = tint == Color.Default ? UINavigationBar.Appearance.BarTintColor : ColorExtensions.ToUIColor(tint);
        if (tint == Color.Default)
          this.NavigationBar.TintColor = UINavigationBar.Appearance.TintColor;
        else
          this.NavigationBar.TintColor = tint.Luminosity > 0.5 ? UIColor.Black : UIColor.White;
      }
      else
        this.NavigationBar.TintColor = tint == Color.Default ? (UIColor) null : ColorExtensions.ToUIColor(tint);
    }

    private void UpdateBarBackgroundColor()
    {
      Color barBackgroundColor = ((NavigationPage) this.Element).BarBackgroundColor;
      if (Forms.IsiOS7OrNewer)
        this.NavigationBar.BarTintColor = barBackgroundColor == Color.Default ? UINavigationBar.Appearance.BarTintColor : ColorExtensions.ToUIColor(barBackgroundColor);
      else
        this.NavigationBar.TintColor = barBackgroundColor == Color.Default ? UINavigationBar.Appearance.TintColor : ColorExtensions.ToUIColor(barBackgroundColor);
    }

    private void UpdateBarTextColor()
    {
      Color barTextColor = ((NavigationPage) this.Element).BarTextColor;
      UITextAttributes titleTextAttributes = UINavigationBar.Appearance.GetTitleTextAttributes();
      if (barTextColor == Color.Default)
      {
        if (this.NavigationBar.TitleTextAttributes != null)
          this.NavigationBar.TitleTextAttributes = new UIStringAttributes()
          {
            ForegroundColor = titleTextAttributes.TextColor,
            Font = titleTextAttributes.Font
          };
      }
      else
      {
        UIStringAttributes stringAttributes = new UIStringAttributes();
        stringAttributes.Font = titleTextAttributes.Font;
        stringAttributes.ForegroundColor = barTextColor == Color.Default ? stringAttributes.ForegroundColor ?? UINavigationBar.Appearance.TintColor : ColorExtensions.ToUIColor(barTextColor);
        this.NavigationBar.TitleTextAttributes = stringAttributes;
      }
      if (Forms.IsiOS7OrNewer)
        this.NavigationBar.TintColor = barTextColor == Color.Default ? UINavigationBar.Appearance.TintColor : ColorExtensions.ToUIColor(barTextColor);
      if (barTextColor.Luminosity > 0.5)
        UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackTranslucent;
      else
        UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
    }

    private void UpdateToolBarVisible()
    {
      if (this.secondaryToolbar == null)
        return;
      if (this.TopViewController != null && this.TopViewController.ToolbarItems != null && Enumerable.Any<UIBarButtonItem>((IEnumerable<UIBarButtonItem>) this.TopViewController.ToolbarItems))
      {
        this.secondaryToolbar.Hidden = false;
        this.secondaryToolbar.Items = this.TopViewController.ToolbarItems;
      }
      else
        this.secondaryToolbar.Hidden = true;
    }

    public override void ViewDidLayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidLayoutSubviews();
      this.UpdateToolBarVisible();
      CGRect frame1 = this.NavigationBar.Frame;
      UIToolbar uiToolbar = this.secondaryToolbar;
      nfloat y1 = this.NavigationBarHidden || !NavigationPage.GetHasNavigationBar((BindableObject) this.Current) ? (nfloat) 0 : frame1.Bottom;
      uiToolbar.Frame = new CGRect((nfloat) 0, y1, this.View.Frame.Width, uiToolbar.Frame.Height);
      CGRect frame2;
      nfloat nfloat1;
      if (!uiToolbar.Hidden)
      {
        frame2 = uiToolbar.Frame;
        nfloat1 = frame2.Bottom;
      }
      else
        nfloat1 = y1;
      double num = (double) nfloat1;
      Size size = this.queuedSize.IsZero ? this.Element.Bounds.Size : this.queuedSize;
      NavigationPage navigationPage = (NavigationPage) this.Element;
      double x = 0.0;
      nfloat nfloat2;
      if (!uiToolbar.Hidden)
      {
        frame2 = uiToolbar.Frame;
        nfloat2 = frame2.Height;
      }
      else
        nfloat2 = (nfloat) 0;
      double y2 = (double) nfloat2;
      double width = size.Width;
      double height = size.Height - num;
      Rectangle rectangle = new Rectangle(x, y2, width, height);
      navigationPage.ContainerArea = rectangle;
      if (!this.queuedSize.IsZero)
      {
        this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, this.queuedSize.Width, this.queuedSize.Height));
        this.queuedSize = Size.Zero;
      }
      this.loaded = true;
      foreach (UIView uiView in this.View.Subviews)
      {
        if (uiView != this.NavigationBar && uiView != this.secondaryToolbar)
          uiView.Frame = this.View.Bounds;
      }
    }

    public override UIViewController[] PopToRootViewController(bool animated)
    {
      if (!this.ignorePopCall)
        this.RemoveViewControllers(animated);
      // ISSUE: reference to a compiler-generated method
      return base.PopToRootViewController(animated);
    }

    public override UIViewController PopViewController(bool animated)
    {
      this.RemoveViewControllers(animated);
      // ISSUE: reference to a compiler-generated method
      return base.PopViewController(animated);
    }

    public Task<bool> PushPageAsync(Page page, bool animated = true)
    {
      return this.OnPushAsync(page, animated);
    }

    private void RemoveViewControllers(bool animated)
    {
      NavigationRenderer.ParentingViewController controller = this.TopViewController as NavigationRenderer.ParentingViewController;
      if (controller == null)
        return;
      int count = this.ViewControllers.Length;
      this.GetAppearedOrDisappearedTask(controller.Child).ContinueWith<Task>((Func<Task<bool>, Task>) (async t =>
      {
        if (!t.Result)
        {
          this.ignorePopCall = true;
          int removed = count - this.ViewControllers.Length;
          for (int i = 0; i < removed; ++i)
          {
            Page page = await ((NavigationPage) this.Element).PopAsyncInner(animated, true);
          }
          controller.Dispose();
          this.ignorePopCall = false;
        }
      }), TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void UpdateLeftBarButtonItem(NavigationRenderer.ParentingViewController containerController)
    {
      Page child = containerController.Child;
      Page page = Enumerable.LastOrDefault<Page>((IEnumerable<Page>) ((NavigationPage) this.Element).get_StackCopy());
      if (child != page && NavigationPage.GetHasBackButton(child) || this.parentMasterDetailPage == null)
        return;
      if (!this.parentMasterDetailPage.ShouldShowToolbarButton())
      {
        containerController.NavigationItem.LeftBarButtonItem = (UIBarButtonItem) null;
      }
      else
      {
        bool flag = this.parentMasterDetailPage.Master.Icon != null;
        if (flag)
        {
          try
          {
            containerController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(new UIImage((string) this.parentMasterDetailPage.Master.Icon), UIBarButtonItemStyle.Plain, (EventHandler) ((o, e) => this.parentMasterDetailPage.IsPresented = !this.parentMasterDetailPage.IsPresented));
          }
          catch (Exception ex)
          {
            flag = false;
          }
        }
        if (flag)
          return;
        containerController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(this.parentMasterDetailPage.Master.Title, UIBarButtonItemStyle.Plain, (EventHandler) ((o, e) => this.parentMasterDetailPage.IsPresented = !this.parentMasterDetailPage.IsPresented));
      }
    }

    private NavigationRenderer.ParentingViewController CreateViewControllerForPage(Page page)
    {
      if (Platform.GetRenderer((VisualElement) page) == null)
        Platform.SetRenderer((VisualElement) page, Platform.CreateRenderer((VisualElement) page));
      NavigationRenderer.ParentingViewController containerController = new NavigationRenderer.ParentingViewController(this)
      {
        Child = page
      };
      if (!string.IsNullOrWhiteSpace(page.Title))
        containerController.NavigationItem.Title = page.Title;
      this.UpdateLeftBarButtonItem(containerController);
      FileImageSource titleIcon = NavigationPage.GetTitleIcon((BindableObject) page);
      if (!string.IsNullOrEmpty((string) titleIcon))
      {
        try
        {
          containerController.NavigationItem.TitleView = (UIView) new UIImageView(new UIImage((string) titleIcon));
        }
        catch
        {
        }
      }
      string backButtonTitle = NavigationPage.GetBackButtonTitle((BindableObject) page);
      if (backButtonTitle != null)
      {
        int num;
        containerController.NavigationItem.BackBarButtonItem = new UIBarButtonItem(backButtonTitle, UIBarButtonItemStyle.Plain, (EventHandler) (async (o, e) => num = await this.PopViewAsync(page, true) ? 1 : 0));
      }
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
      // ISSUE: reference to a compiler-generated method
      containerController.View.AddSubview(renderer.ViewController.View);
      // ISSUE: reference to a compiler-generated method
      containerController.AddChildViewController(renderer.ViewController);
      // ISSUE: reference to a compiler-generated method
      renderer.ViewController.DidMoveToParentViewController((UIViewController) containerController);
      return containerController;
    }

    protected virtual async Task<bool> OnPushAsync(Page page, bool animated)
    {
      NavigationRenderer.ParentingViewController controllerForPage = this.CreateViewControllerForPage(page);
      Task<bool> orDisappearedTask = this.GetAppearedOrDisappearedTask(page);
      // ISSUE: reference to a compiler-generated method
      this.PushViewController((UIViewController) controllerForPage, animated);
      int num = await orDisappearedTask ? 1 : 0;
      this.UpdateToolBarVisible();
      return num != 0;
    }

    public Task<bool> PopViewAsync(Page page, bool animated = true)
    {
      return this.OnPopViewAsync(page, animated);
    }

    private Task<bool> GetAppearedOrDisappearedTask(Page page)
    {
      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
      NavigationRenderer.ParentingViewController parentViewController = Platform.GetRenderer((VisualElement) page).ViewController.ParentViewController as NavigationRenderer.ParentingViewController;
      if (parentViewController == null)
        throw new NotSupportedException("ParentingViewController parent could not be found. Please file a bug.");
      EventHandler appearing = (EventHandler) null;
      EventHandler disappearing = (EventHandler) null;
      appearing = (EventHandler) ((s, e) =>
      {
        parentViewController.Appearing -= appearing;
        parentViewController.Disappearing -= disappearing;
        Device.BeginInvokeOnMainThread((Action) (() => tcs.SetResult(true)));
      });
      disappearing = (EventHandler) ((s, e) =>
      {
        parentViewController.Appearing -= appearing;
        parentViewController.Disappearing -= disappearing;
        Device.BeginInvokeOnMainThread((Action) (() => tcs.SetResult(false)));
      });
      parentViewController.Appearing += appearing;
      parentViewController.Disappearing += disappearing;
      return tcs.Task;
    }

    protected virtual async Task<bool> OnPopViewAsync(Page page, bool animated)
    {
      bool flag;
      if (this.ignorePopCall)
      {
        flag = true;
      }
      else
      {
        IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
        if (renderer == null || renderer.ViewController == null)
        {
          flag = false;
        }
        else
        {
          if (page != ((NavigationRenderer.ParentingViewController) this.TopViewController).Child)
            throw new NotSupportedException("Popped page does not appear on top of current navigation stack, please file a bug.");
          Task<bool> orDisappearedTask = this.GetAppearedOrDisappearedTask(page);
          // ISSUE: reference to a compiler-generated method
          UIViewController poppedViewController = base.PopViewController(animated);
          int num = !await orDisappearedTask ? 1 : 0;
          poppedViewController.Dispose();
          this.UpdateToolBarVisible();
          flag = num != 0;
        }
      }
      return flag;
    }

    public Task<bool> PopToRootAsync(Page page, bool animated = true)
    {
      return this.OnPopToRoot(page, animated);
    }

    protected virtual async Task<bool> OnPopToRoot(Page page, bool animated)
    {
      this.ignorePopCall = true;
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) page);
      bool flag;
      if (renderer == null || renderer.ViewController == null)
      {
        flag = false;
      }
      else
      {
        Task<bool> orDisappearedTask = this.GetAppearedOrDisappearedTask(page);
        // ISSUE: reference to a compiler-generated method
        this.PopToRootViewController(animated);
        this.ignorePopCall = false;
        int num = !await orDisappearedTask ? 1 : 0;
        this.UpdateToolBarVisible();
        flag = num != 0;
      }
      return flag;
    }

    private void RemovePage(Page page)
    {
      if (page == null)
        throw new ArgumentNullException("page");
      if (page == this.Current)
        throw new NotSupportedException();
      UIViewController parentViewController = Platform.GetRenderer((VisualElement) page).ViewController.ParentViewController;
      if (this.removeControllers == null)
      {
        this.removeControllers = ArrayExtensions.Remove<UIViewController>(this.ViewControllers, parentViewController);
        this.ViewControllers = this.removeControllers;
        Device.BeginInvokeOnMainThread((Action) (() => this.removeControllers = (UIViewController[]) null));
      }
      else
      {
        this.removeControllers = ArrayExtensions.Remove<UIViewController>(this.removeControllers, parentViewController);
        this.ViewControllers = this.removeControllers;
      }
    }

    private void InsertPageBefore(Page page, Page before)
    {
      if (before == null)
        throw new ArgumentNullException("before");
      if (page == null)
        throw new ArgumentNullException("page");
      NavigationRenderer.ParentingViewController controllerForPage = this.CreateViewControllerForPage(page);
      this.ViewControllers = ArrayExtensions.Insert<UIViewController>(this.ViewControllers, EnumerableExtensions.IndexOf<UIViewController>((IEnumerable<M0>) this.ViewControllers, (M0) Platform.GetRenderer((VisualElement) before).ViewController.ParentViewController), (UIViewController) controllerForPage);
    }

    public override void ViewDidAppear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidAppear(animated);
      if (this.appeared || this.Element == null)
        return;
      this.appeared = true;
      ((Page) this.Element).SendAppearing();
      // ISSUE: reference to a compiler-generated method
      this.View.SetNeedsLayout();
    }

    public override void ViewDidDisappear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewDidDisappear(animated);
      if (!this.appeared || this.Element == null)
        return;
      this.appeared = false;
      ((Page) this.Element).SendDisappearing();
    }

    public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
    {
      // ISSUE: reference to a compiler-generated method
      base.DidRotate(fromInterfaceOrientation);
      // ISSUE: reference to a compiler-generated method
      this.View.SetNeedsLayout();
      this.UpdateLeftBarButtonItem((NavigationRenderer.ParentingViewController) Enumerable.Last<UIViewController>((IEnumerable<UIViewController>) this.ViewControllers));
    }

    private class SecondaryToolbar : UIToolbar
    {
      private List<UIView> Lines = new List<UIView>();

      public override UIBarButtonItem[] Items
      {
        get
        {
          return base.Items;
        }
        set
        {
          base.Items = value;
          this.SetupLines();
        }
      }

      public SecondaryToolbar()
      {
        this.TintColor = UIColor.White;
      }

      public override void LayoutSubviews()
      {
        // ISSUE: reference to a compiler-generated method
        base.LayoutSubviews();
        if (this.Items == null || this.Items.Length == 0)
          return;
        nfloat nfloat = (nfloat) 11f;
        nfloat width = (this.Bounds.Width - nfloat) / (nfloat) this.Items.Length - nfloat;
        nfloat x1 = nfloat;
        nfloat height = this.Bounds.Height - (nfloat) 10;
        foreach (UIBarButtonItem uiBarButtonItem in this.Items)
        {
          uiBarButtonItem.CustomView.Frame = new CGRect(x1, (nfloat) 5, width, height);
          x1 += width + nfloat;
        }
        nfloat x2 = width + nfloat * (nfloat) 1.5f;
        nfloat midY = RectangleFExtensions.GetMidY(this.Bounds);
        foreach (UIView uiView in this.Lines)
        {
          uiView.Center = new CGPoint(x2, midY);
          x2 += width + nfloat;
        }
      }

      private void SetupLines()
      {
        // ISSUE: reference to a compiler-generated method
        this.Lines.ForEach((Action<UIView>) (l => l.RemoveFromSuperview()));
        this.Lines.Clear();
        if (this.Items == null)
          return;
        for (int index = 1; index < this.Items.Length; ++index)
        {
          UIView view = new UIView(new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 1, (nfloat) 24))
          {
            BackgroundColor = new UIColor((nfloat) 0, (nfloat) 0, (nfloat) 0, (nfloat) 0.2f)
          };
          // ISSUE: reference to a compiler-generated method
          this.AddSubview(view);
          this.Lines.Add(view);
        }
      }
    }

    private class ParentingViewController : UIViewController
    {
      private ToolbarTracker tracker = new ToolbarTracker();
      private Page child;
      private readonly WeakReference<NavigationRenderer> navigation;

      public Page Child
      {
        get
        {
          return this.child;
        }
        set
        {
          if (this.child == value)
            return;
          if (this.child != null)
            this.child.remove_PropertyChanged(new PropertyChangedEventHandler(this.HandleChildPropertyChanged));
          this.child = value;
          if (this.child != null)
            this.child.add_PropertyChanged(new PropertyChangedEventHandler(this.HandleChildPropertyChanged));
          this.UpdateHasBackButton();
        }
      }

      public event EventHandler Appearing;

      public event EventHandler Disappearing;

      public ParentingViewController(NavigationRenderer navigation)
      {
        if (Forms.IsiOS7OrNewer)
          this.AutomaticallyAdjustsScrollViewInsets = false;
        this.navigation = new WeakReference<NavigationRenderer>(navigation);
      }

      private void UpdateHasBackButton()
      {
        if (this.Child == null)
          return;
        this.NavigationItem.HidesBackButton = !NavigationPage.GetHasBackButton(this.Child);
      }

      private void HandleChildPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == NavigationPage.HasNavigationBarProperty.PropertyName)
          this.UpdateNavigationBarVisibility(true);
        else if (e.PropertyName == Page.TitleProperty.PropertyName)
        {
          this.NavigationItem.Title = this.Child.Title;
        }
        else
        {
          if (!(e.PropertyName == NavigationPage.HasBackButtonProperty.PropertyName))
            return;
          this.UpdateHasBackButton();
        }
      }

      public override void ViewDidLayoutSubviews()
      {
        IVisualElementRenderer renderer;
        if (this.Child != null && (renderer = Platform.GetRenderer((VisualElement) this.Child)) != null)
          renderer.NativeView.Frame = RectangleExtensions.ToRectangleF(this.Child.Bounds);
        // ISSUE: reference to a compiler-generated method
        base.ViewDidLayoutSubviews();
      }

      public override void ViewDidLoad()
      {
        // ISSUE: reference to a compiler-generated method
        base.ViewDidLoad();
        this.tracker.Target = this.Child;
        this.tracker.set_AdditionalTargets(ViewExtensions.GetParentPages(this.Child));
        this.tracker.add_CollectionChanged(new EventHandler(this.TrackerOnCollectionChanged));
        this.UpdateToolbarItems();
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
        {
          this.Child = (Page) null;
          this.tracker.Target = (Page) null;
          this.tracker.remove_CollectionChanged(new EventHandler(this.TrackerOnCollectionChanged));
          this.tracker = (ToolbarTracker) null;
          if (this.NavigationItem.RightBarButtonItems != null)
          {
            for (int index = 0; index < this.NavigationItem.RightBarButtonItems.Length; ++index)
              this.NavigationItem.RightBarButtonItems[index].Dispose();
          }
          if (this.ToolbarItems != null)
          {
            for (int index = 0; index < this.ToolbarItems.Length; ++index)
              this.ToolbarItems[index].Dispose();
          }
        }
        // ISSUE: reference to a compiler-generated method
        base.Dispose(disposing);
      }

      private void TrackerOnCollectionChanged(object sender, EventArgs eventArgs)
      {
        this.UpdateToolbarItems();
      }

      private void UpdateToolbarItems()
      {
        if (this.NavigationItem.RightBarButtonItems != null)
        {
          for (int index = 0; index < this.NavigationItem.RightBarButtonItems.Length; ++index)
            this.NavigationItem.RightBarButtonItems[index].Dispose();
        }
        if (this.ToolbarItems != null)
        {
          for (int index = 0; index < this.ToolbarItems.Length; ++index)
            this.ToolbarItems[index].Dispose();
        }
        List<UIBarButtonItem> list1 = (List<UIBarButtonItem>) null;
        List<UIBarButtonItem> list2 = (List<UIBarButtonItem>) null;
        foreach (ToolbarItem toolbarItem in this.tracker.get_ToolbarItems())
        {
          if (toolbarItem.Order == ToolbarItemOrder.Secondary)
            (list2 = list2 ?? new List<UIBarButtonItem>()).Add(ToolbarItemExtensions.ToUIBarButtonItem(toolbarItem, true));
          else
            (list1 = list1 ?? new List<UIBarButtonItem>()).Add(ToolbarItemExtensions.ToUIBarButtonItem(toolbarItem, false));
        }
        if (list1 != null)
          list1.Reverse();
        // ISSUE: reference to a compiler-generated method
        this.NavigationItem.SetRightBarButtonItems(list1 == null ? new UIBarButtonItem[0] : list1.ToArray(), false);
        this.ToolbarItems = list2 == null ? new UIBarButtonItem[0] : list2.ToArray();
        NavigationRenderer target;
        if (!this.navigation.TryGetTarget(out target))
          return;
        target.UpdateToolBarVisible();
      }

      public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
      {
        // ISSUE: reference to a compiler-generated method
        base.DidRotate(fromInterfaceOrientation);
        // ISSUE: reference to a compiler-generated method
        this.View.SetNeedsLayout();
      }

      public override void ViewWillAppear(bool animated)
      {
        this.UpdateNavigationBarVisibility(animated);
        // ISSUE: reference to a compiler-generated method
        base.ViewWillAppear(animated);
      }

      public override void ViewDidAppear(bool animated)
      {
        // ISSUE: reference to a compiler-generated method
        base.ViewDidAppear(animated);
        // ISSUE: reference to a compiler-generated field
        EventHandler eventHandler = this.Appearing;
        if (eventHandler == null)
          return;
        eventHandler((object) this, EventArgs.Empty);
      }

      public override void ViewDidDisappear(bool animated)
      {
        // ISSUE: reference to a compiler-generated method
        base.ViewDidDisappear(animated);
        // ISSUE: reference to a compiler-generated field
        EventHandler eventHandler = this.Disappearing;
        if (eventHandler == null)
          return;
        eventHandler((object) this, EventArgs.Empty);
      }

      private void UpdateNavigationBarVisibility(bool animated)
      {
        Page child = this.Child;
        if (child == null || this.NavigationController == null)
          return;
        bool hasNavigationBar = NavigationPage.GetHasNavigationBar((BindableObject) child);
        if (this.NavigationController.NavigationBarHidden != hasNavigationBar)
          return;
        // ISSUE: reference to a compiler-generated method
        this.NavigationController.SetNavigationBarHidden(!hasNavigationBar, animated);
      }
    }
  }
}
