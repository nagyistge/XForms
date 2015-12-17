using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class SliderRenderer : ViewRenderer<Slider, UISlider>
  {
    private CGSize fitSize;

    public override CGSize SizeThatFits(CGSize size)
    {
      return this.fitSize;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.Control != null)
        this.Control.ValueChanged -= new EventHandler(this.OnControlValueChanged);
      base.Dispose(disposing);
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
    {
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          this.SetNativeControl(new UISlider()
          {
            Continuous = true
          });
          this.Control.ValueChanged += new EventHandler(this.OnControlValueChanged);
          // ISSUE: reference to a compiler-generated method
          this.Control.SizeToFit();
          this.fitSize = this.Control.Bounds.Size;
          if (this.fitSize.Width <= (nfloat) 0 || this.fitSize.Height <= (nfloat) 0)
            this.fitSize = new CGSize((nfloat) 22, (nfloat) 22);
        }
        this.UpdateMaximum();
        this.UpdateMinimum();
        this.UpdateValue();
      }
      this.OnElementChanged(e);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Slider.MaximumProperty.PropertyName)
        this.UpdateMaximum();
      else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
      {
        this.UpdateMinimum();
      }
      else
      {
        if (!(e.PropertyName == Slider.ValueProperty.PropertyName))
          return;
        this.UpdateValue();
      }
    }

    private void UpdateMaximum()
    {
      this.Control.MaxValue = (float) this.Element.Maximum;
    }

    private void UpdateMinimum()
    {
      this.Control.MinValue = (float) this.Element.Minimum;
    }

    private void UpdateValue()
    {
      if (this.Element.Value == (double) this.Control.Value)
        return;
      this.Control.Value = (float) this.Element.Value;
    }

    private void OnControlValueChanged(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(Slider.ValueProperty, (object) this.Control.Value);
    }
  }
}
