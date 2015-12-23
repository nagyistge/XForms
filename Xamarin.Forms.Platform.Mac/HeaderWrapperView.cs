using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	internal class HeaderWrapperView : NSView
	{
		public override void Layout ()
		{
			base.Layout ();
			foreach (NSView uiView in this.Subviews)
				uiView.Frame = this.Bounds;
		}
	}
}
