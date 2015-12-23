using System;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class CellRenderer : IRegisterable
	{
		private static readonly BindableProperty RealCellProperty = 
			BindableProperty.CreateAttached ("RealCell", 
				typeof(NSTableCellView), 
				typeof(Cell), 
				(object)null, 
				BindingMode.OneWay, 
				(BindableProperty.ValidateValueDelegate)null, 
				(BindableProperty.BindingPropertyChangedDelegate)null, 
				(BindableProperty.BindingPropertyChangingDelegate)null, 
				(BindableProperty.CoerceValueDelegate)null, 
				(BindableProperty.CreateDefaultValueDelegate)null);

		public virtual NSTableCellView GetCell (Cell item, NSTableCellView reusableCell, NSTableView tv)
		{
			var cellTableViewCell = reusableCell as CellTableViewCell ?? new CellTableViewCell ();
			
			cellTableViewCell.Cell = item;
			cellTableViewCell.TextField.StringValue = item.ToString();
			UpdateBackground (cellTableViewCell, item);

			return cellTableViewCell;
		}

		protected void UpdateBackground (NSTableCellView tableViewCell, Cell cell)
		{
			if (TemplatedItemsList<ItemsView<Cell>, Cell>.GetIsGroupHeader ((BindableObject)cell))
			{
				var color = NSColor.FromDeviceRgba((nfloat)0.9686275f, (nfloat)0.9686275f, (nfloat)0.9686275f, (nfloat)1);
				tableViewCell.SetBackgroundColor( color);
			}
			else
			{
				NSColor uiColor = NSColor.White;
				VisualElement visualElement = cell.Parent as VisualElement;
				if (visualElement != null)
					uiColor = visualElement.BackgroundColor == Color.Default ? uiColor : ColorExtensions.ToUIColor (visualElement.BackgroundColor);
				tableViewCell.SetBackgroundColor( uiColor );
			}
		}

		internal static NSTableCellView GetRealCell (BindableObject cell)
		{
			return (NSTableCellView)cell.GetValue (CellRenderer.RealCellProperty);
		}

		internal static void SetRealCell (BindableObject cell, NSTableCellView renderer)
		{
			cell.SetValue (CellRenderer.RealCellProperty, (object)renderer);
		}
	}
}
