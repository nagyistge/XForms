using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class TextCellRenderer : CellRenderer
  {
    private static readonly Color DefaultDetailColor = new Color(0.32, 0.4, 0.57);
    private static readonly Color DefaultTextColor = Color.Black;

    public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
    {
      TextCell entryCell = (TextCell) item;
      CellTableViewCell cell = reusableCell as CellTableViewCell;
      if (cell == null)
        cell = new CellTableViewCell(UITableViewCellStyle.Subtitle, ((object) item).GetType().FullName);
      else
        cell.Cell.remove_PropertyChanged(new PropertyChangedEventHandler(cell.HandlePropertyChanged));
      cell.Cell = (Cell) entryCell;
      entryCell.add_PropertyChanged(new PropertyChangedEventHandler(cell.HandlePropertyChanged));
      cell.PropertyChanged = new Action<object, PropertyChangedEventArgs>(this.HandlePropertyChanged);
      cell.TextLabel.Text = entryCell.Text;
      cell.DetailTextLabel.Text = entryCell.Detail;
      cell.TextLabel.TextColor = ColorExtensions.ToUIColor(entryCell.TextColor, TextCellRenderer.DefaultTextColor);
      cell.DetailTextLabel.TextColor = ColorExtensions.ToUIColor(entryCell.DetailColor, TextCellRenderer.DefaultDetailColor);
      TextCellRenderer.UpdateIsEnabled(cell, entryCell);
      this.UpdateBackground((UITableViewCell) cell, item);
      return (UITableViewCell) cell;
    }

    private static void UpdateIsEnabled(CellTableViewCell cell, TextCell entryCell)
    {
      cell.UserInteractionEnabled = entryCell.IsEnabled;
      cell.TextLabel.Enabled = entryCell.IsEnabled;
      cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
    }

    protected virtual void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      CellTableViewCell cell = (CellTableViewCell) sender;
      TextCell entryCell = (TextCell) cell.Cell;
      if (args.PropertyName == TextCell.TextProperty.PropertyName)
      {
        cell.TextLabel.Text = ((TextCell) cell.Cell).Text;
        // ISSUE: reference to a compiler-generated method
        cell.TextLabel.SizeToFit();
      }
      else if (args.PropertyName == TextCell.DetailProperty.PropertyName)
      {
        cell.DetailTextLabel.Text = ((TextCell) cell.Cell).Detail;
        // ISSUE: reference to a compiler-generated method
        cell.DetailTextLabel.SizeToFit();
      }
      else if (args.PropertyName == TextCell.TextColorProperty.PropertyName)
        cell.TextLabel.TextColor = ColorExtensions.ToUIColor(entryCell.TextColor, TextCellRenderer.DefaultTextColor);
      else if (args.PropertyName == TextCell.DetailColorProperty.PropertyName)
      {
        cell.DetailTextLabel.TextColor = ColorExtensions.ToUIColor(entryCell.DetailColor, TextCellRenderer.DefaultTextColor);
      }
      else
      {
        if (!(args.PropertyName == Cell.IsEnabledProperty.PropertyName))
          return;
        TextCellRenderer.UpdateIsEnabled(cell, entryCell);
      }
    }
  }
}
