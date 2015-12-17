using CoreGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  [Flags]
  public enum VisualElementRendererFlags
  {
    Disposed = 1,
    AutoTrack = 2,
    AutoPackage = 4,
  }

  public class VisualElementRenderer<TElement> : UIView, IVisualElementRenderer, IDisposable, IRegisterable where TElement : VisualElement
  {
    private readonly List<EventHandler<VisualElementChangedEventArgs>> elementChangedHandlers = new List<EventHandler<VisualElementChangedEventArgs>>();
    private UIColor defaultColor = UIColor.Clear;
    private VisualElementRendererFlags flags = VisualElementRendererFlags.AutoTrack | VisualElementRendererFlags.AutoPackage;
    private VisualElementPackager packager;
    private VisualElementTracker tracker;
    private EventTracker events;
    private readonly PropertyChangedEventHandler propertyChangedHandler;

    public UIView NativeView
    {
      get
      {
        return (UIView) this;
      }
    }

    public UIViewController ViewController
    {
      get
      {
        return (UIViewController) null;
      }
    }

    public TElement Element { get; private set; }

    VisualElement IVisualElementRenderer.Element
    {
      get
      {
        return (VisualElement) this.Element;
      }
    }

    public override sealed UIColor BackgroundColor
    {
      get
      {
        return base.BackgroundColor;
      }
      set
      {
        base.BackgroundColor = value;
      }
    }

    protected bool AutoPackage
    {
      get
      {
        return (uint) (this.flags & VisualElementRendererFlags.AutoPackage) > 0U;
      }
      set
      {
        if (value)
          this.flags = this.flags | VisualElementRendererFlags.AutoPackage;
        else
          this.flags = this.flags & ~VisualElementRendererFlags.AutoPackage;
      }
    }

    protected bool AutoTrack
    {
      get
      {
        return (uint) (this.flags & VisualElementRendererFlags.AutoTrack) > 0U;
      }
      set
      {
        if (value)
          this.flags = this.flags | VisualElementRendererFlags.AutoTrack;
        else
          this.flags = this.flags & ~VisualElementRendererFlags.AutoTrack;
      }
    }

    public event EventHandler<ElementChangedEventArgs<TElement>> ElementChanged;

    event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
    {
      add
      {
        this.elementChangedHandlers.Add(value);
      }
      remove
      {
        this.elementChangedHandlers.Remove(value);
      }
    }

    protected VisualElementRenderer()
      : base(CGRect.Empty)
    {
      this.propertyChangedHandler = new PropertyChangedEventHandler(this.OnElementPropertyChanged);
      this.BackgroundColor = UIColor.Clear;
    }

    void IVisualElementRenderer.SetElement(VisualElement element)
    {
      this.SetElement((TElement) element);
    }

    public void SetElement(TElement element)
    {
      TElement element1 = this.Element;
      this.Element = element;
      if ((object) element1 != null)
        element1.remove_PropertyChanged(this.propertyChangedHandler);
      if ((object) element != null)
      {
        if (element.BackgroundColor != Color.Default || (object) element1 != null && element.BackgroundColor != element1.BackgroundColor)
          this.SetBackgroundColor(element.BackgroundColor);
        this.UpdateClipToBounds();
        if (this.tracker == null)
        {
          this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
          this.tracker.NativeControlUpdated += (EventHandler) ((sender, e) => this.UpdateNativeWidget());
        }
        if (this.AutoPackage && this.packager == null)
        {
          this.packager = new VisualElementPackager((IVisualElementRenderer) this);
          this.packager.Load();
        }
        if (this.AutoTrack && this.events == null)
        {
          this.events = new EventTracker((IVisualElementRenderer) this);
          this.events.LoadEvents((UIView) this);
        }
        element.add_PropertyChanged(this.propertyChangedHandler);
      }
      this.OnElementChanged(new ElementChangedEventArgs<TElement>(element1, element));
      if ((object) element == null)
        return;
      this.SendVisualElementInitialized((VisualElement) element, (UIView) this);
    }

    internal virtual void SendVisualElementInitialized(VisualElement element, UIView nativeView)
    {
      Forms.SendViewInitialized(element, nativeView);
    }

    public void SetElementSize(Size size)
    {
      this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
    }

    public virtual SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, -1.0, -1.0);
    }

    protected virtual void OnElementChanged(ElementChangedEventArgs<TElement> e)
    {
      VisualElementChangedEventArgs e1 = new VisualElementChangedEventArgs((VisualElement) e.OldElement, (VisualElement) e.NewElement);
      for (int index = 0; index < this.elementChangedHandlers.Count; ++index)
        this.elementChangedHandlers[index]((object) this, e1);
      // ISSUE: reference to a compiler-generated field
      EventHandler<ElementChangedEventArgs<TElement>> eventHandler = this.ElementChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, e);
    }

    protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
      {
        this.SetBackgroundColor(this.Element.BackgroundColor);
      }
      else
      {
        if (!(e.PropertyName == Layout.IsClippedToBoundsProperty.PropertyName))
          return;
        this.UpdateClipToBounds();
      }
    }

    private void UpdateClipToBounds()
    {
      Layout layout = (object) this.Element as Layout;
      if (layout == null)
        return;
      this.ClipsToBounds = layout.IsClippedToBounds;
    }

    protected virtual void SetBackgroundColor(Color color)
    {
      if (color == Color.Default)
        this.BackgroundColor = this.defaultColor;
      else
        this.BackgroundColor = ColorExtensions.ToUIColor(color);
    }

    protected virtual void UpdateNativeWidget()
    {
    }

    public override CGSize SizeThatFits(CGSize size)
    {
      return new CGSize((nfloat) 0, (nfloat) 0);
    }

    protected override void Dispose(bool disposing)
    {
      if ((this.flags & VisualElementRendererFlags.Disposed) != (VisualElementRendererFlags) 0)
        return;
      this.flags = this.flags | VisualElementRendererFlags.Disposed;
      if (disposing)
      {
        if (this.events != null)
        {
          this.events.Dispose();
          this.events = (EventTracker) null;
        }
        if (this.tracker != null)
        {
          this.tracker.Dispose();
          this.tracker = (VisualElementTracker) null;
        }
        if (this.packager != null)
        {
          this.packager.Dispose();
          this.packager = (VisualElementPackager) null;
        }
        Platform.SetRenderer((VisualElement) this.Element, (IVisualElementRenderer) null);
        this.SetElement(default (TElement));
        this.Element = default (TElement);
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }
  }
}
