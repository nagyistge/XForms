using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class SearchBarRenderer : ViewRenderer<SearchBar, NSSearchField>
	{
		private NSColor defaultTextColor;
		private NSColor cancelButtonTextColorDefaultNormal;
		private NSColor cancelButtonTextColorDefaultHighlighted;
		private NSColor cancelButtonTextColorDefaultDisabled;
		private NSColor defaultTintColor;

		protected override void OnElementChanged (ElementChangedEventArgs<SearchBar> e)
		{
			if (e.NewElement != null)
			{
				if (this.Control == null)
				{
					var uiSearchBar = new NSSearchField ((CGRect)RectangleF.Empty);

					// TODO: Cancel Button
					//uiSearchBar.ShowsCancelButton = true;

					//long num = 0;
					//uiSearchBar.BarStyle = (UIBarStyle)num;
					NSSearchField uiview = uiSearchBar;
					NSButton descendantView = NSViewExtensions.FindDescendantView<NSButton> ((NSView)uiview);

					// TODO: Colors
					/*
					this.cancelButtonTextColorDefaultNormal = descendantView.TitleColor (UIControlState.Normal);
					this.cancelButtonTextColorDefaultHighlighted = descendantView.TitleColor (UIControlState.Highlighted);
					this.cancelButtonTextColorDefaultDisabled = descendantView.TitleColor (UIControlState.Disabled);
					*/
					this.SetNativeControl (uiview);

					Control.Changed += Search_Changed;
					Control.EditingBegan += Search_EditingBegan;
					Control.EditingEnded += Search_EditingEnded;
					Control.SearchingStarted += Search_SearchingStarted;
					Control.Cell.CancelButtonCell.Activated += Search_Canceled;
				}
				this.UpdatePlaceholder ();
				this.UpdateText ();
				this.UpdateFont ();
				this.UpdateIsEnabled ();
				this.UpdateCancelButton ();
				this.UpdateAlignment ();
				this.UpdateTextColor ();
			}
			this.OnElementChanged (e);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == SearchBar.PlaceholderProperty.PropertyName || e.PropertyName == SearchBar.PlaceholderColorProperty.PropertyName)
				this.UpdatePlaceholder ();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				this.UpdateIsEnabled ();
				this.UpdateTextColor ();
				this.UpdatePlaceholder ();
			}
			else if (e.PropertyName == SearchBar.TextColorProperty.PropertyName)
				this.UpdateTextColor ();
			else if (e.PropertyName == SearchBar.TextProperty.PropertyName)
				this.UpdateText ();
			else if (e.PropertyName == SearchBar.CancelButtonColorProperty.PropertyName)
				this.UpdateCancelButton ();
			else if (e.PropertyName == SearchBar.FontAttributesProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == SearchBar.FontFamilyProperty.PropertyName)
				this.UpdateFont ();
			else if (e.PropertyName == SearchBar.FontSizeProperty.PropertyName)
			{
				this.UpdateFont ();
			}
			else
			{
				if (!(e.PropertyName == SearchBar.HorizontalTextAlignmentProperty.PropertyName))
					return;
				this.UpdateAlignment ();
			}
		}

		void Search_EditingBegan (object sender, EventArgs e)
		{
			if (this.Element == null)
				return;
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
		}

		void Search_Changed (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (SearchBar.TextProperty, this.Control.StringValue);
		}

		void Search_EditingEnded (object sender, EventArgs e)
		{
			if (this.Element == null)
				return;
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
		}

		void Search_SearchingStarted (object sender, EventArgs e)
		{
			this.Element.OnSearchButtonPressed ();
			this.Control.ResignFirstResponder ();
		}

		void Search_Canceled (object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer (SearchBar.TextProperty, null);
			this.Control.ResignFirstResponder ();
		}




		protected override void SetBackgroundColor (Color color)
		{
			base.SetBackgroundColor (color);

			//TODO: Background Color
			/*
			if (this.Control == null)
				return;
			if (this.defaultTintColor == null)
				this.defaultTintColor = !Forms.IsiOS7OrNewer ? this.Control.TintColor : this.Control.BarTintColor;
			if (Forms.IsiOS7OrNewer)
				this.Control.BarTintColor = ColorExtensions.ToUIColor (color, this.defaultTintColor);
			else
				this.Control.TintColor = ColorExtensions.ToUIColor (color, this.defaultTintColor);
			if (color.A < 1.0)
			{
				this.Control.SetBackgroundImage (new UIImage (), UIBarPosition.Any, UIBarMetrics.Default);
			}
			
			this.UpdateCancelButton ();
			*/
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && this.Control != null)
			{
				Control.Changed -= Search_Changed;
				Control.EditingBegan -= Search_EditingBegan;
				Control.EditingEnded -= Search_EditingEnded;
				Control.SearchingStarted -= Search_SearchingStarted;
				Control.Cell.CancelButtonCell.Activated -= Search_Canceled;
			}
			base.Dispose (disposing);
		}

		private void UpdatePlaceholder ()
		{
			// TODO: Placeholder
			/*
			this.textField = this.textField ?? NSViewExtensions.FindDescendantView<UITextField> ((NSView)this.Control);
			if (this.textField == null)
				return;
			FormattedString formattedString = (FormattedString)this.Element.Placeholder ?? (FormattedString)string.Empty;
			Color placeholderColor = this.Element.PlaceholderColor;
			Color defaultForegroundColor = !this.Element.IsEnabled || placeholderColor.IsDefault ? ColorExtensions.ToColor (ColorExtensions.SeventyPercentGrey) : placeholderColor;
			this.textField.AttributedPlaceholder = FormattedStringExtensions.ToAttributed (formattedString, (Element)this.Element, defaultForegroundColor);
			*/
		}

		private void UpdateTextColor ()
		{
			// TODO: Text Color
			/*
			this.textField = this.textField ?? NSViewExtensions.FindDescendantView<UITextField> ((NSView)this.Control);
			if (this.textField == null)
				return;
			this.defaultTextColor = this.defaultTextColor ?? this.textField.TextColor;
			Color textColor = this.Element.TextColor;
			this.textField.TextColor = ColorExtensions.ToUIColor (!this.Element.IsEnabled || textColor.IsDefault ? ColorExtensions.ToColor (this.defaultTextColor) : textColor);
			*/
		}

		private void UpdateIsEnabled ()
		{
			this.Control.Enabled = this.Element.IsEnabled;
		}

		private void UpdateText ()
		{
			this.Control.StringValue = this.Element.Text;
			this.UpdateCancelButton ();
		}

		private void UpdateCancelButton ()
		{
			// TODO: Cancel Button
			/*
			this.Control.ShowsCancelButton = !string.IsNullOrEmpty (this.Control.Text);
			NSButton descendantView = NSViewExtensions.FindDescendantView<NSButton> ((NSView)this.Control);
			if (descendantView == null)
				return;
			if (this.Element.CancelButtonColor == Color.Default)
			{
				descendantView.SetTitleColor (this.cancelButtonTextColorDefaultNormal, UIControlState.Normal);
				descendantView.SetTitleColor (this.cancelButtonTextColorDefaultHighlighted, UIControlState.Highlighted);
				descendantView.SetTitleColor (this.cancelButtonTextColorDefaultDisabled, UIControlState.Disabled);
			}
			else
			{
				descendantView.SetTitleColor (ColorExtensions.ToUIColor (this.Element.CancelButtonColor), UIControlState.Normal);
				descendantView.SetTitleColor (ColorExtensions.ToUIColor (this.Element.CancelButtonColor), UIControlState.Highlighted);
				descendantView.SetTitleColor (this.cancelButtonTextColorDefaultDisabled, UIControlState.Disabled);
			}
			*/
		}

		private void UpdateFont ()
		{
			// TODO: Font
			/*
			this.textField = this.textField ?? NSViewExtensions.FindDescendantView<UITextField> ((NSView)this.Control);
			if (this.textField == null)
				return;
			this.textField.Font = FontExtensions.ToUIFont ((IFontElement)this.Element);
			*/
		}

		private void UpdateAlignment ()
		{
			// TODO: Alignment
			/*
			this.textField = this.textField ?? NSViewExtensions.FindDescendantView<UITextField> ((NSView)this.Control);
			if (this.textField == null)
				return;
			this.textField.TextAlignment = AlignmentExtensions.ToNativeTextAlignment (this.Element.HorizontalTextAlignment);
			*/
		}
	}
}
