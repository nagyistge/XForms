using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class Platform : BindableObject, IPlatform, INavigation, IDisposable
	{
		internal static readonly BindableProperty RendererProperty =  BindableProperty.CreateAttached(
			"Renderer", 
			typeof(IVisualElementRenderer), 
			typeof(Platform), 
			null, 
			BindingMode.OneWay, 
			null, 
			(BindableObject bindable, object oldvalue, object newvalue) => 
				{
					VisualElement visualElement = bindable as VisualElement;
					if (visualElement != null)
					{
						visualElement.IsPlatformEnabled = newvalue != null;
					}
				}, null, null, null);

		private bool animateModals = true;
		private int alertPadding = 10;
		private readonly List<Page> modals;
		private Page root;
		private bool appeared;
		private readonly PlatformRenderer renderer;
		private bool disposed;

		internal NSViewController ViewController
		{
			get	{ return (NSViewController)this.renderer; }
		}

		IReadOnlyList<Page> INavigation.NavigationStack
		{
			get	{ return (IReadOnlyList<Page>)new List<Page> (); }
		}

		IReadOnlyList<Page> INavigation.ModalStack
		{
			get	{ return (IReadOnlyList<Page>)this.modals;	}
		}

		private Page Page
		{
			get	{ return this.root;	}
		}

		private Application TargetApplication
		{
			get
			{
				if (this.root == null)
					return null;
				return this.root.Parent as Application;
			}
		}

		internal Platform ()
		{
			this.renderer = new PlatformRenderer (this);
			this.modals = new List<Page> ();
			int busyCount = 0;
			MessagingCenter.Subscribe<Page, bool> (this, "Xamarin.BusySet", (sender, enabled) =>
			{
				if (!this.PageIsChildOfPlatform (sender))
					return;
				busyCount = Math.Max (0, enabled ? busyCount + 1 : busyCount - 1);

				// Not for Mac
				//NSApplication.SharedApplication.NetworkActivityIndicatorVisible = busyCount > 0;

			}, null);

			MessagingCenter.Subscribe<Page, AlertArguments> (this, "Xamarin.SendAlert", (sender, arguments) =>
			{
				if (!this.PageIsChildOfPlatform (sender))
					return;
				NSAlert uiAlertView;
				if (arguments.Accept != null) {
					//uiAlertView = NSAlert.WithMessage (arguments.Title, arguments.Message, null, arguments.Cancel, new [] { arguments.Accept });
					uiAlertView = NSAlert.WithMessage (arguments.Title, arguments.Cancel, arguments.Accept, null, arguments.Message);
				}
				else
				{
					//uiAlertView = NSAlert.WithMessage (arguments.Title, arguments.Message, null, arguments.Cancel, new [] {} );
					uiAlertView = NSAlert.WithMessage (arguments.Title, arguments.Cancel, null, null, arguments.Message);

				}

				var result = uiAlertView.RunModal();
				arguments.SetResult(result != 0);

				//uiAlertView.Dismissed += (o, args) => arguments.SetResult (args.ButtonIndex != 0);

			}, null);

			MessagingCenter.Subscribe<Page, ActionSheetArguments> (this, "Xamarin.ShowActionSheet", (sender, arguments) =>
			{
				if (!this.PageIsChildOfPlatform (sender))
					return;
				
				Page page = sender;
				while (!page.Parent.IsApplicationOrNull())
					page = (Page)page.Parent;
				
				IVisualElementRenderer pageRenderer = Platform.GetRenderer ((VisualElement)page);
				/*
				if (Forms.IsiOS8OrNewer)
				{
					var alert = NSAlert.WithMessage (arguments.Title, null);
					if (arguments.Cancel != null)
					{
						var btn = alert.AddButton(arguments.Cancel);
						btn.Activated += (s, e) => arguments.SetResult(arguments.Cancel);
					}
					if (arguments.Destruction != null)
					{
						var btn = alert.AddButton(arguments.Destruction);
						btn.Activated += (s, e) => arguments.SetResult(arguments.Destruction);
					}
					foreach (string str in arguments.Buttons)
					{
						if (str != null)
						{
							var btn = alert.AddButton(arguments.Destruction);
							btn.Activated += (s, e) => arguments.SetResult(str);
						}
					}

					alert.RunModal();

					//pageRenderer.ViewController.PresentViewController( (NSViewController)alert, true, (Action)null);
				}
				else
				*/
				{
					var btns = arguments.Buttons.ToArray();
					string btn1 = (btns.Length >= 1) ? btns[0] : null;
					string btn2 = (btns.Length >= 2) ? btns[1] : null;
					string btn3 = (btns.Length >= 3) ? btns[2] : null;

					var alert = NSAlert.WithMessage(arguments.Title, btn1, btn2, btn3, null);
					var result = alert.RunSheetModal(null);

					if (result >= 0 && result < btns.Length)
						arguments.SetResult( btns[(int)result] );
					else
						arguments.SetResult(null);

					// TODO: Need to test
					/*
					UIActionSheet actionSheet = new UIActionSheet (arguments.Title, (UIActionSheetDelegate)null, arguments.Cancel, arguments.Destruction, Enumerable.ToArray<string> (arguments.get_Buttons ()));
					actionSheet.ShowInView (pageRenderer.NativeView);
					actionSheet.Clicked += (EventHandler<UIButtonEventArgs>)((o, args) =>
					{
						string result = (string)null;
						if (args.ButtonIndex != (nint) - 1)
						{
							// ISSUE: reference to a compiler-generated method
							result = actionSheet.ButtonTitle (args.ButtonIndex);
						}
						arguments.get_Result ().TrySetResult (result);
					});
					*/
				}
			}, null);
		}

		public static IVisualElementRenderer GetRenderer (VisualElement bindable)
		{
			return (IVisualElementRenderer)bindable.GetValue (Platform.RendererProperty);
		}

		public static void SetRenderer (VisualElement bindable, IVisualElementRenderer value)
		{
			bindable.SetValue (Platform.RendererProperty, (object)value);
		}

		public static IVisualElementRenderer CreateRenderer (VisualElement element)
		{
			Type type = element.GetType();
			object handler = Registrar.Registered.GetHandler<IVisualElementRenderer>(type);
			if (handler == null)
			{
				handler = new Platform.DefaultRenderer();
			}
			((IVisualElementRenderer)handler).SetElement(element);
			return (IVisualElementRenderer)handler;
		}

		void IDisposable.Dispose ()
		{
			if (this.disposed)
				return;
			this.disposed = true;
			this.root.DescendantRemoved -= HandleChildRemoved;

			MessagingCenter.Unsubscribe<Page, ActionSheetArguments> (this, "Xamarin.ShowActionSheet");
			MessagingCenter.Unsubscribe<Page, AlertArguments> (this, "Xamarin.SendAlert");
			MessagingCenter.Unsubscribe<Page, bool> (this, "Xamarin.BusySet");
			this.DisposeModelAndChildrenRenderers (this.root);
			foreach (Element view in this.modals)
				this.DisposeModelAndChildrenRenderers (view);

			//renderer = null;
		}

		private bool PageIsChildOfPlatform (Page page)
		{
			while (!Application.IsApplicationOrNull (page.Parent))
				page = (Page)page.Parent;
			if (this.root != page)
				return this.modals.Contains (page);
			return true;
		}

		internal void WillAppear ()
		{
			if (this.appeared)
				return;

			renderer.View.SetBackgroundColor (NSColor.White);

			// TODO: Fix this
			/*
			renderer.View.ContentMode = UIViewContentMode.Redraw;
			root.Platform = (IPlatform)this;
			*/

			AddChild ((VisualElement)this.root);
			root.DescendantRemoved -= HandleChildRemoved;
			appeared = true;
		}

		internal void DidAppear ()
		{
			animateModals = false;
			TargetApplication.NavigationProxy.Inner = (INavigation)this;
			animateModals = true;
		}

		internal void SetPage (Page newRoot)
		{
			if (newRoot == null)
				return;
			if (this.root != null)
				throw new NotImplementedException ();
			this.root = newRoot;
			if (!this.appeared)
				return;

			root.Platform = (IPlatform)this;

			AddChild ((VisualElement)root);
			root.DescendantRemoved += HandleChildRemoved;

			// TODO: Fix this
			//this.TargetApplication.NavigationProxy.Inner = (INavigation)this;
		}

		internal void DisposeModelAndChildrenRenderers (Element view)
		{
			foreach (VisualElement bindable in view.Descendants())
			{
				IVisualElementRenderer renderer = Platform.GetRenderer (bindable);
				BindableProperty property = Platform.RendererProperty;
				bindable.ClearValue (property);
				if (renderer != null)
				{
					// ISSUE: reference to a compiler-generated method
					renderer.NativeView.RemoveFromSuperview ();
					renderer.Dispose ();
				}
			}
			IVisualElementRenderer renderer1 = Platform.GetRenderer ((VisualElement)view);
			if (renderer1 != null)
			{
				if (renderer1.ViewController != null)
				{
					ModalWrapper modalWrapper = renderer1.ViewController.ParentViewController as ModalWrapper;

					// Not required for Mac
					//if (modalWrapper != null) 
					//    modalWrapper.Dispose ();
				}

				renderer1.NativeView.RemoveFromSuperview ();
				renderer1.Dispose ();
			}
			view.ClearValue (Platform.RendererProperty);
		}

		internal void DisposeRendererAndChildren (IVisualElementRenderer rendererToRemove)
		{
			if (rendererToRemove == null)
				return;
			if (rendererToRemove.Element != null && Platform.GetRenderer (rendererToRemove.Element) == rendererToRemove)
				rendererToRemove.Element.ClearValue (Platform.RendererProperty);
			foreach (NSView uiView in rendererToRemove.NativeView.Subviews)
			{
				IVisualElementRenderer rendererToRemove1 = uiView as IVisualElementRenderer;
				if (rendererToRemove1 != null)
					this.DisposeRendererAndChildren (rendererToRemove1);
			}
			rendererToRemove.NativeView.RemoveFromSuperview ();
			rendererToRemove.Dispose ();
		}

		private void HandleChildRemoved (object sender, ElementEventArgs e)
		{
			this.DisposeModelAndChildrenRenderers (e.Element);
		}

		Task INavigation.PushAsync (Page root)
		{
			return ((INavigation)this).PushAsync (root, true);
		}

		Task<Page> INavigation.PopAsync ()
		{
			return ((INavigation)this).PopAsync (true);
		}

		Task INavigation.PopToRootAsync ()
		{
			return ((INavigation)this).PopToRootAsync (true);
		}

		Task INavigation.PushAsync (Page root, bool animated)
		{
			throw new InvalidOperationException ("PushAsync is not supported globally on Mac, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopAsync (bool animated)
		{
			throw new InvalidOperationException ("PopAsync is not supported globally on Mac, please use a NavigationPage.");
		}

		Task INavigation.PopToRootAsync (bool animated)
		{
			throw new InvalidOperationException ("PopToRootAsync is not supported globally on Mac, please use a NavigationPage.");
		}

		void INavigation.RemovePage (Page page)
		{
			throw new InvalidOperationException ("RemovePage is not supported globally on Mac, please use a NavigationPage.");
		}

		void INavigation.InsertPageBefore (Page page, Page before)
		{
			throw new InvalidOperationException ("InsertPageBefore is not supported globally on Mac, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync (Page modal)
		{
			return ((INavigation)this).PushModalAsync (modal, true);
		}

		Task INavigation.PushModalAsync (Page modal, bool animated)
		{
			this.modals.Add (modal);
			modal.Platform = (IPlatform)this;

			modal.DescendantRemoved += HandleChildRemoved;

			if (this.appeared)
				return this.PresentModal (modal, this.animateModals & animated);
			
			return (Task)Task.FromResult<object> (null);
		}

		private async Task PresentModal (Page modal, bool animated)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer ((VisualElement)modal);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer ((VisualElement)modal);
				Platform.SetRenderer ((VisualElement)modal, renderer);
			}
			ModalWrapper wrapper = new ModalWrapper (renderer);
			if (this.modals.Count > 1)
			{
				var uiViewController = Platform.GetRenderer ((VisualElement)this.modals [this.modals.Count - 2]) as NSViewController;
				if (uiViewController != null)
				{
					uiViewController.PresentViewController((NSViewController)wrapper, null);
					await Task.Delay (5);

					return;
				}
			}

			this.renderer.PresentViewController ((NSViewController)wrapper, null);
			await Task.Delay (5);
		}

		Task<Page> INavigation.PopModalAsync ()
		{
			return ((INavigation)this).PopModalAsync (true);
		}

		Task<Page> INavigation.PopModalAsync (bool animated)
		{
			Page modal = Enumerable.Last<Page> ((IEnumerable<Page>)this.modals);
			this.modals.Remove (modal);
			modal.DescendantRemoved -= HandleChildRemoved;

			var uiViewController = Platform.GetRenderer ((VisualElement)modal) as NSViewController;
			if (this.modals.Count >= 1 && uiViewController != null)
			{
				uiViewController.DismissViewController(uiViewController.PresentingViewController);
			}
			else
			{
				renderer.DismissViewController (uiViewController.PresentingViewController);
			}
			this.DisposeModelAndChildrenRenderers ((Element)modal);
			return Task.FromResult(modal);
		}

		internal void LayoutSubviews ()
		{
			if (this.root == null)
				return;
			IVisualElementRenderer renderer = Platform.GetRenderer ((VisualElement)this.root);
			if (renderer == null)
				return;
			renderer.SetElementSize (new Size ((double)this.renderer.View.Bounds.Width, (double)this.renderer.View.Bounds.Height));
		}

		private void AddChild (VisualElement view)
		{
			if (!Application.IsApplicationOrNull (view.Parent))
				Console.Error.WriteLine ("Tried to add parented view to canvas directly");
			if (Platform.GetRenderer (view) == null)
			{
				IVisualElementRenderer renderer = Platform.CreateRenderer (view);
				Platform.SetRenderer (view, renderer);
				// ISSUE: reference to a compiler-generated method
				this.renderer.View.AddSubview (renderer.NativeView);
				if (renderer.ViewController != null)
				{
					// ISSUE: reference to a compiler-generated method
					this.renderer.AddChildViewController (renderer.ViewController);
				}
				renderer.NativeView.Frame = new CGRect ((nfloat)0, (nfloat)0, this.renderer.View.Bounds.Width, this.renderer.View.Bounds.Height);
				renderer.SetElementSize (new Size ((double)this.renderer.View.Bounds.Width, (double)this.renderer.View.Bounds.Height));
			}
			else
				Console.Error.WriteLine ("Potential view double add");
		}

		SizeRequest IPlatform.GetNativeSize (VisualElement view, double widthConstraint, double heightConstraint)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer (view);
			if (renderer == null || renderer.NativeView == null)
				return new SizeRequest (Size.Zero);
			return renderer.GetDesiredSize (widthConstraint, heightConstraint);
		}

		protected override void OnBindingContextChanged ()
		{
			BindableObject.SetInheritedBindingContext ((BindableObject)this.Page, this.BindingContext);
			base.OnBindingContextChanged ();
		}

		internal class DefaultRenderer : VisualElementRenderer<VisualElement>
		{
		}
	}
}
