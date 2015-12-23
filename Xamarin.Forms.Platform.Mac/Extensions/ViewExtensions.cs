using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public static class ViewExtensions
	{
		public static IEnumerable<Page> GetParentPages (this Page target)
		{
			var list = new List<Page> ();
			for (Page page = target.Parent as Page; !IsApplicationOrNull ((Element)page); page = page.Parent as Page)
				list.Add (page);
			return list;
		}

		public static bool IsApplicationOrNull(this Element element)
		{
			if (element == null)
				return true;
			return element is Application;
		}

	}


}
