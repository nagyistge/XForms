using AppKit;
using Foundation;

namespace XForms.Mac
{
	public partial class AppDelegate : 
		global::Xamarin.Forms.Platform.Mac.FormsApplicationDelegate
		//NSApplicationDelegate
	{
		//MainWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{	
			//mainWindowController = new MainWindowController ();
			//mainWindowController.Window.MakeKeyAndOrderFront (this);

			global::Xamarin.Forms.Forms.Init ();

			LoadApplication (new App ());

			base.DidFinishLaunching (notification);
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}
