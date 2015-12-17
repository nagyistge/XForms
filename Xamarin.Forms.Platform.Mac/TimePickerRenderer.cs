using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class TimePickerRenderer : ViewRenderer<TimePicker, UITextField>
  {
    private UIDatePicker picker;

    protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
    {
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          NoCaretField noCaretField = new NoCaretField();
          long num1 = 3;
          noCaretField.BorderStyle = (UITextBorderStyle) num1;
          NoCaretField entry = noCaretField;
          entry.Started += new EventHandler(this.OnStarted);
          entry.Ended += new EventHandler(this.OnEnded);
          this.picker = new UIDatePicker()
          {
            Mode = UIDatePickerMode.Time,
            TimeZone = new NSTimeZone("UTC")
          };
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
          this.picker.ValueChanged += new EventHandler(this.OnValueChanged);
          this.SetNativeControl((UITextField) entry);
        }
        this.UpdateTime();
      }
      this.OnElementChanged(e);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Control.Started -= new EventHandler(this.OnStarted);
        this.Control.Ended -= new EventHandler(this.OnEnded);
        this.picker.ValueChanged -= new EventHandler(this.OnValueChanged);
      }
      base.Dispose(disposing);
    }

    private void OnEnded(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) false);
    }

    private void OnStarted(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) true);
    }

    private void OnValueChanged(object sender, EventArgs e)
    {
      this.Element.SetValueFromRenderer(TimePicker.TimeProperty, (object) (DateExtensions.ToDateTime(this.picker.Date) - new DateTime(1, 1, 1)));
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (!(e.PropertyName == TimePicker.TimeProperty.PropertyName) && !(e.PropertyName == TimePicker.FormatProperty.PropertyName))
        return;
      this.UpdateTime();
    }

    private void UpdateTime()
    {
      this.picker.Date = DateExtensions.ToNSDate(new DateTime(1, 1, 1).Add(this.Element.get_Time()));
      UITextField control = this.Control;
      DateTime dateTime = DateTime.Today;
      dateTime = dateTime.Add(this.Element.get_Time());
      string str = dateTime.ToString(this.Element.Format);
      control.Text = str;
    }
  }
}
