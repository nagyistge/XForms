using System;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class VisualElementPackager : IDisposable
  {
    private bool isDisposed;
    private VisualElement element;

    protected IVisualElementRenderer Renderer { get; set; }

    public VisualElementPackager(IVisualElementRenderer renderer)
    {
      if (renderer == null)
        throw new ArgumentNullException("renderer");
      this.Renderer = renderer;
      renderer.ElementChanged += new EventHandler<VisualElementChangedEventArgs>(this.OnRendererElementChanged);
      this.SetElement((VisualElement) null, renderer.Element);
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    public void Load()
    {
      for (int index = 0; index < this.Renderer.Element.get_LogicalChildren().Count; ++index)
      {
        VisualElement view = this.Renderer.Element.get_LogicalChildren()[index] as VisualElement;
        if (view != null)
          this.OnChildAdded(view);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
      {
        this.SetElement(this.element, (VisualElement) null);
        if (this.Renderer != null)
        {
          this.Renderer.ElementChanged -= new EventHandler<VisualElementChangedEventArgs>(this.OnRendererElementChanged);
          this.Renderer = (IVisualElementRenderer) null;
        }
      }
      this.isDisposed = true;
    }

    private void SetElement(VisualElement oldElement, VisualElement newElement)
    {
      if (oldElement != null)
      {
        oldElement.remove_ChildAdded(new EventHandler<ElementEventArgs>(this.OnChildAdded));
        oldElement.remove_ChildRemoved(new EventHandler<ElementEventArgs>(this.OnChildRemoved));
        oldElement.remove_ChildrenReordered(new EventHandler(this.UpdateChildrenOrder));
        if (newElement != null)
        {
          new RendererPool(this.Renderer, oldElement).UpdateNewElement(newElement);
          this.EnsureChildrenOrder();
        }
        else
        {
          for (int index = 0; index < oldElement.get_LogicalChildren().Count; ++index)
          {
            VisualElement view = oldElement.get_LogicalChildren()[index] as VisualElement;
            if (view != null)
              this.OnChildRemoved(view);
          }
        }
      }
      this.element = newElement;
      if (newElement == null)
        return;
      newElement.add_ChildAdded(new EventHandler<ElementEventArgs>(this.OnChildAdded));
      newElement.add_ChildRemoved(new EventHandler<ElementEventArgs>(this.OnChildRemoved));
      newElement.add_ChildrenReordered(new EventHandler(this.UpdateChildrenOrder));
    }

    private void UpdateChildrenOrder(object sender, EventArgs e)
    {
      this.EnsureChildrenOrder();
    }

    private void EnsureChildrenOrder()
    {
      if (this.Renderer.Element.get_LogicalChildren().Count == 0)
        return;
      for (int index = 0; index < this.Renderer.Element.get_LogicalChildren().Count; ++index)
      {
        VisualElement bindable = this.Renderer.Element.get_LogicalChildren()[index] as VisualElement;
        if (bindable != null)
        {
          IVisualElementRenderer renderer = Platform.GetRenderer(bindable);
          if (renderer != null)
          {
            UIView nativeView = renderer.NativeView;
            // ISSUE: reference to a compiler-generated method
            this.Renderer.NativeView.BringSubviewToFront(nativeView);
            nativeView.Layer.ZPosition = (nfloat) (index * 1000);
          }
        }
      }
    }

    private void OnChildAdded(object sender, ElementEventArgs e)
    {
      VisualElement view = e.Element as VisualElement;
      if (view == null)
        return;
      this.OnChildAdded(view);
    }

    protected virtual void OnChildAdded(VisualElement view)
    {
      IVisualElementRenderer renderer = Platform.CreateRenderer(view);
      Platform.SetRenderer(view, renderer);
      // ISSUE: reference to a compiler-generated method
      this.Renderer.NativeView.AddSubview(renderer.NativeView);
      if (this.Renderer.ViewController != null && renderer.ViewController != null)
      {
        // ISSUE: reference to a compiler-generated method
        this.Renderer.ViewController.AddChildViewController(renderer.ViewController);
      }
      this.EnsureChildrenOrder();
    }

    private void OnChildRemoved(object sender, ElementEventArgs e)
    {
      VisualElement view = e.Element as VisualElement;
      if (view == null)
        return;
      this.OnChildRemoved(view);
    }

    protected virtual void OnChildRemoved(VisualElement view)
    {
      IVisualElementRenderer renderer = Platform.GetRenderer(view);
      if (renderer == null || renderer.NativeView == null)
        return;
      // ISSUE: reference to a compiler-generated method
      renderer.NativeView.RemoveFromSuperview();
      if (this.Renderer.ViewController == null || renderer.ViewController == null)
        return;
      // ISSUE: reference to a compiler-generated method
      renderer.ViewController.RemoveFromParentViewController();
    }

    private void OnRendererElementChanged(object sender, VisualElementChangedEventArgs args)
    {
      if (args.NewElement == this.element)
        return;
      this.SetElement(this.element, args.NewElement);
    }
  }
}
