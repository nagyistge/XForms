using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class UIViewExtensions
  {
    public static IEnumerable<UIView> Descendants(this UIView self)
    {
      if (self.Subviews == null)
        return Enumerable.Empty<UIView>();
      return Enumerable.Concat<UIView>((IEnumerable<UIView>) self.Subviews, Enumerable.SelectMany<UIView, UIView>((IEnumerable<UIView>) self.Subviews, (Func<UIView, IEnumerable<UIView>>) (s => UIViewExtensions.Descendants(s))));
    }

    public static SizeRequest GetSizeRequest(this UIView self, double widthConstraint, double heightConstraint, double minimumWidth = -1.0, double minimumHeight = -1.0)
    {
      // ISSUE: reference to a compiler-generated method
      CGSize cgSize = self.SizeThatFits((CGSize) new SizeF((float) widthConstraint, (float) heightConstraint));
      Xamarin.Forms.Size request = new Xamarin.Forms.Size(cgSize.Width == (nfloat) float.PositiveInfinity ? double.PositiveInfinity : (double) cgSize.Width, cgSize.Height == (nfloat) float.PositiveInfinity ? double.PositiveInfinity : (double) cgSize.Height);
      Xamarin.Forms.Size minimum = new Xamarin.Forms.Size(minimumWidth < 0.0 ? request.Width : minimumWidth, minimumHeight < 0.0 ? request.Height : minimumHeight);
      return new SizeRequest(request, minimum);
    }

    internal static T FindDescendantView<T>(this UIView view) where T : UIView
    {
      Queue<UIView> queue = new Queue<UIView>();
      queue.Enqueue(view);
      while (queue.Count > 0)
      {
        UIView uiView = queue.Dequeue();
        T obj = uiView as T;
        if ((object) obj != null)
          return obj;
        for (int index = 0; index < uiView.Subviews.Length; ++index)
          queue.Enqueue(uiView.Subviews[index]);
      }
      return default (T);
    }

    internal static UIView FindFirstResponder(this UIView view)
    {
      if (view.IsFirstResponder)
        return view;
      foreach (UIView view1 in view.Subviews)
      {
        UIView firstResponder = UIViewExtensions.FindFirstResponder(view1);
        if (firstResponder != null)
          return firstResponder;
      }
      return (UIView) null;
    }
  }
}
