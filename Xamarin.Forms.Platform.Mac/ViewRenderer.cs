using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView> where TView : View where TNativeView : UIView
  {
    private UIColor defaultColor;

    public TNativeView Control { get; private set; }

    protected override void OnElementChanged(ElementChangedEventArgs<TView> e)
    {
      base.OnElementChanged(e);
      if ((object) e.OldElement != null)
        e.OldElement.remove_FocusChangeRequested(new EventHandler<VisualElement.FocusRequestArgs>(this.ViewOnFocusChangeRequested));
      if ((object) e.NewElement != null)
      {
        if ((object) this.Control != null && (object) e.OldElement != null && e.OldElement.BackgroundColor != e.NewElement.BackgroundColor || e.NewElement.BackgroundColor != Color.Default)
          this.SetBackgroundColor(e.NewElement.BackgroundColor);
        e.NewElement.add_FocusChangeRequested(new EventHandler<VisualElement.FocusRequestArgs>(this.ViewOnFocusChangeRequested));
      }
      this.UpdateIsEnabled();
    }

    private void ViewOnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs focusRequestArgs)
    {
      if ((object) this.Control == null)
        return;
      VisualElement.FocusRequestArgs focusRequestArgs1 = focusRequestArgs;
      // ISSUE: reference to a compiler-generated method
      // ISSUE: reference to a compiler-generated method
      int num = focusRequestArgs1.Focus ? (this.Control.BecomeFirstResponder() ? 1 : 0) : (this.Control.ResignFirstResponder() ? 1 : 0);
      focusRequestArgs1.Result = num != 0;
    }

    protected void SetNativeControl(TNativeView uiview)
    {
      this.defaultColor = uiview.BackgroundColor;
      this.Control = uiview;
      if (this.Element.BackgroundColor != Color.Default)
        this.SetBackgroundColor(this.Element.BackgroundColor);
      this.UpdateIsEnabled();
      // ISSUE: reference to a compiler-generated method
      this.AddSubview((UIView) uiview);
    }

    internal override void SendVisualElementInitialized(VisualElement element, UIView nativeView)
    {
      base.SendVisualElementInitialized(element, (UIView) this.Control);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ((object) this.Control != null)
      {
        if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
          this.UpdateIsEnabled();
        else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
          this.SetBackgroundColor(this.Element.BackgroundColor);
      }
      base.OnElementPropertyChanged(sender, e);
    }

    protected override void SetBackgroundColor(Color color)
    {
      if ((object) this.Control == null)
        return;
      if (color == Color.Default)
        this.Control.BackgroundColor = this.defaultColor;
      else
        this.Control.BackgroundColor = ColorExtensions.ToUIColor(color);
    }

    public override void LayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.LayoutSubviews();
      if ((object) this.Control == null)
        return;
      this.Control.Frame = new CGRect((nfloat) 0, (nfloat) 0, (nfloat) this.Element.Width, (nfloat) this.Element.Height);
    }

    public override CGSize SizeThatFits(CGSize size)
    {
      // ISSUE: reference to a compiler-generated method
      return this.Control.SizeThatFits(size);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing || (object) this.Control == null)
        return;
      this.Control.Dispose();
      this.Control = default (TNativeView);
    }

    private void UpdateIsEnabled()
    {
      if ((object) this.Element == null || (object) this.Control == null)
        return;
      UIControl uiControl = (object) this.Control as UIControl;
      if (uiControl == null)
        return;
      uiControl.Enabled = this.Element.IsEnabled;
    }
  }
  
  public abstract class ViewRenderer : ViewRenderer<View, UIView>
  {
  }

}
