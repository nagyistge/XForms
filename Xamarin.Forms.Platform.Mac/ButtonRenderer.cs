using CoreGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	// TODO: http://stackoverflow.com/questions/13109964/nsbutton-how-to-color-the-text

	public class ButtonRenderer : ViewRenderer<Button, NSButton>
	{
		private NSColor buttonTextColorDefaultNormal;
		private NSColor buttonTextColorDefaultHighlighted;
		private NSColor buttonTextColorDefaultDisabled;

		protected override void OnElementChanged (ElementChangedEventArgs<Button> e)
		{
			this.OnElementChanged (e);
			if (e.NewElement == null)
				return;
			if (Control == null)
			{
				var button = new NSButton (CGRect.Empty);
				button.SetButtonType (NSButtonType.MomentaryPushIn);
				SetNativeControl (button);

				buttonTextColorDefaultNormal = NSColor.Black;
				buttonTextColorDefaultHighlighted = NSColor.Black;
				buttonTextColorDefaultDisabled = NSColor.Black;

				Control.Activated += OnActivated;
			}
			UpdateText ();
			UpdateFont ();
			UpdateBorder ();
			UpdateImage ();
			UpdateTextColor ();
		}

		protected override void Dispose (bool disposing)
		{
			if (this.Control != null)
				this.Control.Activated -= OnActivated;
			base.Dispose (disposing);
		}

		private void OnActivated (object sender, EventArgs eventArgs)
		{
			if (this.Element == null)
				return;
			((IButtonController)this.Element).SendClicked ();
		}

		/*
		public override CGSize SizeThatFits (CGSize size)
		{
			CGSize width = base.SizeThatFits (size);
			width.Height = 44;
			if (base.Control.Image != null)
			{
				width.Width = width.Width + 10;
			}
			return width;
		}
		*/


		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == Button.TextProperty.PropertyName)
				this.UpdateText ();
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
				this.UpdateTextColor ();
			else if (e.PropertyName == Button.FontProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Button.BorderRadiusProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName)
				this.UpdateBorder ();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				this.UpdateBackgroundVisibility ();
			}
			else
			{
				if (!(e.PropertyName == Button.ImageProperty.PropertyName))
					return;
				this.UpdateImage ();
			}
		}

		private void UpdateBackgroundVisibility ()
		{
			bool flag = this.Element.BackgroundColor == Color.Default;

			foreach (NSView view in this.Control.Subviews.Where(sv => !(sv is NSTextField)))
			{
				view.AlphaValue = (nfloat)(flag ? 1f : 0.0f);
			}
		}

		private void UpdateBorder ()
		{
			NSButton control = this.Control;
			Button element = this.Element;
			if (element.BorderColor != Color.Default)
				control.Layer.BorderColor = ColorExtensions.ToCGColor (element.BorderColor);
			control.Layer.BorderWidth = (nfloat)((float)element.BorderWidth);
			control.Layer.CornerRadius = (nfloat)element.BorderRadius;
			this.UpdateBackgroundVisibility ();
		}

		private void UpdateFont ()
		{
			// TODO: Button Font
			//this.Control.TitleLabel.Font = FontExtensions.ToUIFont ((IFontElement)this.Element);
		}

		private void UpdateText ()
		{
			Control.Title = Element.Text;
		}

		private void UpdateTextColor ()
		{
			Control.SetBackgroundColor (Element.BackgroundColor);

			//TODO: Button Text Color
		}

		private async void UpdateImage ()
		{
			IImageSourceHandler imageSourceHandler = Registrar.Registered.GetHandler<IImageSourceHandler> (Element.Image.GetType ());
			if (Element.Image != null && imageSourceHandler != null)
			{
				NSImage image2;
				try
				{
					image2 = await imageSourceHandler.LoadImageAsync( Element.Image, new CancellationToken ());
				}
				catch (OperationCanceledException ex)
				{
					image2 = null;
				}
				if (Control != null && image2 != null)
				{
					Control.Image = image2;

					//Control.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					//this.Control.ImageEdgeInsets = new UIEdgeInsets ((nfloat)0, (nfloat)0, (nfloat)0, (nfloat)10);
					//this.Control.TitleEdgeInsets = new UIEdgeInsets ((nfloat)0, (nfloat)10, (nfloat)0, (nfloat)0);
				}
			}
			else
			{
				this.Control.Image = null;

				//this.Control.ImageEdgeInsets = new UIEdgeInsets ((nfloat)0, (nfloat)0, (nfloat)0, (nfloat)0);
				//this.Control.TitleEdgeInsets = new UIEdgeInsets ((nfloat)0, (nfloat)0, (nfloat)0, (nfloat)0);
			}

			((IVisualElementController)Element).NativeSizeChanged();
		}
	}
}
