using CoreGraphics;
using System;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	internal class FormsUITableViewController : NSTableViewDelegate
	{
		private bool refreshAdded;
		// TODO: Refresh control for Table View Controller
		//private NSRefreshControl refresh;
		private ListView list;
		public NSTableView TableView { get; private set; }

		public FormsUITableViewController (ListView element)
		{
			//this.refresh = new UIRefreshControl code();
			//this.refresh.ValueChanged += new EventHandler (this.OnRefreshingChanged);
			this.list = element;
		}

		public void UpdateIsRefreshing (bool refreshing)
		{
			if (refreshing)
			{
				/*
				if (!this.refreshAdded)
				{
					this.RefreshControl = this.refresh;
					this.refreshAdded = true;
				}
				if (this.refresh.Refreshing)
					return;
				this.refresh.BeginRefreshing ();
				*/

				this.CheckContentSize ();

				TableView = new NSTableView ();
				TableView.Delegate = this;

				/*
				nfloat x = (nfloat)0;
				nfloat y = (nfloat)0;
				CGRect bounds = this.refresh.Bounds;
				nfloat width = bounds.Width;
				bounds = this.refresh.Bounds;
				nfloat height = bounds.Height;
				CGRect rect = new CGRect (x, y, width, height);
				int num = 1;
				TableView.ScrollRectToVisible (rect, num != 0);
				*/

			}
			else
			{
				//this.refresh.EndRefreshing ();
				if (this.list.IsPullToRefreshEnabled)
					return;
				//this.RemoveRefresh ();
			}
		}

		/*
		public void UpdatePullToRefreshEnabled (bool pullToRefreshEnabled)
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
					this.refresh.EndRefreshing ();
				}
				this.RefreshControl = (UIRefreshControl)null;
				this.refreshAdded = false;
			}
		}


		public void UpdateShowHideRefresh (bool shouldHide)
		{
			if (this.list.IsPullToRefreshEnabled)
				return;
			if (shouldHide)
				this.RemoveRefresh ();
			else
				this.UpdateIsRefreshing (this.list.IsRefreshing);
		}

		private void OnRefreshingChanged (object sender, EventArgs eventArgs)
		{
			if (!this.refresh.Refreshing)
				return;
			((IListViewController)this.list).SendRefreshing ();
		}

		private void RemoveRefresh ()
		{
			if (!this.refreshAdded)
				return;
			if (this.refresh.Refreshing)
			{
				// ISSUE: reference to a compiler-generated method
				this.refresh.EndRefreshing ();
			}
			this.RefreshControl = (UIRefreshControl)null;
			this.refreshAdded = false;
		}
		*/


		private void CheckContentSize ()
		{
			CGSize contentSize = TableView.FittingSize;
			if (!(contentSize.Height == (nfloat)0))
				return;

			//TODO: Resize
			//this.TableView.ContentSize = new CGSize (contentSize.Width, (nfloat)1);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (!disposing)
				return;

			/*
			this.refresh.ValueChanged -= new EventHandler (this.OnRefreshingChanged);
			this.refresh.EndRefreshing ();
			this.refresh.Dispose ();
			this.refresh = (UIRefreshControl)null;
			*/
		}
	}
}
