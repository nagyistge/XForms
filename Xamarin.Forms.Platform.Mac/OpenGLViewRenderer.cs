using CoreAnimation;
using CoreGraphics;
using Foundation;
using GLKit;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class OpenGLViewRenderer : ViewRenderer<OpenGLView, GLKView>
  {
    private CADisplayLink displayLink;

    protected override void OnElementChanged(ElementChangedEventArgs<OpenGLView> e)
    {
      if (e.OldElement != null)
        this.Element.remove_DisplayRequested(new EventHandler(this.Display));
      if (e.NewElement != null)
      {
        EAGLContext eaglContext = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
        GLKView uiview = new GLKView(CGRect.Empty);
        uiview.Context = eaglContext;
        uiview.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
        OpenGLViewRenderer.Delegate @delegate = new OpenGLViewRenderer.Delegate(this.Element);
        uiview.Delegate = (IGLKViewDelegate) @delegate;
        this.SetNativeControl(uiview);
        this.Element.add_DisplayRequested(new EventHandler(this.Display));
        this.SetupRenderLoop(false);
      }
      this.OnElementChanged(e);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.displayLink != null)
      {
        // ISSUE: reference to a compiler-generated method
        this.displayLink.Invalidate();
        this.displayLink.Dispose();
        this.displayLink = (CADisplayLink) null;
        if (this.Element != null)
          this.Element.remove_DisplayRequested(new EventHandler(this.Display));
      }
      base.Dispose(disposing);
    }

    public void Display(object sender, EventArgs eventArgs)
    {
      if (this.Element.HasRenderLoop)
        return;
      this.SetupRenderLoop(true);
    }

    private void SetupRenderLoop(bool oneShot)
    {
      if (this.displayLink != null || !oneShot && !this.Element.HasRenderLoop)
        return;
      this.displayLink = CADisplayLink.Create((Action) (() =>
      {
        GLKView control = this.Control;
        OpenGLView element = this.Element;
        if (control != null)
        {
          // ISSUE: reference to a compiler-generated method
          control.Display();
        }
        if (control != null && element != null && element.HasRenderLoop)
          return;
        // ISSUE: reference to a compiler-generated method
        this.displayLink.Invalidate();
        this.displayLink.Dispose();
        this.displayLink = (CADisplayLink) null;
      }));
      // ISSUE: reference to a compiler-generated method
      this.displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (!(e.PropertyName == OpenGLView.HasRenderLoopProperty.PropertyName))
        return;
      this.SetupRenderLoop(false);
    }

    private class Delegate : GLKViewDelegate
    {
      private readonly OpenGLView model;

      public Delegate(OpenGLView model)
      {
        this.model = model;
      }

      public override void DrawInRect(GLKView view, CGRect rect)
      {
        Action<Rectangle> onDisplay = this.model.get_OnDisplay();
        if (onDisplay == null)
          return;
        onDisplay(RectangleExtensions.ToRectangle(rect));
      }
    }
  }
}
