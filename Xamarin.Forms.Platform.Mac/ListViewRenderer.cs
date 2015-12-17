using CoreGraphics;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ListViewRenderer : ViewRenderer<ListView, UITableView>
  {
    private bool shouldEstimateRowHeight = true;
    private KeyboardInsetTracker insetTracker;
    private ListViewRenderer.ListViewDataSource dataSource;
    private ScrollToRequestedEventArgs requestedScroll;
    private IVisualElementRenderer headerRenderer;
    private IVisualElementRenderer footerRenderer;
    private bool estimatedRowHeight;
    private FormsUITableViewController tableViewController;
    private CGRect previousFrame;
    private const int DefaultRowHeight = 44;

    public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest((UIView) this.Control, widthConstraint, heightConstraint, 44.0, 44.0);
    }

    public override void LayoutSubviews()
    {
      base.LayoutSubviews();
      double num = (double) this.Bounds.Height;
      double width = (double) this.Bounds.Width;
      if (this.headerRenderer != null)
      {
        VisualElement element = this.headerRenderer.Element;
        double widthConstraint = width;
        double heightConstraint = double.PositiveInfinity;
        SizeRequest sizeRequest = element.GetSizeRequest(widthConstraint, heightConstraint);
        Rectangle bounds = new Rectangle(0.0, 0.0, width, Math.Ceiling(sizeRequest.Request.Height));
        element.Layout(bounds);
        Device.BeginInvokeOnMainThread((Action) (() =>
        {
          if (this.headerRenderer == null)
            return;
          this.Control.TableHeaderView = this.headerRenderer.NativeView;
        }));
      }
      if (this.footerRenderer != null)
      {
        VisualElement element = this.footerRenderer.Element;
        double widthConstraint = width;
        double heightConstraint = num;
        SizeRequest sizeRequest = element.GetSizeRequest(widthConstraint, heightConstraint);
        Rectangle bounds = new Rectangle(0.0, 0.0, width, Math.Ceiling(sizeRequest.Request.Height));
        element.Layout(bounds);
        Device.BeginInvokeOnMainThread((Action) (() =>
        {
          if (this.footerRenderer == null)
            return;
          this.Control.TableFooterView = this.footerRenderer.NativeView;
        }));
      }
      if (this.requestedScroll != null && this.Superview != null)
      {
        ScrollToRequestedEventArgs e = this.requestedScroll;
        this.requestedScroll = (ScrollToRequestedEventArgs) null;
        this.OnScrollToRequested((object) this, e);
      }
      if (!(this.previousFrame != this.Frame))
        return;
      this.previousFrame = this.Frame;
      KeyboardInsetTracker keyboardInsetTracker = this.insetTracker;
      if (keyboardInsetTracker == null)
        return;
      keyboardInsetTracker.UpdateInsets();
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
        if (this.Element != null)
        {
          this.Element.TemplatedItems.remove_CollectionChanged(new NotifyCollectionChangedEventHandler(this.OnCollectionChanged));
          this.Element.TemplatedItems.remove_GroupedCollectionChanged(new NotifyCollectionChangedEventHandler(this.OnGroupedCollectionChanged));
        }
        if (this.tableViewController != null)
        {
          this.tableViewController.Dispose();
          this.tableViewController = (FormsUITableViewController) null;
        }
      }
      if (disposing)
      {
        if (this.headerRenderer != null)
        {
          Platform platform = this.headerRenderer.Element.Platform as Platform;
          if (platform != null)
            platform.DisposeModelAndChildrenRenderers((Element) this.headerRenderer.Element);
          this.headerRenderer = (IVisualElementRenderer) null;
        }
        if (this.footerRenderer != null)
        {
          Platform platform = this.footerRenderer.Element.Platform as Platform;
          if (platform != null)
            platform.DisposeModelAndChildrenRenderers((Element) this.footerRenderer.Element);
          this.footerRenderer = (IVisualElementRenderer) null;
        }
      }
      base.Dispose(disposing);
    }

    protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
    {
      this.requestedScroll = (ScrollToRequestedEventArgs) null;
      if (e.OldElement != null)
      {
        ListView oldElement = e.OldElement;
        VisualElement visualElement1 = (VisualElement) ((IListViewController) oldElement).HeaderElement;
        if (visualElement1 != null)
          visualElement1.remove_MeasureInvalidated(new EventHandler(this.OnHeaderMeasureInvalidated));
        VisualElement visualElement2 = (VisualElement) ((IListViewController) oldElement).FooterElement;
        if (visualElement2 != null)
          visualElement2.remove_MeasureInvalidated(new EventHandler(this.OnFooterMeasureInvalidated));
        e.OldElement.remove_ScrollToRequested(new EventHandler<ScrollToRequestedEventArgs>(this.OnScrollToRequested));
        e.OldElement.TemplatedItems.remove_CollectionChanged(new NotifyCollectionChangedEventHandler(this.OnCollectionChanged));
        e.OldElement.TemplatedItems.remove_GroupedCollectionChanged(new NotifyCollectionChangedEventHandler(this.OnGroupedCollectionChanged));
      }
      if (e.NewElement != null)
      {
        if (this.Control == null)
        {
          this.tableViewController = new FormsUITableViewController(e.NewElement);
          this.SetNativeControl(this.tableViewController.TableView);
          this.insetTracker = new KeyboardInsetTracker((UIView) this.tableViewController.TableView, (Func<UIWindow>) (() => this.Control.Window), (Action<UIEdgeInsets>) (insets => this.Control.ContentInset = this.Control.ScrollIndicatorInsets = insets), (Action<CGPoint>) (point =>
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
        this.shouldEstimateRowHeight = true;
        if (e.NewElement.TakePerformanceHit)
          this.shouldEstimateRowHeight = false;
        e.NewElement.add_ScrollToRequested(new EventHandler<ScrollToRequestedEventArgs>(this.OnScrollToRequested));
        e.NewElement.TemplatedItems.add_CollectionChanged(new NotifyCollectionChangedEventHandler(this.OnCollectionChanged));
        e.NewElement.TemplatedItems.add_GroupedCollectionChanged(new NotifyCollectionChangedEventHandler(this.OnGroupedCollectionChanged));
        this.UpdateRowHeight();
        this.Control.Source = (UITableViewSource) (this.dataSource = e.NewElement.HasUnevenRows ? (ListViewRenderer.ListViewDataSource) new ListViewRenderer.UnevenListViewDataSource(e.NewElement, this.tableViewController) : new ListViewRenderer.ListViewDataSource(e.NewElement, this.tableViewController));
        this.UpdateEstimatedRowHeight();
        this.UpdateHeader();
        this.UpdateFooter();
        this.UpdatePullToRefreshEnabled();
        this.UpdateIsRefreshing();
        this.UpdateSeparatorColor();
        this.UpdateSeparatorVisibility();
        object selectedItem = e.NewElement.SelectedItem;
        if (selectedItem != null)
          this.dataSource.OnItemSelected((object) null, new SelectedItemChangedEventArgs(selectedItem));
      }
      this.OnElementChanged(e);
    }

    private void UpdateSeparatorColor()
    {
      this.Control.SeparatorColor = ColorExtensions.ToUIColor(this.Element.SeparatorColor, UIColor.Gray);
    }

    private void UpdateSeparatorVisibility()
    {
      switch (this.Element.SeparatorVisibility)
      {
        case SeparatorVisibility.Default:
          this.Control.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
          break;
        case SeparatorVisibility.None:
          this.Control.SeparatorStyle = UITableViewCellSeparatorStyle.None;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void UpdateHeader()
    {
      Element headerElement = ((IListViewController) this.Element).HeaderElement;
      View view = (View) headerElement;
      if (view != null)
      {
        if (this.headerRenderer != null)
        {
          this.headerRenderer.Element.remove_MeasureInvalidated(new EventHandler(this.OnHeaderMeasureInvalidated));
          if (headerElement != null && this.headerRenderer.GetType() == Registrar.Registered.GetHandlerType(((object) headerElement).GetType()))
          {
            this.headerRenderer.SetElement((VisualElement) view);
            return;
          }
          this.Control.TableHeaderView = (UIView) null;
          Platform platform = this.headerRenderer.Element.Platform as Platform;
          if (platform != null)
            platform.DisposeModelAndChildrenRenderers((Element) this.headerRenderer.Element);
          this.headerRenderer = (IVisualElementRenderer) null;
        }
        this.headerRenderer = Platform.CreateRenderer((VisualElement) view);
        Platform.SetRenderer((VisualElement) view, this.headerRenderer);
        this.Control.TableHeaderView = this.headerRenderer.NativeView;
        view.add_MeasureInvalidated(new EventHandler(this.OnHeaderMeasureInvalidated));
      }
      else
      {
        if (this.headerRenderer == null)
          return;
        this.Control.TableHeaderView = (UIView) null;
        Platform platform = this.headerRenderer.Element.Platform as Platform;
        if (platform != null)
          platform.DisposeModelAndChildrenRenderers((Element) this.headerRenderer.Element);
        this.headerRenderer.Dispose();
        this.headerRenderer = (IVisualElementRenderer) null;
      }
    }

    private void OnHeaderMeasureInvalidated(object sender, EventArgs eventArgs)
    {
      double width = (double) this.Bounds.Width;
      if (width == 0.0)
        return;
      VisualElement visualElement = (VisualElement) sender;
      double widthConstraint = width;
      double heightConstraint = double.PositiveInfinity;
      SizeRequest sizeRequest = visualElement.GetSizeRequest(widthConstraint, heightConstraint);
      Rectangle bounds = new Rectangle(0.0, 0.0, width, sizeRequest.Request.Height);
      visualElement.Layout(bounds);
      this.Control.TableHeaderView = this.headerRenderer.NativeView;
    }

    private void UpdateFooter()
    {
      Element footerElement = ((IListViewController) this.Element).FooterElement;
      View view = (View) footerElement;
      if (view != null)
      {
        if (this.footerRenderer != null)
        {
          this.footerRenderer.Element.remove_MeasureInvalidated(new EventHandler(this.OnFooterMeasureInvalidated));
          if (footerElement != null && this.footerRenderer.GetType() == Registrar.Registered.GetHandlerType(((object) footerElement).GetType()))
          {
            this.footerRenderer.SetElement((VisualElement) view);
            return;
          }
          this.Control.TableFooterView = (UIView) null;
          Platform platform = this.footerRenderer.Element.Platform as Platform;
          if (platform != null)
            platform.DisposeModelAndChildrenRenderers((Element) this.footerRenderer.Element);
          this.footerRenderer.Dispose();
          this.footerRenderer = (IVisualElementRenderer) null;
        }
        this.footerRenderer = Platform.CreateRenderer((VisualElement) view);
        Platform.SetRenderer((VisualElement) view, this.footerRenderer);
        this.Control.TableFooterView = this.footerRenderer.NativeView;
        view.add_MeasureInvalidated(new EventHandler(this.OnFooterMeasureInvalidated));
      }
      else
      {
        if (this.footerRenderer == null)
          return;
        this.Control.TableFooterView = (UIView) null;
        Platform platform = this.footerRenderer.Element.Platform as Platform;
        if (platform != null)
          platform.DisposeModelAndChildrenRenderers((Element) this.footerRenderer.Element);
        this.footerRenderer.Dispose();
        this.footerRenderer = (IVisualElementRenderer) null;
      }
    }

    private void OnFooterMeasureInvalidated(object sender, EventArgs eventArgs)
    {
      double width = (double) this.Bounds.Width;
      if (width == 0.0)
        return;
      VisualElement visualElement = (VisualElement) sender;
      double widthConstraint = width;
      double heightConstraint = double.PositiveInfinity;
      SizeRequest sizeRequest = visualElement.GetSizeRequest(widthConstraint, heightConstraint);
      Rectangle bounds = new Rectangle(0.0, 0.0, width, sizeRequest.Request.Height);
      visualElement.Layout(bounds);
      this.Control.TableFooterView = this.footerRenderer.NativeView;
    }

    private UITableViewScrollPosition GetScrollPosition(ScrollToPosition position)
    {
      switch (position)
      {
        case ScrollToPosition.Start:
          return UITableViewScrollPosition.Top;
        case ScrollToPosition.Center:
          return UITableViewScrollPosition.Middle;
        case ScrollToPosition.End:
          return UITableViewScrollPosition.Bottom;
        default:
          return UITableViewScrollPosition.None;
      }
    }

    private void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
    {
      if (this.Superview == null)
      {
        this.requestedScroll = e;
      }
      else
      {
        UITableViewScrollPosition scrollPosition = this.GetScrollPosition(e.Position);
        if (this.Element.IsGroupingEnabled)
        {
          Tuple<int, int> groupAndIndexOfItem = this.Element.TemplatedItems.GetGroupAndIndexOfItem(e.Group, e.Item);
          if (groupAndIndexOfItem.Item1 == -1 || groupAndIndexOfItem.Item2 == -1)
            return;
          // ISSUE: reference to a compiler-generated method
          // ISSUE: reference to a compiler-generated method
          this.Control.ScrollToRow(NSIndexPath.FromRowSection((nint) groupAndIndexOfItem.Item2, (nint) groupAndIndexOfItem.Item1), scrollPosition, e.ShouldAnimate);
        }
        else
        {
          int globalIndexOfItem = this.Element.TemplatedItems.GetGlobalIndexOfItem(e.Item);
          if (globalIndexOfItem == -1)
            return;
          // ISSUE: reference to a compiler-generated method
          // ISSUE: reference to a compiler-generated method
          this.Control.ScrollToRow(NSIndexPath.FromRowSection((nint) globalIndexOfItem, (nint) 0), scrollPosition, e.ShouldAnimate);
        }
      }
    }

    private void OnGroupedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      int section = this.Element.TemplatedItems.IndexOf(((TemplatedItemsList<ItemsView<Cell>, Cell>) sender).HeaderContent);
      this.UpdateItems(e, section, false);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UpdateItems(e, 0, true);
    }

    private void UpdateItems(NotifyCollectionChangedEventArgs e, int section, bool resetWhenGrouped)
    {
      NotifyCollectionChangedEventArgsEx changedEventArgsEx = e as NotifyCollectionChangedEventArgsEx;
      if (changedEventArgsEx != null)
        this.dataSource.Counts[section] = changedEventArgsEx.Count;
      bool flag = resetWhenGrouped && this.Element.IsGroupingEnabled;
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          this.UpdateEstimatedRowHeight();
          if (!(e.NewStartingIndex == -1 | flag))
          {
            // ISSUE: reference to a compiler-generated method
            this.Control.BeginUpdates();
            // ISSUE: reference to a compiler-generated method
            this.Control.InsertRows(this.GetPaths(section, e.NewStartingIndex, e.NewItems.Count), UITableViewRowAnimation.Automatic);
            // ISSUE: reference to a compiler-generated method
            this.Control.EndUpdates();
            break;
          }
          goto case NotifyCollectionChangedAction.Reset;
        case NotifyCollectionChangedAction.Remove:
          if (!(e.OldStartingIndex == -1 | flag))
          {
            // ISSUE: reference to a compiler-generated method
            this.Control.BeginUpdates();
            // ISSUE: reference to a compiler-generated method
            this.Control.DeleteRows(this.GetPaths(section, e.OldStartingIndex, e.OldItems.Count), UITableViewRowAnimation.Automatic);
            // ISSUE: reference to a compiler-generated method
            this.Control.EndUpdates();
            if (!this.estimatedRowHeight || this.Element.TemplatedItems.Count != 0)
              break;
            this.estimatedRowHeight = false;
            break;
          }
          goto case NotifyCollectionChangedAction.Reset;
        case NotifyCollectionChangedAction.Replace:
          if (!(e.OldStartingIndex == -1 | flag))
          {
            // ISSUE: reference to a compiler-generated method
            this.Control.BeginUpdates();
            // ISSUE: reference to a compiler-generated method
            this.Control.ReloadRows(this.GetPaths(section, e.OldStartingIndex, e.OldItems.Count), UITableViewRowAnimation.Automatic);
            // ISSUE: reference to a compiler-generated method
            this.Control.EndUpdates();
            if (!this.estimatedRowHeight || e.OldStartingIndex != 0)
              break;
            this.estimatedRowHeight = false;
            break;
          }
          goto case NotifyCollectionChangedAction.Reset;
        case NotifyCollectionChangedAction.Move:
          if (((e.OldStartingIndex == -1 ? 1 : (e.NewStartingIndex == -1 ? 1 : 0)) | (flag ? 1 : 0)) == 0)
          {
            // ISSUE: reference to a compiler-generated method
            this.Control.BeginUpdates();
            for (int index = 0; index < e.OldItems.Count; ++index)
            {
              int oldStartingIndex = e.OldStartingIndex;
              int newStartingIndex = e.NewStartingIndex;
              if (e.NewStartingIndex < e.OldStartingIndex)
              {
                oldStartingIndex += index;
                newStartingIndex += index;
              }
              // ISSUE: reference to a compiler-generated method
              // ISSUE: reference to a compiler-generated method
              // ISSUE: reference to a compiler-generated method
              this.Control.MoveRow(NSIndexPath.FromRowSection((nint) oldStartingIndex, (nint) section), NSIndexPath.FromRowSection((nint) newStartingIndex, (nint) section));
            }
            // ISSUE: reference to a compiler-generated method
            this.Control.EndUpdates();
            if (!this.estimatedRowHeight || e.OldStartingIndex != 0)
              break;
            this.estimatedRowHeight = false;
            break;
          }
          goto case NotifyCollectionChangedAction.Reset;
        case NotifyCollectionChangedAction.Reset:
          this.estimatedRowHeight = false;
          // ISSUE: reference to a compiler-generated method
          this.Control.ReloadData();
          break;
      }
    }

    private NSIndexPath[] GetPaths(int section, int index, int count)
    {
      NSIndexPath[] nsIndexPathArray = new NSIndexPath[count];
      for (int index1 = 0; index1 < nsIndexPathArray.Length; ++index1)
      {
        // ISSUE: reference to a compiler-generated method
        nsIndexPathArray[index1] = NSIndexPath.FromRowSection((nint) (index + index1), (nint) section);
      }
      return nsIndexPathArray;
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == ListView.RowHeightProperty.PropertyName)
        this.UpdateRowHeight();
      else if (e.PropertyName == ListView.IsGroupingEnabledProperty.PropertyName)
        this.dataSource.UpdateGrouping();
      else if (e.PropertyName == ListView.HasUnevenRowsProperty.PropertyName)
      {
        this.estimatedRowHeight = false;
        this.Control.Source = (UITableViewSource) (this.dataSource = this.Element.HasUnevenRows ? (ListViewRenderer.ListViewDataSource) new ListViewRenderer.UnevenListViewDataSource(this.dataSource) : new ListViewRenderer.ListViewDataSource(this.dataSource));
      }
      else if (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName)
        this.UpdatePullToRefreshEnabled();
      else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
        this.UpdateIsRefreshing();
      else if (e.PropertyName == ListView.SeparatorColorProperty.PropertyName)
        this.UpdateSeparatorColor();
      else if (e.PropertyName == ListView.SeparatorVisibilityProperty.PropertyName)
        this.UpdateSeparatorVisibility();
      else if (e.PropertyName == "HeaderElement")
        this.UpdateHeader();
      else if (e.PropertyName == "FooterElement")
      {
        this.UpdateFooter();
      }
      else
      {
        if (!(e.PropertyName == "RefreshAllowed"))
          return;
        this.UpdatePullToRefreshEnabled();
      }
    }

    private void UpdateIsRefreshing()
    {
      bool isRefreshing = this.Element.IsRefreshing;
      if (this.tableViewController == null)
        return;
      this.tableViewController.UpdateIsRefreshing(isRefreshing);
    }

    private void UpdatePullToRefreshEnabled()
    {
      if (this.tableViewController == null)
        return;
      this.tableViewController.UpdatePullToRefreshEnabled(this.Element.IsPullToRefreshEnabled && ((IListViewController) this.Element).RefreshAllowed);
    }

    private void UpdateRowHeight()
    {
      int rowHeight = this.Element.RowHeight;
      if (this.Element.HasUnevenRows && rowHeight == -1 && Forms.IsiOS7OrNewer)
      {
        if (!Forms.IsiOS8OrNewer)
          return;
        this.Control.RowHeight = UITableView.AutomaticDimension;
      }
      else
        this.Control.RowHeight = (nfloat) (rowHeight <= 0 ? 44 : rowHeight);
    }

    private void UpdateEstimatedRowHeight()
    {
      if (this.estimatedRowHeight)
        return;
      if (this.Element.HasUnevenRows && this.Element.RowHeight == -1)
      {
        ListViewRenderer.UnevenListViewDataSource listViewDataSource = this.dataSource as ListViewRenderer.UnevenListViewDataSource;
        if (!this.shouldEstimateRowHeight)
          return;
        if (this.Element.TemplatedItems.Count > 0 && listViewDataSource != null)
        {
          this.Control.EstimatedRowHeight = listViewDataSource.CalculateHeightForCell(this.Control, Enumerable.First<Cell>((IEnumerable<Cell>) this.Element.TemplatedItems));
          this.estimatedRowHeight = true;
        }
        else
          this.Control.EstimatedRowHeight = (nfloat) 44;
      }
      else
      {
        if (Forms.IsiOS7OrNewer)
          this.Control.EstimatedRowHeight = (nfloat) 0;
        this.estimatedRowHeight = true;
      }
    }

    internal class UnevenListViewDataSource : ListViewRenderer.ListViewDataSource
    {
      private IVisualElementRenderer prototype;

      public UnevenListViewDataSource(ListView list, FormsUITableViewController uiTableViewController)
        : base(list, uiTableViewController)
      {
      }

      public UnevenListViewDataSource(ListViewRenderer.ListViewDataSource source)
        : base(source)
      {
      }

      public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
      {
        Cell cellForPath = this.GetCellForPath(indexPath);
        if (this.list.RowHeight == -1 && cellForPath.Height == -1.0 && cellForPath is ViewCell)
        {
          if (Forms.IsiOS8OrNewer)
            return UITableView.AutomaticDimension;
          return this.CalculateHeightForCell(tableView, cellForPath);
        }
        double renderHeight = cellForPath.RenderHeight;
        if (renderHeight <= 0.0)
          return (nfloat) 44;
        return (nfloat) renderHeight;
      }

      internal nfloat CalculateHeightForCell(UITableView tableView, Cell cell)
      {
        ViewCell viewCell = cell as ViewCell;
        if (viewCell != null && viewCell.View != null)
        {
          View view = viewCell.View;
          if (this.prototype == null)
          {
            this.prototype = Platform.CreateRenderer((VisualElement) view);
            Platform.SetRenderer((VisualElement) view, this.prototype);
          }
          else
          {
            this.prototype.SetElement((VisualElement) view);
            Platform.SetRenderer((VisualElement) view, this.prototype);
          }
          SizeRequest sizeRequest = view.GetSizeRequest((double) tableView.Frame.Width, double.PositiveInfinity);
          view.ClearValue(Platform.RendererProperty);
          foreach (BindableObject bindableObject in view.Descendants())
            bindableObject.ClearValue(Platform.RendererProperty);
          return (nfloat) sizeRequest.Request.Height;
        }
        double renderHeight = cell.RenderHeight;
        if (renderHeight <= 0.0)
          return (nfloat) 44;
        return (nfloat) renderHeight;
      }
    }

    internal class ListViewDataSource : UITableViewSource
    {
      protected readonly ListView list;
      private readonly UITableView uiTableView;
      private readonly FormsUITableViewController uiTableViewController;
      private readonly nfloat defaultSectionHeight;
      private bool selectionFromNative;
      private bool isDragging;

      public Dictionary<int, int> Counts { get; set; }

      private UIColor DefaultBackgroundColor
      {
        get
        {
          return UIColor.Clear;
        }
      }

      public ListViewDataSource(ListViewRenderer.ListViewDataSource source)
      {
        this.uiTableViewController = source.uiTableViewController;
        this.list = source.list;
        this.uiTableView = source.uiTableView;
        this.defaultSectionHeight = source.defaultSectionHeight;
        this.selectionFromNative = source.selectionFromNative;
        this.Counts = new Dictionary<int, int>();
      }

      public ListViewDataSource(ListView list, FormsUITableViewController uiTableViewController)
      {
        this.uiTableViewController = uiTableViewController;
        this.uiTableView = uiTableViewController.TableView;
        this.defaultSectionHeight = Forms.IsiOS8OrNewer ? (nfloat) 44 : this.uiTableView.SectionHeaderHeight;
        this.list = list;
        this.list.add_ItemSelected(new EventHandler<SelectedItemChangedEventArgs>(this.OnItemSelected));
        this.UpdateShortNameListener();
        this.Counts = new Dictionary<int, int>();
      }

      public override void DraggingStarted(UIScrollView scrollView)
      {
        this.isDragging = true;
      }

      public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
      {
        this.isDragging = false;
        this.uiTableViewController.UpdateShowHideRefresh(false);
      }

      public override void Scrolled(UIScrollView scrollView)
      {
        if (!this.isDragging || !(scrollView.ContentOffset.Y < (nfloat) 0))
          return;
        this.uiTableViewController.UpdateShowHideRefresh(true);
      }

      public void UpdateGrouping()
      {
        this.UpdateShortNameListener();
        // ISSUE: reference to a compiler-generated method
        this.uiTableView.ReloadData();
      }

      public override nint NumberOfSections(UITableView tableView)
      {
        if (this.list.IsGroupingEnabled)
          return (nint) this.list.TemplatedItems.Count;
        return (nint) 1;
      }

      public override nint RowsInSection(UITableView tableview, nint section)
      {
        int num;
        if (this.Counts.TryGetValue((int) section, out num))
        {
          this.Counts.Remove((int) section);
          return (nint) num;
        }
        if (this.list.IsGroupingEnabled)
          return (nint) ((ICollection) ((IList) this.list.TemplatedItems)[(int) section]).Count;
        return (nint) this.list.TemplatedItems.Count;
      }

      public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
      {
        UITableViewCell cell1;
        switch (this.list.CachingStrategy)
        {
          case ListViewCachingStrategy.RetainElement:
            Cell cellForPath1 = this.GetCellForPath(indexPath);
            cell1 = CellTableViewCell.GetNativeCell(tableView, cellForPath1, false);
            break;
          case ListViewCachingStrategy.RecycleElement:
            // ISSUE: reference to a compiler-generated method
            cell1 = tableView.DequeueReusableCell("ContextActionsCell");
            if (cell1 == null)
            {
              Cell cellForPath2 = this.GetCellForPath(indexPath);
              cell1 = CellTableViewCell.GetNativeCell(tableView, cellForPath2, true);
              break;
            }
            TemplatedItemsList<ItemsView<Cell>, Cell> group = this.list.TemplatedItems.GetGroup(indexPath.Section);
            Cell cell2 = (Cell) ((INativeElementView) cell1).Element;
            cell2.SendDisappearing();
            Cell content = cell2;
            int row = indexPath.Row;
            group.UpdateContent(content, row);
            cell2.SendAppearing();
            break;
          default:
            throw new NotSupportedException();
        }
        UIColor color = tableView.IndexPathForSelectedRow == null || !tableView.IndexPathForSelectedRow.Equals((NSObject) indexPath) ? this.DefaultBackgroundColor : UIColor.Clear;
        ListViewRenderer.ListViewDataSource.SetCellBackgroundColor(cell1, color);
        return cell1;
      }

      public override nfloat GetHeightForHeader(UITableView tableView, nint section)
      {
        if (!this.list.IsGroupingEnabled)
          return (nfloat) 0;
        nfloat nfloat = (nfloat) ((float) this.list.TemplatedItems[(int) section].RenderHeight);
        if (nfloat == (nfloat) -1)
          nfloat = this.defaultSectionHeight;
        return nfloat;
      }

      public override UIView GetViewForHeader(UITableView tableView, nint section)
      {
        if (!this.list.IsGroupingEnabled || this.list.GroupHeaderTemplate == null)
          return (UIView) null;
        Cell cell1 = this.list.TemplatedItems[(int) section];
        if (cell1.HasContextActions)
          throw new NotSupportedException("Header cells do not support context actions");
        CellRenderer cellRenderer = (CellRenderer) Registrar.Registered.GetHandler(((object) cell1).GetType());
        HeaderWrapperView headerWrapperView = new HeaderWrapperView();
        UITableViewCell cell2 = cellRenderer.GetCell(cell1, (UITableViewCell) null, tableView);
        // ISSUE: reference to a compiler-generated method
        headerWrapperView.AddSubview((UIView) cell2);
        return (UIView) headerWrapperView;
      }

      public override string TitleForHeader(UITableView tableView, nint section)
      {
        if (!this.list.IsGroupingEnabled)
          return (string) null;
        TemplatedItemsList<ItemsView<Cell>, Cell> sectionList = this.GetSectionList((int) section);
        PropertyChangedEventHandler changedEventHandler1 = new PropertyChangedEventHandler(this.OnSectionPropertyChanged);
        sectionList.remove_PropertyChanged(changedEventHandler1);
        PropertyChangedEventHandler changedEventHandler2 = new PropertyChangedEventHandler(this.OnSectionPropertyChanged);
        sectionList.add_PropertyChanged(changedEventHandler2);
        return sectionList.Name;
      }

      private void OnSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        NSIndexPath pathForSelectedRow = this.uiTableView.IndexPathForSelectedRow;
        TemplatedItemsList<ItemsView<Cell>, Cell> templatedItemsList = (TemplatedItemsList<ItemsView<Cell>, Cell>) sender;
        int num = ((IList) this.list.TemplatedItems).IndexOf((object) templatedItemsList);
        if (num == -1)
        {
          templatedItemsList.remove_PropertyChanged(new PropertyChangedEventHandler(this.OnSectionPropertyChanged));
        }
        else
        {
          // ISSUE: reference to a compiler-generated method
          // ISSUE: reference to a compiler-generated method
          this.uiTableView.ReloadSections(NSIndexSet.FromIndex((nint) num), UITableViewRowAnimation.Automatic);
          // ISSUE: reference to a compiler-generated method
          this.uiTableView.SelectRow(pathForSelectedRow, false, UITableViewScrollPosition.None);
        }
      }

      public override string[] SectionIndexTitles(UITableView tableView)
      {
        if (this.list.TemplatedItems.get_ShortNames() == null)
          return (string[]) null;
        return Enumerable.ToArray<string>((IEnumerable<string>) this.list.TemplatedItems.get_ShortNames());
      }

      public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
      {
        // ISSUE: reference to a compiler-generated method
        UITableViewCell cell1 = tableView.CellAt(indexPath);
        if (cell1 == null)
          return;
        Cell cell2 = (Cell) null;
        if (this.list.CachingStrategy == ListViewCachingStrategy.RecycleElement)
          cell2 = (Cell) ((INativeElementView) cell1).Element;
        ListViewRenderer.ListViewDataSource.SetCellBackgroundColor(cell1, UIColor.Clear);
        this.selectionFromNative = true;
        // ISSUE: reference to a compiler-generated method
        UIView_UITextField.EndEditing((UIView) tableView, true);
        this.list.NotifyRowTapped(indexPath.Section, indexPath.Row, cell2);
      }

      public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
      {
        // ISSUE: reference to a compiler-generated method
        UITableViewCell cell = tableView.CellAt(indexPath);
        if (cell == null)
          return;
        ListViewRenderer.ListViewDataSource.SetCellBackgroundColor(cell, this.DefaultBackgroundColor);
      }

      public void OnItemSelected(object sender, SelectedItemChangedEventArgs eventArg)
      {
        if (this.selectionFromNative)
        {
          this.selectionFromNative = false;
        }
        else
        {
          Tuple<int, int> groupAndIndexOfItem = this.list.TemplatedItems.GetGroupAndIndexOfItem(eventArg.SelectedItem);
          if (groupAndIndexOfItem.Item1 == -1 || groupAndIndexOfItem.Item2 == -1)
          {
            NSIndexPath pathForSelectedRow = this.uiTableView.IndexPathForSelectedRow;
            bool animated = true;
            if (pathForSelectedRow != null)
            {
              // ISSUE: reference to a compiler-generated method
              ContextActionsCell contextActionsCell = this.uiTableView.CellAt(pathForSelectedRow) as ContextActionsCell;
              if (contextActionsCell != null)
              {
                contextActionsCell.PrepareForDeselect();
                if (contextActionsCell.IsOpen)
                  animated = false;
              }
            }
            if (pathForSelectedRow == null)
              return;
            // ISSUE: reference to a compiler-generated method
            this.uiTableView.DeselectRow(pathForSelectedRow, animated);
          }
          else
          {
            // ISSUE: reference to a compiler-generated method
            // ISSUE: reference to a compiler-generated method
            this.uiTableView.SelectRow(NSIndexPath.FromRowSection((nint) groupAndIndexOfItem.Item2, (nint) groupAndIndexOfItem.Item1), true, UITableViewScrollPosition.Middle);
          }
        }
      }

      protected Cell GetCellForPath(NSIndexPath indexPath)
      {
        TemplatedItemsList<ItemsView<Cell>, Cell> templatedItemsList = this.list.TemplatedItems;
        if (this.list.IsGroupingEnabled)
          templatedItemsList = (TemplatedItemsList<ItemsView<Cell>, Cell>) ((IList) templatedItemsList)[indexPath.Section];
        return templatedItemsList[indexPath.Row];
      }

      private TemplatedItemsList<ItemsView<Cell>, Cell> GetSectionList(int section)
      {
        return (TemplatedItemsList<ItemsView<Cell>, Cell>) ((IList) this.list.TemplatedItems)[section];
      }

      private void OnShortNamesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // ISSUE: reference to a compiler-generated method
        this.uiTableView.ReloadSectionIndexTitles();
      }

      private void UpdateShortNameListener()
      {
        if (this.list.IsGroupingEnabled)
        {
          if (this.list.TemplatedItems.get_ShortNames() == null)
            return;
          ((INotifyCollectionChanged) this.list.TemplatedItems.get_ShortNames()).CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnShortNamesCollectionChanged);
        }
        else
        {
          if (this.list.TemplatedItems.get_ShortNames() == null)
            return;
          ((INotifyCollectionChanged) this.list.TemplatedItems.get_ShortNames()).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnShortNamesCollectionChanged);
        }
      }

      private static void SetCellBackgroundColor(UITableViewCell cell, UIColor color)
      {
        ContextActionsCell contextActionsCell = cell as ContextActionsCell;
        cell.BackgroundColor = color;
        if (contextActionsCell == null)
          return;
        contextActionsCell.ContentCell.BackgroundColor = color;
      }
    }
  }
}
