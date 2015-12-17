using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class TableViewModelRenderer : UITableViewSource
  {
    private Dictionary<nint, Cell> headerCells = new Dictionary<nint, Cell>();
    protected TableView view;
    protected bool hasBoundGestures;
    protected UITableView table;

    public bool AutomaticallyDeselect { get; set; }

    public TableViewModelRenderer(TableView model)
    {
      this.view = model;
      this.view.add_ModelChanged((EventHandler) ((s, e) =>
      {
        if (this.table == null)
          return;
        // ISSUE: reference to a compiler-generated method
        this.table.ReloadData();
      }));
      this.AutomaticallyDeselect = true;
    }

    public void LongPress(UILongPressGestureRecognizer gesture)
    {
      // ISSUE: reference to a compiler-generated method
      // ISSUE: reference to a compiler-generated method
      NSIndexPath nsIndexPath = this.table.IndexPathForRowAtPoint(gesture.LocationInView((UIView) this.table));
      if (nsIndexPath == null)
        return;
      this.view.Model.RowLongPressed(nsIndexPath.Section, nsIndexPath.Row);
    }

    public override nint RowsInSection(UITableView tableview, nint section)
    {
      return (nint) this.view.Model.GetRowCount((int) section);
    }

    public override nint NumberOfSections(UITableView tableView)
    {
      this.BindGestures(tableView);
      return (nint) this.view.Model.GetSectionCount();
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
      Cell cell = this.view.Model.GetCell(indexPath.Section, indexPath.Row);
      return CellTableViewCell.GetNativeCell(tableView, cell, false);
    }

    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
      this.view.Model.RowSelected(indexPath.Section, indexPath.Row);
      if (!this.AutomaticallyDeselect)
        return;
      // ISSUE: reference to a compiler-generated method
      tableView.DeselectRow(indexPath, true);
    }

    public override string[] SectionIndexTitles(UITableView tableView)
    {
      return this.view.Model.GetSectionIndexTitles();
    }

    public override string TitleForHeader(UITableView tableView, nint section)
    {
      return this.view.Model.GetSectionTitle((int) section);
    }

    public override UIView GetViewForHeader(UITableView tableView, nint section)
    {
      if (!this.headerCells.ContainsKey((nint) (int) section))
        this.headerCells[section] = this.view.Model.GetHeaderCell((int) section);
      Cell cell = this.headerCells[section];
      if (cell == null)
        return (UIView) null;
      // ISSUE: reference to a compiler-generated method
      UITableViewCell reusableCell = tableView.DequeueReusableCell(((object) cell).GetType().FullName);
      return (UIView) ((CellRenderer) Registrar.Registered.GetHandler<CellRenderer>(((object) cell).GetType())).GetCell(cell, reusableCell, this.table);
    }

    public override nfloat GetHeightForHeader(UITableView tableView, nint section)
    {
      if (!this.headerCells.ContainsKey((nint) (int) section))
        this.headerCells[section] = this.view.Model.GetHeaderCell((int) section);
      Cell cell = this.headerCells[section];
      if (cell != null)
        return (nfloat) cell.Height;
      return UITableView.AutomaticDimension;
    }

    private void Tap(UITapGestureRecognizer gesture)
    {
      // ISSUE: reference to a compiler-generated method
      UIView_UITextField.EndEditing(gesture.View, true);
    }

    private void BindGestures(UITableView tableview)
    {
      if (this.hasBoundGestures)
        return;
      this.hasBoundGestures = true;
      // ISSUE: reference to a compiler-generated method
      tableview.AddGestureRecognizer((UIGestureRecognizer) new UILongPressGestureRecognizer(new Action<UILongPressGestureRecognizer>(this.LongPress))
      {
        MinimumPressDuration = 2.0
      });
      UITapGestureRecognizer gestureRecognizer = new UITapGestureRecognizer(new Action<UITapGestureRecognizer>(this.Tap));
      gestureRecognizer.CancelsTouchesInView = false;
      // ISSUE: reference to a compiler-generated method
      tableview.AddGestureRecognizer((UIGestureRecognizer) gestureRecognizer);
      this.table = tableview;
    }
  }
}
