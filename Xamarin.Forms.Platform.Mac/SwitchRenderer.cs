using CoreGraphics;
using System;
using System.Drawing;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class SwitchRenderer : ViewRenderer<Switch, NSButton>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Switch> e)
		{
			if (e.OldElement != null)
				e.OldElement.Toggled -= OnElementToggled;
			if (e.NewElement != null)
			{
				if (this.Control == null)
				{
					this.SetNativeControl (new NSButton(RectangleF.Empty));
					Control.SetButtonType (NSButtonType.Switch);

					// TODO: Value changed
					//Control.Activated += OnControlValueChanged;
				}
				this.Control.IntValue = Element.IsToggled ? 1 : 0;

				e.NewElement.Toggled += OnElementToggled;
			}
			this.OnElementChanged (e);
		}

		private void OnElementToggled (object sender, EventArgs e)
		{
			this.Control.State = Element.IsToggled ? NSCellStateValue.On : NSCellStateValue.Off;
		}

		private void OnControlValueChanged (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (Switch.IsToggledProperty, Control.IntValue == 0 ? false : true);
		}

		protected override void Dispose (bool disposing)
		{
			//if (disposing)
			//	this.Control.ValueChanged -= new EventHandler (this.OnControlValueChanged);
			base.Dispose (disposing);
		}
	}
}
