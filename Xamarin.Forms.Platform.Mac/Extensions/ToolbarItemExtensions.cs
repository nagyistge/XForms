using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Mac;

namespace Xamarin.Forms.Platform.Mac
{
	public static class ToolbarItemExtensions
	{

		public static NSToolbarItem ToNSToolbarItem (this ToolbarItem item, bool forceName = false)
		{
			if (item.Order != ToolbarItemOrder.Secondary)
				return (NSToolbarItem)new ToolbarItemExtensions.PrimaryToolbarItem (item, forceName);
			return (NSToolbarItem)new ToolbarItemExtensions.SecondaryToolbarItem (item);
		}

		private sealed class PrimaryToolbarItem : NSToolbarItem
		{
			private ToolbarItem item;
			private bool forceName;

			public PrimaryToolbarItem (ToolbarItem item, bool forceName)
			{
				this.forceName = forceName;
				this.item = item;
				if (!string.IsNullOrEmpty ((string)item.Icon) && !forceName)
					this.UpdateIconAndStyle ();
				else
					this.UpdateTextAndStyle ();
				this.UpdateIsEnabled ();

				//this.Clicked += (EventHandler)((sender, e) => item.Activate ());
				item.PropertyChanged += OnPropertyChanged;
			}

			private void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "IsEnabled")
					this.UpdateIsEnabled ();
				else if (e.PropertyName == "Text")
				{
					if (!string.IsNullOrEmpty ((string)this.item.Icon) && !this.forceName)
						return;
					this.UpdateTextAndStyle ();
				}
				else
				{
					if (!(e.PropertyName == "Icon") || this.forceName)
						return;
					if (!string.IsNullOrEmpty ((string)this.item.Icon))
						this.UpdateIconAndStyle ();
					else
						this.UpdateTextAndStyle ();
				}
			}

			private void UpdateIsEnabled ()
			{
				this.Enabled = this.item.IsEnabled;
			}

			private void UpdateTextAndStyle ()
			{
				this.Label = this.item.Text;
				//this.Style = UIBarButtonItemStyle.Bordered;
				this.Image = null;
			}

			private void UpdateIconAndStyle ()
			{
				this.Image = new NSImage((string)this.item.Icon);
				//this.Style = UIBarButtonItemStyle.Plain;
			}

			protected override void Dispose (bool disposing)
			{
				if (disposing)
					this.item.PropertyChanged -= OnPropertyChanged;
				base.Dispose (disposing);
			}
		}

		private sealed class SecondaryToolbarItem : NSToolbarItem
		{
			private ToolbarItem item;

			public SecondaryToolbarItem (ToolbarItem item)
				: base ("SecondaryToolbarItem")
			{
				this.item = item;
				this.UpdateText ();
				this.UpdateIcon ();
				this.UpdateIsEnabled ();

				//((UIControl)this.CustomView).TouchUpInside += (EventHandler)((sender, e) => item.Activate ());
				item.PropertyChanged += OnPropertyChanged;
			}

			private void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == MenuItem.TextProperty.PropertyName)
					this.UpdateText ();
				else if (e.PropertyName == MenuItem.IconProperty.PropertyName)
				{
					this.UpdateIcon ();
				}
				else
				{
					if (!(e.PropertyName == "IsEnabled"))
						return;
					this.UpdateIsEnabled ();
				}
			}

			private void UpdateText ()
			{
				// TODO: Toolbar Text
				//((ToolbarItemExtensions.SecondaryToolbarItem.SecondaryToolbarItemContent)this.CustomView).Text = this.item.Text;
			}

			private void UpdateIcon ()
			{
				// TODO: Toolbar Icon
				//((ToolbarItemExtensions.SecondaryToolbarItem.SecondaryToolbarItemContent)this.CustomView).Image = string.IsNullOrEmpty ((string)this.item.Icon) ? (UIImage)null : new UIImage ((string)this.item.Icon);
			}

			private void UpdateIsEnabled ()
			{
				// TODO: Update Customer View
				//((UIControl)this.CustomView).Enabled = this.item.IsEnabled;
			}

			protected override void Dispose (bool disposing)
			{
				if (disposing)
					this.item.PropertyChanged -= OnPropertyChanged;
				base.Dispose (disposing);
			}

			private sealed class SecondaryToolbarItemContent : NSControl
			{
				private NSImageView imageView;
				private NSTextField label;

				public override bool Enabled
				{
					get	{ return base.Enabled; }
					set
					{
						base.Enabled = value;
						this.label.Enabled = value;
						this.imageView.AlphaValue = (nfloat)(value ? 1f : 0.25f);
					}
				}

				public string Text
				{
					get	{ return this.label.StringValue; }
					set	{ this.label.StringValue = value; }
				}

				public NSImage Image
				{
					get	{ return this.imageView.Image; }
					set { this.imageView.Image = value; }
				}

				public Color BackgroundColor
				{
					get { return this.GetBackgroundColor (); }
					set { this.SetBackgroundColor (value); }
				}

				public SecondaryToolbarItemContent ()
					: base (new CGRect ((nfloat)0, (nfloat)0, (nfloat)75, (nfloat)20))
				{
					BackgroundColor = Color.Transparent;

					var imageView = new NSImageView ();
					imageView.SetBackgroundColor(Color.Transparent);

					this.imageView = imageView;

					this.AddSubview ((NSView)this.imageView);

					var textField = new NSTextField ();
					textField.SetBackgroundColor(Color.Transparent);
					textField.MaximumNumberOfLines = 1;
					textField.LineBreakMode = NSLineBreakMode.TruncatingTail;
					textField.Font = NSFont.SystemFontOfSize ((nfloat)10);
					this.label = textField;

					this.AddSubview (this.label);
				}

				public override void Layout ()
				{
					base.Layout ();

					CGSize size = this.imageView.SizeThatFits (this.Bounds.Size);
					CGSize cgSize1 = this.label.SizeThatFits (this.Bounds.Size);

					if (size.Width > (nfloat)0 && (string.IsNullOrEmpty (this.Text) || cgSize1.Width > this.Bounds.Width / (nfloat)3))
					{
						this.imageView.Frame = new CGRect (CGPoint.Empty, size);
						// TODO: I dunno.  Center image or something
						//this.imageView.CenterXAnchor = RectangleFExtensions.GetMidX (this.Bounds);
						//this.imageView.CenterYAnchor = RectangleFExtensions.GetMidY (this.Bounds);
						this.label.Hidden = true;
					}
					else
					{
						this.label.Hidden = false;
						CGSize cgSize2 = this.label.SizeThatFits (new CGSize (this.Bounds.Width - (nfloat)15f - size.Width, this.Bounds.Height - (nfloat)10f));
						CGRect cgRect = new CGRect (new CGPoint ((this.Bounds.Width - cgSize2.Width - size.Width) / (nfloat)2, RectangleFExtensions.GetMidY (this.Bounds) - size.Height / (nfloat)2), size);
						this.imageView.Frame = cgRect;
						cgRect.X = cgRect.Right + (nfloat)(size.Width > (nfloat)0 ? 5f : 0.0f);
						cgRect.Size = cgSize2;
						cgRect.Height = this.Bounds.Height;
						cgRect.Y = (nfloat)0;
						this.label.Frame = cgRect;
					}
				}
			}
		}
	}
}
