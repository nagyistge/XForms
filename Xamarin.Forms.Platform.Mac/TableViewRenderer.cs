using CoreGraphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class TableViewRenderer : ViewRenderer<TableView, UITableView>
  {
    private UIView originalBackgroundView;
    private KeyboardInsetTracker insetTracker;
    private CGRect previousFrame;

    protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
    {
      if (e.NewElement != null)
      {
        UITableViewStyle style = UITableViewStyle.Plain;
        if (e.NewElement.Intent != TableIntent.Data)
          style = UITableViewStyle.Grouped;
        if (this.Control == null || this.Control.Style != style)
        {
          if (this.Control != null)
          {
            this.insetTracker.Dispose();
            this.Control.Dispose();
          }
          UITableView uiview = new UITableView(CGRect.Empty, style);
          this.originalBackgroundView = uiview.BackgroundView;
          this.SetNativeControl(uiview);
          this.insetTracker = new KeyboardInsetTracker((UIView) uiview, (Func<UIWindow>) (() => this.Control.Window), (Action<UIEdgeInsets>) (insets => this.Control.ContentInset = this.Control.ScrollIndicatorInsets = insets), (Action<CGPoint>) (point =>
          {
            CGPoint contentOffset = this.Control.ContentOffset;
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            CGPoint& local = @contentOffset;
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            (^local).Y = (^local).Y + point.Y;
            // ISSUE: reference to a compiler-generated method
            this.Control.SetContentOffset(contentOffset, true);
          }));
        }
        this.SetSource();
        this.UpdateRowHeight();
        this.UpdateBackgroundView();
      }
      this.OnElementChanged(e);
    }

    public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest((UIView) this.Control, widthConstraint, heightConstraint, 44.0, 44.0);
    }

    public override void LayoutSubviews()
    {
      base.LayoutSubviews();
      if (!(this.previousFrame != this.Frame))
        return;
      this.previousFrame = this.Frame;
      KeyboardInsetTracker keyboardInsetTracker = this.insetTracker;
      if (keyboardInsetTracker == null)
        return;
      keyboardInsetTracker.UpdateInsets();
    }

    private void SetSource()
    {
      TableView element = this.Element;
      this.Control.Source = element.HasUnevenRows ? (UITableViewSource) new UnEvenTableViewModelRenderer(element) : (UITableViewSource) new TableViewModelRenderer(element);
    }

    private void UpdateRowHeight()
    {
      int num = this.Element.RowHeight;
      if (num <= 0)
        num = 44;
      this.Control.RowHeight = (nfloat) num;
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == "RowHeight")
        this.UpdateRowHeight();
      else if (e.PropertyName == "HasUnevenRows")
      {
        this.SetSource();
      }
      else
      {
        if (!(e.PropertyName == "BackgroundColor"))
          return;
        this.UpdateBackgroundView();
      }
    }

    private void UpdateBackgroundView()
    {
      this.Control.BackgroundView = this.Element.BackgroundColor == Color.Default ? this.originalBackgroundView : (UIView) null;
    }

    protected override void UpdateNativeWidget()
    {
      if (this.Element.Opacity < 1.0)
      {
        if (!this.Control.Layer.ShouldRasterize)
        {
          this.Control.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
          this.Control.Layer.ShouldRasterize = true;
        }
      }
      else
        this.Control.Layer.ShouldRasterize = false;
      base.UpdateNativeWidget();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.insetTracker != null)
      {
        this.insetTracker.Dispose();
        this.insetTracker = (KeyboardInsetTracker) null;
        Stack<UIView> stack = new Stack<UIView>((IEnumerable<UIView>) this.Subviews);
        while (stack.Count > 0)
        {
          UIView uiView1 = stack.Pop();
          ViewCellRenderer.ViewTableCell viewTableCell = uiView1 as ViewCellRenderer.ViewTableCell;
          if (viewTableCell != null)
          {
            viewTableCell.Dispose();
          }
          else
          {
            foreach (UIView uiView2 in uiView1.Subviews)
              stack.Push(uiView2);
          }
        }
      }
      base.Dispose(disposing);
    }
  }
}
