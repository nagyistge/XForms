using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class EntryRenderer : ViewRenderer<Entry, NSTextField>
	{
		private NSColor defaultTextColor;

		public EntryRenderer ()
		{
			this.Frame = (CGRect)new RectangleF (0.0f, 20f, 320f, 40f);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			this.OnElementChanged (e);
			var control = this.Control;
			if (this.Control == null)
			{
				NSTextField uiTextField;

				if (Element.IsPassword)
					uiTextField = new NSSecureTextField ();
				else
					uiTextField = new NSTextField ();
				
				this.SetNativeControl (uiTextField);
				this.defaultTextColor = uiTextField.TextColor;
				//uiTextField.BorderStyle = UITextBorderStyle.RoundedRect;

				uiTextField.Changed += new EventHandler (this.OnEditingChanged);
				//uiTextField.ShouldReturn = new UITextFieldCondition (this.OnShouldReturn);
				uiTextField.EditingBegan += new EventHandler (this.OnEditingBegan);
				uiTextField.EditingEnded += new EventHandler (this.OnEditingEnded);
			}
			if (e.NewElement == null)
				return;
			this.UpdatePlaceholder ();
			this.UpdatePassword ();
			this.UpdateText ();
			this.UpdateColor ();
			this.UpdateFont ();
			//this.UpdateKeyboard ();
			this.UpdateAlignment ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && this.Control != null)
			{
				this.Control.EditingBegan -= new EventHandler (this.OnEditingBegan);
				this.Control.Changed -= new EventHandler (this.OnEditingChanged);
				this.Control.EditingEnded -= new EventHandler (this.OnEditingEnded);
			}
			base.Dispose (disposing);
		}

		private bool OnShouldReturn (NSTextField view)
		{
			this.Control.ResignFirstResponder ();
			this.Element.SendCompleted ();
			return true;
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Entry.PlaceholderProperty.PropertyName || e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
				this.UpdatePlaceholder ();
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
				this.UpdatePassword ();
			else if (e.PropertyName == Entry.TextProperty.PropertyName)
				this.UpdateText ();
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
				this.UpdateColor ();
			//else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
			//	this.UpdateKeyboard ();
			else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
				this.UpdateAlignment ();
			else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				this.UpdateColor ();
				this.UpdatePlaceholder ();
			}
			base.OnElementPropertyChanged (sender, e);
		}

		private void OnEditingBegan (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
		}

		private void OnEditingEnded (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
		}

		private void OnEditingChanged (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (Entry.TextProperty, this.Control.StringValue);
		}

		private void UpdateColor ()
		{
			Color textColor = this.Element.TextColor;
			if (textColor.IsDefault || !this.Element.IsEnabled)
				this.Control.TextColor = this.defaultTextColor;
			else
				this.Control.TextColor = ColorExtensions.ToUIColor (textColor);
		}

		private void UpdateFont ()
		{
			this.Control.Font = FontExtensions.ToUIFont ((IFontElement)this.Element);
		}

		private void UpdatePlaceholder ()
		{
			FormattedString formattedString = (FormattedString)this.Element.Placeholder;
			if (formattedString == null)
				return;
			Color placeholderColor = this.Element.PlaceholderColor;
			Color defaultForegroundColor = !this.Element.IsEnabled || placeholderColor.IsDefault ? ColorExtensions.ToColor (ColorExtensions.SeventyPercentGrey) : placeholderColor;

			//this.Control.AttributedPlaceholder = FormattedStringExtensions.ToAttributed (formattedString, (Element)this.Element, defaultForegroundColor);
		}

		private void UpdatePassword ()
		{
			if (this.Element.IsPassword && this.Control.IsFirstResponder())
			{
				this.Control.Editable = this.Element.IsEnabled;
				this.Control.Selectable = this.Element.IsEnabled;
				this.Control.BecomeFirstResponder ();
			}
		}

		private void UpdateText ()
		{
			if (Control.StringValue == Element.Text)
				return;
			Control.StringValue = Element.Text;
		}

		/*
		private void UpdateKeyboard ()
		{
			Extensions.ApplyKeyboard ((IUITextInput)this.Control, this.Element.Keyboard);
		}
		*/

		private void UpdateAlignment ()
		{
			this.Control.Alignment = AlignmentExtensions.ToNativeTextAlignment (this.Element.HorizontalTextAlignment);
		}
	}
}
