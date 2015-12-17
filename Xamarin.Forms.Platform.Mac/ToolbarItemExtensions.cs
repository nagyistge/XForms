using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public static class ToolbarItemExtensions
  {
    public static UIBarButtonItem ToUIBarButtonItem(this ToolbarItem item, bool forceName = false)
    {
      if (item.Order != ToolbarItemOrder.Secondary)
        return (UIBarButtonItem) new ToolbarItemExtensions.PrimaryToolbarItem(item, forceName);
      return (UIBarButtonItem) new ToolbarItemExtensions.SecondaryToolbarItem(item);
    }

    private sealed class PrimaryToolbarItem : UIBarButtonItem
    {
      private ToolbarItem item;
      private bool forceName;

      public PrimaryToolbarItem(ToolbarItem item, bool forceName)
      {
        this.forceName = forceName;
        this.item = item;
        if (!string.IsNullOrEmpty((string) item.Icon) && !forceName)
          this.UpdateIconAndStyle();
        else
          this.UpdateTextAndStyle();
        this.UpdateIsEnabled();
        this.Clicked += (EventHandler) ((sender, e) => item.Activate());
        item.add_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
      }

      private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == MenuItem.IsEnabledProperty.PropertyName)
          this.UpdateIsEnabled();
        else if (e.PropertyName == MenuItem.TextProperty.PropertyName)
        {
          if (!string.IsNullOrEmpty((string) this.item.Icon) && !this.forceName)
            return;
          this.UpdateTextAndStyle();
        }
        else
        {
          if (!(e.PropertyName == MenuItem.IconProperty.PropertyName) || this.forceName)
            return;
          if (!string.IsNullOrEmpty((string) this.item.Icon))
            this.UpdateIconAndStyle();
          else
            this.UpdateTextAndStyle();
        }
      }

      private void UpdateIsEnabled()
      {
        this.Enabled = this.item.IsEnabled;
      }

      private void UpdateTextAndStyle()
      {
        this.Title = this.item.Text;
        this.Style = UIBarButtonItemStyle.Bordered;
        this.Image = (UIImage) null;
      }

      private void UpdateIconAndStyle()
      {
        // ISSUE: reference to a compiler-generated method
        this.Image = UIImage.FromBundle((string) this.item.Icon);
        this.Style = UIBarButtonItemStyle.Plain;
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
          this.item.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
        // ISSUE: reference to a compiler-generated method
        base.Dispose(disposing);
      }
    }

    private sealed class SecondaryToolbarItem : UIBarButtonItem
    {
      private ToolbarItem item;

      public SecondaryToolbarItem(ToolbarItem item)
        : base((UIView) new ToolbarItemExtensions.SecondaryToolbarItem.SecondaryToolbarItemContent())
      {
        this.item = item;
        this.UpdateText();
        this.UpdateIcon();
        this.UpdateIsEnabled();
        ((UIControl) this.CustomView).TouchUpInside += (EventHandler) ((sender, e) => item.Activate());
        item.add_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
      }

      private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == MenuItem.TextProperty.PropertyName)
          this.UpdateText();
        else if (e.PropertyName == MenuItem.IconProperty.PropertyName)
        {
          this.UpdateIcon();
        }
        else
        {
          if (!(e.PropertyName == MenuItem.IsEnabledProperty.PropertyName))
            return;
          this.UpdateIsEnabled();
        }
      }

      private void UpdateText()
      {
        ((ToolbarItemExtensions.SecondaryToolbarItem.SecondaryToolbarItemContent) this.CustomView).Text = this.item.Text;
      }

      private void UpdateIcon()
      {
        ((ToolbarItemExtensions.SecondaryToolbarItem.SecondaryToolbarItemContent) this.CustomView).Image = string.IsNullOrEmpty((string) this.item.Icon) ? (UIImage) null : new UIImage((string) this.item.Icon);
      }

      private void UpdateIsEnabled()
      {
        ((UIControl) this.CustomView).Enabled = this.item.IsEnabled;
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
          this.item.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnPropertyChanged));
        // ISSUE: reference to a compiler-generated method
        base.Dispose(disposing);
      }

      private sealed class SecondaryToolbarItemContent : UIControl
      {
        private UIImageView imageView;
        private UILabel label;

        public override bool Enabled
        {
          get
          {
            return base.Enabled;
          }
          set
          {
            base.Enabled = value;
            this.label.Enabled = value;
            this.imageView.Alpha = (nfloat) (value ? 1f : 0.25f);
          }
        }

        public string Text
        {
          get
          {
            return this.label.Text;
          }
          set
          {
            this.label.Text = value;
          }
        }

        public UIImage Image
        {
          get
          {
            return this.imageView.Image;
          }
          set
          {
            this.imageView.Image = value;
          }
        }

        public SecondaryToolbarItemContent()
          : base(new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 75, (nfloat) 20))
        {
          this.BackgroundColor = UIColor.Clear;
          UIImageView uiImageView = new UIImageView();
          UIColor clear1 = UIColor.Clear;
          uiImageView.BackgroundColor = clear1;
          this.imageView = uiImageView;
          // ISSUE: reference to a compiler-generated method
          this.AddSubview((UIView) this.imageView);
          UILabel uiLabel = new UILabel();
          UIColor clear2 = UIColor.Clear;
          uiLabel.BackgroundColor = clear2;
          nint nint = (nint) 1;
          uiLabel.Lines = nint;
          long num = 4;
          uiLabel.LineBreakMode = (UILineBreakMode) num;
          UIFont uiFont = UIFont.SystemFontOfSize((nfloat) 10);
          uiLabel.Font = uiFont;
          this.label = uiLabel;
          // ISSUE: reference to a compiler-generated method
          this.AddSubview((UIView) this.label);
        }

        public override void LayoutSubviews()
        {
          // ISSUE: reference to a compiler-generated method
          base.LayoutSubviews();
          // ISSUE: reference to a compiler-generated method
          CGSize size = this.imageView.SizeThatFits(this.Bounds.Size);
          // ISSUE: reference to a compiler-generated method
          CGSize cgSize1 = this.label.SizeThatFits(this.Bounds.Size);
          if (size.Width > (nfloat) 0 && (string.IsNullOrEmpty(this.Text) || cgSize1.Width > this.Bounds.Width / (nfloat) 3))
          {
            this.imageView.Frame = new CGRect(CGPoint.Empty, size);
            this.imageView.Center = new CGPoint(RectangleFExtensions.GetMidX(this.Bounds), RectangleFExtensions.GetMidY(this.Bounds));
            this.label.Hidden = true;
          }
          else
          {
            this.label.Hidden = false;
            // ISSUE: reference to a compiler-generated method
            CGSize cgSize2 = this.label.SizeThatFits(new CGSize(this.Bounds.Width - (nfloat) 15f - size.Width, this.Bounds.Height - (nfloat) 10f));
            CGRect cgRect = new CGRect(new CGPoint((this.Bounds.Width - cgSize2.Width - size.Width) / (nfloat) 2, RectangleFExtensions.GetMidY(this.Bounds) - size.Height / (nfloat) 2), size);
            this.imageView.Frame = cgRect;
            cgRect.X = cgRect.Right + (nfloat) (size.Width > (nfloat) 0 ? 5f : 0.0f);
            cgRect.Size = cgSize2;
            cgRect.Height = this.Bounds.Height;
            cgRect.Y = (nfloat) 0;
            this.label.Frame = cgRect;
          }
        }
      }
    }
  }
}
