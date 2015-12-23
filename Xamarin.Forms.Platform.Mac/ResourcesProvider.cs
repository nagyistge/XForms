using System;
using System.Collections.Generic;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	internal class ResourcesProvider : ISystemResourcesProvider
	{
		private ResourceDictionary dictionary;

		public ResourcesProvider ()
		{
			/*
			if (!Forms.IsiOS7OrNewer)
				return;
			NSApplication.Notifications.ObserveContentSizeCategoryChanged ((EventHandler<UIContentSizeCategoryChangedEventArgs>)((sender, args) => this.UpdateStyles ()));
			*/
		}

		public IResourceDictionary GetSystemResources ()
		{
			this.dictionary = new ResourceDictionary ();
			this.UpdateStyles ();
			return (IResourceDictionary)this.dictionary;
		}

		private void UpdateStyles ()
		{
			/*
			if (Forms.IsiOS7OrNewer)
			{
				this.dictionary [Device.Styles.TitleStyleKey] = this.GenerateStyle (UIFont.PreferredHeadline);
				this.dictionary [Device.Styles.SubtitleStyleKey] = this.GenerateStyle (UIFont.PreferredSubheadline);
				this.dictionary [Device.Styles.BodyStyleKey] = this.GenerateStyle (UIFont.PreferredBody);
				this.dictionary [Device.Styles.CaptionStyleKey] = this.GenerateStyle (UIFont.PreferredCaption1);
			}
			else
			*/
			{
				this.dictionary [Device.Styles.TitleStyleKey] = this.GenerateStyle (NSFont.BoldSystemFontOfSize ((nfloat)17));
				this.dictionary [Device.Styles.SubtitleStyleKey] = this.GenerateStyle (NSFont.SystemFontOfSize ((nfloat)15));
				this.dictionary [Device.Styles.BodyStyleKey] = this.GenerateStyle (NSFont.SystemFontOfSize ((nfloat)17));
				this.dictionary [Device.Styles.CaptionStyleKey] = this.GenerateStyle (NSFont.SystemFontOfSize ((nfloat)12));
			}
			this.dictionary [Device.Styles.ListItemTextStyleKey] = this.GenerateListItemTextStyle ();

			// Not for Mac
			//this.dictionary [Device.Styles.ListItemDetailTextStyleKey] = this.GenerateListItemDetailTextStyle ();
		}

		private Style GenerateListItemTextStyle ()
		{
			return this.GenerateStyle (new NSTableCellView().TextField.Font);
		}

		/*
		private Style GenerateListItemDetailTextStyle ()
		{
			return this.GenerateStyle (new NSTableCellView (UITableViewCellStyle.Subtitle, "Foobar").DetailTextLabel.Font);
		}
		*/

		private Style GenerateStyle (NSFont font)
		{
			Style style = new Style (typeof(Label));

			style.Setters.Add (new Setter () {
				Property = Label.FontSizeProperty,
				Value = font.PointSize
			});

			style.Setters.Add (new Setter () {
				Property = Label.FontFamilyProperty,
				Value = font.FontName
			});

			return style;
		}
	}
}
