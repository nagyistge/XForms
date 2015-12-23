using CoreGraphics;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	internal class ModalWrapper : NSViewController
	{
		private IVisualElementRenderer modal;

		internal ModalWrapper (IVisualElementRenderer modal)
		{
			this.modal = modal;
			this.View.SetBackgroundColor( NSColor.White );
			this.View.AddSubview (modal.ViewController.View);
			this.AddChildViewController (modal.ViewController);

			// TODO: WT.?
			//modal.ViewController.DidMoveToParentViewController (this);
		}

		public override void ViewDidLayout ()
		{
			base.ViewDidLayout ();

			if (this.modal == null)
				return;
			IVisualElementRenderer visualElementRenderer = this.modal;
			CGRect bounds = this.View.Bounds;
			double width = (double)bounds.Width;
			bounds = this.View.Bounds;
			double height = (double)bounds.Height;
			Size size = new Size (width, height);
			visualElementRenderer.SetElementSize (size);
		}

		public override void ViewWillAppear ()
		{
			this.View.SetBackgroundColor( NSColor.White );
			base.ViewWillAppear ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				this.modal = (IVisualElementRenderer)null;
			base.Dispose (disposing);
		}

		/*
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}
		*/
	}
}
