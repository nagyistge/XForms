using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class DatePickerRenderer : ViewRenderer<DatePicker, UITextField>
  {
    private UIDatePicker picker;

    protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
    {
      this.OnElementChanged(e);
      if (e.OldElement == null)
      {
        NoCaretField noCaretField = new NoCaretField();
        long num1 = 3;
        noCaretField.BorderStyle = (UITextBorderStyle) num1;
        NoCaretField entry = noCaretField;
        entry.Started += new EventHandler(this.OnStarted);
        entry.Ended += new EventHandler(this.OnEnded);
        this.picker = new UIDatePicker()
        {
          Mode = UIDatePickerMode.Date,
          TimeZone = new NSTimeZone("UTC")
        };
        this.picker.ValueChanged += new EventHandler(this.HandleValueChanged);
        UIToolbar uiToolbar1 = new UIToolbar(new CGRect((nfloat) 0, (nfloat) 0, UIScreen.MainScreen.Bounds.Width, (nfloat) 44));
        uiToolbar1.BarStyle = UIBarStyle.Default;
        int num2 = 1;
        uiToolbar1.Translucent = num2 != 0;
        UIToolbar uiToolbar2 = uiToolbar1;
        UIBarButtonItem uiBarButtonItem1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        // ISSUE: reference to a compiler-generated method
        UIBarButtonItem uiBarButtonItem2 = new UIBarButtonItem(UIBarButtonSystemItem.Done, (EventHandler) ((o, a) => entry.ResignFirstResponder()));
        uiToolbar2.SetItems(new UIBarButtonItem[2]
        {
          uiBarButtonItem1,
          uiBarButtonItem2
        }, 0 != 0);
        entry.InputView = (UIView) this.picker;
        entry.InputAccessoryView = (UIView) uiToolbar2;
        this.SetNativeControl((UITextField) entry);
      }
      if (e.NewElement == null)
        return;
      this.UpdateDateFromModel(false);
      this.UpdateMaximumDate();
      this.UpdateMinimumDate();
    }

    private void OnEnded(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) false);
    }

    private void OnStarted(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) true);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
        this.UpdateDateFromModel(true);
      else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
      {
        this.UpdateMinimumDate();
      }
      else
      {
        if (!(e.PropertyName == DatePicker.MaximumDateProperty.PropertyName))
          return;
        this.UpdateMaximumDate();
      }
    }

    private void UpdateMaximumDate()
    {
      this.picker.MaximumDate = DateExtensions.ToNSDate(this.Element.get_MaximumDate());
    }

    private void UpdateMinimumDate()
    {
      this.picker.MinimumDate = DateExtensions.ToNSDate(this.Element.get_MinimumDate());
    }

    private void UpdateDateFromModel(bool animate)
    {
      DateTime dateTime = DateExtensions.ToDateTime(this.picker.Date);
      DateTime date1 = dateTime.Date;
      dateTime = this.Element.get_Date();
      DateTime date2 = dateTime.Date;
      if (date1 != date2)
      {
        // ISSUE: reference to a compiler-generated method
        this.picker.SetDate(DateExtensions.ToNSDate(this.Element.get_Date()), animate);
      }
      this.Control.Text = this.Element.get_Date().ToString(this.Element.Format);
    }

    private void HandleValueChanged(object sender, EventArgs e)
    {
      if (this.Element == null)
        return;
      this.Element.SetValueFromRenderer(DatePicker.DateProperty, (object) DateExtensions.ToDateTime(this.picker.Date).Date);
    }
  }
}
