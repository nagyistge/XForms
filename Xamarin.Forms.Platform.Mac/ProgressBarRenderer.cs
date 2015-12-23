using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class ProgressBarRenderer : ViewRenderer<ProgressBar, NSProgressIndicator>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<ProgressBar> e)
		{
			if (e.NewElement != null)
			{
				if (this.Control == null)
					this.SetNativeControl (new NSProgressIndicator ());
				this.UpdateProgress ();
			}
			this.OnElementChanged (e);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (!(e.PropertyName == ProgressBar.ProgressProperty.PropertyName))
				return;
			this.UpdateProgress ();
		}

		private void UpdateProgress ()
		{
			this.Control.DoubleValue = (double)this.Element.Progress;
		}

		/*
		public override CGSize SizeThatFits (CGSize size)
		{
			return new CGSize ((nfloat)10, base.SizeThatFits (size).Height);
		}
		*/
	}
}
