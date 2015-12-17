using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class EntryCellRenderer : CellRenderer
  {
    private static readonly Color DefaultTextColor = Color.Black;

    public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
    {
      EntryCell entryCell = (EntryCell) item;
      EntryCellRenderer.EntryCellTableViewCell cell = reusableCell as EntryCellRenderer.EntryCellTableViewCell;
      if (cell == null)
      {
        cell = new EntryCellRenderer.EntryCellTableViewCell(((object) item).GetType().FullName);
      }
      else
      {
        cell.Cell.remove_PropertyChanged(new PropertyChangedEventHandler(EntryCellRenderer.OnCellPropertyChanged));
        cell.TextFieldTextChanged -= new EventHandler(EntryCellRenderer.OnTextFieldTextChanged);
        cell.KeyboardDoneButtonPressed -= new EventHandler(EntryCellRenderer.OnKeyBoardDoneButtonPressed);
      }
      CellRenderer.SetRealCell((BindableObject) item, (UITableViewCell) cell);
      cell.Cell = item;
      cell.Cell.add_PropertyChanged(new PropertyChangedEventHandler(EntryCellRenderer.OnCellPropertyChanged));
      cell.TextFieldTextChanged += new EventHandler(EntryCellRenderer.OnTextFieldTextChanged);
      cell.KeyboardDoneButtonPressed += new EventHandler(EntryCellRenderer.OnKeyBoardDoneButtonPressed);
      this.UpdateBackground((UITableViewCell) cell, (Cell) entryCell);
      EntryCellRenderer.UpdateLabel(cell, entryCell);
      EntryCellRenderer.UpdateText(cell, entryCell);
      EntryCellRenderer.UpdateKeyboard(cell, entryCell);
      EntryCellRenderer.UpdatePlaceholder(cell, entryCell);
      EntryCellRenderer.UpdateLabelColor(cell, entryCell);
      EntryCellRenderer.UpdateHorizontalTextAlignment(cell, entryCell);
      EntryCellRenderer.UpdateIsEnabled(cell, entryCell);
      return (UITableViewCell) cell;
    }

    private static void OnKeyBoardDoneButtonPressed(object sender, EventArgs e)
    {
      ((EntryCell) ((CellTableViewCell) sender).Cell).SendCompleted();
    }

    private static void OnTextFieldTextChanged(object sender, EventArgs eventArgs)
    {
      EntryCellRenderer.EntryCellTableViewCell cellTableViewCell = (EntryCellRenderer.EntryCellTableViewCell) sender;
      ((EntryCell) cellTableViewCell.Cell).Text = cellTableViewCell.TextField.Text;
    }

    private static void UpdateHorizontalTextAlignment(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      cell.TextField.TextAlignment = AlignmentExtensions.ToNativeTextAlignment(entryCell.HorizontalTextAlignment);
    }

    private static void UpdateLabel(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      cell.TextLabel.Text = entryCell.Label;
    }

    private static void UpdateText(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      if (cell.TextField.Text == entryCell.Text)
        return;
      cell.TextField.Text = entryCell.Text;
    }

    private static void UpdateKeyboard(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      Extensions.ApplyKeyboard((IUITextInput) cell.TextField, entryCell.Keyboard);
    }

    private static void UpdateLabelColor(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      cell.TextLabel.TextColor = ColorExtensions.ToUIColor(entryCell.LabelColor, EntryCellRenderer.DefaultTextColor);
    }

    private static void UpdatePlaceholder(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      cell.TextField.Placeholder = entryCell.Placeholder;
    }

    private static void UpdateIsEnabled(EntryCellRenderer.EntryCellTableViewCell cell, EntryCell entryCell)
    {
      cell.UserInteractionEnabled = entryCell.IsEnabled;
      cell.TextLabel.Enabled = entryCell.IsEnabled;
      cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
      cell.TextField.Enabled = entryCell.IsEnabled;
    }

    private static void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      EntryCell entryCell = (EntryCell) sender;
      EntryCellRenderer.EntryCellTableViewCell cell = (EntryCellRenderer.EntryCellTableViewCell) CellRenderer.GetRealCell((BindableObject) entryCell);
      if (e.PropertyName == EntryCell.LabelProperty.PropertyName)
        EntryCellRenderer.UpdateLabel(cell, entryCell);
      else if (e.PropertyName == EntryCell.TextProperty.PropertyName)
        EntryCellRenderer.UpdateText(cell, entryCell);
      else if (e.PropertyName == EntryCell.PlaceholderProperty.PropertyName)
        EntryCellRenderer.UpdatePlaceholder(cell, entryCell);
      else if (e.PropertyName == EntryCell.KeyboardProperty.PropertyName)
        EntryCellRenderer.UpdateKeyboard(cell, entryCell);
      else if (e.PropertyName == EntryCell.LabelColorProperty.PropertyName)
        EntryCellRenderer.UpdateLabelColor(cell, entryCell);
      else if (e.PropertyName == EntryCell.HorizontalTextAlignmentProperty.PropertyName)
      {
        EntryCellRenderer.UpdateHorizontalTextAlignment(cell, entryCell);
      }
      else
      {
        if (!(e.PropertyName == Cell.IsEnabledProperty.PropertyName))
          return;
        EntryCellRenderer.UpdateIsEnabled(cell, entryCell);
      }
    }

    private class EntryCellTableViewCell : CellTableViewCell
    {
      public UITextField TextField { get; private set; }

      public event EventHandler KeyboardDoneButtonPressed;

      public event EventHandler TextFieldTextChanged;

      public EntryCellTableViewCell(string cellName)
        : base(UITableViewCellStyle.Value1, cellName)
      {
        this.TextField = new UITextField(new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 100, (nfloat) 30))
        {
          BorderStyle = UITextBorderStyle.None
        };
        this.TextField.EditingChanged += new EventHandler(this.TextFieldOnEditingChanged);
        this.TextField.ShouldReturn = new UITextFieldCondition(this.OnShouldReturn);
        // ISSUE: reference to a compiler-generated method
        this.ContentView.AddSubview((UIView) this.TextField);
      }

      private void TextFieldOnEditingChanged(object sender, EventArgs eventArgs)
      {
        // ISSUE: reference to a compiler-generated field
        EventHandler eventHandler = this.TextFieldTextChanged;
        if (eventHandler == null)
          return;
        eventHandler((object) this, EventArgs.Empty);
      }

      public override void LayoutSubviews()
      {
        // ISSUE: reference to a compiler-generated method
        base.LayoutSubviews();
        CGRect frame1 = this.Frame;
        double val1 = (double) frame1.Width * 0.3;
        frame1 = this.TextLabel.Frame;
        double val2 = (double) (frame1.Right + (nfloat) 10);
        nfloat nfloat = (nfloat) Math.Round(Math.Max(val1, val2));
        UITextField textField = this.TextField;
        nfloat x = nfloat;
        CGRect frame2 = this.Frame;
        nfloat y = (frame2.Height - (nfloat) 30) / (nfloat) 2;
        frame2 = this.Frame;
        nfloat width1 = frame2.Width;
        frame2 = this.TextLabel.Frame;
        nfloat left = frame2.Left;
        nfloat width2 = width1 - left - nfloat;
        nfloat height = (nfloat) 30;
        CGRect cgRect = new CGRect(x, y, width2, height);
        textField.Frame = cgRect;
        this.TextField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
      }

      private bool OnShouldReturn(UITextField view)
      {
        // ISSUE: reference to a compiler-generated field
        EventHandler eventHandler = this.KeyboardDoneButtonPressed;
        if (eventHandler != null)
          eventHandler((object) this, EventArgs.Empty);
        // ISSUE: reference to a compiler-generated method
        this.TextField.ResignFirstResponder();
        return true;
      }
    }
  }
}
