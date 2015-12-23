using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class TabletMasterDetailRenderer : NSSplitViewController, IVisualElementRenderer, IDisposable, IRegisterable
	{
		private VisualElementTracker tracker;
		private EventTracker events;
		private EventedViewController masterController;
		private NSViewController detailController;
		private TabletMasterDetailRenderer.InnerDelegate innerDelegate;
		private MasterDetailPage masterDetailPage;
		private bool masterVisible;
		private bool disposed;

		public VisualElement Element { get; private set; }

		protected MasterDetailPage MasterDetailPage
		{
			get
			{
				return this.masterDetailPage ?? (this.masterDetailPage = (MasterDetailPage)this.Element);
			}
		}

		private NSToolbarItem PresentButton
		{
			get
			{
				if (this.innerDelegate != null)
					return this.innerDelegate.PresentButton;
				
				return null;
			}
		}

		public NSView NativeView
		{
			get
			{
				return this.View;
			}
		}

		public NSViewController ViewController
		{
			get { return this; }
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public void SetElement (VisualElement element)
		{
			VisualElement element1 = this.Element;
			this.Element = element;
			masterController = new EventedViewController ();
			detailController = new ChildViewController ();

			ChildViewControllers = new NSViewController[] {	masterController, detailController };

			// TODO: Delegate
			//this.Delegate = (IUISplitViewControllerDelegate)(this.innerDelegate = new TabletMasterDetailRenderer.InnerDelegate (this.MasterDetailPage.MasterBehavior));

			this.UpdateControllers ();
			this.masterController.WillAppear += new EventHandler (this.MasterControllerWillAppear);
			this.masterController.WillDisappear += new EventHandler (this.MasterControllerWillDisappear);
			//this.PresentsWithGesture = this.MasterDetailPage.IsGestureEnabled;
			this.OnElementChanged (new VisualElementChangedEventArgs (element1, element));
			if (element == null)
				return;
			Forms.SendViewInitialized (element, this.NativeView);
		}

		private void MasterControllerWillDisappear (object sender, EventArgs e)
		{
			this.masterVisible = false;
			if (!this.MasterDetailPage.CanChangeIsPresented)
				return;
			((IElementController)Element).SetValueFromRenderer (MasterDetailPage.IsPresentedProperty, false);
		}

		private void MasterControllerWillAppear (object sender, EventArgs e)
		{
			this.masterVisible = true;
			if (!this.MasterDetailPage.CanChangeIsPresented)
				return;
			((IElementController)Element).SetValueFromRenderer (MasterDetailPage.IsPresentedProperty, true);
		}

		public void SetElementSize (Size size)
		{
			this.Element.Layout (new Rectangle (this.Element.X, this.Element.Width, size.Width, size.Height));
		}

		public SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
		{
			return (NativeView as NSControl).GetSizeRequest (widthConstraint, heightConstraint, -1.0, -1.0);
		}

		/*
		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (!this.MasterDetailPage.ShouldShowSplitMode && this.masterVisible)
			{
				this.MasterDetailPage.CanChangeIsPresented = true;
				if (Forms.IsiOS8OrNewer)
				{
					this.PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
					this.PreferredDisplayMode = UISplitViewControllerDisplayMode.Automatic;
				}
			}
			MasterDetailPage.UpdateMasterBehavior (this.MasterDetailPage);
			MessagingCenter.Send<IVisualElementRenderer> ((IVisualElementRenderer)this, "Xamarin.UpdateToolbarButtons");
			// ISSUE: reference to a compiler-generated method
			base.WillRotate (toInterfaceOrientation, duration);
		}
		*/

		public override void ViewDidLoad ()
		{
			// ISSUE: reference to a compiler-generated method
			base.ViewDidLoad ();
			this.UpdateBackground ();
			this.tracker = new VisualElementTracker ((IVisualElementRenderer)this);
			this.events = new EventTracker ((IVisualElementRenderer)this);
			this.events.LoadEvents (this.NativeView);
		}

		public override void ViewDidLayout ()
		{
			base.ViewDidLayout ();

			if (this.View.Subviews.Length < 2)
				return;
			CGRect frame1 = this.detailController.View.Frame;
			CGRect frame2 = this.masterController.View.Frame;
			if (!frame2.IsEmpty)
				this.MasterDetailPage.MasterBounds = new Rectangle (0.0, 0.0, (double)frame2.Width, (double)frame2.Height);
			if (frame1.IsEmpty)
				return;
			this.MasterDetailPage.DetailBounds = new Rectangle (0.0, 0.0, (double)frame1.Width, (double)frame1.Height);
		}

		public override void ViewWillLayout ()
		{
			base.ViewWillLayout ();
			this.masterController.View.SetBackgroundColor( NSColor.White );
		}

		protected virtual void OnElementChanged (VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
				e.OldElement.PropertyChanged -= HandlePropertyChanged;
			if (e.NewElement != null)
				e.NewElement.PropertyChanged += HandlePropertyChanged;

			EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
			if (eventHandler == null)
				return;
			eventHandler (this, e);
		}

		public new void Dispose ()
		{
			if (this.disposed)
				return;
			if (this.Element != null)
			{
				this.Element.PropertyChanged -= HandlePropertyChanged;
				this.Element = (VisualElement)null;
			}
			if (this.tracker != null)
			{
				this.tracker.Dispose ();
				this.tracker = (VisualElementTracker)null;
			}
			if (this.events != null)
			{
				this.events.Dispose ();
				this.events = (EventTracker)null;
			}
			if (this.masterController != null)
			{
				this.masterController.WillAppear -= new EventHandler (this.MasterControllerWillAppear);
				this.masterController.WillDisappear -= new EventHandler (this.MasterControllerWillDisappear);
			}
			this.disposed = true;
		}

		private void ToggleMaster ()
		{
			if (this.masterVisible == this.MasterDetailPage.IsPresented || this.MasterDetailPage.ShouldShowSplitMode)
				return;
			this.PerformButtonSelector ();
		}

		private void PerformButtonSelector ()
		{
			this.PresentButton.Target.PerformSelector (this.PresentButton.Action, (NSObject)this.PresentButton, 0.0);
		}

		private void HandlePropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (this.tracker == null)
				return;
			if (e.PropertyName == "Master" || e.PropertyName == "Detail")
				this.UpdateControllers ();
			else if (e.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName)
				this.ToggleMaster ();
			/*
			else if (e.PropertyName == MasterDetailPage.IsGestureEnabledProperty.PropertyName)
				this.PresentsWithGesture = this.MasterDetailPage.IsGestureEnabled;
				*/
			MessagingCenter.Send<IVisualElementRenderer> ((IVisualElementRenderer)this, "Xamarin.UpdateToolbarButtons");
		}

		private void UpdateControllers ()
		{
			this.MasterDetailPage.Master.PropertyChanged -= HandleMasterPropertyChanged;
			if (Platform.GetRenderer ((VisualElement)this.MasterDetailPage.Master) == null)
				Platform.SetRenderer ((VisualElement)this.MasterDetailPage.Master, Platform.CreateRenderer ((VisualElement)this.MasterDetailPage.Master));
			if (Platform.GetRenderer ((VisualElement)this.MasterDetailPage.Detail) == null)
				Platform.SetRenderer ((VisualElement)this.MasterDetailPage.Detail, Platform.CreateRenderer ((VisualElement)this.MasterDetailPage.Detail));
			this.ClearControllers ();
			this.MasterDetailPage.Master.PropertyChanged += HandleMasterPropertyChanged;
			NSViewController viewController1 = Platform.GetRenderer ((VisualElement)this.MasterDetailPage.Master).ViewController;
			NSViewController viewController2 = Platform.GetRenderer ((VisualElement)this.MasterDetailPage.Detail).ViewController;

			this.masterController.View.AddSubview (viewController1.View);
			this.masterController.AddChildViewController (viewController1);
			this.detailController.View.AddSubview (viewController2.View);
			this.detailController.AddChildViewController (viewController2);
		}

		private void ClearControllers ()
		{
			foreach (NSViewController uiViewController in this.masterController.ChildViewControllers)
			{
				uiViewController.View.RemoveFromSuperview ();
				uiViewController.RemoveFromParentViewController ();
			}
			foreach (NSViewController uiViewController in this.detailController.ChildViewControllers)
			{
				uiViewController.View.RemoveFromSuperview ();
				uiViewController.RemoveFromParentViewController ();
			}
		}

		private void HandleMasterPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (!(e.PropertyName == Page.IconProperty.PropertyName) && !(e.PropertyName == Page.TitleProperty.PropertyName))
				return;
			MessagingCenter.Send<IVisualElementRenderer> ((IVisualElementRenderer)this, "Xamarin.UpdateToolbarButtons");
		}

		private void UpdateBackground ()
		{
			// TODO: Color from image?
			/*
			if (!string.IsNullOrEmpty (((Page)this.Element).BackgroundImage))
			{
				this.View.SetBackgroundColor( NSColor.FromPatternImage (UIImage.FromBundle (((Page)this.Element).BackgroundImage)));
			}
			else 
			*/

			if (this.Element.BackgroundColor == Color.Default)
				this.View.SetBackgroundColor( NSColor.White );
			else
				this.View.SetBackgroundColor( ColorExtensions.ToUIColor (this.Element.BackgroundColor) );
		}


		/*
		public override void ViewDidAppear (bool animated)
		{
			// ISSUE: reference to a compiler-generated method
			base.ViewDidAppear (animated);
			this.MasterDetailPage.SendAppearing ();
			this.ToggleMaster ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			if (this.masterVisible)
				this.PerformButtonSelector ();
			// ISSUE: reference to a compiler-generated method
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			// ISSUE: reference to a compiler-generated method
			base.ViewDidDisappear (animated);
			this.MasterDetailPage.SendDisappearing ();
		}
		*/

		private class InnerDelegate : NSSplitViewDelegate
		{
			private MasterBehavior masterPresentedDefaultState;

			public NSToolbarItem PresentButton { get; set; }

			public InnerDelegate (MasterBehavior masterPresentedDefaultState)
			{
				this.masterPresentedDefaultState = masterPresentedDefaultState;
			}

			/*
			public override void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc)
			{
				this.PresentButton = barButtonItem;
			}

			public override bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation)
			{
				bool flag;
				switch (this.masterPresentedDefaultState)
				{
				case MasterBehavior.Split:
					flag = false;
					break;
				case MasterBehavior.Popover:
					flag = true;
					break;
				case MasterBehavior.SplitOnPortrait:
					flag = inOrientation != UIInterfaceOrientation.Portrait && inOrientation != UIInterfaceOrientation.PortraitUpsideDown;
					break;
				default:
					flag = inOrientation == UIInterfaceOrientation.Portrait || inOrientation == UIInterfaceOrientation.PortraitUpsideDown;
					break;
				}
				return flag;
			}
			*/
		}
	}
}
