using Foundation;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class FormsApplicationDelegate : NSApplicationDelegate
	{
		private NSWindow window;
		private Application application;
		private bool isSuspended;

		protected FormsApplicationDelegate ()
		{
		}

		protected void LoadApplication (Application application)
		{
			if (application == null)
				throw new ArgumentNullException ("application");
			Application.Current = application;
			this.application = application;
			application.PropertyChanged += ApplicationOnPropertyChanged;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && this.application != null)
				this.application.PropertyChanged -= ApplicationOnPropertyChanged;
			base.Dispose (disposing);
		}

		private void ApplicationOnPropertyChanged (object sender, PropertyChangedEventArgs args)
		{
			if (!(args.PropertyName == "MainPage"))
				return;
			this.UpdateMainPage ();
		}

		public override void WillFinishLaunching (NSNotification notification)
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			window = new NSWindow(NSScreen.MainScreen.Frame, NSWindowStyle.Resizable, NSBackingStore.Buffered, false);
			window.Level = NSWindowLevel.Normal;

			if (this.application == null)
				throw new InvalidOperationException ("You MUST invoke LoadApplication () before calling base.FinishedLaunching ()");
			
			this.SetMainPage ();
			this.application.SendStart ();
		}

		public override void DidBecomeActive (NSNotification notification)
		{
			if (this.application == null || !this.isSuspended)
				return;
			this.isSuspended = false;
			this.application.SendResume ();
		}

		// Not for Mac
		/*
		public override void OnResignActivation (NSApplication uiApplication)
		{
			if (this.application == null)
				return;
			this.isSuspended = true;
			this.application.SendSleepAsync ().Wait ();
		}
		*/

		private void UpdateMainPage ()
		{
			if (this.application.MainPage == null)
				return;
			
			PlatformRenderer platformRenderer = (PlatformRenderer)this.window.WindowController;

			this.window.WindowController = PageExtensions.CreateViewController (this.application.MainPage);
			if (platformRenderer == null)
				return;
			((IDisposable)platformRenderer.Platform).Dispose ();
		}

		private void SetMainPage ()
		{
			this.UpdateMainPage ();
			this.window.MakeKeyAndOrderFront (this);
		}
	}
}
