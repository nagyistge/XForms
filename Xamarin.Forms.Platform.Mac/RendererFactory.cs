using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class RendererFactory
  {
    [Obsolete("Use Platform.CreateRenderer")]
    public static IVisualElementRenderer GetRenderer(VisualElement view)
    {
      return Platform.CreateRenderer(view);
    }
  }
}
