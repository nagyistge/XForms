using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class FontExtensions
  {
    private static Dictionary<FontExtensions.ToUIFontKey, UIFont> toUiFont = new Dictionary<FontExtensions.ToUIFontKey, UIFont>();

    public static UIFont ToUIFont(this Font self)
    {
      float num = (float) self.FontSize;
      if (self.UseNamedSize)
      {
        switch (self.NamedSize)
        {
          case NamedSize.Micro:
            num = 12f;
            break;
          case NamedSize.Small:
            num = 14f;
            break;
          case NamedSize.Medium:
            num = 17f;
            break;
          case NamedSize.Large:
            num = 22f;
            break;
          default:
            num = 17f;
            break;
        }
      }
      bool flag1 = ((Enum) self.FontAttributes).HasFlag((Enum) FontAttributes.Bold);
      bool flag2 = ((Enum) self.FontAttributes).HasFlag((Enum) FontAttributes.Italic);
      if (self.FontFamily != null)
      {
        try
        {
          if (Enumerable.Contains<string>((IEnumerable<string>) UIFont.FamilyNames, self.FontFamily) && Forms.IsiOS7OrNewer)
          {
            // ISSUE: reference to a compiler-generated method
            UIFontDescriptor withFamily = new UIFontDescriptor().CreateWithFamily(self.FontFamily);
            if (flag1 | flag2)
            {
              UIFontDescriptorSymbolicTraits symbolicTraits = UIFontDescriptorSymbolicTraits.ClassUnknown;
              if (flag1)
                symbolicTraits |= UIFontDescriptorSymbolicTraits.Bold;
              if (flag2)
                symbolicTraits |= UIFontDescriptorSymbolicTraits.Italic;
              // ISSUE: reference to a compiler-generated method
              return UIFont.FromDescriptor(withFamily.CreateWithTraits(symbolicTraits), (nfloat) num);
            }
          }
          return UIFont.FromName(self.FontFamily, (nfloat) num);
        }
        catch
        {
        }
      }
      if (flag1 & flag2)
      {
        if (!Forms.IsiOS7OrNewer)
          return UIFont.BoldSystemFontOfSize((nfloat) num);
        // ISSUE: reference to a compiler-generated method
        return UIFont.FromDescriptor(UIFont.SystemFontOfSize((nfloat) num).FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Italic | UIFontDescriptorSymbolicTraits.Bold), (nfloat) 0);
      }
      if (flag1)
        return UIFont.BoldSystemFontOfSize((nfloat) num);
      if (flag2)
        return UIFont.ItalicSystemFontOfSize((nfloat) num);
      return UIFont.SystemFontOfSize((nfloat) num);
    }

    internal static UIFont ToUIFont(this Label label)
    {
      object[] values = label.GetValues(Label.FontFamilyProperty, Label.FontSizeProperty, Label.FontAttributesProperty);
      return FontExtensions.ToUIFont((string) values[0], (float) (double) values[1], (FontAttributes) values[2]) ?? UIFont.SystemFontOfSize(UIFont.LabelFontSize);
    }

    internal static UIFont ToUIFont(this IFontElement element)
    {
      return FontExtensions.ToUIFont(element.FontFamily, (float) element.FontSize, element.FontAttributes);
    }

    private static UIFont ToUIFont(string family, float size, FontAttributes attributes)
    {
      FontExtensions.ToUIFontKey key = new FontExtensions.ToUIFontKey(family, size, attributes);
      Dictionary<FontExtensions.ToUIFontKey, UIFont> dictionary1 = FontExtensions.toUiFont;
      bool lockTaken1 = false;
      try
      {
        Monitor.Enter((object) dictionary1, ref lockTaken1);
        UIFont uiFont;
        if (FontExtensions.toUiFont.TryGetValue(key, out uiFont))
          return uiFont;
      }
      finally
      {
        if (lockTaken1)
          Monitor.Exit((object) dictionary1);
      }
      UIFont uiFont1 = FontExtensions._ToUIFont(family, size, attributes);
      Dictionary<FontExtensions.ToUIFontKey, UIFont> dictionary2 = FontExtensions.toUiFont;
      bool lockTaken2 = false;
      try
      {
        Monitor.Enter((object) dictionary2, ref lockTaken2);
        UIFont uiFont2;
        if (!FontExtensions.toUiFont.TryGetValue(key, out uiFont2))
          FontExtensions.toUiFont.Add(key, uiFont2 = uiFont1);
        return uiFont2;
      }
      finally
      {
        if (lockTaken2)
          Monitor.Exit((object) dictionary2);
      }
    }

    private static UIFont _ToUIFont(string family, float size, FontAttributes attributes)
    {
      bool flag1 = (uint) (attributes & FontAttributes.Bold) > 0U;
      bool flag2 = (uint) (attributes & FontAttributes.Italic) > 0U;
      if (family != null)
      {
        try
        {
          if (Enumerable.Contains<string>((IEnumerable<string>) UIFont.FamilyNames, family) && Forms.IsiOS7OrNewer)
          {
            // ISSUE: reference to a compiler-generated method
            UIFontDescriptor withFamily = new UIFontDescriptor().CreateWithFamily(family);
            if (flag1 | flag2)
            {
              UIFontDescriptorSymbolicTraits symbolicTraits = UIFontDescriptorSymbolicTraits.ClassUnknown;
              if (flag1)
                symbolicTraits |= UIFontDescriptorSymbolicTraits.Bold;
              if (flag2)
                symbolicTraits |= UIFontDescriptorSymbolicTraits.Italic;
              // ISSUE: reference to a compiler-generated method
              UIFont uiFont = UIFont.FromDescriptor(withFamily.CreateWithTraits(symbolicTraits), (nfloat) size);
              if (uiFont != (UIFont) null)
                return uiFont;
            }
          }
          UIFont uiFont1 = UIFont.FromName(family, (nfloat) size);
          if (uiFont1 != (UIFont) null)
            return uiFont1;
        }
        catch
        {
        }
      }
      if (flag1 & flag2)
      {
        UIFont uiFont = UIFont.SystemFontOfSize((nfloat) size);
        if (!Forms.IsiOS7OrNewer)
          return UIFont.BoldSystemFontOfSize((nfloat) size);
        // ISSUE: reference to a compiler-generated method
        return UIFont.FromDescriptor(uiFont.FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Italic | UIFontDescriptorSymbolicTraits.Bold), (nfloat) 0);
      }
      if (flag1)
        return UIFont.BoldSystemFontOfSize((nfloat) size);
      if (flag2)
        return UIFont.ItalicSystemFontOfSize((nfloat) size);
      return UIFont.SystemFontOfSize((nfloat) size);
    }

    internal static bool IsDefault(this Span self)
    {
      if (self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof (Label), true))
        return self.FontAttributes == FontAttributes.None;
      return false;
    }

    private struct ToUIFontKey
    {
      private string family;
      private float size;
      private FontAttributes attributes;

      internal ToUIFontKey(string family, float size, FontAttributes attributes)
      {
        this.family = family;
        this.size = size;
        this.attributes = attributes;
      }
    }
  }
}
