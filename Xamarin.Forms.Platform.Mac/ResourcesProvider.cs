using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class ResourcesProvider : ISystemResourcesProvider
  {
    private ResourceDictionary dictionary;

    public ResourcesProvider()
    {
      if (!Forms.IsiOS7OrNewer)
        return;
      UIApplication.Notifications.ObserveContentSizeCategoryChanged((EventHandler<UIContentSizeCategoryChangedEventArgs>) ((sender, args) => this.UpdateStyles()));
    }

    public IResourceDictionary GetSystemResources()
    {
      this.dictionary = new ResourceDictionary();
      this.UpdateStyles();
      return (IResourceDictionary) this.dictionary;
    }

    private void UpdateStyles()
    {
      if (Forms.IsiOS7OrNewer)
      {
        this.dictionary[Device.Styles.TitleStyleKey] = (object) this.GenerateStyle(UIFont.PreferredHeadline);
        this.dictionary[Device.Styles.SubtitleStyleKey] = (object) this.GenerateStyle(UIFont.PreferredSubheadline);
        this.dictionary[Device.Styles.BodyStyleKey] = (object) this.GenerateStyle(UIFont.PreferredBody);
        this.dictionary[Device.Styles.CaptionStyleKey] = (object) this.GenerateStyle(UIFont.PreferredCaption1);
      }
      else
      {
        this.dictionary[Device.Styles.TitleStyleKey] = (object) this.GenerateStyle(UIFont.BoldSystemFontOfSize((nfloat) 17));
        this.dictionary[Device.Styles.SubtitleStyleKey] = (object) this.GenerateStyle(UIFont.SystemFontOfSize((nfloat) 15));
        this.dictionary[Device.Styles.BodyStyleKey] = (object) this.GenerateStyle(UIFont.SystemFontOfSize((nfloat) 17));
        this.dictionary[Device.Styles.CaptionStyleKey] = (object) this.GenerateStyle(UIFont.SystemFontOfSize((nfloat) 12));
      }
      this.dictionary[Device.Styles.ListItemTextStyleKey] = (object) this.GenerateListItemTextStyle();
      this.dictionary[Device.Styles.ListItemDetailTextStyleKey] = (object) this.GenerateListItemDetailTextStyle();
    }

    private Style GenerateListItemTextStyle()
    {
      return this.GenerateStyle(new UITableViewCell(UITableViewCellStyle.Subtitle, "Foobar").TextLabel.Font);
    }

    private Style GenerateListItemDetailTextStyle()
    {
      return this.GenerateStyle(new UITableViewCell(UITableViewCellStyle.Subtitle, "Foobar").DetailTextLabel.Font);
    }

    private Style GenerateStyle(UIFont font)
    {
      Style style = new Style(typeof (Label));
      IList<Setter> setters1 = style.get_Setters();
      Setter setter1 = new Setter();
      setter1.Property = Label.FontSizeProperty;
      // ISSUE: variable of a boxed type
      __Boxed<double> local = (ValueType) (double) font.PointSize;
      setter1.Value = (object) local;
      setters1.Add(setter1);
      IList<Setter> setters2 = style.get_Setters();
      Setter setter2 = new Setter();
      setter2.Property = Label.FontFamilyProperty;
      string name = font.Name;
      setter2.Value = (object) name;
      setters2.Add(setter2);
      return style;
    }
  }
}
