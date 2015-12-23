using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	internal class PlatformRenderer : NSViewController
	{
		public Platform Platform { get; set; }

		internal PlatformRenderer (Platform platform)
		{
			this.Platform = platform;
		}


		public override void ViewDidLayout ()
		{
			base.ViewDidLayout ();
			Platform.LayoutSubviews ();
		}

		public override void ViewDidAppear ()
		{
			this.Platform.DidAppear ();
			base.ViewDidAppear ();
		}

		public override void ViewWillAppear ()
		{
			View.SetBackgroundColor( NSColor.White );

			this.Platform.WillAppear ();
			base.ViewWillAppear ();
		}
	}
}
