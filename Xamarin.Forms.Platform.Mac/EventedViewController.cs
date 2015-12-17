using System;

namespace Xamarin.Forms.Platform.Mac
{
  internal class EventedViewController : ChildViewController
  {
    public event EventHandler WillAppear;

    public event EventHandler WillDisappear;

    public override void ViewWillAppear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewWillAppear(animated);
      // ISSUE: reference to a compiler-generated field
      EventHandler eventHandler = this.WillAppear;
      if (eventHandler == null)
        return;
      eventHandler((object) this, EventArgs.Empty);
    }

    public override void ViewWillDisappear(bool animated)
    {
      // ISSUE: reference to a compiler-generated method
      base.ViewWillDisappear(animated);
      // ISSUE: reference to a compiler-generated field
      EventHandler eventHandler = this.WillDisappear;
      if (eventHandler == null)
        return;
      eventHandler((object) this, EventArgs.Empty);
    }
  }
}
