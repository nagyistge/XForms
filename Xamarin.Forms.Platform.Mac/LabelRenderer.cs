using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class LabelRenderer : ViewRenderer<Label, UILabel>
  {
    private bool perfectSizeValid;
    private SizeRequest perfectSize;

    protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
    {
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          UILabel uiview = new UILabel(CGRect.Empty);
          UIColor clear = UIColor.Clear;
          uiview.BackgroundColor = clear;
          this.SetNativeControl(uiview);
        }
        this.UpdateText();
        this.UpdateLineBreakMode();
        this.UpdateAlignment();
      }
      this.OnElementChanged(e);
    }

    public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      if (!this.perfectSizeValid)
      {
        this.perfectSize = base.GetDesiredSize(double.PositiveInfinity, double.PositiveInfinity);
        this.perfectSize.Minimum = new Size(Math.Min(10.0, this.perfectSize.Request.Width), this.perfectSize.Request.Height);
        this.perfectSizeValid = true;
      }
      if (widthConstraint >= this.perfectSize.Request.Width && heightConstraint >= this.perfectSize.Request.Height)
        return this.perfectSize;
      SizeRequest desiredSize = base.GetDesiredSize(widthConstraint, heightConstraint);
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      SizeRequest& local = @desiredSize;
      double val1 = 10.0;
      Size request = desiredSize.Request;
      double width1 = request.Width;
      double width2 = Math.Min(val1, width1);
      request = desiredSize.Request;
      double height = request.Height;
      Size size = new Size(width2, height);
      // ISSUE: explicit reference operation
      (^local).Minimum = size;
      return desiredSize;
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName)
        this.UpdateAlignment();
      else if (e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
      {
        // ISSUE: reference to a compiler-generated method
        this.LayoutSubviews();
      }
      else if (e.PropertyName == Label.TextColorProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == Label.FontProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == Label.TextProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
      {
        this.UpdateText();
      }
      else
      {
        if (!(e.PropertyName == Label.LineBreakModeProperty.PropertyName))
          return;
        this.UpdateLineBreakMode();
      }
    }

    private void UpdateText()
    {
      this.perfectSizeValid = false;
      object[] values = this.Element.GetValues(Label.FormattedTextProperty, Label.TextProperty, Label.TextColorProperty);
      FormattedString formattedString = (FormattedString) values[0];
      if (formattedString != null)
      {
        this.Control.AttributedText = FormattedStringExtensions.ToAttributed(formattedString, (Element) this.Element, (Color) values[2]);
      }
      else
      {
        this.Control.Text = (string) values[1];
        this.Control.Font = FontExtensions.ToUIFont(this.Element);
        this.Control.TextColor = ColorExtensions.ToUIColor((Color) values[2], ColorExtensions.Black);
      }
      // ISSUE: reference to a compiler-generated method
      this.LayoutSubviews();
    }

    private void UpdateAlignment()
    {
      this.Control.TextAlignment = AlignmentExtensions.ToNativeTextAlignment(this.Element.HorizontalTextAlignment);
    }

    private void UpdateLineBreakMode()
    {
      this.perfectSizeValid = false;
      switch (this.Element.LineBreakMode)
      {
        case LineBreakMode.NoWrap:
          this.Control.LineBreakMode = UILineBreakMode.Clip;
          this.Control.Lines = (nint) 1;
          break;
        case LineBreakMode.WordWrap:
          this.Control.LineBreakMode = UILineBreakMode.WordWrap;
          this.Control.Lines = (nint) 0;
          break;
        case LineBreakMode.CharacterWrap:
          this.Control.LineBreakMode = UILineBreakMode.CharacterWrap;
          this.Control.Lines = (nint) 0;
          break;
        case LineBreakMode.HeadTruncation:
          this.Control.LineBreakMode = UILineBreakMode.HeadTruncation;
          this.Control.Lines = (nint) 1;
          break;
        case LineBreakMode.TailTruncation:
          this.Control.LineBreakMode = UILineBreakMode.TailTruncation;
          this.Control.Lines = (nint) 1;
          break;
        case LineBreakMode.MiddleTruncation:
          this.Control.LineBreakMode = UILineBreakMode.MiddleTruncation;
          this.Control.Lines = (nint) 1;
          break;
      }
    }

    public override void LayoutSubviews()
    {
      base.LayoutSubviews();
      if (this.Control == null)
        return;
      switch (this.Element.VerticalTextAlignment)
      {
        case TextAlignment.Start:
          // ISSUE: reference to a compiler-generated method
          this.Control.Frame = new CGRect((nfloat) 0, (nfloat) 0, (nfloat) this.Element.Width, (nfloat) Math.Min((double) this.Bounds.Height, (double) this.Control.SizeThatFits(SizeExtensions.ToSizeF(this.Element.Bounds.Size)).Height));
          break;
        case TextAlignment.Center:
          this.Control.Frame = new CGRect((nfloat) 0, (nfloat) 0, (nfloat) this.Element.Width, (nfloat) this.Element.Height);
          break;
        case TextAlignment.End:
          nfloat nfloat = (nfloat) 0;
          // ISSUE: reference to a compiler-generated method
          nfloat height = (nfloat) Math.Min((double) this.Bounds.Height, (double) this.Control.SizeThatFits(SizeExtensions.ToSizeF(this.Element.Bounds.Size)).Height);
          this.Control.Frame = new CGRect((nfloat) 0, (nfloat) (this.Element.Height - (double) height), (nfloat) this.Element.Width, height);
          break;
      }
    }

    protected override void SetBackgroundColor(Color color)
    {
      if (color == Color.Default)
        this.BackgroundColor = UIColor.Clear;
      else
        this.BackgroundColor = ColorExtensions.ToUIColor(color);
    }
  }
}
