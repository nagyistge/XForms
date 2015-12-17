using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class CellTableViewCell : UITableViewCell, INativeElementView
  {
    private Cell cell;
    public Action<object, PropertyChangedEventArgs> PropertyChanged;

    public Cell Cell
    {
      get
      {
        return this.cell;
      }
      set
      {
        if (this.cell == value)
          return;
        if (this.cell != null)
          Device.BeginInvokeOnMainThread(new Action(this.cell.SendDisappearing));
        this.cell = value;
        if (this.cell == null)
          return;
        Device.BeginInvokeOnMainThread(new Action(this.cell.SendAppearing));
      }
    }

    public Element Element
    {
      get
      {
        return (Element) this.Cell;
      }
    }

    public CellTableViewCell(UITableViewCellStyle style, string key)
      : base(style, key)
    {
    }

    public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, e);
    }

    internal static UITableViewCell GetNativeCell(UITableView tableView, Cell cell, bool recycleCells = false)
    {
      string fullName = ((object) cell).GetType().FullName;
      CellRenderer cellRenderer = (CellRenderer) Registrar.Registered.GetHandler(((object) cell).GetType());
      ContextActionsCell contextActionsCell = (ContextActionsCell) null;
      UITableViewCell uiTableViewCell1;
      if (cell.HasContextActions | recycleCells)
      {
        // ISSUE: reference to a compiler-generated method
        contextActionsCell = (ContextActionsCell) tableView.DequeueReusableCell("ContextActionsCell");
        if (contextActionsCell == null)
        {
          contextActionsCell = new ContextActionsCell();
          // ISSUE: reference to a compiler-generated method
          uiTableViewCell1 = tableView.DequeueReusableCell(fullName);
        }
        else
        {
          contextActionsCell.Close();
          uiTableViewCell1 = contextActionsCell.ContentCell;
          if (uiTableViewCell1.ReuseIdentifier.ToString() != fullName)
            uiTableViewCell1 = (UITableViewCell) null;
        }
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        uiTableViewCell1 = tableView.DequeueReusableCell(fullName);
      }
      Cell cell1 = cell;
      UITableViewCell reusableCell = uiTableViewCell1;
      UITableView tv = tableView;
      UITableViewCell nativeCell = cellRenderer.GetCell(cell1, reusableCell, tv);
      UITableViewCell uiTableViewCell2 = nativeCell;
      if (uiTableViewCell2.Layer.Hidden)
        uiTableViewCell2.Layer.Hidden = false;
      if (contextActionsCell != null)
      {
        contextActionsCell.Update(tableView, cell, nativeCell);
        nativeCell = (UITableViewCell) contextActionsCell;
      }
      if (uiTableViewCell2 != null)
      {
        // ISSUE: reference to a compiler-generated method
        uiTableViewCell2.LayoutSubviews();
      }
      return nativeCell;
    }
  }
}
