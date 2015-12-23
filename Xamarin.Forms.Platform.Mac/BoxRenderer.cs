using CoreGraphics;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class BoxRenderer : VisualElementRenderer<BoxView>
	{
		private NSColor colorToRenderer;
		private CGSize previousSize;

		protected override void OnElementChanged (ElementChangedEventArgs<BoxView> e)
		{
			this.OnElementChanged (e);
			if (this.Element == null)
				return;
			this.SetBackgroundColor (this.Element.BackgroundColor);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == BoxView.ColorProperty.PropertyName)
			{
				this.SetBackgroundColor (this.Element.BackgroundColor);
			}
			else
			{
				if (!(e.PropertyName == VisualElement.IsVisibleProperty.PropertyName) || !this.Element.IsVisible)
					return;
				NeedsDisplay = true;
			}
		}

		protected override void SetBackgroundColor (Color color)
		{
			if (this.Element == null)
				return;
			Color color1 = this.Element.Color;
			this.colorToRenderer = color1.IsDefault ? ColorExtensions.ToUIColor (color) : ColorExtensions.ToUIColor (color1);

			NeedsDisplay = true;
		}

		public override void Layout ()
		{
			if (!(this.previousSize != this.Bounds.Size))
				return;
			
			NeedsDisplay = true;
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);

			var context = NSGraphicsContext.CurrentContext;

			using (CGContext currentContext = NSGraphicsContext.CurrentContext.CGContext)
			{
				this.colorToRenderer.SetFill ();
				currentContext.FillRect (dirtyRect);
			}

			this.previousSize = this.Bounds.Size;
		}
	}
}
