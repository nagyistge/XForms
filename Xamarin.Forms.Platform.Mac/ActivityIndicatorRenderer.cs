using CoreGraphics;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;
using System.Drawing;

namespace Xamarin.Forms.Platform.Mac
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, NSProgressIndicator>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (e.NewElement != null)
			{
				if (this.Control == null)
					this.SetNativeControl (new NSProgressIndicator ((CGRect)RectangleF.Empty) {
						Style = NSProgressIndicatorStyle.Spinning
					});
				this.UpdateColor ();
				this.UpdateIsRunning ();
			}
			this.OnElementChanged (e);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
			{
				this.UpdateColor ();
			}
			else
			{
				if (!(e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName))
					return;
				this.UpdateIsRunning ();
			}
		}

		private void UpdateColor ()
		{
			var color = this.Element.Color == Color.Default ? (NSColor)null : ColorExtensions.ToUIColor (this.Element.Color);
			// TODO: Apply color
		}

		private void UpdateIsRunning ()
		{
			if (this.Element.IsRunning)
				this.Control.StartAnimation(null);
			else
				this.Control.StopAnimation(null);
		}
	}
}
