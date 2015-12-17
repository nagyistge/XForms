using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class Platform : BindableObject, IPlatform, INavigation, IDisposable
  {
    internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof (IVisualElementRenderer), typeof (Platform), (object) null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate) null, new BindableProperty.BindingPropertyChangedDelegate(Platform.\u003C\u003Ec.\u003C\u003E9.\u003C\u002Ecctor\u003Eb__48_0), (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    private bool animateModals = true;
    private int alertPadding = 10;
    private readonly List<Page> modals;
    private Page root;
    private bool appeared;
    private readonly PlatformRenderer renderer;
    private bool disposed;

    internal UIViewController ViewController
    {
      get
      {
        return (UIViewController) this.renderer;
      }
    }

    IReadOnlyList<Page> INavigation.NavigationStack
    {
      get
      {
        return (IReadOnlyList<Page>) new List<Page>();
      }
    }

    IReadOnlyList<Page> INavigation.ModalStack
    {
      get
      {
        return (IReadOnlyList<Page>) this.modals;
      }
    }

    private Page Page
    {
      get
      {
        return this.root;
      }
    }

    private Application TargetApplication
    {
      get
      {
        if (this.root == null)
          return (Application) null;
        return this.root.Parent as Application;
      }
    }

    internal Platform()
    {
      this.renderer = new PlatformRenderer(this);
      this.modals = new List<Page>();
      int busyCount = 0;
      MessagingCenter.Subscribe<Page, bool>((object) this, "Xamarin.BusySet", (Action<M0, M1>) ((sender, enabled) =>
      {
        if (!this.PageIsChildOfPlatform(sender))
          return;
        busyCount = Math.Max(0, enabled ? busyCount + 1 : busyCount - 1);
        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = busyCount > 0;
      }), (M0) null);
      MessagingCenter.Subscribe<Page, AlertArguments>((object) this, "Xamarin.SendAlert", (Action<M0, M1>) ((sender, arguments) =>
      {
        if (!this.PageIsChildOfPlatform(sender))
          return;
        if (Forms.IsiOS8OrNewer)
        {
          // ISSUE: reference to a compiler-generated method
          UIAlertController uiAlertController = UIAlertController.Create(arguments.Title, arguments.Message, UIAlertControllerStyle.Alert);
          CGRect frame = uiAlertController.View.Frame;
          uiAlertController.View.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height - (nfloat) (this.alertPadding * 2));
          // ISSUE: reference to a compiler-generated method
          // ISSUE: reference to a compiler-generated method
          uiAlertController.AddAction(UIAlertAction.Create(arguments.Cancel, UIAlertActionStyle.Cancel, (Action<UIAlertAction>) (a => arguments.SetResult(false))));
          if (arguments.Accept != null)
          {
            // ISSUE: reference to a compiler-generated method
            // ISSUE: reference to a compiler-generated method
            uiAlertController.AddAction(UIAlertAction.Create(arguments.Accept, UIAlertActionStyle.Default, (Action<UIAlertAction>) (a => arguments.SetResult(true))));
          }
          // ISSUE: reference to a compiler-generated method
          Platform.GetRenderer(Enumerable.Any<Page>((IEnumerable<Page>) this.modals) ? (VisualElement) Enumerable.Last<Page>((IEnumerable<Page>) this.modals) : (VisualElement) this.root).ViewController.PresentViewController((UIViewController) uiAlertController, true, (Action) null);
        }
        else
        {
          UIAlertView uiAlertView;
          if (arguments.Accept != null)
            uiAlertView = new UIAlertView(arguments.Title, arguments.Message, (UIAlertViewDelegate) null, arguments.Cancel, new string[1]
            {
              arguments.Accept
            });
          else
            uiAlertView = new UIAlertView(arguments.Title, arguments.Message, (UIAlertViewDelegate) null, arguments.Cancel, new string[0]);
          uiAlertView.Dismissed += (EventHandler<UIButtonEventArgs>) ((o, args) => arguments.SetResult(args.ButtonIndex != (nint) 0));
          // ISSUE: reference to a compiler-generated method
          uiAlertView.Show();
        }
      }), (M0) null);
      MessagingCenter.Subscribe<Page, ActionSheetArguments>((object) this, "Xamarin.ShowActionSheet", (Action<M0, M1>) ((sender, arguments) =>
      {
        if (!this.PageIsChildOfPlatform(sender))
          return;
        Page page = sender;
        while (!Application.IsApplicationOrNull(page.Parent))
          page = (Page) page.Parent;
        IVisualElementRenderer pageRenderer = Platform.GetRenderer((VisualElement) page);
        if (Forms.IsiOS8OrNewer)
        {
          // ISSUE: reference to a compiler-generated method
          UIAlertController alert = UIAlertController.Create(arguments.Title, (string) null, UIAlertControllerStyle.ActionSheet);
          if (arguments.Cancel != null)
          {
            // ISSUE: reference to a compiler-generated method
            // ISSUE: reference to a compiler-generated method
            alert.AddAction(UIAlertAction.Create(arguments.Cancel, UIAlertActionStyle.Cancel, (Action<UIAlertAction>) (a => arguments.SetResult(arguments.Cancel))));
          }
          if (arguments.Destruction != null)
          {
            // ISSUE: reference to a compiler-generated method
            // ISSUE: reference to a compiler-generated method
            alert.AddAction(UIAlertAction.Create(arguments.Destruction, UIAlertActionStyle.Destructive, (Action<UIAlertAction>) (a => arguments.SetResult(arguments.Destruction))));
          }
          foreach (string str in arguments.get_Buttons())
          {
            if (str != null)
            {
              string blabel = str;
              // ISSUE: reference to a compiler-generated method
              // ISSUE: reference to a compiler-generated method
              alert.AddAction(UIAlertAction.Create(blabel, UIAlertActionStyle.Default, (Action<UIAlertAction>) (a => arguments.SetResult(blabel))));
            }
          }
          if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
          {
            // ISSUE: reference to a compiler-generated method
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
            NSObject observer = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, (Action<NSNotification>) (n => alert.PopoverPresentationController.SourceRect = pageRenderer.ViewController.View.Bounds));
            arguments.get_Result().Task.ContinueWith((Action<Task<string>>) (t =>
            {
              // ISSUE: reference to a compiler-generated method
              NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
              // ISSUE: reference to a compiler-generated method
              UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
            }), TaskScheduler.FromCurrentSynchronizationContext());
            alert.PopoverPresentationController.SourceView = pageRenderer.ViewController.View;
            alert.PopoverPresentationController.SourceRect = pageRenderer.ViewController.View.Bounds;
            alert.PopoverPresentationController.PermittedArrowDirections = ~UIPopoverArrowDirection.Unknown;
          }
          // ISSUE: reference to a compiler-generated method
          pageRenderer.ViewController.PresentViewController((UIViewController) alert, true, (Action) null);
        }
        else
        {
          UIActionSheet actionSheet = new UIActionSheet(arguments.Title, (UIActionSheetDelegate) null, arguments.Cancel, arguments.Destruction, Enumerable.ToArray<string>(arguments.get_Buttons()));
          // ISSUE: reference to a compiler-generated method
          actionSheet.ShowInView(pageRenderer.NativeView);
          actionSheet.Clicked += (EventHandler<UIButtonEventArgs>) ((o, args) =>
          {
            string result = (string) null;
            if (args.ButtonIndex != (nint) -1)
            {
              // ISSUE: reference to a compiler-generated method
              result = actionSheet.ButtonTitle(args.ButtonIndex);
            }
            arguments.get_Result().TrySetResult(result);
          });
        }
      }), (M0) null);
    }

    public static IVisualElementRenderer GetRenderer(VisualElement bindable)
    {
      return (IVisualElementRenderer) bindable.GetValue(Platform.RendererProperty);
    }

    public static void SetRenderer(VisualElement bindable, IVisualElementRenderer value)
    {
      bindable.SetValue(Platform.RendererProperty, (object) value);
    }

    public static IVisualElementRenderer CreateRenderer(VisualElement element)
    {
      // ISSUE: unable to decompile the method.
    }

    void IDisposable.Dispose()
    {
      if (this.disposed)
        return;
      this.disposed = true;
      this.root.remove_DescendantRemoved(new EventHandler<ElementEventArgs>(this.HandleChildRemoved));
      MessagingCenter.Unsubscribe<Page, ActionSheetArguments>((object) this, "Xamarin.ShowActionSheet");
      MessagingCenter.Unsubscribe<Page, AlertArguments>((object) this, "Xamarin.SendAlert");
      MessagingCenter.Unsubscribe<Page, bool>((object) this, "Xamarin.BusySet");
      this.DisposeModelAndChildrenRenderers((Element) this.root);
      foreach (Element view in this.modals)
        this.DisposeModelAndChildrenRenderers(view);
      this.renderer.Dispose();
    }

    private bool PageIsChildOfPlatform(Page page)
    {
      while (!Application.IsApplicationOrNull(page.Parent))
        page = (Page) page.Parent;
      if (this.root != page)
        return this.modals.Contains(page);
      return true;
    }

    internal void WillAppear()
    {
      if (this.appeared)
        return;
      this.renderer.View.BackgroundColor = UIColor.White;
      this.renderer.View.ContentMode = UIViewContentMode.Redraw;
      this.root.Platform = (IPlatform) this;
      this.AddChild((VisualElement) this.root);
      this.root.add_DescendantRemoved(new EventHandler<ElementEventArgs>(this.HandleChildRemoved));
      this.appeared = true;
    }

    internal void DidAppear()
    {
      this.animateModals = false;
      this.TargetApplication.NavigationProxy.Inner = (INavigation) this;
      this.animateModals = true;
    }

    internal void SetPage(Page newRoot)
    {
      if (newRoot == null)
        return;
      if (this.root != null)
        throw new NotImplementedException();
      this.root = newRoot;
      if (!this.appeared)
        return;
      this.root.Platform = (IPlatform) this;
      this.AddChild((VisualElement) this.root);
      this.root.add_DescendantRemoved(new EventHandler<ElementEventArgs>(this.HandleChildRemoved));
      this.TargetApplication.NavigationProxy.Inner = (INavigation) this;
    }

    internal void DisposeModelAndChildrenRenderers(Element view)
    {
      foreach (VisualElement bindable in view.Descendants())
      {
        IVisualElementRenderer renderer = Platform.GetRenderer(bindable);
        BindableProperty property = Platform.RendererProperty;
        bindable.ClearValue(property);
        if (renderer != null)
        {
          // ISSUE: reference to a compiler-generated method
          renderer.NativeView.RemoveFromSuperview();
          renderer.Dispose();
        }
      }
      IVisualElementRenderer renderer1 = Platform.GetRenderer((VisualElement) view);
      if (renderer1 != null)
      {
        if (renderer1.ViewController != null)
        {
          ModalWrapper modalWrapper = renderer1.ViewController.ParentViewController as ModalWrapper;
          if (modalWrapper != null)
            modalWrapper.Dispose();
        }
        // ISSUE: reference to a compiler-generated method
        renderer1.NativeView.RemoveFromSuperview();
        renderer1.Dispose();
      }
      view.ClearValue(Platform.RendererProperty);
    }

    internal void DisposeRendererAndChildren(IVisualElementRenderer rendererToRemove)
    {
      if (rendererToRemove == null)
        return;
      if (rendererToRemove.Element != null && Platform.GetRenderer(rendererToRemove.Element) == rendererToRemove)
        rendererToRemove.Element.ClearValue(Platform.RendererProperty);
      foreach (UIView uiView in rendererToRemove.NativeView.Subviews)
      {
        IVisualElementRenderer rendererToRemove1 = uiView as IVisualElementRenderer;
        if (rendererToRemove1 != null)
          this.DisposeRendererAndChildren(rendererToRemove1);
      }
      // ISSUE: reference to a compiler-generated method
      rendererToRemove.NativeView.RemoveFromSuperview();
      rendererToRemove.Dispose();
    }

    private void HandleChildRemoved(object sender, ElementEventArgs e)
    {
      this.DisposeModelAndChildrenRenderers(e.Element);
    }

    Task INavigation.PushAsync(Page root)
    {
      return this.PushAsync(root, true);
    }

    Task<Page> INavigation.PopAsync()
    {
      return this.PopAsync(true);
    }

    Task INavigation.PopToRootAsync()
    {
      return this.PopToRootAsync(true);
    }

    Task INavigation.PushAsync(Page root, bool animated)
    {
      throw new InvalidOperationException("PushAsync is not supported globally on iOS, please use a NavigationPage.");
    }

    Task<Page> INavigation.PopAsync(bool animated)
    {
      throw new InvalidOperationException("PopAsync is not supported globally on iOS, please use a NavigationPage.");
    }

    Task INavigation.PopToRootAsync(bool animated)
    {
      throw new InvalidOperationException("PopToRootAsync is not supported globally on iOS, please use a NavigationPage.");
    }

    void INavigation.RemovePage(Page page)
    {
      throw new InvalidOperationException("RemovePage is not supported globally on iOS, please use a NavigationPage.");
    }

    void INavigation.InsertPageBefore(Page page, Page before)
    {
      throw new InvalidOperationException("InsertPageBefore is not supported globally on iOS, please use a NavigationPage.");
    }

    Task INavigation.PushModalAsync(Page modal)
    {
      return this.PushModalAsync(modal, true);
    }

    Task INavigation.PushModalAsync(Page modal, bool animated)
    {
      this.modals.Add(modal);
      modal.Platform = (IPlatform) this;
      modal.add_DescendantRemoved(new EventHandler<ElementEventArgs>(this.HandleChildRemoved));
      if (this.appeared)
        return this.PresentModal(modal, this.animateModals & animated);
      return (Task) Task.FromResult<object>((object) null);
    }

    private async Task PresentModal(Page modal, bool animated)
    {
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) modal);
      if (renderer == null)
      {
        renderer = Platform.CreateRenderer((VisualElement) modal);
        Platform.SetRenderer((VisualElement) modal, renderer);
      }
      ModalWrapper wrapper = new ModalWrapper(renderer);
      if (this.modals.Count > 1)
      {
        UIViewController uiViewController = Platform.GetRenderer((VisualElement) this.modals[this.modals.Count - 2]) as UIViewController;
        if (uiViewController != null)
        {
          // ISSUE: reference to a compiler-generated method
          await uiViewController.PresentViewControllerAsync((UIViewController) wrapper, animated);
          await Task.Delay(5);
          goto label_9;
        }
      }
      // ISSUE: reference to a compiler-generated method
      await this.renderer.PresentViewControllerAsync((UIViewController) wrapper, animated);
      await Task.Delay(5);
label_9:;
    }

    Task<Page> INavigation.PopModalAsync()
    {
      return this.PopModalAsync(true);
    }

    async Task<Page> INavigation.PopModalAsync(bool animated)
    {
      Page modal = Enumerable.Last<Page>((IEnumerable<Page>) this.modals);
      this.modals.Remove(modal);
      modal.remove_DescendantRemoved(new EventHandler<ElementEventArgs>(this.HandleChildRemoved));
      UIViewController uiViewController = Platform.GetRenderer((VisualElement) modal) as UIViewController;
      if (this.modals.Count >= 1 && uiViewController != null)
      {
        // ISSUE: reference to a compiler-generated method
        await uiViewController.DismissViewControllerAsync(animated);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        await this.renderer.DismissViewControllerAsync(animated);
      }
      this.DisposeModelAndChildrenRenderers((Element) modal);
      return modal;
    }

    internal void LayoutSubviews()
    {
      if (this.root == null)
        return;
      IVisualElementRenderer renderer = Platform.GetRenderer((VisualElement) this.root);
      if (renderer == null)
        return;
      renderer.SetElementSize(new Size((double) this.renderer.View.Bounds.Width, (double) this.renderer.View.Bounds.Height));
    }

    private void AddChild(VisualElement view)
    {
      if (!Application.IsApplicationOrNull(view.Parent))
        Console.Error.WriteLine("Tried to add parented view to canvas directly");
      if (Platform.GetRenderer(view) == null)
      {
        IVisualElementRenderer renderer = Platform.CreateRenderer(view);
        Platform.SetRenderer(view, renderer);
        // ISSUE: reference to a compiler-generated method
        this.renderer.View.AddSubview(renderer.NativeView);
        if (renderer.ViewController != null)
        {
          // ISSUE: reference to a compiler-generated method
          this.renderer.AddChildViewController(renderer.ViewController);
        }
        renderer.NativeView.Frame = new CGRect((nfloat) 0, (nfloat) 0, this.renderer.View.Bounds.Width, this.renderer.View.Bounds.Height);
        renderer.SetElementSize(new Size((double) this.renderer.View.Bounds.Width, (double) this.renderer.View.Bounds.Height));
      }
      else
        Console.Error.WriteLine("Potential view double add");
    }

    SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
    {
      IVisualElementRenderer renderer = Platform.GetRenderer(view);
      if (renderer == null || renderer.NativeView == null)
        return new SizeRequest(Size.Zero);
      return renderer.GetDesiredSize(widthConstraint, heightConstraint);
    }

    protected override void OnBindingContextChanged()
    {
      BindableObject.SetInheritedBindingContext((BindableObject) this.Page, this.BindingContext);
      base.OnBindingContextChanged();
    }

    internal class DefaultRenderer : VisualElementRenderer<VisualElement>
    {
    }
  }
}
