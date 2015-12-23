using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class EditorRenderer : ViewRenderer<Editor, NSTextView>
	{
		private NSToolbar accessoryView;

		protected override void OnElementChanged (ElementChangedEventArgs<Editor> e)
		{
			this.OnElementChanged (e);
			if (this.Control == null)
			{
				this.SetNativeControl (new NSTextView (CGRect.Empty));
				if (Device.Idiom == TargetIdiom.Phone)
				{
					accessoryView = new NSToolbar (this.ToString());
					//accessoryView = new NSToolbar (new CGRect ((nfloat)0, (nfloat)0, NSScreen.MainScreen.Frame.Width, (nfloat)44));
					//uiToolbar.BarStyle = UIBarStyle.Default;
					//int num = 1;
					//uiToolbar.Translucent = num != 0;

					var toolBarItem = new NSToolbarItem ("");
					toolBarItem.Activated += (sender, ev) => {
						this.Control.ResignFirstResponder ();
						this.Element.SendCompleted ();
					};

					accessoryView.Items.Append (toolBarItem);

					// TODO: 
					//this.Control.InputAccessoryView = accessoryView;
				}
				this.Control.TextDidChange += HandleChanged;
				this.Control.TextDidBeginEditing += OnStarted;
				this.Control.TextDidEndEditing += OnEnded;
			}
			if (e.NewElement == null)
				return;
			this.UpdateText ();
			this.UpdateFont ();
			this.UpdateTextColor ();
			//this.UpdateKeyboard ();
			this.UpdateEditable ();
		}

		private void OnEnded (object sender, EventArgs eventArgs)
		{
			this.Element.SetValue (VisualElement.IsFocusedPropertyKey, false);
			this.Element.SendCompleted ();
		}

		private void OnStarted (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				this.Control.TextDidChange -= HandleChanged;
				this.Control.TextDidBeginEditing -= OnStarted;
				this.Control.TextDidEndEditing -= OnEnded;
			}
			base.Dispose (disposing);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == Editor.TextProperty.PropertyName)
				this.UpdateText ();
			//else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
			//	this.UpdateKeyboard ();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				this.UpdateEditable ();
			else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
				this.UpdateTextColor ();
			else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
			{
				this.UpdateFont ();
			}
			else
			{
				if (!(e.PropertyName == Editor.FontSizeProperty.PropertyName))
					return;
				this.UpdateFont ();
			}
		}

		private void HandleChanged (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (Editor.TextProperty, this.Control.Value);
		}

		private void UpdateText ()
		{
			if (!(this.Control.Value != this.Element.Text))
				return;
			this.Control.Value = this.Element.Text;
		}

		private void UpdateFont ()
		{
			this.Control.Font = FontExtensions.ToUIFont ((IFontElement)this.Element);
		}

		private void UpdateEditable ()
		{
			this.Control.Editable = this.Element.IsEnabled;
			//this.Control.UserInteractionEnabled = this.Element.IsEnabled;
			/*
			if (this.Control.InputAccessoryView == null)
				return;
			this.Control.InputAccessoryView.Hidden = !this.Element.IsEnabled;
			*/
		}

		// Not for Mac
		/*
		private void UpdateKeyboard ()
		{
			Extensions.ApplyKeyboard ((INSTextInput)this.Control, this.Element.Keyboard);
		}
		*/

		private void UpdateTextColor ()
		{
			Color textColor = this.Element.TextColor;
			if (textColor.IsDefault)
				this.Control.TextColor = NSColor.Black;
			else
				this.Control.TextColor = ColorExtensions.ToUIColor (textColor);
		}
	}
}
