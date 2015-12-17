using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class StepperRenderer : ViewRenderer<Stepper, UIStepper>
  {
    protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
    {
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          this.SetNativeControl(new UIStepper((CGRect) RectangleF.Empty));
          this.Control.ValueChanged += new EventHandler(this.OnValueChanged);
        }
        this.UpdateMinimum();
        this.UpdateMaximum();
        this.UpdateValue();
        this.UpdateIncrement();
      }
      this.OnElementChanged(e);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.Control != null)
        this.Control.ValueChanged -= new EventHandler(this.OnValueChanged);
      base.Dispose(disposing);
    }

    private void OnValueChanged(object sender, EventArgs e)
    {
      this.Element.SetValueFromRenderer(Stepper.ValueProperty, (object) this.Control.Value);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Stepper.MinimumProperty.PropertyName)
        this.UpdateMinimum();
      else if (e.PropertyName == Stepper.MaximumProperty.PropertyName)
        this.UpdateMaximum();
      else if (e.PropertyName == Stepper.ValueProperty.PropertyName)
      {
        this.UpdateValue();
      }
      else
      {
        if (!(e.PropertyName == Stepper.IncrementProperty.PropertyName))
          return;
        this.UpdateIncrement();
      }
    }

    private void UpdateMinimum()
    {
      this.Control.MinimumValue = this.Element.Minimum;
    }

    private void UpdateMaximum()
    {
      this.Control.MaximumValue = this.Element.Maximum;
    }

    private void UpdateValue()
    {
      if (this.Control.Value == this.Element.Value)
        return;
      this.Control.Value = this.Element.Value;
    }

    private void UpdateIncrement()
    {
      this.Control.StepValue = this.Element.Increment;
    }
  }
}
