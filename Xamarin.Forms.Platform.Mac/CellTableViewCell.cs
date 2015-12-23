using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	/*
	public enum UITableViewCellStyle : long
	{
		Default,
		Value1,
		Value2,
		Subtitle,
	}
	*/

	public class CellTableViewCell : NSTableCellView, INativeElementView
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
				
				// TODO: Call internal action
				if (this.cell != null)
					Device.BeginInvokeOnMainThread (new Action (this.cell.SendDisappearing));

				this.cell = value;
				if (this.cell == null)
					return;

				// TODO: Call internal Action
				Device.BeginInvokeOnMainThread (new Action (this.cell.SendAppearing));
			}
		}

		public Element Element
		{
			get	{ return Cell; }
		}

		public void HandlePropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (this.PropertyChanged == null)
				return;
			this.PropertyChanged ((object)this, e);
		}

		internal static NSTableCellView GetNativeCell (NSTableView tableView, Cell cell, bool recycleCells = false)
		{
			string fullName = ((object)cell).GetType ().FullName;

			CellRenderer cellRenderer = (CellRenderer)Registrar.Registered.GetHandler (cell.GetType ());

			ContextActionsCell contextActionsCell = (ContextActionsCell)null;
			NSTableCellView reusableCell = null;
			if (cell.HasContextActions | recycleCells)
			{
				// Not for Mac
				//contextActionsCell = (ContextActionsCell)tableView.DequeueReusableCell ("ContextActionsCell");
				if (contextActionsCell == null)
				{
					contextActionsCell = new ContextActionsCell ();
					reusableCell = new NSTableCellView ();

					//reusableCell = tableView.DequeueReusableCell (fullName);
				}
				/*
				else
				{
					contextActionsCell.Close ();
					reusableCell = contextActionsCell.ContentCell;
					if (reusableCell.ReuseIdentifier.ToString () != fullName)
						reusableCell = null;
				}
				*/
			}
			else
			{
				//reusableCell = tableView.DequeueReusableCell (fullName);
				reusableCell = new NSTableCellView ();
			}

			NSTableView tv = tableView;
			NSTableCellView nativeCell = cellRenderer.GetCell (cell, reusableCell, tv);


			if (nativeCell.Layer.Hidden)
				nativeCell.Layer.Hidden = false;
			if (contextActionsCell != null)
			{
				contextActionsCell.Update (tableView, cell, nativeCell);
				nativeCell = (NSTableCellView)contextActionsCell;
			}
			if (nativeCell != null)
				nativeCell.Layout ();
			
			return nativeCell;
		}
	}
}
