using CoreGraphics;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, UIActivityIndicatorView>
  {
    protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
    {
      if (e.NewElement != null)
      {
        if (this.Control == null)
          this.SetNativeControl(new UIActivityIndicatorView((CGRect) RectangleF.Empty)
          {
            ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray
          });
        this.UpdateColor();
        this.UpdateIsRunning();
      }
      this.OnElementChanged(e);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
      {
        this.UpdateColor();
      }
      else
      {
        if (!(e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName))
          return;
        this.UpdateIsRunning();
      }
    }

    private void UpdateColor()
    {
      this.Control.Color = this.Element.Color == Color.Default ? (UIColor) null : ColorExtensions.ToUIColor(this.Element.Color);
    }

    private void UpdateIsRunning()
    {
      if (this.Element.IsRunning)
      {
        // ISSUE: reference to a compiler-generated method
        this.Control.StartAnimating();
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        this.Control.StopAnimating();
      }
    }
  }
}
