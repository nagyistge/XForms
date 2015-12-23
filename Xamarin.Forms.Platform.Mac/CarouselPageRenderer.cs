using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class CarouselPageRenderer : NSViewController, IVisualElementRenderer, IDisposable, IRegisterable
	{
		private NSScrollView scrollView;
		private Dictionary<Page, NSView> containerMap;
		private VisualElementTracker tracker;
		private EventTracker events;
		private bool disposed;
		private bool appeared;
		private bool ignoreNativeScrolling;

		public VisualElement Element { get; private set; }

		protected int SelectedIndex
		{
			get	{ return (int)(this.scrollView.ContentSize.Width / this.scrollView.Frame.Width); }
			set	{ this.ScrollToPage (value, true); }
		}

		protected CarouselPage Carousel
		{
			get	{ return (CarouselPage)this.Element; }
		}

		public NSView NativeView
		{
			get	{ return this.View; }
		}

		public NSViewController ViewController
		{
			get	{ return this; }
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public void SetElement (VisualElement element)
		{
			VisualElement element1 = this.Element;
			this.Element = element;
			this.containerMap = new Dictionary<Page, NSView> ();
			this.OnElementChanged (new VisualElementChangedEventArgs (element1, element));
			if (element == null)
				return;
			Forms.SendViewInitialized (element, this.NativeView);
		}

		public void SetElementSize (Size size)
		{
			this.Element.Layout (new Rectangle (this.Element.X, this.Element.Y, size.Width, size.Height));
		}

		public SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
		{
			var control = NativeView as NSControl;
			if (control == null)
				return new SizeRequest (new Size (widthConstraint, heightConstraint), new Size(-1.0, -1.0));
			
			return control.GetSizeRequest (widthConstraint, heightConstraint, -1.0, -1.0);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.tracker = new VisualElementTracker ((IVisualElementRenderer)this);
			this.events = new EventTracker ((IVisualElementRenderer)this);
			this.events.LoadEvents (this.View);
			this.scrollView = new NSScrollView () {
				AutohidesScrollers = false
			};

			// TODO: Is there Decellerate Scrollview for OS X
			// this.scrollView.DecelerationEnded += new EventHandler (this.OnDecelerationEnded);

			this.View.AddSubview( scrollView );

			for (int index = 0; index < this.Element.LogicalChildren.Count ; ++index)
			{
				ContentPage page = this.Element.LogicalChildren [index] as ContentPage;
				if (page != null)
					InsertPage (page, index);
			}

			this.PositionChildren ();
			this.Carousel.PropertyChanged += OnPropertyChanged;
			this.Carousel.PagesChanged += OnPagesChanged;
		}

		protected virtual void OnElementChanged (VisualElementChangedEventArgs e)
		{
			EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
			if (eventHandler == null)
				return;
			eventHandler ((object)this, e);
		}

		// TODO: Decelerate
		/*
		private void OnDecelerationEnded (object sender, EventArgs eventArgs)
		{
			if (this.ignoreNativeScrolling || this.SelectedIndex >= this.Element.LogicalChildren ().Count)
				return;
			this.Carousel.CurrentPage = (ContentPage)this.Element.LogicalChildren () [this.SelectedIndex];
		}
		*/

		private void Clear ()
		{
			foreach (KeyValuePair<Page, NSView> keyValuePair in this.containerMap)
			{
				keyValuePair.Value.RemoveFromSuperview ();
				IVisualElementRenderer renderer = Platform.GetRenderer ((VisualElement)keyValuePair.Key);
				if (renderer != null)
				{
					// TODO: View Controller
					//renderer.ViewController.RemoveFromParentViewController ();

					renderer.NativeView.RemoveFromSuperview ();
					Platform.SetRenderer ((VisualElement)keyValuePair.Key, (IVisualElementRenderer)null);
				}
			}
			this.containerMap.Clear ();
		}

		private void Reset ()
		{
			this.Clear ();
			for (int index = 0; index < this.Element.LogicalChildren.Count; ++index)
			{
				ContentPage page = this.Element.LogicalChildren[index] as ContentPage;
				if (page != null)
					InsertPage (page, index);
			}
		}

		private void InsertPage (ContentPage page, int index)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer ((VisualElement)page);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer ((VisualElement)page);
				Platform.SetRenderer ((VisualElement)page, renderer);
			}
			NSView view = (NSView)new CarouselPageRenderer.PageContainer ((VisualElement)page);
			view.AddSubview (renderer.NativeView);

			this.containerMap [(Page)page] = view;
			this.AddChildViewController (renderer.ViewController);

			var otherView = index < scrollView.Subviews.Length ? scrollView.Subviews[index] : (NSView)null;
			this.scrollView.AddSubview( view, NSWindowOrderingMode.Below, otherView );
			if ((index != 0 || this.SelectedIndex != 0) && index >= this.SelectedIndex)
				return;
			this.ScrollToPage (this.SelectedIndex + 1, false);
		}

		private void RemovePage (ContentPage page, int index)
		{
			this.containerMap [(Page)page].RemoveFromSuperview ();
			this.containerMap.Remove ((Page)page);

			IVisualElementRenderer renderer = Platform.GetRenderer ((VisualElement)page);
			if (renderer == null)
				return;
			
			renderer.ViewController.RemoveFromParentViewController ();
			renderer.NativeView.RemoveFromSuperview ();
		}

		private void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (!(e.PropertyName == "CurrentPage"))
				return;
			this.UpdateCurrentPage (true);
		}

		private void OnPagesChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			this.ignoreNativeScrolling = true;
			int num1 = (int)NotifyCollectionChangedEventArgsExtensions.Apply (e, (Action<object, int, bool>)((o, i, c) => this.InsertPage ((ContentPage)o, i)), (Action<object, int>)((o, i) => this.RemovePage ((ContentPage)o, i)), new Action (this.Reset));
			this.PositionChildren ();
			this.ignoreNativeScrolling = false;
			int num2 = 4;
			if (num1 != num2)
				return;
			int index = this.Carousel.CurrentPage != null ? MultiPage<ContentPage>.GetIndex (this.Carousel.CurrentPage) : 0;
			if (index < 0)
				index = 0;
			this.ScrollToPage (index, true);
		}


		public override void ViewDidLayout ()
		{
			base.ViewDidLayout ();
			this.View.Frame = this.View.Superview.Bounds;
			this.scrollView.Frame = this.View.Bounds;
			this.PositionChildren ();
			this.UpdateCurrentPage (false);
		}

		/*
		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			this.ignoreNativeScrolling = true;
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			this.ignoreNativeScrolling = false;
		}
		*/

		private void UpdateCurrentPage (bool animated = true)
		{
			ContentPage currentPage = this.Carousel.CurrentPage;
			if (currentPage == null)
				return;
			this.ScrollToPage (MultiPage<ContentPage>.GetIndex (currentPage), animated);
		}

		private void ScrollToPage (int index, bool animated = true)
		{
			if (this.scrollView.HorizontalScroller.FloatValue == (nfloat)index * this.scrollView.Frame.Width)
				return;

			this.scrollView.HorizontalScroller.FloatValue =(float)(index * scrollView.Frame.Width);
		}

		private void PositionChildren ()
		{
			nfloat x = (nfloat)0;
			CGRect bounds = this.View.Bounds;

			foreach(Page page in Element.LogicalChildren)
			{
				this.containerMap[page].Frame = new CGRect (x, bounds.Y, bounds.Width, bounds.Height);
				x += bounds.Width;
			}

			// TODO: Does this mater
			//this.scrollView.PagingEnabled = true;
			//this.scrollView.ContentSize = new CGSize (bounds.Width * (nfloat)((MultiPage<ContentPage>)this.Element).get_Children ().Count, bounds.Height);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.Carousel.PropertyChanged -= OnPropertyChanged;

				Platform.SetRenderer (this.Element, (IVisualElementRenderer)null);
				this.Clear ();
				if (this.scrollView != null)
				{
					// Not for Mac
					//this.scrollView.DecelerationEnded -= new EventHandler (this.OnDecelerationEnded);

					this.scrollView.RemoveFromSuperview ();
					this.scrollView = (NSScrollView)null;
				}
				if (this.appeared)
				{
					this.appeared = false;
					this.Carousel.SendDisappearing ();
				}
				if (this.events != null)
				{
					this.events.Dispose ();
					this.events = (EventTracker)null;
				}
				if (this.tracker != null)
				{
					this.tracker.Dispose ();
					this.tracker = (VisualElementTracker)null;
				}
				this.Element = (VisualElement)null;
				this.disposed = true;
			}
			// ISSUE: reference to a compiler-generated method
			base.Dispose (disposing);
		}

		public override void ViewDidAppear ()
		{
			base.ViewDidAppear ();

			if (appeared || disposed)
				return;
			
			this.appeared = true;
			this.Carousel.SendAppearing ();
		}

		public override void ViewDidDisappear ()
		{
			base.ViewDidDisappear ();

			if (!appeared || disposed)
				return;
			
			this.appeared = false;
			this.Carousel.SendDisappearing ();
		}

		private class PageContainer : NSView
		{
			public VisualElement Element { get; private set; }

			public PageContainer (VisualElement element)
			{
				this.Element = element;
			}

			public override void Layout ()
			{
				base.Layout ();

				if (this.Subviews.Length == 0)
					return;
				this.Subviews [0].Frame = new CGRect ((nfloat)0, (nfloat)0, (nfloat)((float)this.Element.Width), (nfloat)((float)this.Element.Height));
			}
		}
	}
}
