using Foundation;
using System;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class UnEvenTableViewModelRenderer : TableViewModelRenderer
  {
    public UnEvenTableViewModelRenderer(TableView model)
      : base(model)
    {
    }

    public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
    {
      double height = this.view.Model.GetCell(indexPath.Section, indexPath.Row).Height;
      if (height == -1.0)
        return tableView.RowHeight;
      return (nfloat) height;
    }
  }
}
