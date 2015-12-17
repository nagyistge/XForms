using CoreGraphics;
using System;
using System.ComponentModel;
using System.Threading;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ImageRenderer : ViewRenderer<Image, UIImageView>
  {
    private bool isDisposed;

    protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
    {
      if (this.Control == null)
      {
        UIImageView uiview = new UIImageView(CGRect.Empty);
        uiview.ContentMode = UIViewContentMode.ScaleAspectFit;
        uiview.ClipsToBounds = true;
        this.SetNativeControl(uiview);
      }
      if (e.NewElement != null)
      {
        this.SetAspect();
        this.SetImage(e.OldElement);
        this.SetOpacity();
      }
      this.OnElementChanged(e);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Image.SourceProperty.PropertyName)
        this.SetImage((Image) null);
      else if (e.PropertyName == Image.IsOpaqueProperty.PropertyName)
      {
        this.SetOpacity();
      }
      else
      {
        if (!(e.PropertyName == Image.AspectProperty.PropertyName))
          return;
        this.SetAspect();
      }
    }

    private void SetAspect()
    {
      this.Control.ContentMode = ImageExtensions.ToUIViewContentMode(this.Element.Aspect);
    }

    private async void SetImage(Image oldElement = null)
    {
      ImageSource source1 = this.Element.Source;
      if (oldElement != null)
      {
        ImageSource source2 = oldElement.Source;
        if (object.Equals((object) source2, (object) source1) || source2 is FileImageSource && source1 is FileImageSource && ((FileImageSource) source2).File == ((FileImageSource) source1).File)
          goto label_13;
      }
      this.Element.SetValueFromRenderer(Image.IsLoadingPropertyKey, (object) true);
      IImageSourceHandler imageSourceHandler;
      if (source1 != null && (M0) (imageSourceHandler = (IImageSourceHandler) Registrar.Registered.GetHandler<IImageSourceHandler>(((object) source1).GetType())) != null)
      {
        UIImage uiImage;
        try
        {
          uiImage = await imageSourceHandler.LoadImageAsync(source1, new CancellationToken(), (float) UIScreen.MainScreen.Scale);
        }
        catch (OperationCanceledException ex)
        {
          uiImage = (UIImage) null;
        }
        UIImageView control = this.Control;
        if (control != null)
          control.Image = uiImage;
        if (!this.isDisposed)
          this.Element.NativeSizeChanged();
      }
      else
        this.Control.Image = (UIImage) null;
      if (!this.isDisposed)
        this.Element.SetValueFromRenderer(Image.IsLoadingPropertyKey, (object) false);
label_13:;
    }

    private void SetOpacity()
    {
      this.Control.Opaque = this.Element.IsOpaque;
    }

    protected override void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      UIImage image;
      if (disposing && this.Control != null && (image = this.Control.Image) != null)
        image.Dispose();
      this.isDisposed = true;
      base.Dispose(disposing);
    }
  }
}
