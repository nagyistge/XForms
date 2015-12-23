using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class LabelRenderer : ViewRenderer<Label, NSTextField>
	{
		private bool perfectSizeValid;
		private SizeRequest perfectSize;

		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			if (e.NewElement != null)
			{
				if (this.Control == null)
				{
					var uiview = new NSTextField (CGRect.Empty);
					NSColor clear = NSColor.Clear;
					uiview.BackgroundColor = clear;
					this.SetNativeControl (uiview);
				}
				this.UpdateText ();
				this.UpdateLineBreakMode ();
				this.UpdateAlignment ();
			}
			this.OnElementChanged (e);
		}

		public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
		{
			Size request;
			if (!this.perfectSizeValid)
			{
				this.perfectSize = base.GetDesiredSize (Double.PositiveInfinity, Double.PositiveInfinity);
				request = this.perfectSize.Request;
				double num = Math.Min (10, request.Width);
				request = this.perfectSize.Request;
				this.perfectSize.Minimum = new Size (num, request.Height);
				this.perfectSizeValid = true;
			}
			if (widthConstraint >= this.perfectSize.Request.Width && heightConstraint >= this.perfectSize.Request.Height)
			{
				return this.perfectSize;
			}
			SizeRequest desiredSize = base.GetDesiredSize (widthConstraint, heightConstraint);
			request = desiredSize.Request;
			double num1 = Math.Min (10, request.Width);
			request = desiredSize.Request;
			desiredSize.Minimum = new Size (num1, request.Height);
			return desiredSize;
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName)
				this.UpdateAlignment ();
			else if (e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
			{
				this.Layout ();
			}
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
				this.UpdateText ();
			else if (e.PropertyName == Label.FontProperty.PropertyName)
				this.UpdateText ();
			else if (e.PropertyName == Label.TextProperty.PropertyName)
				this.UpdateText ();
			else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
			{
				this.UpdateText ();
			}
			else
			{
				if (!(e.PropertyName == Label.LineBreakModeProperty.PropertyName))
					return;
				this.UpdateLineBreakMode ();
			}
		}

		private void UpdateText ()
		{
			this.perfectSizeValid = false;

			object[] values = this.Element.GetValues (Label.FormattedTextProperty, Label.TextProperty, Label.TextColorProperty);

			FormattedString formattedString = (FormattedString)values [0];
			if (formattedString != null)
			{
				this.Control.AttributedStringValue = FormattedStringExtensions.ToAttributed (formattedString, (Element)this.Element, (Color)values [2]);
			}
			else
			{
				this.Control.StringValue = (string)values [1];
				this.Control.Font = FontExtensions.ToUIFont (this.Element);
				this.Control.TextColor = ColorExtensions.ToUIColor ((Color)values [2], ColorExtensions.Black);
			}
			this.Layout ();
		}

		private void UpdateAlignment ()
		{
			this.Control.Alignment = AlignmentExtensions.ToNativeTextAlignment (this.Element.HorizontalTextAlignment);
		}

		private void UpdateLineBreakMode ()
		{
			this.perfectSizeValid = false;
			switch (this.Element.LineBreakMode)
			{
			case LineBreakMode.NoWrap:
				this.Control.LineBreakMode = NSLineBreakMode.Clipping;
				this.Control.MaximumNumberOfLines = (nint)1;
				break;
			case LineBreakMode.WordWrap:
				this.Control.LineBreakMode = NSLineBreakMode.ByWordWrapping;
				this.Control.MaximumNumberOfLines = (nint)0;
				break;
			case LineBreakMode.CharacterWrap:
				this.Control.LineBreakMode = NSLineBreakMode.CharWrapping;
				this.Control.MaximumNumberOfLines = (nint)0;
				break;
			case LineBreakMode.HeadTruncation:
				this.Control.LineBreakMode = NSLineBreakMode.TruncatingHead;
				this.Control.MaximumNumberOfLines = (nint)1;
				break;
			case LineBreakMode.TailTruncation:
				this.Control.LineBreakMode = NSLineBreakMode.TruncatingTail;
				this.Control.MaximumNumberOfLines = (nint)1;
				break;
			case LineBreakMode.MiddleTruncation:
				this.Control.LineBreakMode = NSLineBreakMode.TruncatingMiddle;
				this.Control.MaximumNumberOfLines = (nint)1;
				break;
			}
		}

		public override void Layout ()
		{
			base.Layout ();

			if (this.Control == null)
				return;
			switch (this.Element.VerticalTextAlignment)
			{
			case TextAlignment.Start:
				this.Control.Frame = new CGRect ((nfloat)0, (nfloat)0, (nfloat)this.Element.Width, (nfloat)Math.Min ((double)this.Bounds.Height, (double)this.Control.SizeThatFits (SizeExtensions.ToSizeF (this.Element.Bounds.Size)).Height));
				break;
			case TextAlignment.Center:
				this.Control.Frame = new CGRect ((nfloat)0, (nfloat)0, (nfloat)this.Element.Width, (nfloat)this.Element.Height);
				break;
			case TextAlignment.End:
				nfloat nfloat = (nfloat)0;
				nfloat height = (nfloat)Math.Min ((double)this.Bounds.Height, (double)this.Control.SizeThatFits (SizeExtensions.ToSizeF (this.Element.Bounds.Size)).Height);
				this.Control.Frame = new CGRect ((nfloat)0, (nfloat)(this.Element.Height - (double)height), (nfloat)this.Element.Width, height);
				break;
			}
		}
	}
}
