using CoreGraphics;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class BoxRenderer : VisualElementRenderer<BoxView>
  {
    private UIColor colorToRenderer;
    private CGSize previousSize;

    protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
    {
      this.OnElementChanged(e);
      if (this.Element == null)
        return;
      this.SetBackgroundColor(this.Element.BackgroundColor);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == BoxView.ColorProperty.PropertyName)
      {
        this.SetBackgroundColor(this.Element.BackgroundColor);
      }
      else
      {
        if (!(e.PropertyName == VisualElement.IsVisibleProperty.PropertyName) || !this.Element.IsVisible)
          return;
        // ISSUE: reference to a compiler-generated method
        this.SetNeedsDisplay();
      }
    }

    protected override void SetBackgroundColor(Color color)
    {
      if (this.Element == null)
        return;
      Color color1 = this.Element.Color;
      this.colorToRenderer = color1.IsDefault ? ColorExtensions.ToUIColor(color) : ColorExtensions.ToUIColor(color1);
      // ISSUE: reference to a compiler-generated method
      this.SetNeedsDisplay();
    }

    public override void LayoutSubviews()
    {
      if (!(this.previousSize != this.Bounds.Size))
        return;
      // ISSUE: reference to a compiler-generated method
      this.SetNeedsDisplay();
    }

    public override void Draw(CGRect rect)
    {
      using (CGContext currentContext = UIGraphics.GetCurrentContext())
      {
        // ISSUE: reference to a compiler-generated method
        this.colorToRenderer.SetFill();
        currentContext.FillRect(rect);
      }
      // ISSUE: reference to a compiler-generated method
      base.Draw(rect);
      this.previousSize = this.Bounds.Size;
    }
  }
}
