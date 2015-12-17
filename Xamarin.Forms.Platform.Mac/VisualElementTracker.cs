using CoreAnimation;
using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class VisualElementTracker : IDisposable
  {
    private readonly PropertyChangedEventHandler propertyChangedHandler;
    private readonly EventHandler sizeChangedEventHandler;
    private readonly EventHandler<EventArg<VisualElement>> batchCommittedHandler;
    private bool disposed;
    private VisualElement element;
    private CALayer layer;
    private bool inputTransparent;
    private Xamarin.Forms.Rectangle lastBounds;
    private int updateCount;

    private IVisualElementRenderer Renderer { get; set; }

    public event EventHandler NativeControlUpdated;

    public VisualElementTracker(IVisualElementRenderer renderer)
    {
      if (renderer == null)
        throw new ArgumentNullException("renderer");
      this.propertyChangedHandler = new PropertyChangedEventHandler(this.HandlePropertyChanged);
      this.sizeChangedEventHandler = new EventHandler(this.HandleSizeChanged);
      this.batchCommittedHandler = new EventHandler<EventArg<VisualElement>>(this.HandleRedrawNeeded);
      this.Renderer = renderer;
      renderer.ElementChanged += new EventHandler<VisualElementChangedEventArgs>(this.OnRendererElementChanged);
      this.SetElement((VisualElement) null, renderer.Element);
    }

    private void OnRendererElementChanged(object s, VisualElementChangedEventArgs e)
    {
      if (this.element == e.NewElement)
        return;
      this.SetElement(this.element, e.NewElement);
    }

    private void HandleSizeChanged(object sender, EventArgs e)
    {
      this.UpdateNativeControl();
    }

    private void HandleRedrawNeeded(object sender, EventArgs e)
    {
      this.UpdateNativeControl();
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      this.disposed = true;
      if (!disposing)
        return;
      this.SetElement(this.element, (VisualElement) null);
      if (this.layer != null)
      {
        this.layer.Dispose();
        this.layer = (CALayer) null;
      }
      this.Renderer.ElementChanged -= new EventHandler<VisualElementChangedEventArgs>(this.OnRendererElementChanged);
      this.Renderer = (IVisualElementRenderer) null;
    }

    private void SetElement(VisualElement oldElement, VisualElement newElement)
    {
      if (oldElement != null)
      {
        oldElement.remove_PropertyChanged(this.propertyChangedHandler);
        oldElement.remove_SizeChanged(this.sizeChangedEventHandler);
        oldElement.remove_BatchCommitted(this.batchCommittedHandler);
      }
      this.element = newElement;
      if (newElement == null)
        return;
      newElement.add_BatchCommitted(this.batchCommittedHandler);
      newElement.add_SizeChanged(this.sizeChangedEventHandler);
      newElement.add_PropertyChanged(this.propertyChangedHandler);
      this.UpdateNativeControl();
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == VisualElement.XProperty.PropertyName) && !(e.PropertyName == VisualElement.YProperty.PropertyName) && (!(e.PropertyName == VisualElement.WidthProperty.PropertyName) && !(e.PropertyName == VisualElement.HeightProperty.PropertyName)) && (!(e.PropertyName == VisualElement.AnchorXProperty.PropertyName) && !(e.PropertyName == VisualElement.AnchorYProperty.PropertyName) && (!(e.PropertyName == VisualElement.TranslationXProperty.PropertyName) && !(e.PropertyName == VisualElement.TranslationYProperty.PropertyName))) && (!(e.PropertyName == VisualElement.ScaleProperty.PropertyName) && !(e.PropertyName == VisualElement.RotationProperty.PropertyName) && (!(e.PropertyName == VisualElement.RotationXProperty.PropertyName) && !(e.PropertyName == VisualElement.RotationYProperty.PropertyName)) && (!(e.PropertyName == VisualElement.IsVisibleProperty.PropertyName) && !(e.PropertyName == VisualElement.OpacityProperty.PropertyName))))
        return;
      this.UpdateNativeControl();
    }

    private void UpdateNativeControl()
    {
      if (this.disposed)
        return;
      if (this.layer == null)
        this.layer = this.Renderer.NativeView.Layer;
      this.OnUpdateNativeControl(this.layer);
      // ISSUE: reference to a compiler-generated field
      if (this.NativeControlUpdated == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.NativeControlUpdated((object) this, EventArgs.Empty);
    }

    private void OnUpdateNativeControl(CALayer caLayer)
    {
      VisualElement view = this.Renderer.Element;
      UIView nativeView = this.Renderer.NativeView;
      if (view == null || view.Batched)
        return;
      if (this.inputTransparent != view.InputTransparent)
      {
        nativeView.UserInteractionEnabled = !view.InputTransparent;
        this.inputTransparent = view.InputTransparent;
      }
      bool boundsChanged = this.lastBounds != view.Bounds;
      int num = boundsChanged ? 0 : (!caLayer.Frame.IsEmpty ? 1 : 0);
      float anchorX = (float) view.AnchorX;
      float anchorY = (float) view.AnchorY;
      float translationX = (float) view.TranslationX;
      float translationY = (float) view.TranslationY;
      float rotationX = (float) view.RotationX;
      float rotationY = (float) view.RotationY;
      float rotation = (float) view.Rotation;
      float scale = (float) view.Scale;
      float width = (float) view.Width;
      float height = (float) view.Height;
      float x = (float) view.X;
      float y = (float) view.Y;
      float opacity = (float) view.Opacity;
      bool isVisible = (view.IsVisible ? 1 : 0) != 0;
      int updateTarget = Interlocked.Increment(ref this.updateCount);
      Action action = (Action) (() =>
      {
        if (updateTarget != this.updateCount)
          return;
        VisualElement visualElement = view;
        Element parent = view.Parent;
        bool flag = false;
        if (isVisible && caLayer.Hidden)
        {
          caLayer.Hidden = false;
          if (!caLayer.Frame.IsEmpty)
            flag = true;
        }
        if (!isVisible && !caLayer.Hidden)
        {
          caLayer.Hidden = true;
          flag = true;
        }
        CATransform3D caTransform3D = CATransform3D.Identity;
        if (((visualElement is Page && !(visualElement is ContentPage) || ((double) width <= 0.0 || (double) height <= 0.0) ? 0 : (parent != null ? 1 : 0)) & (boundsChanged ? 1 : 0)) != 0)
        {
          RectangleF rectangleF = new RectangleF(x, y, width, height);
          caLayer.Transform = caTransform3D;
          caLayer.Frame = (CGRect) rectangleF;
          if (flag)
          {
            // ISSUE: reference to a compiler-generated method
            caLayer.LayoutSublayers();
          }
        }
        else if ((double) width <= 0.0 || (double) height <= 0.0)
        {
          caLayer.Hidden = true;
          return;
        }
        caLayer.AnchorPoint = (CGPoint) new PointF(anchorX, anchorY);
        caLayer.Opacity = opacity;
        if (Math.Abs((double) anchorX - 0.5) > 0.001)
          caTransform3D = caTransform3D.Translate((nfloat) ((anchorX - 0.5f) * width), (nfloat) 0, (nfloat) 0);
        if (Math.Abs((double) anchorY - 0.5) > 0.001)
          caTransform3D = caTransform3D.Translate((nfloat) 0, (nfloat) ((anchorY - 0.5f) * height), (nfloat) 0);
        if ((double) Math.Abs(translationX) > 0.001 || (double) Math.Abs(translationY) > 0.001)
          caTransform3D = caTransform3D.Translate((nfloat) translationX, (nfloat) translationY, (nfloat) 0);
        if ((double) Math.Abs(scale - 1f) > 0.001)
          caTransform3D = caTransform3D.Scale((nfloat) scale);
        if ((double) Math.Abs(rotationY % 180f) > 0.001 || (double) Math.Abs(rotationX % 180f) > 0.001)
          caTransform3D.m34 = (nfloat) (-1.0 / 400.0);
        if ((double) Math.Abs(rotationX % 360f) > 0.001)
          caTransform3D = caTransform3D.Rotate((nfloat) ((float) ((double) rotationX * 3.14159274101257 / 180.0)), (nfloat) 1f, (nfloat) 0.0f, (nfloat) 0.0f);
        if ((double) Math.Abs(rotationY % 360f) > 0.001)
          caTransform3D = caTransform3D.Rotate((nfloat) ((float) ((double) rotationY * 3.14159274101257 / 180.0)), (nfloat) 0.0f, (nfloat) 1f, (nfloat) 0.0f);
        caTransform3D = caTransform3D.Rotate((nfloat) ((float) ((double) rotation * 3.14159274101257 / 180.0)), (nfloat) 0.0f, (nfloat) 0.0f, (nfloat) 1f);
        caLayer.Transform = caTransform3D;
      });
      if (num != 0)
        CADisplayLinkTicker.Default.Invoke(action);
      else
        action();
      this.lastBounds = view.Bounds;
    }
  }
}
