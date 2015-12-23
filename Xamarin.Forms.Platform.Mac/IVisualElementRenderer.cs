using System;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public interface IVisualElementRenderer : IDisposable, IRegisterable
  {
    VisualElement Element { get; }

    NSView NativeView { get; }

    NSViewController ViewController { get; }

    event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    void SetElement(VisualElement element);

    SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

    void SetElementSize(Size size);
  }
}
