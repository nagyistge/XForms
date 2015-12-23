using System;

namespace Xamarin.Forms.Platform.Mac
{
	internal class EventedViewController : ChildViewController
	{
		public event EventHandler WillAppear;
		public event EventHandler WillDisappear;

		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			EventHandler eventHandler = this.WillAppear;
			if (eventHandler == null)
				return;
			
			eventHandler (this, EventArgs.Empty);
		}

		public override void ViewWillDisappear ()
		{
			base.ViewWillDisappear ();

			EventHandler eventHandler = this.WillDisappear;
			if (eventHandler == null)
				return;
			eventHandler (this, EventArgs.Empty);
		}
	}
}
