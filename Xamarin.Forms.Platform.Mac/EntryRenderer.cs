using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class EntryRenderer : ViewRenderer<Entry, UITextField>
  {
    private UIColor defaultTextColor;

    public EntryRenderer()
    {
      this.Frame = (CGRect) new RectangleF(0.0f, 20f, 320f, 40f);
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
    {
      this.OnElementChanged(e);
      UITextField control = this.Control;
      if (this.Control == null)
      {
        UITextField uiTextField;
        this.SetNativeControl(uiTextField = new UITextField((CGRect) RectangleF.Empty));
        this.defaultTextColor = uiTextField.TextColor;
        uiTextField.BorderStyle = UITextBorderStyle.RoundedRect;
        uiTextField.EditingChanged += new EventHandler(this.OnEditingChanged);
        uiTextField.ShouldReturn = new UITextFieldCondition(this.OnShouldReturn);
        uiTextField.EditingDidBegin += new EventHandler(this.OnEditingBegan);
        uiTextField.EditingDidEnd += new EventHandler(this.OnEditingEnded);
      }
      if (e.NewElement == null)
        return;
      this.UpdatePlaceholder();
      this.UpdatePassword();
      this.UpdateText();
      this.UpdateColor();
      this.UpdateFont();
      this.UpdateKeyboard();
      this.UpdateAlignment();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Control != null)
      {
        this.Control.EditingDidBegin -= new EventHandler(this.OnEditingBegan);
        this.Control.EditingChanged -= new EventHandler(this.OnEditingChanged);
        this.Control.EditingDidEnd -= new EventHandler(this.OnEditingEnded);
      }
      base.Dispose(disposing);
    }

    private bool OnShouldReturn(UITextField view)
    {
      // ISSUE: reference to a compiler-generated method
      this.Control.ResignFirstResponder();
      this.Element.SendCompleted();
      return true;
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Entry.PlaceholderProperty.PropertyName || e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
        this.UpdatePlaceholder();
      else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
        this.UpdatePassword();
      else if (e.PropertyName == Entry.TextProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
        this.UpdateColor();
      else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
        this.UpdateKeyboard();
      else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
        this.UpdateAlignment();
      else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
      {
        this.UpdateColor();
        this.UpdatePlaceholder();
      }
      base.OnElementPropertyChanged(sender, e);
    }

    private void OnEditingBegan(object sender, EventArgs e)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) true);
    }

    private void OnEditingEnded(object sender, EventArgs e)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) false);
    }

    private void OnEditingChanged(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(Entry.TextProperty, (object) this.Control.Text);
    }

    private void UpdateColor()
    {
      Color textColor = this.Element.TextColor;
      if (textColor.IsDefault || !this.Element.IsEnabled)
        this.Control.TextColor = this.defaultTextColor;
      else
        this.Control.TextColor = ColorExtensions.ToUIColor(textColor);
    }

    private void UpdateFont()
    {
      this.Control.Font = FontExtensions.ToUIFont((IFontElement) this.Element);
    }

    private void UpdatePlaceholder()
    {
      FormattedString formattedString = (FormattedString) this.Element.Placeholder;
      if (formattedString == null)
        return;
      Color placeholderColor = this.Element.PlaceholderColor;
      Color defaultForegroundColor = !this.Element.IsEnabled || placeholderColor.IsDefault ? ColorExtensions.ToColor(ColorExtensions.SeventyPercentGrey) : placeholderColor;
      this.Control.AttributedPlaceholder = FormattedStringExtensions.ToAttributed(formattedString, (Element) this.Element, defaultForegroundColor);
    }

    private void UpdatePassword()
    {
      if (this.Element.IsPassword && this.Control.IsFirstResponder)
      {
        this.Control.Enabled = false;
        this.Control.SecureTextEntry = true;
        this.Control.Enabled = this.Element.IsEnabled;
        // ISSUE: reference to a compiler-generated method
        this.Control.BecomeFirstResponder();
      }
      else
        this.Control.SecureTextEntry = this.Element.IsPassword;
    }

    private void UpdateText()
    {
      if (!(this.Control.Text != this.Element.Text))
        return;
      this.Control.Text = this.Element.Text;
    }

    private void UpdateKeyboard()
    {
      Extensions.ApplyKeyboard((IUITextInput) this.Control, this.Element.Keyboard);
    }

    private void UpdateAlignment()
    {
      this.Control.TextAlignment = AlignmentExtensions.ToNativeTextAlignment(this.Element.HorizontalTextAlignment);
    }
  }
}
