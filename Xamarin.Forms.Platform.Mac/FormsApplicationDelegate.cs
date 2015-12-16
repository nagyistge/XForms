using System;
using AppKit;
using System.ComponentModel;
using Foundation;

namespace Xamarin.Forms.Platform.Mac
{

	public class FormsApplicationDelegate : NSApplicationDelegate
	{
		private NSWindow window;
		private Application application;
		private bool isSuspended;

		protected FormsApplicationDelegate()
		{
		}

		protected void LoadApplication(Application application)
		{
			if (application == null)
				throw new ArgumentNullException("application");
			Application.Current = application;
			this.application = application;
			application.PropertyChanged += ApplicationOnPropertyChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.application != null)
				this.application.PropertyChanged -= ApplicationOnPropertyChanged;
			base.Dispose(disposing);
		}

		private void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (!(args.PropertyName == "MainPage"))
				return;
			this.UpdateMainPage();
		}

		public override bool WillFinishLaunching(NSApplication application, NSDictionary launchOptions)
		{
			return true;
		}

		public override bool FinishedLaunching(NSApplication application, NSDictionary launchOptions)
		{
			//this.window = new NSWindow(NSScreen.MainScreen.Bounds);
			this.window = new NSWindow(null,NSWindowStyle.DocModal,NSBackingStore.Retained, false);
			if (this.application == null)
				throw new InvalidOperationException("You MUST invoke LoadApplication () before calling base.FinishedLaunching ()");
			this.SetMainPage();
			//this.application.SendStart();
			return true;
		}

		public override void OnActivated(NSApplication application)
		{
			if (this.application == null || !this.isSuspended)
				return;
			this.isSuspended = false;
			//this.application.SendResume();
		}

		public override void OnResignActivation(NSApplication application)
		{
			if (this.application == null)
				return;
			this.isSuspended = true;
			this.application.SendSleepAsync().Wait();
		}

		public override void DidEnterBackground(NSApplication application)
		{
		}

		public override void WillEnterForeground(NSApplication application)
		{
		}

		public override void WillTerminate(NSApplication application)
		{
		}

		private void UpdateMainPage()
		{
			if (this.application.MainPage == null)
				return;
			PlatformRenderer platformRenderer = (PlatformRenderer) this.window.RootViewController;
			this.window.RootViewController = PageExtensions.CreateViewController(this.application.MainPage);
			if (platformRenderer == null)
				return;
			((IDisposable) platformRenderer.Platform).Dispose();
		}

		private void SetMainPage()
		{
			this.UpdateMainPage();
			// ISSUE: reference to a compiler-generated method
			this.window.MakeKeyAndVisible();
		}
	}




}

