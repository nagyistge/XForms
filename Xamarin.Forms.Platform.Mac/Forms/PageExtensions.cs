using System;
using UIKit;

namespace Xamarin.Forms
{
  public static class PageExtensions
  {
    public static UIViewController CreateViewController(this Page view)
    {
      if (!Forms.IsInitialized)
        throw new InvalidOperationException("call Forms.Init() before this");
      if (!(view.Parent is Application))
        new PageExtensions.DefaultApplication().MainPage = view;
      Xamarin.Forms.Platform.Mac.Platform platform = new Xamarin.Forms.Platform.Mac.Platform();
      Page newRoot = view;
      platform.SetPage(newRoot);
      return platform.ViewController;
    }

    private class DefaultApplication : Application
    {
    }
  }
}