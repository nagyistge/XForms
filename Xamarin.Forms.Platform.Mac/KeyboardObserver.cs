using System;
using UIKit;

namespace Xamarin.Forms.Platform.Mac
{
  internal static class KeyboardObserver
  {
    public static event EventHandler<UIKeyboardEventArgs> KeyboardWillShow;

    public static event EventHandler<UIKeyboardEventArgs> KeyboardWillHide;

    static KeyboardObserver()
    {
      UIKeyboard.Notifications.ObserveWillShow(new EventHandler<UIKeyboardEventArgs>(KeyboardObserver.OnKeyboardShown));
      UIKeyboard.Notifications.ObserveWillHide(new EventHandler<UIKeyboardEventArgs>(KeyboardObserver.OnKeyboardHidden));
    }

    private static void OnKeyboardShown(object sender, UIKeyboardEventArgs args)
    {
      // ISSUE: reference to a compiler-generated field
      EventHandler<UIKeyboardEventArgs> eventHandler = KeyboardObserver.KeyboardWillShow;
      if (eventHandler == null)
        return;
      eventHandler(sender, args);
    }

    private static void OnKeyboardHidden(object sender, UIKeyboardEventArgs args)
    {
      // ISSUE: reference to a compiler-generated field
      EventHandler<UIKeyboardEventArgs> eventHandler = KeyboardObserver.KeyboardWillHide;
      if (eventHandler == null)
        return;
      eventHandler(sender, args);
    }
  }
}
