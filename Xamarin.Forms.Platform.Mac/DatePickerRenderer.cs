using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, NSTextView>
	{
		private NSDatePicker picker;


		protected override void OnElementChanged (ElementChangedEventArgs<DatePicker> e)
		{
			this.OnElementChanged (e);
			if (e.OldElement == null)
			{
				NoCaretField noCaretField = new NoCaretField ();

				// Not for Mac
				//long num1 = 3;
				//noCaretField.BorderStyle = (UITextBorderStyle) num1;

				NoCaretField entry = noCaretField;
				entry.TextDidBeginEditing += OnStarted;
				entry.TextDidEndEditing += OnEnded;

				picker = new NSDatePicker () {
					DatePickerMode = NSDatePickerMode.Single,
					TimeZone = new NSTimeZone ("UTC")
				};
				picker.ValidateProposedDateValue += Picker_ValidateProposedDateValue;

				// TODO: What is the intent here?
				/*
				NSToolbar uiToolbar1 = new NSToolbar (new CGRect ((nfloat)0, (nfloat)0, NSScreen.MainScreen.Frame.Width, (nfloat)44));
				uiToolbar1.BarStyle = UIBarStyle.Default;
				int num2 = 1;
				uiToolbar1.Translucent = num2 != 0;
				UIToolbar uiToolbar2 = uiToolbar1;

				var uiBarButtonItem1 = new NSStatusBarButton new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
				// ISSUE: reference to a compiler-generated method
				UIBarButtonItem uiBarButtonItem2 = new UIBarButtonItem (UIBarButtonSystemItem.Done, (EventHandler)((o, a) => entry.ResignFirstResponder ()));
				uiToolbar2.SetItems (new UIBarButtonItem[2] {
					uiBarButtonItem1,
					uiBarButtonItem2
				}, 0 != 0);
				entry.InputView = (NSView)this.picker;
				entry.InputAccessoryView = (NSView)uiToolbar2;
				*/
				this.SetNativeControl( entry );
			}

			if (e.NewElement == null)
				return;
			this.UpdateDateFromModel (false);
			this.UpdateMaximumDate ();
			this.UpdateMinimumDate ();
		}

		void Picker_ValidateProposedDateValue (object sender, NSDatePickerValidatorEventArgs e)
		{
			if (this.Element == null)
				return;
			((IElementController)Element).SetValueFromRenderer (DatePicker.DateProperty, DateExtensions.ToDateTime (e.ProposedDateValue).Date);
		}

		private void OnEnded (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
		}

		private void OnStarted (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
				this.UpdateDateFromModel (true);
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
			{
				this.UpdateMinimumDate ();
			}
			else
			{
				if (!(e.PropertyName == DatePicker.MaximumDateProperty.PropertyName))
					return;
				this.UpdateMaximumDate ();
			}
		}

		private void UpdateMaximumDate ()
		{
			picker.MaxDate = DateExtensions.ToNSDate (Element.MaximumDate);
		}

		private void UpdateMinimumDate ()
		{
			picker.MinDate = DateExtensions.ToNSDate (this.Element.MinimumDate);
		}

		private void UpdateDateFromModel (bool animate)
		{
			DateTime dateTime = DateExtensions.ToDateTime (picker.DateValue);
			DateTime date1 = dateTime.Date;
			dateTime = Element.Date;
			DateTime date2 = dateTime.Date;
			if (date1 != date2)
			{
				picker.DateValue = DateExtensions.ToNSDate (Element.Date);
			}

			Control.Value = Element.Date.ToString (Element.Format);
		}

	}
}
