using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class EditorRenderer : ViewRenderer<Editor, UITextView>
  {
    private UIToolbar accessoryView;

    protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
    {
      this.OnElementChanged(e);
      if (this.Control == null)
      {
        this.SetNativeControl(new UITextView(CGRect.Empty));
        if (Device.Idiom == TargetIdiom.Phone)
        {
          UIToolbar uiToolbar = new UIToolbar(new CGRect((nfloat) 0, (nfloat) 0, UIScreen.MainScreen.Bounds.Width, (nfloat) 44));
          uiToolbar.BarStyle = UIBarStyle.Default;
          int num = 1;
          uiToolbar.Translucent = num != 0;
          this.accessoryView = uiToolbar;
          this.accessoryView.SetItems(new UIBarButtonItem[2]
          {
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, (EventHandler) ((o, a) =>
            {
              // ISSUE: reference to a compiler-generated method
              this.Control.ResignFirstResponder();
              this.Element.SendCompleted();
            }))
          }, 0 != 0);
          this.Control.InputAccessoryView = (UIView) this.accessoryView;
        }
        this.Control.Changed += new EventHandler(this.HandleChanged);
        this.Control.Started += new EventHandler(this.OnStarted);
        this.Control.Ended += new EventHandler(this.OnEnded);
      }
      if (e.NewElement == null)
        return;
      this.UpdateText();
      this.UpdateFont();
      this.UpdateTextColor();
      this.UpdateKeyboard();
      this.UpdateEditable();
    }

    private void OnEnded(object sender, EventArgs eventArgs)
    {
      this.Element.SetValue(VisualElement.IsFocusedPropertyKey, (object) false);
      this.Element.SendCompleted();
    }

    private void OnStarted(object sender, EventArgs eventArgs)
    {
      this.Element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, (object) true);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Control.Changed -= new EventHandler(this.HandleChanged);
        this.Control.Started -= new EventHandler(this.OnStarted);
        this.Control.Ended -= new EventHandler(this.OnEnded);
      }
      base.Dispose(disposing);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Editor.TextProperty.PropertyName)
        this.UpdateText();
      else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
        this.UpdateKeyboard();
      else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
        this.UpdateEditable();
      else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
        this.UpdateTextColor();
      else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
        this.UpdateFont();
      else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
      {
        this.UpdateFont();
      }
      else
      {
        if (!(e.PropertyName == Editor.FontSizeProperty.PropertyName))
          return;
        this.UpdateFont();
      }
    }

    private void HandleChanged(object sender, EventArgs e)
    {
      this.Element.SetValueFromRenderer(Editor.TextProperty, (object) this.Control.Text);
    }

    private void UpdateText()
    {
      if (!(this.Control.Text != this.Element.Text))
        return;
      this.Control.Text = this.Element.Text;
    }

    private void UpdateFont()
    {
      this.Control.Font = FontExtensions.ToUIFont((IFontElement) this.Element);
    }

    private void UpdateEditable()
    {
      this.Control.Editable = this.Element.IsEnabled;
      this.Control.UserInteractionEnabled = this.Element.IsEnabled;
      if (this.Control.InputAccessoryView == null)
        return;
      this.Control.InputAccessoryView.Hidden = !this.Element.IsEnabled;
    }

    private void UpdateKeyboard()
    {
      Extensions.ApplyKeyboard((IUITextInput) this.Control, this.Element.Keyboard);
    }

    private void UpdateTextColor()
    {
      Color textColor = this.Element.TextColor;
      if (textColor.IsDefault)
        this.Control.TextColor = UIColor.Black;
      else
        this.Control.TextColor = ColorExtensions.ToUIColor(textColor);
    }

    public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      if (!Forms.IsiOS7OrNewer)
        return base.GetDesiredSize(Math.Min(widthConstraint, 2000.0), Math.Min(heightConstraint, 2000.0));
      return base.GetDesiredSize(widthConstraint, heightConstraint);
    }
  }
}
