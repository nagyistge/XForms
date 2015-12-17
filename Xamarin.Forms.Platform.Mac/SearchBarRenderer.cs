using CoreGraphics;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class SearchBarRenderer : ViewRenderer<SearchBar, UISearchBar>
  {
    private UIColor defaultTextColor;
    private UIColor cancelButtonTextColorDefaultNormal;
    private UIColor cancelButtonTextColorDefaultHighlighted;
    private UIColor cancelButtonTextColorDefaultDisabled;
    private UIColor defaultTintColor;
    private UITextField textField;

    protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
    {
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          UISearchBar uiSearchBar = new UISearchBar((CGRect) RectangleF.Empty);
          uiSearchBar.ShowsCancelButton = true;
          long num = 0;
          uiSearchBar.BarStyle = (UIBarStyle) num;
          UISearchBar uiview = uiSearchBar;
          UIButton descendantView = UIViewExtensions.FindDescendantView<UIButton>((UIView) uiview);
          // ISSUE: reference to a compiler-generated method
          this.cancelButtonTextColorDefaultNormal = descendantView.TitleColor(UIControlState.Normal);
          // ISSUE: reference to a compiler-generated method
          this.cancelButtonTextColorDefaultHighlighted = descendantView.TitleColor(UIControlState.Highlighted);
          // ISSUE: reference to a compiler-generated method
          this.cancelButtonTextColorDefaultDisabled = descendantView.TitleColor(UIControlState.Disabled);
          this.SetNativeControl(uiview);
          this.Control.CancelButtonClicked += new EventHandler(this.OnCancelClicked);
          this.Control.SearchButtonClicked += new EventHandler(this.OnSearchButtonClicked);
          this.Control.TextChanged += new EventHandler<UISearchBarTextChangedEventArgs>(this.OnTextChanged);
          this.Control.OnEditingStarted += new EventHandler(this.OnEditingStarted);
          this.Control.OnEditingStopped += new EventHandler(this.OnEditingEnded);
        }
        this.UpdatePlaceholder();
        this.UpdateText();
        this.UpdateFont();
        this.UpdateIsEnabled();
        this.UpdateCancelButton();
        this.UpdateAlignment();
        this.UpdateTextColor();
      }
      this.OnElementChanged(e);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == SearchBar.PlaceholderProperty.PropertyName || e.PropertyName == SearchBar.PlaceholderColorProperty.PropertyName)
        this.UpdatePlaceholder();
      else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
      {
        this.UpdateIsEnabled();
        this.UpdateTextColor();
        this.UpdatePlaceholder();
      }
      else if (e.PropertyName == SearchBar.TextColorProperty.PropertyName)
        this.UpdateTextColor();
      else if (e.PropertyName == SearchBar.TextProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == SearchBar.CancelButtonColorProperty.PropertyName)
        this.UpdateCancelButton();
      else if (e.PropertyName == SearchBar.FontAttributesProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == SearchBar.FontFamilyProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == SearchBar.FontSizeProperty.PropertyName)
      {
        this.UpdateFont();
      }
      else
      {
        if (!(e.PropertyName == SearchBar.HorizontalTextAlignmentProperty.PropertyName))
          return;
        this.UpdateAlignment();
      }
    }

    protected override void SetBackgroundColor(Color color)
    {
      base.SetBackgroundColor(color);
      if (this.Control == null)
        return;
      if (this.defaultTintColor == null)
        this.defaultTintColor = !Forms.IsiOS7OrNewer ? this.Control.TintColor : this.Control.BarTintColor;
      if (Forms.IsiOS7OrNewer)
        this.Control.BarTintColor = ColorExtensions.ToUIColor(color, this.defaultTintColor);
      else
        this.Control.TintColor = ColorExtensions.ToUIColor(color, this.defaultTintColor);
      if (color.A < 1.0)
      {
        // ISSUE: reference to a compiler-generated method
        this.Control.SetBackgroundImage(new UIImage(), UIBarPosition.Any, UIBarMetrics.Default);
      }
      this.UpdateCancelButton();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Control != null)
      {
        this.Control.CancelButtonClicked -= new EventHandler(this.OnCancelClicked);
        this.Control.SearchButtonClicked -= new EventHandler(this.OnSearchButtonClicked);
        this.Control.TextChanged -= new EventHandler<UISearchBarTextChangedEventArgs>(this.OnTextChanged);
        this.Control.OnEditingStarted -= new EventHandler(this.OnEditingEnded);
        this.Control.OnEditingStopped -= new EventHandler(this.OnEditingStarted);
      }
      base.Dispose(disposing);
    }

    private void OnEditingStarted(object sender, EventArgs e)
    {
      if (this.Element == null)
        return;
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) true);
    }

    private void OnEditingEnded(object sender, EventArgs e)
    {
      if (this.Element == null)
        return;
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) false);
    }

    private void OnTextChanged(object sender, UISearchBarTextChangedEventArgs a)
    {
      this.Element.SetValueFromRenderer(SearchBar.TextProperty, (object) this.Control.Text);
    }

    private void OnSearchButtonClicked(object sender, EventArgs e)
    {
      this.Element.OnSearchButtonPressed();
      // ISSUE: reference to a compiler-generated method
      this.Control.ResignFirstResponder();
    }

    private void OnCancelClicked(object sender, EventArgs args)
    {
      this.Element.SetValueFromRenderer(SearchBar.TextProperty, (object) null);
      // ISSUE: reference to a compiler-generated method
      this.Control.ResignFirstResponder();
    }

    private void UpdatePlaceholder()
    {
      this.textField = this.textField ?? UIViewExtensions.FindDescendantView<UITextField>((UIView) this.Control);
      if (this.textField == null)
        return;
      FormattedString formattedString = (FormattedString) this.Element.Placeholder ?? (FormattedString) string.Empty;
      Color placeholderColor = this.Element.PlaceholderColor;
      Color defaultForegroundColor = !this.Element.IsEnabled || placeholderColor.IsDefault ? ColorExtensions.ToColor(ColorExtensions.SeventyPercentGrey) : placeholderColor;
      this.textField.AttributedPlaceholder = FormattedStringExtensions.ToAttributed(formattedString, (Element) this.Element, defaultForegroundColor);
    }

    private void UpdateTextColor()
    {
      this.textField = this.textField ?? UIViewExtensions.FindDescendantView<UITextField>((UIView) this.Control);
      if (this.textField == null)
        return;
      this.defaultTextColor = this.defaultTextColor ?? this.textField.TextColor;
      Color textColor = this.Element.TextColor;
      this.textField.TextColor = ColorExtensions.ToUIColor(!this.Element.IsEnabled || textColor.IsDefault ? ColorExtensions.ToColor(this.defaultTextColor) : textColor);
    }

    private void UpdateIsEnabled()
    {
      this.Control.UserInteractionEnabled = this.Element.IsEnabled;
    }

    private void UpdateText()
    {
      this.Control.Text = this.Element.Text;
      this.UpdateCancelButton();
    }

    private void UpdateCancelButton()
    {
      this.Control.ShowsCancelButton = !string.IsNullOrEmpty(this.Control.Text);
      UIButton descendantView = UIViewExtensions.FindDescendantView<UIButton>((UIView) this.Control);
      if (descendantView == null)
        return;
      if (this.Element.CancelButtonColor == Color.Default)
      {
        // ISSUE: reference to a compiler-generated method
        descendantView.SetTitleColor(this.cancelButtonTextColorDefaultNormal, UIControlState.Normal);
        // ISSUE: reference to a compiler-generated method
        descendantView.SetTitleColor(this.cancelButtonTextColorDefaultHighlighted, UIControlState.Highlighted);
        // ISSUE: reference to a compiler-generated method
        descendantView.SetTitleColor(this.cancelButtonTextColorDefaultDisabled, UIControlState.Disabled);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        descendantView.SetTitleColor(ColorExtensions.ToUIColor(this.Element.CancelButtonColor), UIControlState.Normal);
        // ISSUE: reference to a compiler-generated method
        descendantView.SetTitleColor(ColorExtensions.ToUIColor(this.Element.CancelButtonColor), UIControlState.Highlighted);
        // ISSUE: reference to a compiler-generated method
        descendantView.SetTitleColor(this.cancelButtonTextColorDefaultDisabled, UIControlState.Disabled);
      }
    }

    private void UpdateFont()
    {
      this.textField = this.textField ?? UIViewExtensions.FindDescendantView<UITextField>((UIView) this.Control);
      if (this.textField == null)
        return;
      this.textField.Font = FontExtensions.ToUIFont((IFontElement) this.Element);
    }

    private void UpdateAlignment()
    {
      this.textField = this.textField ?? UIViewExtensions.FindDescendantView<UITextField>((UIView) this.Control);
      if (this.textField == null)
        return;
      this.textField.TextAlignment = AlignmentExtensions.ToNativeTextAlignment(this.Element.HorizontalTextAlignment);
    }
  }
}
