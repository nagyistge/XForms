using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class Extensions
  {
    public static void ApplyKeyboard(this IUITextInput textInput, Keyboard keyboard)
    {
      textInput.AutocapitalizationType = UITextAutocapitalizationType.None;
      textInput.AutocorrectionType = UITextAutocorrectionType.No;
      textInput.SpellCheckingType = UITextSpellCheckingType.No;
      if (keyboard == Keyboard.Default)
      {
        textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
        textInput.AutocorrectionType = UITextAutocorrectionType.Default;
        textInput.SpellCheckingType = UITextSpellCheckingType.Default;
      }
      else if (keyboard == Keyboard.Chat)
      {
        textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
        textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
      }
      else if (keyboard == Keyboard.Email)
        textInput.KeyboardType = UIKeyboardType.EmailAddress;
      else if (keyboard == Keyboard.Numeric)
        textInput.KeyboardType = UIKeyboardType.DecimalPad;
      else if (keyboard == Keyboard.Telephone)
        textInput.KeyboardType = UIKeyboardType.PhonePad;
      else if (keyboard == Keyboard.Text)
      {
        textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
        textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
        textInput.SpellCheckingType = UITextSpellCheckingType.Yes;
      }
      else if (keyboard == Keyboard.Url)
      {
        textInput.KeyboardType = UIKeyboardType.Url;
      }
      else
      {
        if (!(keyboard is CustomKeyboard))
          return;
        CustomKeyboard customKeyboard = (CustomKeyboard) keyboard;
        bool flag1 = (customKeyboard.Flags & KeyboardFlags.CapitalizeSentence) == KeyboardFlags.CapitalizeSentence;
        bool flag2 = (customKeyboard.Flags & KeyboardFlags.Spellcheck) == KeyboardFlags.Spellcheck;
        bool flag3 = (customKeyboard.Flags & KeyboardFlags.Suggestions) == KeyboardFlags.Suggestions;
        textInput.AutocapitalizationType = flag1 ? UITextAutocapitalizationType.Sentences : UITextAutocapitalizationType.None;
        textInput.AutocorrectionType = flag3 ? UITextAutocorrectionType.Yes : UITextAutocorrectionType.No;
        textInput.SpellCheckingType = flag2 ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No;
      }
    }

    internal static DeviceOrientation ToDeviceOrientation(this UIDeviceOrientation orientation)
    {
      long num1 = (long) (orientation - 1L);
      long num2 = 3;
      if ((ulong) num1 <= (ulong) num2)
      {
        switch ((uint) num1)
        {
          case 0:
            return DeviceOrientation.Portrait;
          case 1:
            return DeviceOrientation.PortraitDown;
          case 2:
            return DeviceOrientation.LandscapeLeft;
          case 3:
            return DeviceOrientation.LandscapeRight;
        }
      }
      return DeviceOrientation.Other;
    }
  }
}
