using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class NavigationMenuRenderer : ViewRenderer
  {
    private UICollectionView collectionView;

    protected override void OnElementChanged(ElementChangedEventArgs<View> e)
    {
      this.OnElementChanged(e);
      int num1 = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 1 : 0;
      int num2 = num1 != 0 ? 220 : 142;
      int num3 = num1 != 0 ? 27 : 12;
      int num4 = (int) ((double) num3 * 0.8);
      UICollectionView uiCollectionView = new UICollectionView(new CGRect((nfloat) 0, (nfloat) 0, (nfloat) 100, (nfloat) 100), (UICollectionViewLayout) new UICollectionViewFlowLayout()
      {
        ItemSize = new CGSize((nfloat) num2, (nfloat) (num2 + 30)),
        ScrollDirection = UICollectionViewScrollDirection.Vertical,
        SectionInset = new UIEdgeInsets((nfloat) num3, (nfloat) num3, (nfloat) num4, (nfloat) num3),
        MinimumInteritemSpacing = (nfloat) num3,
        MinimumLineSpacing = (nfloat) num3
      });
      uiCollectionView.DataSource = (IUICollectionViewDataSource) new NavigationMenuRenderer.DataSource((NavigationMenu) this.Element);
      UIColor white = UIColor.White;
      uiCollectionView.BackgroundColor = white;
      this.collectionView = uiCollectionView;
      using (NSString reuseIdentifier = new NSString("NavigationCell"))
        this.collectionView.RegisterClassForCell(typeof (NavigationMenuRenderer.NavigationCell), reuseIdentifier);
      this.SetNativeControl((UIView) this.collectionView);
    }

    private sealed class NavigationCell : UICollectionViewCell
    {
      private string icon;
      private readonly UILabel nameLabel;
      private readonly UIButton image;

      public Action Selected { get; set; }

      public string Name
      {
        get
        {
          return this.nameLabel.Text;
        }
        set
        {
          this.nameLabel.Text = value;
        }
      }

      public string Icon
      {
        get
        {
          return this.icon;
        }
        set
        {
          this.icon = value;
          this.image.SetImage(new UIImage(this.icon), UIControlState.Normal);
        }
      }

      [Export("initWithFrame:")]
      public NavigationCell(CGRect frame)
      {
        UILabel uiLabel = new UILabel(CGRect.Empty);
        UIColor clear = UIColor.Clear;
        uiLabel.BackgroundColor = clear;
        long num = 1;
        uiLabel.TextAlignment = (UITextAlignment) num;
        UIFont uiFont = UIFont.SystemFontOfSize((nfloat) 14);
        uiLabel.Font = uiFont;
        this.nameLabel = uiLabel;
        this.image = new UIButton(CGRect.Empty);
        // ISSUE: explicit constructor call
        base.\u002Ector(frame);
        this.SetupLayer();
        this.image.TouchUpInside += (EventHandler) ((sender, e) =>
        {
          if (this.Selected == null)
            return;
          this.Selected();
        });
        this.image.ContentMode = UIViewContentMode.ScaleAspectFit;
        this.image.Center = this.ContentView.Center;
        // ISSUE: reference to a compiler-generated method
        this.ContentView.AddSubview((UIView) this.image);
        // ISSUE: reference to a compiler-generated method
        this.ContentView.AddSubview((UIView) this.nameLabel);
      }

      private void SetupLayer()
      {
        CALayer layer = this.image.Layer;
        nfloat nfloat = (nfloat) 6;
        layer.ShadowRadius = nfloat;
        CGColor cgColor = UIColor.Black.CGColor;
        layer.ShadowColor = cgColor;
        double num1 = 0.200000002980232;
        layer.ShadowOpacity = (float) num1;
        CGSize cgSize = new CGSize();
        layer.ShadowOffset = cgSize;
        nfloat scale = UIScreen.MainScreen.Scale;
        layer.RasterizationScale = scale;
        int num2 = 1;
        layer.ShouldRasterize = num2 != 0;
      }

      public override void LayoutSubviews()
      {
        // ISSUE: reference to a compiler-generated method
        base.LayoutSubviews();
        UIButton uiButton = this.image;
        nfloat x1 = (nfloat) 0;
        nfloat y1 = (nfloat) 0;
        CGRect frame1 = this.ContentView.Frame;
        nfloat width1 = frame1.Width;
        frame1 = this.ContentView.Frame;
        nfloat height1 = frame1.Height - (nfloat) 30;
        CGRect cgRect1 = new CGRect(x1, y1, width1, height1);
        uiButton.Frame = cgRect1;
        // ISSUE: reference to a compiler-generated method
        CGSize cgSize = this.nameLabel.SizeThatFits(this.ContentView.Frame.Size);
        UILabel uiLabel = this.nameLabel;
        nfloat x2 = (nfloat) 0;
        CGRect frame2 = this.ContentView.Frame;
        nfloat y2 = frame2.Height - (nfloat) 15 - cgSize.Height / (nfloat) 2;
        frame2 = this.ContentView.Frame;
        nfloat width2 = frame2.Width;
        nfloat height2 = cgSize.Height;
        CGRect cgRect2 = new CGRect(x2, y2, width2, height2);
        uiLabel.Frame = cgRect2;
      }
    }

    private class DataSource : UICollectionViewDataSource
    {
      private readonly NavigationMenu menu;

      public DataSource(NavigationMenu menu)
      {
        this.menu = menu;
      }

      public override nint GetItemsCount(UICollectionView collectionView, nint section)
      {
        return (nint) Enumerable.Count<Page>(this.menu.get_Targets());
      }

      public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
      {
        // ISSUE: reference to a compiler-generated method
        NavigationMenuRenderer.NavigationCell navigationCell = (NavigationMenuRenderer.NavigationCell) collectionView.DequeueReusableCell(new NSString("NavigationCell"), indexPath);
        Page target = Enumerable.FirstOrDefault<Page>(Enumerable.Skip<Page>(this.menu.get_Targets(), indexPath.Row));
        if (target != null)
        {
          navigationCell.Name = target.Title;
          navigationCell.Icon = (string) target.Icon;
          navigationCell.Selected = (Action) (() => this.menu.SendTargetSelected(target));
        }
        else
        {
          navigationCell.Selected = (Action) null;
          navigationCell.Icon = "";
          navigationCell.Name = "";
        }
        return (UICollectionViewCell) navigationCell;
      }
    }
  }
}
