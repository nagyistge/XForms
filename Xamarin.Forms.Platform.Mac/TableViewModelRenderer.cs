using Foundation;
using System;
using System.Collections.Generic;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class TableViewModelRenderer : NSTableViewSource
	{
		private Dictionary<nint, Cell> headerCells = new Dictionary<nint, Cell> ();
		protected TableView view;
		protected bool hasBoundGestures;
		protected NSTableView table;

		public bool AutomaticallyDeselect { get; set; }

		public TableViewModelRenderer (TableView model)
		{
			this.view = model;
			this.view.ModelChanged += (s, e) =>
			{
				if (this.table == null)
					return;
				this.table.ReloadData ();
			};
			this.AutomaticallyDeselect = true;
		}

		/*
		public void LongPress (UILongPressGestureRecognizer gesture)
		{
			// ISSUE: reference to a compiler-generated method
			// ISSUE: reference to a compiler-generated method
			NSIndexPath nsIndexPath = this.table.IndexPathForRowAtPoint (gesture.LocationInView ((NSView)this.table));
			if (nsIndexPath == null)
				return;
			this.view.Model.RowLongPressed (nsIndexPath.Section, nsIndexPath.Row);
		}
		*/

		public override nint GetRowCount (NSTableView tableView)
		{
			return (nint)this.view.Model.GetRowCount (0);
		}

		public override NSCell GetCell (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			Cell cell = this.view.Model.GetCell (0, (int)row);
			var nativeCell =  CellTableViewCell.GetNativeCell (tableView, cell, false);

			// TODO: Figure out Cells
			return null;
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			NSTableView tableView = notification.Object as NSTableView;

			this.view.Model.RowSelected (0, (int)tableView.SelectedRow);
			if (!this.AutomaticallyDeselect)
				return;

			tableView.DeselectRow (tableView.SelectedRow);
		}

		/*
		public override string[] SectionIndexTitles (NSTableView tableView)
		{
			return this.view.Model.GetSectionIndexTitles ();
		}

		public override string TitleForHeader (NSTableView tableView, nint section)
		{
			return this.view.Model.GetSectionTitle ((int)section);
		}

		public override NSView GetViewForHeader (NSTableView tableView, nint section)
		{
			if (!this.headerCells.ContainsKey ((nint)(int)section))
				this.headerCells [section] = this.view.Model.GetHeaderCell ((int)section);
			Cell cell = this.headerCells [section];
			if (cell == null)
				return (NSView)null;
			// ISSUE: reference to a compiler-generated method
			NSTableCellView reusableCell = tableView.DequeueReusableCell ((cell).GetType ().FullName);
			return (NSView)((CellRenderer)Registrar.Registered.GetHandler<CellRenderer> ((cell).GetType ())).GetCell (cell, reusableCell, this.table);
		}

		public override nfloat GetHeightForHeader (NSTableView tableView, nint section)
		{
			if (!this.headerCells.ContainsKey ((nint)(int)section))
				this.headerCells [section] = this.view.Model.GetHeaderCell ((int)section);
			Cell cell = this.headerCells [section];
			if (cell != null)
				return (nfloat)cell.Height;
			return NSTableView.AutomaticDimension;
		}

		private void Tap (UITapGestureRecognizer gesture)
		{
			// ISSUE: reference to a compiler-generated method
			UIView_UITextField.EndEditing (gesture.View, true);
		}

		private void BindGestures (NSTableView tableview)
		{
			if (this.hasBoundGestures)
				return;
			this.hasBoundGestures = true;
			// ISSUE: reference to a compiler-generated method
			tableview.AddGestureRecognizer ((UIGestureRecognizer)new UILongPressGestureRecognizer (new Action<UILongPressGestureRecognizer> (this.LongPress)) {
				MinimumPressDuration = 2.0
			});
			UITapGestureRecognizer gestureRecognizer = new UITapGestureRecognizer (new Action<UITapGestureRecognizer> (this.Tap));
			gestureRecognizer.CancelsTouchesInView = false;
			// ISSUE: reference to a compiler-generated method
			tableview.AddGestureRecognizer ((UIGestureRecognizer)gestureRecognizer);
			this.table = tableview;
		}
		*/
	}
}
