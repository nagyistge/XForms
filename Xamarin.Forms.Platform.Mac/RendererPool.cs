using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public sealed class RendererPool
  {
    private readonly Dictionary<Type, Stack<IVisualElementRenderer>> freeRenderers = new Dictionary<Type, Stack<IVisualElementRenderer>>();
    private readonly IVisualElementRenderer parent;
    private readonly VisualElement oldElement;

    public RendererPool(IVisualElementRenderer renderer, VisualElement oldElement)
    {
      if (renderer == null)
        throw new ArgumentNullException("renderer");
      if (oldElement == null)
        throw new ArgumentNullException("oldElement");
      this.oldElement = oldElement;
      this.parent = renderer;
    }

    public void UpdateNewElement(VisualElement newElement)
    {
      if (newElement == null)
        throw new ArgumentNullException("newElement");
      bool flag = true;
      ReadOnlyCollection<Element> logicalChildren1 = this.oldElement.get_LogicalChildren();
      ReadOnlyCollection<Element> logicalChildren2 = newElement.get_LogicalChildren();
      if (logicalChildren1.Count == logicalChildren2.Count)
      {
        for (int index = 0; index < logicalChildren1.Count; ++index)
        {
          if (((object) logicalChildren1[index]).GetType() != ((object) logicalChildren2[index]).GetType())
          {
            flag = false;
            break;
          }
        }
      }
      else
        flag = false;
      if (!flag)
      {
        this.ClearRenderers(this.parent);
        this.FillChildrenWithRenderers(newElement);
      }
      else
        this.UpdateRenderers((Element) newElement);
    }

    public IVisualElementRenderer GetFreeRenderer(VisualElement view)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      Stack<IVisualElementRenderer> stack;
      if (!this.freeRenderers.TryGetValue(Registrar.Registered.GetHandlerType(((object) view).GetType()) ?? typeof (ViewRenderer), out stack) || stack.Count == 0)
        return (IVisualElementRenderer) null;
      IVisualElementRenderer visualElementRenderer = stack.Pop();
      VisualElement element = view;
      visualElementRenderer.SetElement(element);
      return visualElementRenderer;
    }

    private void UpdateRenderers(Element newElement)
    {
      if (newElement.get_LogicalChildren().Count == 0)
        return;
      foreach (UIView uiView in this.parent.NativeView.Subviews)
      {
        IVisualElementRenderer visualElementRenderer = uiView as IVisualElementRenderer;
        if (visualElementRenderer != null)
        {
          int index = (int) visualElementRenderer.NativeView.Layer.ZPosition / 1000;
          VisualElement visualElement = newElement.get_LogicalChildren()[index] as VisualElement;
          if (visualElement != null)
          {
            if (visualElementRenderer.Element != null && visualElementRenderer == Platform.GetRenderer(visualElementRenderer.Element))
              visualElementRenderer.Element.ClearValue(Platform.RendererProperty);
            visualElementRenderer.SetElement(visualElement);
            Platform.SetRenderer(visualElement, visualElementRenderer);
          }
        }
      }
    }

    private void FillChildrenWithRenderers(VisualElement element)
    {
      foreach (Element element1 in element.get_LogicalChildren())
      {
        VisualElement visualElement = element1 as VisualElement;
        if (visualElement != null)
        {
          IVisualElementRenderer visualElementRenderer = this.GetFreeRenderer(visualElement) ?? Platform.CreateRenderer(visualElement);
          Platform.SetRenderer(visualElement, visualElementRenderer);
          // ISSUE: reference to a compiler-generated method
          this.parent.NativeView.AddSubview(visualElementRenderer.NativeView);
        }
      }
    }

    private void ClearRenderers(IVisualElementRenderer renderer)
    {
      if (renderer == null)
        return;
      UIView[] subviews = renderer.NativeView.Subviews;
      for (int index = 0; index < subviews.Length; ++index)
      {
        IVisualElementRenderer renderer1 = subviews[index] as IVisualElementRenderer;
        if (renderer1 != null)
        {
          this.PushRenderer(renderer1);
          if (renderer1 == Platform.GetRenderer(renderer1.Element))
            renderer1.Element.ClearValue(Platform.RendererProperty);
        }
        // ISSUE: reference to a compiler-generated method
        subviews[index].RemoveFromSuperview();
      }
    }

    private void PushRenderer(IVisualElementRenderer renderer)
    {
      Type type = renderer.GetType();
      Stack<IVisualElementRenderer> stack;
      if (!this.freeRenderers.TryGetValue(type, out stack))
        this.freeRenderers[type] = stack = new Stack<IVisualElementRenderer>();
      stack.Push(renderer);
    }
  }
}
