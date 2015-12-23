using Foundation;
using System;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class UnEvenTableViewModelRenderer : TableViewModelRenderer
	{
		public UnEvenTableViewModelRenderer (TableView model)
			: base (model)
		{
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			double height = this.view.Model.GetCell (0, (int)row).Height;
			if (height == -1.0)
				return tableView.RowHeight;
			return (nfloat)height;
		}
	}
}
