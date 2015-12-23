using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class NavigationMenuRenderer : ViewRenderer
	{
		private NSCollectionView collectionView;

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			this.OnElementChanged (e);

			var uiCollectionView = new NSCollectionView (new CGRect ((nfloat)0, (nfloat)0, (nfloat)100, (nfloat)100));
			uiCollectionView.MinItemSize = new CGSize(220, 44);
			uiCollectionView.DataSource = new NavigationMenuRenderer.DataSource ((NavigationMenu)Element);
			uiCollectionView.SetBackgroundColor (NSColor.White);

			uiCollectionView.RegisterClassForItem (typeof(NavigationMenuRenderer.NavigationCell), "NavigationCell");
			collectionView = uiCollectionView;
			SetNativeControl ((NSView)this.collectionView);
		}

		private sealed class NavigationCell : NSCollectionViewItem
		{
			private string icon;
			private readonly NSTextField nameLabel;
			private readonly NSButton image;

			public Action Selected { get; set; }

			public string Name
			{
				get	{	return this.nameLabel.StringValue;	}
				set	{	this.nameLabel.StringValue = value;	}
			}

			public string Icon
			{
				get	{	return this.icon;	}
				set
				{
					icon = value;
					image.Image = new NSImage (icon);
				}
			}

			[Export ("initWithFrame:")]
			public NavigationCell (CGRect frame)
			{
				var uiLabel = new NSTextField (CGRect.Empty);
				NSColor clear = NSColor.Clear;
				uiLabel.BackgroundColor = clear;

				uiLabel.Alignment = NSTextAlignment.Left;
				uiLabel.Font = NSFont.SystemFontOfSize(14);
				this.nameLabel = uiLabel;
				this.image = new NSButton (CGRect.Empty);

				this.SetupLayer ();

				this.image.Activated += (sender, e) =>
				{
					if (this.Selected == null)
						return;
					this.Selected ();
				};

				//this.image.ContentMode = UIViewContentMode.ScaleAspectFit;
				//this.image.Center = this.ContentView.Center;

				View.AddSubview(image);
				View.AddSubview(nameLabel);
			}

			private void SetupLayer ()
			{
				CALayer layer = this.image.Layer;

				layer.ShadowRadius = 6;
				layer.ShadowColor = NSColor.Black.CGColor;
				layer.ShadowOpacity = 0.200000002980232f;
				layer.ShadowOffset = new CGSize ();
				layer.ContentsScale = NSScreen.MainScreen.BackingScaleFactor;
			}

			public override void ViewWillLayout ()
			{
				base.ViewWillLayout ();

				image.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height - 30);

				CGSize cgSize = this.nameLabel.SizeThatFits (View.Frame.Size);
				var y2 = View.Frame.Height - 15f - cgSize.Height / 2f;

				nameLabel.Frame = new CGRect (0, y2, View.Frame.Width, cgSize.Height);
			}
		}

		private class DataSource : NSCollectionViewDataSource
		{
			private readonly NavigationMenu menu;

			public DataSource (NavigationMenu menu)
			{
				this.menu = menu;
			}

			public override nint GetNumberOfSections (NSCollectionView collectionView)
			{
				return 1;
			}

			public override nint GetNumberofItems (NSCollectionView collectionView, nint section)
			{
				return (nint)Enumerable.Count<Page> (menu.Targets);
			}

			public override NSCollectionViewItem GetItem (NSCollectionView collectionView, NSIndexPath indexPath)
			{
				// No view recycling on Mac
				//var navigationCell = (NavigationMenuRenderer.NavigationCell)collectionView.DequeueReusableCell (new NSString ("NavigationCell"), indexPath);
				var navigationCell = new NavigationMenuRenderer.NavigationCell(collectionView.Frame);

				var target = menu.Targets.Skip((int)indexPath.Item).FirstOrDefault ();
				if (target != null)
				{
					navigationCell.Name = target.Title;
					navigationCell.Icon = (string)target.Icon;
					navigationCell.Selected = (Action)(() => menu.SendTargetSelected (target));
				}
				else
				{
					navigationCell.Selected = (Action)null;
					navigationCell.Icon = "";
					navigationCell.Name = "";
				}
				return navigationCell;
			}
		}
	}
}
