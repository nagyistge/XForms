using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, NSTextField>
	{
		private NSDatePicker picker;



		protected override void OnElementChanged (ElementChangedEventArgs<TimePicker> e)
		{
			if (e.NewElement != null)
			{
				if (this.Control == null)
				{
					// TODO: Figure this out
					/*
					NoCaretField noCaretField = new NoCaretField ();
					long num1 = 3;
					noCaretField.BorderStyle = (UITextBorderStyle)num1;
					NoCaretField entry = noCaretField;
					entry.Started += new EventHandler (this.OnStarted);
					entry.Ended += new EventHandler (this.OnEnded);
					this.picker = new UIDatePicker () {
						Mode = UIDatePickerMode.Time,
						TimeZone = new NSTimeZone ("UTC")
					};
					UIToolbar uiToolbar1 = new UIToolbar (new CGRect ((nfloat)0, (nfloat)0, UIScreen.MainScreen.Bounds.Width, (nfloat)44));
					uiToolbar1.BarStyle = UIBarStyle.Default;
					int num2 = 1;
					uiToolbar1.Translucent = num2 != 0;
					UIToolbar uiToolbar2 = uiToolbar1;
					UIBarButtonItem uiBarButtonItem1 = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
					// ISSUE: reference to a compiler-generated method
					UIBarButtonItem uiBarButtonItem2 = new UIBarButtonItem (UIBarButtonSystemItem.Done, (EventHandler)((o, a) => entry.ResignFirstResponder ()));
					uiToolbar2.SetItems (new UIBarButtonItem[2] {
						uiBarButtonItem1,
						uiBarButtonItem2
					}, 0 != 0);
					entry.InputView = (NSView)this.picker;
					entry.InputAccessoryView = (NSView)uiToolbar2;
					this.picker.ValueChanged += new EventHandler (this.OnValueChanged);
					this.SetNativeControl ((UITextField)entry);
					*/
				}
				this.UpdateTime ();
			}
			this.OnElementChanged (e);
		}

		/*
		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				this.Control.Started -= new EventHandler (this.OnStarted);
				this.Control.Ended -= new EventHandler (this.OnEnded);
				this.picker.ValueChanged -= new EventHandler (this.OnValueChanged);
			}
			base.Dispose (disposing);
		}
		*/

		private void OnEnded (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
		}

		private void OnStarted (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
		}

		private void OnValueChanged (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (TimePicker.TimeProperty, (DateExtensions.ToDateTime (this.picker.DateValue) - new DateTime (1, 1, 1)));
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (!(e.PropertyName == TimePicker.TimeProperty.PropertyName) && !(e.PropertyName == TimePicker.FormatProperty.PropertyName))
				return;
			this.UpdateTime ();
		}

		private void UpdateTime ()
		{
			this.picker.DateValue = DateExtensions.ToNSDate (new DateTime (1, 1, 1).Add (Element.Time));

			NSTextField control = this.Control;
			DateTime dateTime = DateTime.Today;
			dateTime = dateTime.Add (Element.Time);

			string str = dateTime.ToString (this.Element.Format);
			control.StringValue = str;
		}
	}
}
