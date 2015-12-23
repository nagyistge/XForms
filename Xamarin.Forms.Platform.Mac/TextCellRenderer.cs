using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class TextCellRenderer : CellRenderer
	{
		private static readonly Color DefaultDetailColor = new Color (0.32, 0.4, 0.57);
		private static readonly Color DefaultTextColor = Color.Black;

		public override NSTableCellView GetCell (Cell item, NSTableCellView reusableCell, NSTableView tv)
		{
			TextCell entryCell = (TextCell)item;
			CellTableViewCell cell = reusableCell as CellTableViewCell;
			if (cell == null)
				cell = new CellTableViewCell ();	// (UITableViewCellStyle.Subtitle, (item).GetType ().FullName);
			else
				cell.Cell.PropertyChanged -= cell.HandlePropertyChanged;
			
			cell.Cell = (Cell)entryCell;
			entryCell.PropertyChanged += cell.HandlePropertyChanged;

			cell.PropertyChanged = new Action<object, PropertyChangedEventArgs> (this.HandlePropertyChanged);
			cell.TextField.StringValue = entryCell.Text;
			cell.TextField.TextColor = ColorExtensions.ToUIColor (entryCell.TextColor, TextCellRenderer.DefaultTextColor);

			//cell.DetailTextLabel.Text = entryCell.Detail;
			//cell.DetailTextLabel.TextColor = ColorExtensions.ToUIColor (entryCell.DetailColor, TextCellRenderer.DefaultDetailColor);

			TextCellRenderer.UpdateIsEnabled (cell, entryCell);
			this.UpdateBackground ((NSTableCellView)cell, item);
			return (NSTableCellView)cell;
		}

		private static void UpdateIsEnabled (CellTableViewCell cell, TextCell entryCell)
		{
			//cell.UserInteractionEnabled = entryCell.IsEnabled;
			cell.TextField.Enabled = entryCell.IsEnabled;
			//cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
		}

		protected virtual void HandlePropertyChanged (object sender, PropertyChangedEventArgs args)
		{
			CellTableViewCell cell = (CellTableViewCell)sender;
			TextCell entryCell = (TextCell)cell.Cell;
			if (args.PropertyName == TextCell.TextProperty.PropertyName)
			{
				cell.TextField.StringValue = ((TextCell)cell.Cell).Text;

				cell.TextField.SizeToFit ();
			}
			/*
			else if (args.PropertyName == TextCell.DetailProperty.PropertyName)
			{
				cell.DetailTextLabel.Text = ((TextCell)cell.Cell).Detail;
				cell.DetailTextLabel.SizeToFit ();
			}
			*/
			else if (args.PropertyName == TextCell.TextColorProperty.PropertyName)
				cell.TextField.TextColor = ColorExtensions.ToUIColor (entryCell.TextColor, TextCellRenderer.DefaultTextColor);
			/*
			else if (args.PropertyName == TextCell.DetailColorProperty.PropertyName)
			{
				cell.DetailTextLabel.TextColor = ColorExtensions.ToUIColor (entryCell.DetailColor, TextCellRenderer.DefaultTextColor);
			}
			*/
			else
			{
				if (!(args.PropertyName == Cell.IsEnabledProperty.PropertyName))
					return;
				TextCellRenderer.UpdateIsEnabled (cell, entryCell);
			}
		}
	}
}
