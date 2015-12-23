using CoreGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	[Flags]
	public enum VisualElementRendererFlags
	{
		Disposed = 1,
		AutoTrack = 2,
		AutoPackage = 4,
	}

	public class VisualElementRenderer<TElement> : NSView, IVisualElementRenderer, IDisposable, IRegisterable where TElement : VisualElement
	{
		private readonly List<EventHandler<VisualElementChangedEventArgs>> elementChangedHandlers = new List<EventHandler<VisualElementChangedEventArgs>> ();
		private NSColor defaultColor = NSColor.Clear;
		private VisualElementRendererFlags flags = VisualElementRendererFlags.AutoTrack | VisualElementRendererFlags.AutoPackage;
		private VisualElementPackager packager;
		private VisualElementTracker tracker;
		private EventTracker events;
		private readonly PropertyChangedEventHandler propertyChangedHandler;

		public NSView NativeView
		{
			get	{	return (NSView)this;	}
		}

		public NSViewController ViewController
		{
			get	{	return (NSViewController)null;	}
		}

		public TElement Element { get; private set; }

		VisualElement IVisualElementRenderer.Element
		{
			get	{	return (VisualElement)this.Element;	}
		}

		protected bool AutoPackage
		{
			get
			{
				return (uint)(this.flags & VisualElementRendererFlags.AutoPackage) > 0U;
			}
			set
			{
				if (value)
					this.flags = this.flags | VisualElementRendererFlags.AutoPackage;
				else
					this.flags = this.flags & ~VisualElementRendererFlags.AutoPackage;
			}
		}

		protected bool AutoTrack
		{
			get
			{
				return (uint)(this.flags & VisualElementRendererFlags.AutoTrack) > 0U;
			}
			set
			{
				if (value)
					this.flags = this.flags | VisualElementRendererFlags.AutoTrack;
				else
					this.flags = this.flags & ~VisualElementRendererFlags.AutoTrack;
			}
		}

		public event EventHandler<ElementChangedEventArgs<TElement>> ElementChanged;

		event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
		{
			add
			{
				this.elementChangedHandlers.Add (value);
			}
			remove
			{
				this.elementChangedHandlers.Remove (value);
			}
		}

		protected VisualElementRenderer ()
			: base (CGRect.Empty)
		{
			this.propertyChangedHandler = new PropertyChangedEventHandler (this.OnElementPropertyChanged);
			this.SetBackgroundColor( NSColor.Clear );
		}

		void IVisualElementRenderer.SetElement (VisualElement element)
		{
			this.SetElement ((TElement)element);
		}

		public void SetElement (TElement element)
		{
			TElement element1 = this.Element;
			this.Element = element;
			if ((object)element1 != null)
				element1.PropertyChanged -= this.propertyChangedHandler;
			if ((object)element != null)
			{
				if (element.BackgroundColor != Color.Default || (object)element1 != null && element.BackgroundColor != element1.BackgroundColor)
					this.SetBackgroundColor (element.BackgroundColor);

				// ToDo: ClipToBounds
				//this.UpdateClipToBounds ();

				if (this.tracker == null)
				{
					this.tracker = new VisualElementTracker ((IVisualElementRenderer)this);
					this.tracker.NativeControlUpdated += (EventHandler)((sender, e) => this.UpdateNativeWidget ());
				}
				if (this.AutoPackage && this.packager == null)
				{
					this.packager = new VisualElementPackager ((IVisualElementRenderer)this);
					this.packager.Load ();
				}
				if (this.AutoTrack && this.events == null)
				{
					this.events = new EventTracker ((IVisualElementRenderer)this);
					this.events.LoadEvents ((NSView)this);
				}
				element.PropertyChanged += this.propertyChangedHandler;
			}
			this.OnElementChanged (new ElementChangedEventArgs<TElement> (element1, element));
			if ((object)element == null)
				return;
			this.SendVisualElementInitialized ((VisualElement)element, (NSView)this);
		}

		internal virtual void SendVisualElementInitialized (VisualElement element, NSView nativeView)
		{
			Forms.SendViewInitialized (element, nativeView);
		}

		public void SetElementSize (Size size)
		{
			this.Element.Layout (new Rectangle (this.Element.X, this.Element.Y, size.Width, size.Height));
		}

		public virtual SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
		{
			return (NativeView as NSControl).GetSizeRequest (widthConstraint, heightConstraint, -1.0, -1.0);
		}

		protected virtual void OnElementChanged (ElementChangedEventArgs<TElement> e)
		{
			VisualElementChangedEventArgs e1 = new VisualElementChangedEventArgs ((VisualElement)e.OldElement, (VisualElement)e.NewElement);
			for (int index = 0; index < this.elementChangedHandlers.Count; ++index)
				this.elementChangedHandlers [index] ((object)this, e1);
			
			var eventHandler = this.ElementChanged;
			if (eventHandler == null)
				return;
			eventHandler (this, e);
		}

		protected virtual void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				this.SetBackgroundColor (this.Element.BackgroundColor);
			}
			// TODO: ClippedToBounds
			/*
			else
			{
				if (!(e.PropertyName == Layout.IsClippedToBoundsProperty.PropertyName))
					return;
				this.UpdateClipToBounds ();
			}
			*/
		}

		/*
		private void UpdateClipToBounds ()
		{
			Layout layout = (object)this.Element as Layout;
			if (layout == null)
				return;
			this.ClipsToBounds = layout.IsClippedToBounds;
		}
		*/

		protected virtual void UpdateNativeWidget ()
		{
		}

		protected virtual void SetBackgroundColor (Color color)
		{
		}

		/*
		public override CGSize SizeThatFits (CGSize size)
		{
			return new CGSize ((nfloat)0, (nfloat)0);
		}
		*/

		protected override void Dispose (bool disposing)
		{
			if ((this.flags & VisualElementRendererFlags.Disposed) != (VisualElementRendererFlags)0)
				return;
			this.flags = this.flags | VisualElementRendererFlags.Disposed;
			if (disposing)
			{
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
				if (this.packager != null)
				{
					this.packager.Dispose ();
					this.packager = (VisualElementPackager)null;
				}
				Platform.SetRenderer ((VisualElement)this.Element, (IVisualElementRenderer)null);
				this.SetElement (default (TElement));
				this.Element = default (TElement);
			}

			base.Dispose (disposing);
		}
	}
}
