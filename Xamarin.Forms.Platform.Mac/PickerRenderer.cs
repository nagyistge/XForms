using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class PickerRenderer : ViewRenderer<Picker, NSTextField>
	{
		private NSPopUpButton picker;

		protected override void OnElementChanged (ElementChangedEventArgs<Picker> e)
		{
			/*
			if (e.OldElement != null)
				((ObservableCollection<string>)e.OldElement.get_Items ()).CollectionChanged -= new NotifyCollectionChangedEventHandler (this.RowsCollectionChanged);
			if (e.NewElement != null)
			{
				if (this.Control == null)
				{
					NoCaretField noCaretField = new NoCaretField ();
					long num1 = 3;
					noCaretField.BorderStyle = (UITextBorderStyle)num1;
					NoCaretField entry = noCaretField;
					entry.Started += new EventHandler (this.OnStarted);
					entry.Ended += new EventHandler (this.OnEnded);
					this.picker = new UIPickerView ();
					UIToolbar uiToolbar1 = new UIToolbar (new CGRect ((nfloat)0, (nfloat)0, UIScreen.MainScreen.Bounds.Width, (nfloat)44));
					uiToolbar1.BarStyle = UIBarStyle.Default;
					int num2 = 1;
					uiToolbar1.Translucent = num2 != 0;
					UIToolbar uiToolbar2 = uiToolbar1;
					UIBarButtonItem uiBarButtonItem1 = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
					UIBarButtonItem uiBarButtonItem2 = new UIBarButtonItem (UIBarButtonSystemItem.Done, (EventHandler)((o, a) =>
					{
						PickerRenderer.PickerSource s = (PickerRenderer.PickerSource)this.picker.Model;
						if (s.SelectedIndex == -1 && this.Element.get_Items () != null && this.Element.get_Items ().Count > 0)
							this.UpdatePickerSelectedIndex (0);
						this.UpdatePickerFromModel (s);
						// ISSUE: reference to a compiler-generated method
						entry.ResignFirstResponder ();
					}));
					uiToolbar2.SetItems (new UIBarButtonItem[2] {
						uiBarButtonItem1,
						uiBarButtonItem2
					}, 0 != 0);
					entry.InputView = (NSView)this.picker;
					entry.InputAccessoryView = (NSView)uiToolbar2;
					this.SetNativeControl ((UITextField)entry);
				}
				this.picker.Model = (UIPickerViewModel)new PickerRenderer.PickerSource (this);
				this.UpdatePicker ();
				((ObservableCollection<string>)e.NewElement.get_Items ()).CollectionChanged += new NotifyCollectionChangedEventHandler (this.RowsCollectionChanged);
			}
			*/
			this.OnElementChanged (e);
		}

		private void OnEnded (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, false);
		}

		private void OnStarted (object sender, EventArgs eventArgs)
		{
			((IElementController)Element).SetValueFromRenderer (VisualElement.IsFocusedPropertyKey, true);
		}

		private void RowsCollectionChanged (object sender, EventArgs e)
		{
			this.UpdatePicker ();
		}

		private void UpdatePicker ()
		{
			/*
			int selectedIndex = this.Element.SelectedIndex;
			IList<string> items = this.Element.get_Items ();
			this.Control.Placeholder = this.Element.Title;
			string text = this.Control.Text;
			this.Control.Text = selectedIndex == -1 || items == null ? "" : items [selectedIndex];
			this.UpdatePickerNativeSize (text);
			// ISSUE: reference to a compiler-generated method
			this.picker.ReloadAllComponents ();
			if (items == null || items.Count == 0)
				return;
			this.UpdatePickerSelectedIndex (selectedIndex);
			*/
		}

		private void UpdatePickerSelectedIndex (int formsIndex)
		{
			/*
			PickerRenderer.PickerSource pickerSource = (PickerRenderer.PickerSource)this.picker.Model;
			int num = formsIndex;
			pickerSource.SelectedIndex = num;
			string str = formsIndex >= 0 ? this.Element.get_Items () [formsIndex] : (string)null;
			pickerSource.SelectedItem = str;
			// ISSUE: reference to a compiler-generated method
			this.picker.Select ((nint)Math.Max (formsIndex, 0), (nint)0, true);
			*/
		}

		private void UpdatePickerFromModel (PickerRenderer.PickerSource s)
		{
			/*
			if (this.Element == null)
				return;
			string text = this.Control.Text;
			((IElementController)Element).SetValueFromRenderer (Picker.SelectedIndexProperty, s.SelectedIndex);
			this.Control.Text = s.SelectedItem;
			this.UpdatePickerNativeSize (text);
			*/
		}

		private void UpdatePickerNativeSize (string oldText)
		{
			/*
			if (!(oldText != this.Control.Text))
				return;
			this.Element.NativeSizeChanged ();
			*/
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == Picker.TitleProperty.PropertyName)
				this.UpdatePicker ();
			if (!(e.PropertyName == Picker.SelectedIndexProperty.PropertyName))
				return;
			this.UpdatePicker ();
		}

		private class PickerSource 	// : UIPickerViewModel
		{
			private PickerRenderer renderer;

			public string SelectedItem { get; internal set; }

			public int SelectedIndex { get; internal set; }

			public PickerSource (PickerRenderer model)
			{
				this.renderer = model;
			}

			public nint GetRowsInComponent ( NSPopUpButton pickerView, nint component)
			{
				return (nint)(this.renderer.Element.Items != null ? this.renderer.Element.Items.Count : 0);
			}

			public nint GetComponentCount (NSPopUpButton picker)
			{
				return (nint)1;
			}

			public string GetTitle (NSPopUpButton picker, nint row, nint component)
			{
				return this.renderer.Element.Items [(int)row];
			}

			public void Selected (NSPopUpButton picker, nint row, nint component)
			{
				if (this.renderer.Element.Items.Count == 0)
				{
					this.SelectedItem = (string)null;
					this.SelectedIndex = -1;
				}
				else
				{
					this.SelectedItem = this.renderer.Element.Items [(int)row];
					this.SelectedIndex = (int)row;
				}
				this.renderer.UpdatePickerFromModel (this);
			}
		}
	}
}
