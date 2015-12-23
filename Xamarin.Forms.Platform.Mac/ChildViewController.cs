using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	internal class ChildViewController : NSViewController
	{
		public override void ViewDidLayout ()
		{
			foreach (var uiViewController in this.ChildViewControllers)
				uiViewController.View.Frame = this.View.Bounds;
		}
	}
}
