using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class ViewExtensions
  {
    public static IEnumerable<Page> GetParentPages(this Page target)
    {
      List<Page> list = new List<Page>();
      for (Page page = target.Parent as Page; !Application.IsApplicationOrNull((Element) page); page = page.Parent as Page)
        list.Add(page);
      return (IEnumerable<Page>) list;
    }
  }
}
