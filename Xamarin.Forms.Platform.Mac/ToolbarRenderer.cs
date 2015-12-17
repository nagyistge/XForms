using CoreGraphics;
using Foundation;
using System;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ToolbarRenderer : ViewRenderer
  {
    protected override void OnElementChanged(ElementChangedEventArgs<View> e)
    {
      this.OnElementChanged(e);
      this.SetNativeControl((UIView) new UIToolbar((CGRect) RectangleF.Empty));
      this.UpdateItems(false);
      ((Toolbar) this.Element).add_ItemAdded(new EventHandler<ToolbarItemEventArgs>(this.OnItemAdded));
      ((Toolbar) this.Element).add_ItemRemoved(new EventHandler<ToolbarItemEventArgs>(this.OnItemRemoved));
    }

    private void OnItemRemoved(object sender, ToolbarItemEventArgs eventArg)
    {
      this.UpdateItems(true);
    }

    private void OnItemAdded(object sender, ToolbarItemEventArgs eventArg)
    {
      this.UpdateItems(true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        ((Toolbar) this.Element).remove_ItemAdded(new EventHandler<ToolbarItemEventArgs>(this.OnItemAdded));
        ((Toolbar) this.Element).remove_ItemRemoved(new EventHandler<ToolbarItemEventArgs>(this.OnItemRemoved));
        if (((UIToolbar) this.Control).Items != null)
        {
          foreach (NSObject nsObject in ((UIToolbar) this.Control).Items)
            nsObject.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    private void UpdateItems(bool animated)
    {
      if (((UIToolbar) this.Control).Items != null)
      {
        for (int index = 0; index < ((UIToolbar) this.Control).Items.Length; ++index)
          ((UIToolbar) this.Control).Items[index].Dispose();
      }
      UIBarButtonItem[] items = new UIBarButtonItem[((Toolbar) this.Element).get_Items().Count];
      for (int index = 0; index < ((Toolbar) this.Element).get_Items().Count; ++index)
        items[index] = ToolbarItemExtensions.ToUIBarButtonItem(((Toolbar) this.Element).get_Items()[index], false);
      ((UIToolbar) this.Control).SetItems(items, animated);
    }
  }
}
