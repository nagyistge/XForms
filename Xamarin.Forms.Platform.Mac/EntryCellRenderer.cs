using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class EntryCellRenderer : CellRenderer
	{
		private static readonly Color DefaultTextColor = Color.Black;

		public override NSTableCellView GetCell (Cell item, NSTableCellView reusableCell, NSTableView tv)
		{
			EntryCell entryCell = (EntryCell)item;
			EntryCellRenderer.EntryCellTableViewCell cell = reusableCell as EntryCellRenderer.EntryCellTableViewCell;
			if (cell == null)
			{
				cell = new EntryCellRenderer.EntryCellTableViewCell ((item).GetType ().FullName);
			}
			else
			{
				cell.Cell.PropertyChanged -= OnCellPropertyChanged;
				cell.TextFieldTextChanged -= new EventHandler (EntryCellRenderer.OnTextFieldTextChanged);
				cell.KeyboardDoneButtonPressed -= new EventHandler (EntryCellRenderer.OnKeyBoardDoneButtonPressed);
			}
			CellRenderer.SetRealCell ((BindableObject)item, (NSTableCellView)cell);
			cell.Cell = item;
			cell.Cell.PropertyChanged += OnCellPropertyChanged;
			cell.TextFieldTextChanged += new EventHandler (EntryCellRenderer.OnTextFieldTextChanged);
			cell.KeyboardDoneButtonPressed += new EventHandler (EntryCellRenderer.OnKeyBoardDoneButtonPressed);
			this.UpdateBackground ((NSTableCellView)cell, (Cell)entryCell);
			EntryCellRenderer.UpdateLabel (cell, entryCell);
			EntryCellRenderer.UpdateText (cell, entryCell);
			//EntryCellRenderer.UpdateKeyboard (cell, entryCell);
			EntryCellRenderer.UpdatePlaceholder (cell, entryCell);
			EntryCellRenderer.UpdateLabelColor (cell, entryCell);
			EntryCellRenderer.UpdateHorizontalTextAlignment (cell, entryCell);
			EntryCellRenderer.UpdateIsEnabled (cell, entryCell);
			return (NSTableCellView)cell;
		}

		private static void OnKeyBoardDoneButtonPressed (object sender, EventArgs e)
		{
			((EntryCell)((CellTableViewCell)sender).Cell).SendCompleted ();
		}

		private static void OnTextFieldTextChanged (object sender, EventArgs eventArgs)
		{
			EntryCellRenderer.EntryCellTableViewCell cellTableViewCell = (EntryCellRenderer.EntryCellTableViewCell)sender;
			((EntryCell)cellTableViewCell.Cell).Text = cellTableViewCell.TextField.StringValue;
		}

		private static void UpdateHorizontalTextAlignment (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextField.Alignment = AlignmentExtensions.ToNativeTextAlignment (entryCell.HorizontalTextAlignment);
		}

		private static void UpdateLabel (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextField.StringValue = entryCell.Label;
		}

		private static void UpdateText (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			if (cell.TextField.StringValue == entryCell.Text)
				return;
			cell.TextField.StringValue = entryCell.Text;
		}

		// Not for Mac
		/*
		private static void UpdateKeyboard (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			Extensions.ApplyKeyboard ((IUITextInput)cell.TextField, entryCell.Keyboard);
		}
		*/

		private static void UpdateLabelColor (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextField.TextColor = ColorExtensions.ToUIColor (entryCell.LabelColor, EntryCellRenderer.DefaultTextColor);
		}

		private static void UpdatePlaceholder (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			cell.TextField.PlaceholderString = entryCell.Placeholder;
		}

		private static void UpdateIsEnabled (EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
		{
			//cell.UserInteractionEnabled = entryCell.IsEnabled;
			//cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
			cell.TextField.Enabled = entryCell.IsEnabled;
		}

		private static void OnCellPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			EntryCell entryCell = (EntryCell)sender;
			EntryCellRenderer.EntryCellTableViewCell cell = (EntryCellRenderer.EntryCellTableViewCell)CellRenderer.GetRealCell ((BindableObject)entryCell);
			if (e.PropertyName == EntryCell.LabelProperty.PropertyName)
				EntryCellRenderer.UpdateLabel (cell, entryCell);
			else if (e.PropertyName == EntryCell.TextProperty.PropertyName)
				EntryCellRenderer.UpdateText (cell, entryCell);
			else if (e.PropertyName == EntryCell.PlaceholderProperty.PropertyName)
				EntryCellRenderer.UpdatePlaceholder (cell, entryCell);
			//else if (e.PropertyName == EntryCell.KeyboardProperty.PropertyName)
			//	EntryCellRenderer.UpdateKeyboard (cell, entryCell);
			else if (e.PropertyName == EntryCell.LabelColorProperty.PropertyName)
				EntryCellRenderer.UpdateLabelColor (cell, entryCell);
			else if (e.PropertyName == EntryCell.HorizontalTextAlignmentProperty.PropertyName)
			{
				EntryCellRenderer.UpdateHorizontalTextAlignment (cell, entryCell);
			}
			else
			{
				if (!(e.PropertyName == Cell.IsEnabledProperty.PropertyName))
					return;
				EntryCellRenderer.UpdateIsEnabled (cell, entryCell);
			}
		}

		private class EntryCellTableViewCell : CellTableViewCell
		{
			public event EventHandler KeyboardDoneButtonPressed;

			public event EventHandler TextFieldTextChanged;

			public EntryCellTableViewCell (string cellName)
			{
				this.TextField = new NSTextField (new CGRect ((nfloat)0, (nfloat)0, (nfloat)100, (nfloat)30));
					
				this.TextField.Changed += TextFieldOnEditingChanged;
				//this.TextField.ShouldReturn = new UITextFieldCondition (this.OnShouldReturn);
				this.AddSubview (TextField);
			}

			private void TextFieldOnEditingChanged (object sender, EventArgs eventArgs)
			{
				EventHandler eventHandler = this.TextFieldTextChanged;
				if (eventHandler == null)
					return;
				eventHandler (this, EventArgs.Empty);
			}

			public override void Layout ()
			{
				base.Layout ();

				CGRect frame1 = this.Frame;
				double val1 = (double)frame1.Width * 0.3;
				frame1 = this.TextField.Frame;
				double val2 = (double)(frame1.Right + (nfloat)10);
				nfloat nfloat = (nfloat)Math.Round (Math.Max (val1, val2));
				var textField = this.TextField;
				nfloat x = nfloat;
				CGRect frame2 = this.Frame;
				nfloat y = (frame2.Height - (nfloat)30) / (nfloat)2;
				frame2 = this.Frame;
				nfloat width1 = frame2.Width;
				frame2 = this.TextField.Frame;
				nfloat left = frame2.Left;
				nfloat width2 = width1 - left - nfloat;
				nfloat height = (nfloat)30;
				CGRect cgRect = new CGRect (x, y, width2, height);
				textField.Frame = cgRect;

				//this.TextField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			}

			private bool OnShouldReturn (NSTextField view)
			{
				EventHandler eventHandler = this.KeyboardDoneButtonPressed;
				if (eventHandler != null)
					eventHandler (this, EventArgs.Empty);

				this.TextField.ResignFirstResponder ();
				return true;
			}
		}
	}
}
