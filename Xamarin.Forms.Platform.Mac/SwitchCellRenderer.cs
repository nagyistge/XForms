using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class SwitchCellRenderer : CellRenderer
	{
		private const string CellName = "Xamarin.SwitchCell";

		public override NSTableCellView GetCell (Cell item, NSTableCellView reusableCell, NSTableView tv)
		{
			return null;

			// TODO: WT.? need to figure this out
			/*
			CellTableViewCell cell = reusableCell as CellTableViewCell;
			NSButton uiSwitch = null;
			if (cell == null)
			{
				cell = new CellTableViewCell ();	// (UITableViewCellStyle.Value1, "Xamarin.SwitchCell");
			}
			else
			{
				uiSwitch = cell.AccessoryView as UISwitch;
				cell.Cell.PropertyChanged -= OnCellPropertyChanged;
			}
			CellRenderer.SetRealCell ((BindableObject)item, (NSTableCellView)cell);
			if (uiSwitch == null)
			{
				uiSwitch = new UISwitch ((CGRect)new RectangleF ());
				uiSwitch.ValueChanged += new EventHandler (this.OnSwitchValueChanged);
				cell.AccessoryView = (NSView)uiSwitch;
			}
			SwitchCell switchCell = (SwitchCell)item;
			cell.Cell = item;
			cell.Cell.add_PropertyChanged (new PropertyChangedEventHandler (this.OnCellPropertyChanged));
			cell.AccessoryView = (NSView)uiSwitch;
			cell.TextLabel.Text = switchCell.Text;
			uiSwitch.On = switchCell.On;
			this.UpdateBackground ((NSTableCellView)cell, item);
			this.UpdateIsEnabled (cell, switchCell);
			return (NSTableCellView)cell;
			*/
		}

		private void OnCellPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			/*
			SwitchCell switchCell = (SwitchCell)sender;
			CellTableViewCell cell = (CellTableViewCell)CellRenderer.GetRealCell ((BindableObject)switchCell);
			if (e.PropertyName == SwitchCell.OnProperty.PropertyName)
			{
				// ISSUE: reference to a compiler-generated method
				((UISwitch)cell.AccessoryView).SetState (switchCell.On, true);
			}
			else if (e.PropertyName == SwitchCell.TextProperty.PropertyName)
			{
				cell.TextLabel.Text = switchCell.Text;
			}
			else
			{
				if (!(e.PropertyName == Cell.IsEnabledProperty.PropertyName))
					return;
				this.UpdateIsEnabled (cell, switchCell);
			}
			*/
		}

		private void UpdateIsEnabled (CellTableViewCell cell, SwitchCell switchCell)
		{
			/*
			cell.UserInteractionEnabled = switchCell.IsEnabled;
			cell.TextLabel.Enabled = switchCell.IsEnabled;
			cell.DetailTextLabel.Enabled = switchCell.IsEnabled;
			UISwitch uiSwitch = cell.AccessoryView as UISwitch;
			if (uiSwitch == null)
				return;
			uiSwitch.Enabled = switchCell.IsEnabled;
			*/
		}

		private void OnSwitchValueChanged (object sender, EventArgs eventArgs)
		{
			/*
			NSView uiView = (NSView)sender;
			UISwitch uiSwitch = (UISwitch)uiView;
			CellTableViewCell cellTableViewCell;
			for (cellTableViewCell = (CellTableViewCell)null; uiView.Superview != null && cellTableViewCell == null; cellTableViewCell = uiView as CellTableViewCell)
				uiView = uiView.Superview;
			if (cellTableViewCell == null)
				return;
			((SwitchCell)cellTableViewCell.Cell).On = uiSwitch.On;
			*/
		}
	}
}
