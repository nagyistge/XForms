using CoreGraphics;
using Foundation;
using System;
using System.Drawing;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	// TODO: Toolbar Renderer
	public class ToolbarRenderer : ViewRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			this.OnElementChanged (e);
			this.SetNativeControl (new NSToolbar("ToolbarRenderer"));
			this.UpdateItems (false);
			((Toolbar)this.Element).ItemAdded += OnItemAdded;
			((Toolbar)this.Element).ItemRemoved += OnItemRemoved;
		}

		private void OnItemRemoved (object sender, ToolbarItemEventArgs eventArg)
		{
			this.UpdateItems (true);
		}

		private void OnItemAdded (object sender, ToolbarItemEventArgs eventArg)
		{
			this.UpdateItems (true);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				((Toolbar)this.Element).ItemAdded -= OnItemAdded;
				((Toolbar)this.Element).ItemRemoved -= OnItemRemoved;
				if (((NSToolbar)this.Control).Items != null)
				{
					foreach (NSObject nsObject in ((NSToolbar) this.Control).Items)
						nsObject.Dispose ();
				}
			}
			base.Dispose (disposing);
		}

		private void UpdateItems (bool animated)
		{
			if (((NSToolbar)this.Control).Items != null)
			{
				for (int index = 0; index < ((NSToolbar)this.Control).Items.Length; ++index)
					((NSToolbar)this.Control).Items [index].Dispose ();
			}
			NSToolbarItem[] items = new NSToolbarItem[ ((Toolbar)this.Element).Items.Count ];
			for (int index = 0; index < ((Toolbar)this.Element).Items.Count; ++index)
				items [index] = ToolbarItemExtensions.ToNSToolbarItem (((Toolbar)this.Element).Items [index], false);

			foreach (var item in items)
			{
				((NSToolbar)this.Control).InsertItem (item, 0);
			}
			//((NSToolbar)this.Control).SetItems (items, animated);
		}
	}
}
