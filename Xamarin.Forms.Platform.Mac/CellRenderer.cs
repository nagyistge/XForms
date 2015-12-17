using System;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class CellRenderer : IRegisterable
  {
    private static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof (UITableViewCell), typeof (Cell), (object) null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);

    public virtual UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
    {
      CellTableViewCell cellTableViewCell = reusableCell as CellTableViewCell ?? new CellTableViewCell(UITableViewCellStyle.Default, ((object) item).GetType().FullName);
      cellTableViewCell.Cell = item;
      cellTableViewCell.TextLabel.Text = ((object) item).ToString();
      this.UpdateBackground((UITableViewCell) cellTableViewCell, item);
      return (UITableViewCell) cellTableViewCell;
    }

    protected void UpdateBackground(UITableViewCell tableViewCell, Cell cell)
    {
      if (TemplatedItemsList<ItemsView<Cell>, Cell>.GetIsGroupHeader((BindableObject) cell))
      {
        if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
          return;
        tableViewCell.BackgroundColor = new UIColor((nfloat) 0.9686275f, (nfloat) 0.9686275f, (nfloat) 0.9686275f, (nfloat) 1);
      }
      else
      {
        UIColor uiColor = UIColor.White;
        VisualElement visualElement = cell.Parent as VisualElement;
        if (visualElement != null)
          uiColor = visualElement.BackgroundColor == Color.Default ? uiColor : ColorExtensions.ToUIColor(visualElement.BackgroundColor);
        tableViewCell.BackgroundColor = uiColor;
      }
    }

    internal static UITableViewCell GetRealCell(BindableObject cell)
    {
      return (UITableViewCell) cell.GetValue(CellRenderer.RealCellProperty);
    }

    internal static void SetRealCell(BindableObject cell, UITableViewCell renderer)
    {
      cell.SetValue(CellRenderer.RealCellProperty, (object) renderer);
    }
  }
}
