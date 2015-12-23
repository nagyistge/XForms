using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView> 
		where TView : View 
		where TNativeView : NSView
	{
		private NSColor defaultColor;

		public TNativeView Control { get; private set; }

		protected override void OnElementChanged (ElementChangedEventArgs<TView> e)
		{
			base.OnElementChanged (e);
			if ((object)e.NewElement != null)
			{
				if ((object)this.Control != null && (object)e.OldElement != null && e.OldElement.BackgroundColor != e.NewElement.BackgroundColor || e.NewElement.BackgroundColor != Color.Default)
					this.SetBackgroundColor (e.NewElement.BackgroundColor);
			}
			this.UpdateIsEnabled ();
		}

		protected void SetNativeControl (TNativeView uiview)
		{
			if (uiview.WantsLayer)
				this.defaultColor = NSColor.FromCGColor(uiview.Layer.BackgroundColor);
			
			this.Control = uiview;
			if (this.Element.BackgroundColor != Color.Default)
				this.SetBackgroundColor (this.Element.BackgroundColor);
			this.UpdateIsEnabled ();
			this.AddSubview ((NSView)uiview);
		}

		internal override void SendVisualElementInitialized (VisualElement element, NSView nativeView)
		{
			base.SendVisualElementInitialized (element, (NSView)this.Control);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if ((object)this.Control != null)
			{
				if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
					this.UpdateIsEnabled ();
				else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
					this.SetBackgroundColor (this.Element.BackgroundColor);
			}
			base.OnElementPropertyChanged (sender, e);
		}

		public override void Layout ()
		{
			base.Layout ();
			if (Control == null)
				return;
			
			Control.Frame = new CGRect ((nfloat)0, (nfloat)0, (nfloat)this.Element.Width, (nfloat)this.Element.Height);
		}

		protected override void SetBackgroundColor (Color color)
		{
			var control = Control as NSControl;
			if (control == null)
				return;

			if (color == Color.Default)
				control.SetBackgroundColor (defaultColor);
			else
				control.SetBackgroundColor(color);
		}


		/*
		public override CGSize SizeThatFits (CGSize size)
		{
			return Control.FittingSize;

			// TODO: SizeThatFits
			//return this.Control.SizeThatFits (size);
		}
		*/

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (!disposing || (object)this.Control == null)
				return;
			this.Control.Dispose ();
			this.Control = default (TNativeView);
		}

		private void UpdateIsEnabled ()
		{
			if ((object)this.Element == null || (object)this.Control == null)
				return;
			NSControl uiControl = Control as NSControl;
			if (uiControl == null)
				return;
			
			uiControl.Enabled = this.Element.IsEnabled;
		}
	}

	public abstract class ViewRenderer : ViewRenderer<View, NSView>
	{
	}

}
