using CoreGraphics;
using System;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class FormsUITableViewController : UITableViewController
  {
    private bool refreshAdded;
    private UIRefreshControl refresh;
    private ListView list;

    public FormsUITableViewController(ListView element)
    {
      this.refresh = new UIRefreshControl();
      this.refresh.ValueChanged += new EventHandler(this.OnRefreshingChanged);
      this.list = element;
    }

    public override void ViewWillAppear(bool animated)
    {
    }

    public void UpdateIsRefreshing(bool refreshing)
    {
      if (refreshing)
      {
        if (!this.refreshAdded)
        {
          this.RefreshControl = this.refresh;
          this.refreshAdded = true;
        }
        if (this.refresh.Refreshing)
          return;
        // ISSUE: reference to a compiler-generated method
        this.refresh.BeginRefreshing();
        this.CheckContentSize();
        UITableView tableView = this.TableView;
        nfloat x = (nfloat) 0;
        nfloat y = (nfloat) 0;
        CGRect bounds = this.refresh.Bounds;
        nfloat width = bounds.Width;
        bounds = this.refresh.Bounds;
        nfloat height = bounds.Height;
        CGRect rect = new CGRect(x, y, width, height);
        int num = 1;
        // ISSUE: reference to a compiler-generated method
        tableView.ScrollRectToVisible(rect, num != 0);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        this.refresh.EndRefreshing();
        if (this.list.IsPullToRefreshEnabled)
          return;
        this.RemoveRefresh();
      }
    }

    public void UpdatePullToRefreshEnabled(bool pullToRefreshEnabled)
    {
      if (pullToRefreshEnabled)
      {
        if (this.refreshAdded)
          return;
        this.refreshAdded = true;
        this.RefreshControl = this.refresh;
      }
      else
      {
        if (!this.refreshAdded)
          return;
        if (this.refresh.Refreshing)
        {
          // ISSUE: reference to a compiler-generated method
          this.refresh.EndRefreshing();
        }
        this.RefreshControl = (UIRefreshControl) null;
        this.refreshAdded = false;
      }
    }

    public void UpdateShowHideRefresh(bool shouldHide)
    {
      if (this.list.IsPullToRefreshEnabled)
        return;
      if (shouldHide)
        this.RemoveRefresh();
      else
        this.UpdateIsRefreshing(this.list.IsRefreshing);
    }

    private void OnRefreshingChanged(object sender, EventArgs eventArgs)
    {
      if (!this.refresh.Refreshing)
        return;
      ((IListViewController) this.list).SendRefreshing();
    }

    private void RemoveRefresh()
    {
      if (!this.refreshAdded)
        return;
      if (this.refresh.Refreshing)
      {
        // ISSUE: reference to a compiler-generated method
        this.refresh.EndRefreshing();
      }
      this.RefreshControl = (UIRefreshControl) null;
      this.refreshAdded = false;
    }

    private void CheckContentSize()
    {
      CGSize contentSize = this.TableView.ContentSize;
      if (!(contentSize.Height == (nfloat) 0))
        return;
      this.TableView.ContentSize = new CGSize(contentSize.Width, (nfloat) 1);
    }

    protected override void Dispose(bool disposing)
    {
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
      if (!disposing || this.refresh == null)
        return;
      this.refresh.ValueChanged -= new EventHandler(this.OnRefreshingChanged);
      // ISSUE: reference to a compiler-generated method
      this.refresh.EndRefreshing();
      this.refresh.Dispose();
      this.refresh = (UIRefreshControl) null;
    }
  }
}
