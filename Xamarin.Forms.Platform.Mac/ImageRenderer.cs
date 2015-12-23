using CoreGraphics;
using System;
using System.ComponentModel;
using System.Threading;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class ImageRenderer : ViewRenderer<Image, NSImageView>
	{
		private bool isDisposed;

		protected override void OnElementChanged (ElementChangedEventArgs<Image> e)
		{
			if (this.Control == null)
			{
				NSImageView uiview = new NSImageView (CGRect.Empty);
				uiview.ImageScaling = NSImageScale.ProportionallyUpOrDown;
				//uiview.ContentMode = UIViewContentMode.ScaleAspectFit;
				//uiview.ClipsToBounds = true;
				this.SetNativeControl (uiview);
			}
			if (e.NewElement != null)
			{
				this.SetAspect ();
				this.SetImage (e.OldElement);
				this.SetOpacity ();
			}
			this.OnElementChanged (e);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == Image.SourceProperty.PropertyName)
				this.SetImage ((Image)null);
			else if (e.PropertyName == Image.IsOpaqueProperty.PropertyName)
			{
				this.SetOpacity ();
			}
			else
			{
				if (!(e.PropertyName == Image.AspectProperty.PropertyName))
					return;
				this.SetAspect ();
			}
		}

		private void SetAspect ()
		{
			this.Control.ImageScaling = ImageExtensions.ToNSImageScale( Element.Aspect );
		}

		private async void SetImage (Image oldElement = null)
		{
			ImageSource source1 = this.Element.Source;
			if (oldElement != null)
			{
				ImageSource source2 = oldElement.Source;
				if (object.Equals (source2, source1) || 
					source2 is FileImageSource && source1 is FileImageSource && 
					((FileImageSource)source2).File == ((FileImageSource)source1).File)
					return;
			}
			((IElementController)Element).SetValueFromRenderer (Image.IsLoadingPropertyKey, true);
			IImageSourceHandler imageSourceHandler;

			imageSourceHandler = Registrar.Registered.GetHandler<IImageSourceHandler> (source1.GetType ());
			if (source1 != null && imageSourceHandler != null)
			{
				NSImage uiImage;
				try
				{
					uiImage = await imageSourceHandler.LoadImageAsync( source1, new CancellationToken ());
				}
				catch (OperationCanceledException ex)
				{
					uiImage = null;
				}
				NSImageView control = this.Control;
				if (control != null)
					control.Image = uiImage;
				if (!this.isDisposed)
					((IVisualElementController)Element).NativeSizeChanged ();
			}
			else
				this.Control.Image = null;
			
			if (!this.isDisposed)
				((IElementController)Element).SetValueFromRenderer (Image.IsLoadingPropertyKey, false);
		}

		private void SetOpacity ()
		{
			// Not supported fully on Mac
			//this.Control.AlphaValue = this.Element.IsOpaque;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.isDisposed)
				return;
			NSImage image;
			if (disposing && this.Control != null && (image = this.Control.Image) != null)
				image.Dispose ();
			this.isDisposed = true;
			base.Dispose (disposing);
		}
	}
}
