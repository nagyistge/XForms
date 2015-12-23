using System;
using AppKit;

namespace Xamarin.Forms
{
	public static class PageExtensions
	{
		public static NSViewController CreateViewController (this Page view)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException ("call Forms.Init() before this");
			
			if (!(view.Parent is Application))
				new PageExtensions.DefaultApplication ().MainPage = view;
			
			Xamarin.Forms.Platform.Mac.Platform platform = new Xamarin.Forms.Platform.Mac.Platform ();
			Page newRoot = view;
			platform.SetPage (newRoot);
			return platform.ViewController;
		}

		internal static Boolean IsApplicationOrNull (Element element)
		{
			if (element == null)
				return true;
			return element is Application;
		}

		private class DefaultApplication : Application
		{
		}
	}
}