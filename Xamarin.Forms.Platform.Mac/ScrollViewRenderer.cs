using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class ScrollViewRenderer : NSScrollView, IVisualElementRenderer, IDisposable, IRegisterable
	{
		private VisualElementPackager packager;
		private VisualElementTracker tracker;
		private EventTracker events;
		//private KeyboardInsetTracker insetTracker;
		private ScrollToRequestedEventArgs requestedScroll;
		private CGRect previousFrame;

		protected IScrollViewController Controller
		{
			get	{ return Element as IScrollViewController; }
		}

		public VisualElement Element { get; set; }

		public NSView NativeView
		{
			get	{ return this; }
		}

		private ScrollView scrollView
		{
			get	{ return this.Element as ScrollView; }
		}

		public NSViewController ViewController
		{
			get	{ return null; }
		}

		public ScrollViewRenderer() : base(CGRect.Empty)
		{
			// Not for Mac
			//base.ScrollAnimationEnded += new EventHandler(this.HandleScrollAnimationEnded);
			//base.Scrolled += new EventHandler(this.HandleScrolled);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.packager == null)
					return;

				this.SetElement(null);
				this.packager.Dispose();
				this.packager = null;
				this.tracker.NativeControlUpdated -= new EventHandler(this.OnNativeControlUpdated);
				this.tracker.Dispose();
				this.tracker = null;
				this.events.Dispose();
				this.events = null;

				//this.insetTracker.Dispose();
				//this.insetTracker = null;

				// Not for Mac
				//base.ScrollAnimationEnded -= new EventHandler(this.HandleScrollAnimationEnded);
				//base.Scrolled -= new EventHandler(this.HandleScrolled);
			}
			base.Dispose(disposing);
		}

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return (NativeView as NSControl).GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ScrollView.ContentSizeProperty.PropertyName)
			{
				this.UpdateContentSize();
				return;
			}
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				this.UpdateBackgroundColor();
			}
		}

		private void HandleScrollAnimationEnded(object sender, EventArgs e)
		{
			this.Controller.SendScrollFinished();
		}

		private void HandleScrolled(object sender, EventArgs e)
		{
			this.UpdateScrollPosition();
		}

		public override void Layout()
		{
			base.Layout();

			if (this.requestedScroll != null && this.Superview != null)
			{
				ScrollToRequestedEventArgs scrollToRequestedEventArg = this.requestedScroll;
				this.requestedScroll = null;
				this.OnScrollToRequested(this, scrollToRequestedEventArg);
			}
			if (this.previousFrame != this.Frame)
			{
				this.previousFrame = this.Frame;
				// Not for Mac
				/*
				KeyboardInsetTracker keyboardInsetTracker = this.insetTracker;
				if (keyboardInsetTracker == null)
				{
					return;
				}
				keyboardInsetTracker.UpdateInsets();
				*/
			}
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		private void OnNativeControlUpdated(object sender, EventArgs eventArgs)
		{
			// WT.?
			//this.ContentSize = this.Bounds.Size;
			this.UpdateContentSize();
		}

		private void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
		{
			if (this.Superview == null)
			{
				this.requestedScroll = e;
				return;
			}
			if (e.Mode != ScrollToMode.Position)
			{
				Point scrollPositionForElement = this.Controller.GetScrollPositionForElement(e.Element as VisualElement, e.Position);
				if (scrollView.Orientation != ScrollOrientation.Horizontal)
				{
					//TODO: Scroll to Position
					//VerticalScroller.FloatValue = e.Position;

					//SetContentOffset(new CGPoint(contentOffset.X, (nfloat)scrollPositionForElement.Y), e.ShouldAnimate);
				}
				else
				{
					// TODO: Scroll to Position
					//HorizontalScroller.FloatValue = e.Position;

					//nfloat x = (nfloat)scrollPositionForElement.X;
					//SetContentOffset(new CGPoint(x, contentOffset.Y), e.ShouldAnimate);
				}
			}
			else
			{
				VerticalScroller.FloatValue = (float)e.ScrollY;
				HorizontalScroller.FloatValue = (float)e.ScrollX;
				//this.SetContentOffset(new CGPoint((nfloat)e.ScrollX, (nfloat)e.ScrollY), e.ShouldAnimate);
			}
			if (!e.ShouldAnimate)
			{
				this.Controller.SendScrollFinished();
			}
		}

		public void SetElement(VisualElement element)
		{
			this.requestedScroll = null;
			VisualElement visualElement = this.Element;
			this.Element = element;
			if (visualElement != null)
			{
				visualElement.PropertyChanged -= new PropertyChangedEventHandler(this.HandlePropertyChanged);
				((IScrollViewController)visualElement).ScrollToRequested -= new EventHandler<ScrollToRequestedEventArgs>(this.OnScrollToRequested);
			}
			if (element != null)
			{
				element.PropertyChanged += new PropertyChangedEventHandler(this.HandlePropertyChanged);
				((IScrollViewController)element).ScrollToRequested += new EventHandler<ScrollToRequestedEventArgs>(this.OnScrollToRequested);
				if (this.packager == null)
				{
					//this.DelaysContentTouches = true;

					this.packager = new VisualElementPackager(this);
					this.packager.Load();
					this.tracker = new VisualElementTracker(this);
					this.tracker.NativeControlUpdated += new EventHandler(this.OnNativeControlUpdated);
					this.events = new EventTracker(this);
					this.events.LoadEvents(this);

					// TODO: WT.?
					/*
					this.insetTracker = new KeyboardInsetTracker(this, () => this.Window, (UIEdgeInsets insets) => {
						UIEdgeInsets uIEdgeInset = insets;
						UIEdgeInsets uIEdgeInset1 = uIEdgeInset;
						this.ScrollIndicatorInsets = uIEdgeInset;
						this.ContentInset = uIEdgeInset1;
					}, (CGPoint point) => {
						CGPoint contentOffset = this.ContentOffset;
						contentOffset.Y = contentOffset.Y + point.Y;
						this.SetContentOffset(contentOffset, true);
					});
					*/
				}
				this.UpdateContentSize();
				this.UpdateBackgroundColor();
				this.OnElementChanged(new VisualElementChangedEventArgs(visualElement, element));
				if (element != null)
				{
					element.SendViewInitialized(this);
				}
			}
		}

		public void SetElementSize(Size size)
		{
			this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
		}

		private void UpdateBackgroundColor()
		{
			this.BackgroundColor = this.Element.BackgroundColor.ToUIColor(Color.Transparent);
		}

		private void UpdateContentSize()
		{
			CGSize sizeF = ((ScrollView)this.Element).ContentSize.ToSizeF();
			if (!sizeF.IsEmpty)
			{
				// TODO: Update content size
				//this.ContentSize = sizeF;
			}
		}

		private void UpdateScrollPosition()
		{
			if (this.scrollView != null)
			{
				Controller.SetScrolledPosition(HorizontalScroller.FloatValue, VerticalScroller.FloatValue);
			}
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
	}
}
