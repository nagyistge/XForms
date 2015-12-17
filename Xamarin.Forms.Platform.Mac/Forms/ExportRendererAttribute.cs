using System;
using UIKit;

namespace Xamarin.Forms
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public sealed class ExportRendererAttribute : HandlerAttribute
  {
    internal bool Idiomatic { get; private set; }

    internal UIUserInterfaceIdiom Idiom { get; private set; }

    public ExportRendererAttribute(Type handler, Type target, UIUserInterfaceIdiom idiom)
    {
      base.\u002Ector(handler, target);
      this.Idiomatic = true;
      this.Idiom = idiom;
    }

    public ExportRendererAttribute(Type handler, Type target)
    {
      base.\u002Ector(handler, target);
      this.Idiomatic = false;
    }

    public override bool ShouldRegister()
    {
      if (this.Idiomatic)
        return this.Idiom == UIDevice.CurrentDevice.UserInterfaceIdiom;
      return true;
    }
  }
}