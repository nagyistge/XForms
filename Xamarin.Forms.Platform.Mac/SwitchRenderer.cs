using CoreGraphics;
using System;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class SwitchRenderer : ViewRenderer<Switch, UISwitch>
  {
    protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
    {
      if (e.OldElement != null)
        e.OldElement.remove_Toggled(new EventHandler<ToggledEventArgs>(this.OnElementToggled));
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          this.SetNativeControl(new UISwitch((CGRect) RectangleF.Empty));
          this.Control.ValueChanged += new EventHandler(this.OnControlValueChanged);
        }
        this.Control.On = this.Element.IsToggled;
        e.NewElement.add_Toggled(new EventHandler<ToggledEventArgs>(this.OnElementToggled));
      }
      this.OnElementChanged(e);
    }

    private void OnElementToggled(object sender, EventArgs e)
    {
      // ISSUE: reference to a compiler-generated method
      this.Control.SetState(this.Element.IsToggled, true);
    }

    private void OnControlValueChanged(object sender, EventArgs e)
    {
      this.Element.SetValueFromRenderer(Switch.IsToggledProperty, (object) (bool) (this.Control.On ? 1 : 0));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.Control.ValueChanged -= new EventHandler(this.OnControlValueChanged);
      base.Dispose(disposing);
    }
  }
}
