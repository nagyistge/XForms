using CoreGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ButtonRenderer : ViewRenderer<Button, UIButton>
  {
    private UIColor buttonTextColorDefaultNormal;
    private UIColor buttonTextColorDefaultHighlighted;
    private UIColor buttonTextColorDefaultDisabled;

    protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
    {
      this.OnElementChanged(e);
      if (e.NewElement == null)
        return;
      if (this.Control == null)
      {
        this.SetNativeControl(new UIButton(UIButtonType.RoundedRect));
        // ISSUE: reference to a compiler-generated method
        this.buttonTextColorDefaultNormal = this.Control.TitleColor(UIControlState.Normal);
        // ISSUE: reference to a compiler-generated method
        this.buttonTextColorDefaultHighlighted = this.Control.TitleColor(UIControlState.Highlighted);
        // ISSUE: reference to a compiler-generated method
        this.buttonTextColorDefaultDisabled = this.Control.TitleColor(UIControlState.Disabled);
        this.Control.TouchUpInside += new EventHandler(this.OnButtonTouchUpInside);
      }
      this.UpdateText();
      this.UpdateFont();
      this.UpdateBorder();
      this.UpdateImage();
      this.UpdateTextColor();
    }

    protected override void Dispose(bool disposing)
    {
      if (this.Control != null)
        this.Control.TouchUpInside -= new EventHandler(this.OnButtonTouchUpInside);
      base.Dispose(disposing);
    }

    private void OnButtonTouchUpInside(object sender, EventArgs eventArgs)
    {
      if (this.Element == null)
        return;
      ((IButtonController) this.Element).SendClicked();
    }

    public override CGSize SizeThatFits(CGSize size)
    {
      CGSize cgSize = base.SizeThatFits(size);
      cgSize.Height = (nfloat) 44;
      if (!this.Control.ImageView.Hidden)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        CGSize& local = @cgSize;
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        (^local).Width = (^local).Width + (nfloat) 10;
      }
      return cgSize;
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Button.TextProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == Button.TextColorProperty.PropertyName)
        this.UpdateTextColor();
      else if (e.PropertyName == Button.FontProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Button.BorderRadiusProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName)
        this.UpdateBorder();
      else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
      {
        this.UpdateBackgroundVisibility();
      }
      else
      {
        if (!(e.PropertyName == Button.ImageProperty.PropertyName))
          return;
        this.UpdateImage();
      }
    }

    private void UpdateBackgroundVisibility()
    {
      if (Forms.IsiOS7OrNewer)
        return;
      bool flag = this.Element.BackgroundColor == Color.Default;
      foreach (UIView uiView in Enumerable.Where<UIView>((IEnumerable<UIView>) this.Control.Subviews, (Func<UIView, bool>) (sv => !(sv is UILabel))))
        uiView.Alpha = (nfloat) (flag ? 1f : 0.0f);
    }

    private void UpdateBorder()
    {
      UIButton control = this.Control;
      Button element = this.Element;
      if (element.BorderColor != Color.Default)
        control.Layer.BorderColor = ColorExtensions.ToCGColor(element.BorderColor);
      control.Layer.BorderWidth = (nfloat) ((float) element.BorderWidth);
      control.Layer.CornerRadius = (nfloat) element.BorderRadius;
      this.UpdateBackgroundVisibility();
    }

    private void UpdateFont()
    {
      this.Control.TitleLabel.Font = FontExtensions.ToUIFont((IFontElement) this.Element);
    }

    private void UpdateText()
    {
      // ISSUE: reference to a compiler-generated method
      this.Control.SetTitle(this.Element.Text, UIControlState.Normal);
    }

    private void UpdateTextColor()
    {
      if (this.Element.TextColor == Color.Default)
      {
        // ISSUE: reference to a compiler-generated method
        this.Control.SetTitleColor(this.buttonTextColorDefaultNormal, UIControlState.Normal);
        // ISSUE: reference to a compiler-generated method
        this.Control.SetTitleColor(this.buttonTextColorDefaultHighlighted, UIControlState.Highlighted);
        // ISSUE: reference to a compiler-generated method
        this.Control.SetTitleColor(this.buttonTextColorDefaultDisabled, UIControlState.Disabled);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        this.Control.SetTitleColor(ColorExtensions.ToUIColor(this.Element.TextColor), UIControlState.Normal);
        // ISSUE: reference to a compiler-generated method
        this.Control.SetTitleColor(ColorExtensions.ToUIColor(this.Element.TextColor), UIControlState.Highlighted);
        // ISSUE: reference to a compiler-generated method
        this.Control.SetTitleColor(this.buttonTextColorDefaultDisabled, UIControlState.Disabled);
        if (!Forms.IsiOS7OrNewer)
          return;
        this.Control.TintColor = ColorExtensions.ToUIColor(this.Element.TextColor);
      }
    }

    private async void UpdateImage()
    {
      FileImageSource image1 = this.Element.Image;
      IImageSourceHandler imageSourceHandler;
      if (image1 != null && (M0) (imageSourceHandler = (IImageSourceHandler) Registrar.Registered.GetHandler<IImageSourceHandler>(((object) image1).GetType())) != null)
      {
        UIImage image2;
        try
        {
          image2 = await imageSourceHandler.LoadImageAsync((ImageSource) image1, new CancellationToken(), (float) UIScreen.MainScreen.Scale);
        }
        catch (OperationCanceledException ex)
        {
          image2 = (UIImage) null;
        }
        UIButton control = this.Control;
        if (control != null && image2 != null)
        {
          if (Forms.IsiOS7OrNewer)
          {
            // ISSUE: reference to a compiler-generated method
            // ISSUE: reference to a compiler-generated method
            control.SetImage(image2.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
          }
          else
          {
            // ISSUE: reference to a compiler-generated method
            control.SetImage(image2, UIControlState.Normal);
          }
          control.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
          this.Control.ImageEdgeInsets = new UIEdgeInsets((nfloat) 0, (nfloat) 0, (nfloat) 0, (nfloat) 10);
          this.Control.TitleEdgeInsets = new UIEdgeInsets((nfloat) 0, (nfloat) 10, (nfloat) 0, (nfloat) 0);
        }
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        this.Control.SetImage((UIImage) null, UIControlState.Normal);
        this.Control.ImageEdgeInsets = new UIEdgeInsets((nfloat) 0, (nfloat) 0, (nfloat) 0, (nfloat) 0);
        this.Control.TitleEdgeInsets = new UIEdgeInsets((nfloat) 0, (nfloat) 0, (nfloat) 0, (nfloat) 0);
      }
      this.Element.NativeSizeChanged();
    }
  }
}
