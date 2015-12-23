using CoreGraphics;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class ListViewRenderer : ViewRenderer<ListView, NSTableView>
	{
		private bool shouldEstimateRowHeight = true;
		//private KeyboardInsetTracker insetTracker;
		private ListViewRenderer.ListViewDataSource dataSource;
		private ScrollToRequestedEventArgs requestedScroll;
		private IVisualElementRenderer headerRenderer;
		private IVisualElementRenderer footerRenderer;
		private bool estimatedRowHeight;
		private FormsUITableViewController tableViewController;
		private CGRect previousFrame;
		private const int DefaultRowHeight = 44;

		public override SizeRequest GetDesiredSize (double widthConstraint, double heightConstraint)
		{
			return Control.GetSizeRequest(widthConstraint, heightConstraint, 44.0, 44.0);
		}

		public override void Layout ()
		{
			base.Layout ();
			double num = (double)this.Bounds.Height;
			double width = (double)this.Bounds.Width;
			if (this.headerRenderer != null)
			{
				VisualElement element = this.headerRenderer.Element;
				double widthConstraint = width;
				double heightConstraint = double.PositiveInfinity;
				SizeRequest sizeRequest = element.GetSizeRequest (widthConstraint, heightConstraint);
				Rectangle bounds = new Rectangle (0.0, 0.0, width, Math.Ceiling (sizeRequest.Request.Height));
				element.Layout (bounds);
				Device.BeginInvokeOnMainThread ((Action)(() =>
				{
					if (this.headerRenderer == null)
						return;

					// TODO: WT.? Need to figure out header view
					Control.HeaderView.AddSubview( headerRenderer.NativeView );
				}));
			}
			if (this.footerRenderer != null)
			{
				VisualElement element = this.footerRenderer.Element;
				double widthConstraint = width;
				double heightConstraint = num;
				SizeRequest sizeRequest = element.GetSizeRequest (widthConstraint, heightConstraint);
				Rectangle bounds = new Rectangle (0.0, 0.0, width, Math.Ceiling (sizeRequest.Request.Height));
				element.Layout (bounds);
				Device.BeginInvokeOnMainThread ((Action)(() =>
				{
					if (this.footerRenderer == null)
						return;

					// TODO: WT.? Need to figure this one out
					//Control.PageFooter = this.footerRenderer.NativeView;
				}));
			}
			if (this.requestedScroll != null && this.Superview != null)
			{
				ScrollToRequestedEventArgs e = this.requestedScroll;
				this.requestedScroll = (ScrollToRequestedEventArgs)null;
				this.OnScrollToRequested ((object)this, e);
			}
			if (!(this.previousFrame != this.Frame))
				return;
			this.previousFrame = this.Frame;
			// Not for Mac
			/*
			KeyboardInsetTracker keyboardInsetTracker = this.insetTracker;
			if (keyboardInsetTracker == null)
				return;
			keyboardInsetTracker.UpdateInsets ();
			*/
		}

		protected override void Dispose (bool disposing)
		{
			/*
			if (disposing && this.insetTracker != null)
			{
				this.insetTracker.Dispose ();
				this.insetTracker = (KeyboardInsetTracker)null;
				Stack<NSView> stack = new Stack<NSView> ((IEnumerable<NSView>)this.Subviews);
				while (stack.Count > 0)
				{
					NSView uiView1 = stack.Pop ();
					ViewCellRenderer.ViewTableCell viewTableCell = uiView1 as ViewCellRenderer.ViewTableCell;
					if (viewTableCell != null)
					{
						//viewTableCell.Dispose ();
					}
					else
					{
						foreach (NSView uiView2 in uiView1.Subviews)
							stack.Push (uiView2);
					}
				}
				if (this.Element != null)
				{
					this.Element.TemplatedItems.CollectionChanged -= OnCollectionChanged;
					this.Element.TemplatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
				}
				if (this.tableViewController != null)
				{
					//this.tableViewController.Dispose ();
					this.tableViewController = null;
				}
			}
			*/
			if (disposing)
			{
				if (this.headerRenderer != null)
				{
					Platform platform = this.headerRenderer.Element.Platform as Platform;

					if (platform != null)
						platform.DisposeModelAndChildrenRenderers ((Element)this.headerRenderer.Element);
					
					this.headerRenderer = (IVisualElementRenderer)null;
				}
				if (this.footerRenderer != null)
				{
					Platform platform = this.footerRenderer.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers ((Element)this.footerRenderer.Element);
					this.footerRenderer = (IVisualElementRenderer)null;
				}
			}
			base.Dispose (disposing);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<ListView> e)
		{
			ListViewRenderer.ListViewDataSource unevenListViewDataSource;
			this.requestedScroll = null;
			if (e.OldElement != null)
			{
				ListView oldElement = e.OldElement;
				VisualElement headerElement = (VisualElement)((IListViewController)oldElement).HeaderElement;
				if (headerElement != null)
				{
					headerElement.MeasureInvalidated -= new EventHandler (this.OnHeaderMeasureInvalidated);
				}
				VisualElement footerElement = (VisualElement)((IListViewController)oldElement).FooterElement;
				if (footerElement != null)
				{
					footerElement.MeasureInvalidated -= new EventHandler (this.OnFooterMeasureInvalidated);
				}

				// Not for Mac
				//e.OldElement.ScrollToRequested -= new EventHandler<ScrollToRequestedEventArgs> (this.OnScrollToRequested);

				// TODO: TemplatedItems
				//e.OldElement.TemplatedItems.CollectionChanged -= OnCollectionChanged;
				//e.OldElement.TemplatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;

			}
			if (e.NewElement != null)
			{
				if (base.Control == null)
				{
					tableViewController = new FormsUITableViewController (e.NewElement);

					base.SetNativeControl (tableViewController.TableView);

					// Not for Mac
					/*
					this.insetTracker = new KeyboardInsetTracker (
						this.tableViewController.TableView, () => base.Control.Window, 
						(UIEdgeInsets insets) =>
						{
							UITableView control = base.Control;
							UIEdgeInsets uIEdgeInset = insets;
							UIEdgeInsets uIEdgeInset1 = uIEdgeInset;
							base.Control.ScrollIndicatorInsets = uIEdgeInset;
							control.ContentInset = uIEdgeInset1;
						}, 
						(CGPoint point) =>
						{
							CGPoint contentOffset = base.Control.ContentOffset;
							contentOffset.Y = contentOffset.Y + point.Y;
							base.Control.SetContentOffset (contentOffset, true);
						});
						*/
				}
				this.shouldEstimateRowHeight = true;
				if (e.NewElement.TakePerformanceHit)
				{
					this.shouldEstimateRowHeight = false;
				}
				e.NewElement.ScrollToRequested += new EventHandler<ScrollToRequestedEventArgs> (this.OnScrollToRequested);
				e.NewElement.TemplatedItems.CollectionChanged += new NotifyCollectionChangedEventHandler (this.OnCollectionChanged);
				e.NewElement.TemplatedItems.GroupedCollectionChanged += new NotifyCollectionChangedEventHandler (this.OnGroupedCollectionChanged);
				this.UpdateRowHeight ();

				var uITableView = base.Control;
				if (e.NewElement.HasUnevenRows)
				{
					unevenListViewDataSource = new ListViewRenderer.UnevenListViewDataSource (e.NewElement, this.tableViewController);
				}
				else
				{
					unevenListViewDataSource = new ListViewRenderer.ListViewDataSource (e.NewElement, this.tableViewController);
				}
				ListViewRenderer.ListViewDataSource listViewDataSource = unevenListViewDataSource;
				this.dataSource = unevenListViewDataSource;
				uITableView.Source = listViewDataSource;
				//this.UpdateEstimatedRowHeight ();
				this.UpdateHeader ();
				this.UpdateFooter ();
				this.UpdatePullToRefreshEnabled ();
				this.UpdateIsRefreshing ();
				this.UpdateSeparatorColor ();
				this.UpdateSeparatorVisibility ();
				object selectedItem = e.NewElement.SelectedItem;
				if (selectedItem != null)
				{
					this.dataSource.OnItemSelected (null, new SelectedItemChangedEventArgs (selectedItem));
				}
			}
			base.OnElementChanged (e);
		}

		private void UpdateSeparatorColor ()
		{
			// TODO: Separator Color
			//Control.SeparatorColor = ColorExtensions.ToUIColor (this.Element.SeparatorColor, NSColor.Gray);
		}

		private void UpdateSeparatorVisibility ()
		{
			// TODO: Separator Visibility
			/*
			switch (this.Element.SeparatorVisibility)
			{
			case SeparatorVisibility.Default:
				this.Control.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
				break;
			case SeparatorVisibility.None:
				this.Control.SeparatorStyle = UITableViewCellSeparatorStyle.None;
				break;
			default:
				throw new ArgumentOutOfRangeException ();
			}
			*/
		}

		private void UpdateHeader ()
		{
			Element headerElement = ((IListViewController)this.Element).HeaderElement;
			View view = (View)headerElement;
			if (view != null)
			{
				if (this.headerRenderer != null)
				{
					this.headerRenderer.Element.MeasureInvalidated -= OnHeaderMeasureInvalidated;
					if (headerElement != null && this.headerRenderer.GetType () == Registrar.Registered.GetHandlerType (((object)headerElement).GetType ()))
					{
						this.headerRenderer.SetElement ((VisualElement)view);
						return;
					}
					this.Control.HeaderView = null;
					Platform platform = this.headerRenderer.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers ((Element)this.headerRenderer.Element);
					this.headerRenderer = (IVisualElementRenderer)null;
				}
				this.headerRenderer = Platform.CreateRenderer ((VisualElement)view);
				Platform.SetRenderer ((VisualElement)view, this.headerRenderer);

				// TODO: Header View
				//Control.HeaderView = this.headerRenderer.NativeView;

				view.MeasureInvalidated += OnHeaderMeasureInvalidated;
			}
			else
			{
				if (this.headerRenderer == null)
					return;
				this.Control.HeaderView = null;
				Platform platform = this.headerRenderer.Element.Platform as Platform;
				if (platform != null)
					platform.DisposeModelAndChildrenRenderers ((Element)this.headerRenderer.Element);
				this.headerRenderer.Dispose ();
				this.headerRenderer = null;
			}
		}

		private void OnHeaderMeasureInvalidated (object sender, EventArgs eventArgs)
		{
			double width = (double)this.Bounds.Width;
			if (width == 0.0)
				return;
			VisualElement visualElement = (VisualElement)sender;
			double widthConstraint = width;
			double heightConstraint = double.PositiveInfinity;
			SizeRequest sizeRequest = visualElement.GetSizeRequest (widthConstraint, heightConstraint);
			Rectangle bounds = new Rectangle (0.0, 0.0, width, sizeRequest.Request.Height);
			visualElement.Layout (bounds);

			// TODO: Header View
			// this.Control.HeaderView = this.headerRenderer.NativeView;
		}

		private void UpdateFooter ()
		{
			Element footerElement = ((IListViewController)this.Element).FooterElement;
			View view = (View)footerElement;
			if (view != null)
			{
				if (this.footerRenderer != null)
				{
					this.footerRenderer.Element.MeasureInvalidated -= OnFooterMeasureInvalidated;
					if (footerElement != null && this.footerRenderer.GetType () == Registrar.Registered.GetHandlerType (((object)footerElement).GetType ()))
					{
						this.footerRenderer.SetElement ((VisualElement)view);
						return;
					}

					// TODO: Page Footer
					//this.Control.PageFooter = null;

					Platform platform = this.footerRenderer.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers ((Element)this.footerRenderer.Element);
					this.footerRenderer.Dispose ();
					this.footerRenderer = (IVisualElementRenderer)null;
				}
				this.footerRenderer = Platform.CreateRenderer ((VisualElement)view);
				Platform.SetRenderer ((VisualElement)view, this.footerRenderer);

				// TODO: Page Footer
				//this.Control.PageFooter = this.footerRenderer.NativeView;
				view.MeasureInvalidated += OnFooterMeasureInvalidated;
			}
			else
			{
				if (this.footerRenderer == null)
					return;

				// TODO: Page Footer
				//this.Control.PageFooter = null;

				Platform platform = this.footerRenderer.Element.Platform as Platform;
				if (platform != null)
					platform.DisposeModelAndChildrenRenderers ((Element)this.footerRenderer.Element);
				this.footerRenderer.Dispose ();
				this.footerRenderer = (IVisualElementRenderer)null;
			}
		}

		private void OnFooterMeasureInvalidated (object sender, EventArgs eventArgs)
		{
			double width = (double)this.Bounds.Width;
			if (width == 0.0)
				return;
			VisualElement visualElement = (VisualElement)sender;
			double widthConstraint = width;
			double heightConstraint = double.PositiveInfinity;
			SizeRequest sizeRequest = visualElement.GetSizeRequest (widthConstraint, heightConstraint);
			Rectangle bounds = new Rectangle (0.0, 0.0, width, sizeRequest.Request.Height);
			visualElement.Layout (bounds);

			// TODO: Page Footer
			//this.Control.PageFooter = this.footerRenderer.NativeView;
		}

		// Not for Mac
		/*
		private NSTableViewScrollPosition GetScrollPosition (ScrollToPosition position)
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
		*/

		private void OnScrollToRequested (object sender, ScrollToRequestedEventArgs e)
		{
			if (this.Superview == null)
			{
				this.requestedScroll = e;
			}
			else
			{
				// Not for Mac
				//UITableViewScrollPosition scrollPosition = this.GetScrollPosition (e.Position);
				/*
				if (this.Element.IsGroupingEnabled)
				{
					Tuple<int, int> groupAndIndexOfItem = this.Element.TemplatedItems.GetGroupAndIndexOfItem (e.Group, e.Item);
					if (groupAndIndexOfItem.Item1 == -1 || groupAndIndexOfItem.Item2 == -1)
						return;

					this.Control.ScrollRowToVisible(
						NSIndexPath.FromRowSection ((nint)groupAndIndexOfItem.Item2, (nint)groupAndIndexOfItem.Item1), scrollPosition, e.ShouldAnimate);
				}
				else
				*/
				{
					int globalIndexOfItem = this.Element.TemplatedItems.GetGlobalIndexOfItem (e.Item);
					if (globalIndexOfItem == -1)
						return;

					this.Control.ScrollRowToVisible (globalIndexOfItem);
				}
			}
		}

		private void OnGroupedCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			int section = this.Element.TemplatedItems.IndexOf (((TemplatedItemsList<ItemsView<Cell>, Cell>)sender).HeaderContent);
			this.UpdateItems (e, section, false);
		}

		private void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			this.UpdateItems (e, 0, true);
		}

		private void UpdateItems (NotifyCollectionChangedEventArgs e, int section, bool resetWhenGrouped)
		{
			NotifyCollectionChangedEventArgsEx changedEventArgsEx = e as NotifyCollectionChangedEventArgsEx;
			if (changedEventArgsEx != null)
				this.dataSource.Counts [section] = changedEventArgsEx.Count;
			bool flag = resetWhenGrouped && this.Element.IsGroupingEnabled;
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				//this.UpdateEstimatedRowHeight ();
				if (!(e.NewStartingIndex == -1 | flag))
				{
					this.Control.BeginUpdates ();

					var indexSet = NSIndexSet.FromNSRange (new NSRange (e.NewStartingIndex, e.NewItems.Count));
						
					this.Control.InsertRows( indexSet, NSTableViewAnimation.SlideUp);
					this.Control.EndUpdates ();
					break;
				}
				goto case NotifyCollectionChangedAction.Reset;
			case NotifyCollectionChangedAction.Remove:
				if (!(e.OldStartingIndex == -1 | flag))
				{
					this.Control.BeginUpdates ();

					var indexSet = NSIndexSet.FromNSRange (new NSRange (e.OldStartingIndex, e.OldItems.Count));

					Control.RemoveRows( indexSet, NSTableViewAnimation.SlideDown );
					this.Control.EndUpdates ();

					if (!this.estimatedRowHeight || this.Element.TemplatedItems.Count != 0)
						break;
					this.estimatedRowHeight = false;
					break;
				}
				goto case NotifyCollectionChangedAction.Reset;
			case NotifyCollectionChangedAction.Replace:
				if (!(e.OldStartingIndex == -1 | flag))
				{
					Control.BeginUpdates ();
					var indexSet = NSIndexSet.FromNSRange (new NSRange (e.OldStartingIndex, e.OldItems.Count));
					Control.ReloadData (indexSet, null);
					this.Control.EndUpdates ();

					if (!this.estimatedRowHeight || e.OldStartingIndex != 0)
						break;
					this.estimatedRowHeight = false;
					break;
				}
				goto case NotifyCollectionChangedAction.Reset;
			case NotifyCollectionChangedAction.Move:
				if (((e.OldStartingIndex == -1 ? 1 : (e.NewStartingIndex == -1 ? 1 : 0)) | (flag ? 1 : 0)) == 0)
				{
					this.Control.BeginUpdates ();
					for (int index = 0; index < e.OldItems.Count; ++index)
					{
						int oldStartingIndex = e.OldStartingIndex;
						int newStartingIndex = e.NewStartingIndex;
						if (e.NewStartingIndex < e.OldStartingIndex)
						{
							oldStartingIndex += index;
							newStartingIndex += index;
						}

						this.Control.MoveRow (oldStartingIndex, newStartingIndex);
					}
					this.Control.EndUpdates ();
					if (!this.estimatedRowHeight || e.OldStartingIndex != 0)
						break;
					this.estimatedRowHeight = false;
					break;
				}
				goto case NotifyCollectionChangedAction.Reset;
			case NotifyCollectionChangedAction.Reset:
				this.estimatedRowHeight = false;
				this.Control.ReloadData ();
				break;
			}
		}

		private NSIndexPath[] GetPaths (int section, int index, int count)
		{
			NSIndexPath[] nsIndexPathArray = new NSIndexPath[count];
			for (int index1 = 0; index1 < nsIndexPathArray.Length; ++index1)
			{
				nsIndexPathArray [index1] = NSIndexPath.FromItemSection (index + index1, section);
			}
			return nsIndexPathArray;
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == ListView.RowHeightProperty.PropertyName)
				this.UpdateRowHeight ();
			else if (e.PropertyName == ListView.IsGroupingEnabledProperty.PropertyName)
				this.dataSource.UpdateGrouping ();
			else if (e.PropertyName == ListView.HasUnevenRowsProperty.PropertyName)
			{
				this.estimatedRowHeight = false;
				this.dataSource = this.Element.HasUnevenRows ? 
					new ListViewRenderer.UnevenListViewDataSource (this.dataSource) : 
					new ListViewRenderer.ListViewDataSource (dataSource);
				this.Control.Source = dataSource;
			}
			else if (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName)
				this.UpdatePullToRefreshEnabled ();
			else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
				this.UpdateIsRefreshing ();
			else if (e.PropertyName == ListView.SeparatorColorProperty.PropertyName)
				this.UpdateSeparatorColor ();
			else if (e.PropertyName == ListView.SeparatorVisibilityProperty.PropertyName)
				this.UpdateSeparatorVisibility ();
			else if (e.PropertyName == "HeaderElement")
				this.UpdateHeader ();
			else if (e.PropertyName == "FooterElement")
			{
				this.UpdateFooter ();
			}
			else
			{
				if (!(e.PropertyName == "RefreshAllowed"))
					return;
				this.UpdatePullToRefreshEnabled ();
			}
		}

		private void UpdateIsRefreshing ()
		{
			bool isRefreshing = this.Element.IsRefreshing;
			if (this.tableViewController == null)
				return;
			this.tableViewController.UpdateIsRefreshing (isRefreshing);
		}

		private void UpdatePullToRefreshEnabled ()
		{
			if (this.tableViewController == null)
				return;
			// Not for Mac
			//this.tableViewController.UpdatePullToRefreshEnabled (this.Element.IsPullToRefreshEnabled && ((IListViewController)this.Element).RefreshAllowed);
		}

		private void UpdateRowHeight ()
		{
			int rowHeight = this.Element.RowHeight;
			this.Control.RowHeight = (nfloat)(rowHeight <= 0 ? 44 : rowHeight);
		}

		// Not for Mac
		/*
		private void UpdateEstimatedRowHeight ()
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
					this.Control.EstimatedRowHeight = listViewDataSource.CalculateHeightForCell (this.Control, Enumerable.First<Cell> ((IEnumerable<Cell>)this.Element.TemplatedItems));
					this.estimatedRowHeight = true;
				}
				else
					this.Control.EstimatedRowHeight = (nfloat)44;
			}
			else
			{
				if (Forms.IsiOS7OrNewer)
					this.Control.EstimatedRowHeight = (nfloat)0;
				this.estimatedRowHeight = true;
			}
		}
		*/

		internal class UnevenListViewDataSource : ListViewRenderer.ListViewDataSource
		{
			private IVisualElementRenderer prototype;

			public UnevenListViewDataSource (ListView list, FormsUITableViewController uiTableViewController)
				: base (list, uiTableViewController)
			{
			}

			public UnevenListViewDataSource (ListViewRenderer.ListViewDataSource source)
				: base (source)
			{
			}

			public override nfloat GetRowHeight (NSTableView tableView, nint row)
			{
				Cell cellForPath = GetCellFor (0, row);
				if (this.list.RowHeight == -1 && cellForPath.Height == -1.0 && cellForPath is ViewCell)
				{
					return this.CalculateHeightForCell (tableView, cellForPath);
				}
				double renderHeight = cellForPath.RenderHeight;
				if (renderHeight <= 0.0)
					return (nfloat)44;
				return (nfloat)renderHeight;
			}

			internal nfloat CalculateHeightForCell (NSTableView tableView, Cell cell)
			{
				ViewCell viewCell = cell as ViewCell;
				if (viewCell != null && viewCell.View != null)
				{
					View view = viewCell.View;
					if (this.prototype == null)
					{
						this.prototype = Platform.CreateRenderer ((VisualElement)view);
						Platform.SetRenderer ((VisualElement)view, this.prototype);
					}
					else
					{
						this.prototype.SetElement ((VisualElement)view);
						Platform.SetRenderer ((VisualElement)view, this.prototype);
					}
					SizeRequest sizeRequest = view.GetSizeRequest ((double)tableView.Frame.Width, double.PositiveInfinity);
					view.ClearValue (Platform.RendererProperty);
					foreach (BindableObject bindableObject in view.Descendants())
						bindableObject.ClearValue (Platform.RendererProperty);
					return (nfloat)sizeRequest.Request.Height;
				}
				double renderHeight = cell.RenderHeight;
				if (renderHeight <= 0.0)
					return (nfloat)44;
				return (nfloat)renderHeight;
			}
		}

		internal class ListViewDataSource : NSTableViewSource
		{
			protected readonly ListView list;
			private readonly NSTableView uiTableView;
			private readonly FormsUITableViewController uiTableViewController;
			private readonly nfloat defaultSectionHeight;
			private bool selectionFromNative;
			private bool isDragging;

			public Dictionary<int, int> Counts { get; set; }

			private NSColor DefaultBackgroundColor
			{
				get
				{
					return NSColor.Clear;
				}
			}

			public ListViewDataSource (ListViewRenderer.ListViewDataSource source)
			{
				this.uiTableViewController = source.uiTableViewController;
				this.list = source.list;
				this.uiTableView = source.uiTableView;
				this.defaultSectionHeight = source.defaultSectionHeight;
				this.selectionFromNative = source.selectionFromNative;
				this.Counts = new Dictionary<int, int> ();
			}

			public ListViewDataSource (ListView list, FormsUITableViewController uiTableViewController)
			{
				this.uiTableViewController = uiTableViewController;
				this.uiTableView = uiTableViewController.TableView;
				this.defaultSectionHeight = (nfloat)44;
				this.list = list;
				this.list.ItemSelected += OnItemSelected;
				this.UpdateShortNameListener ();
				this.Counts = new Dictionary<int, int> ();
			}

			public override void DraggingSessionWillBegin (NSTableView tableView, NSDraggingSession draggingSession, CGPoint willBeginAtScreenPoint, NSIndexSet rowIndexes)
			{
				this.isDragging = true;
			}

			public override void DraggingSessionEnded (NSTableView tableView, NSDraggingSession draggingSession, CGPoint endedAtScreenPoint, NSDragOperation operation)
			{
				this.isDragging = false;
				//this.uiTableViewController.UpdateShowHideRefresh (false);
			}

			/*
			public override void Scrolled (NSScrollView scrollView)
			{
				if (!this.isDragging || !(scrollView.ContentOffset.Y < (nfloat)0))
					return;
				this.uiTableViewController.UpdateShowHideRefresh (true);
			}
			*/

			public void UpdateGrouping ()
			{
				this.UpdateShortNameListener ();
				this.uiTableView.ReloadData ();
			}

			/*
			public override nint NumberOfSections (NSTableView tableView)
			{
				if (this.list.IsGroupingEnabled)
					return (nint)this.list.TemplatedItems.Count;
				return (nint)1;
			}

			public override nint RowsInSection (NSTableView tableview, nint section)
			{
				int num;
				if (this.Counts.TryGetValue ((int)section, out num))
				{
					this.Counts.Remove ((int)section);
					return (nint)num;
				}
				if (this.list.IsGroupingEnabled)
					return (nint)((ICollection)((IList)this.list.TemplatedItems) [(int)section]).Count;
				return (nint)this.list.TemplatedItems.Count;
			}
			*/

			public override NSCell GetCell (NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				NSTableCellView cell1;
				NSTableViewCell tableViewCell;
				NSCell cell;
				nint section = 0;

				switch (this.list.CachingStrategy)
				{
				case ListViewCachingStrategy.RetainElement:
					Cell cellForPath1 = this.GetCellFor (section, row);
					cell1 = CellTableViewCell.GetNativeCell (tableView, cellForPath1, false);
					break;
				case ListViewCachingStrategy.RecycleElement:

					cell1 = null; // tableView.DequeueReusableCell ("ContextActionsCell");
					if (cell1 == null)
					{
						Cell cellForPath2 = this.GetCellFor (section, row);
						cell1 = CellTableViewCell.GetNativeCell (tableView, cellForPath2, true);
						break;
					}
					TemplatedItemsList<ItemsView<Cell>, Cell> group = this.list.TemplatedItems.GetGroup((int)section);
					Cell cell2 = (Cell)((INativeElementView)cell1).Element;
					cell2.SendDisappearing ();
					Cell content = cell2;

					group.UpdateContent (content, (int)row);
					cell2.SendAppearing ();
					break;
				default:
					throw new NotSupportedException ();
				}
				// TODO: Figure this out
				//NSColor color = tableView.IndexPathForSelectedRow == null || 
				//	!tableView.IndexPathForSelectedRow.Equals (indexPath) ? this.DefaultBackgroundColor : NSColor.Clear;
				NSColor color = NSColor.Clear;
				ListViewRenderer.ListViewDataSource.SetCellBackgroundColor (cell1, color);

				// TODO: WT.? damit. NSCell, NSTableViewCell, NSTableCellView.  Soo confusing
				//return cell1;
				return null;
			}

			/*
			public override nfloat GetHeightForHeader (NSTableView tableView, nint section)
			{
				if (!this.list.IsGroupingEnabled)
					return (nfloat)0;
				nfloat nfloat = (nfloat)((float)this.list.TemplatedItems [(int)section].RenderHeight);
				if (nfloat == (nfloat) - 1)
					nfloat = this.defaultSectionHeight;
				return nfloat;
			}


			public override NSView GetViewForHeader (NSTableView tableView, nint section)
			{
				if (!this.list.IsGroupingEnabled || this.list.GroupHeaderTemplate == null)
					return (NSView)null;
				Cell cell1 = this.list.TemplatedItems [(int)section];
				if (cell1.HasContextActions)
					throw new NotSupportedException ("Header cells do not support context actions");
				CellRenderer cellRenderer = (CellRenderer)Registrar.Registered.GetHandler (((object)cell1).GetType ());
				HeaderWrapperView headerWrapperView = new HeaderWrapperView ();
				NSTableCellView cell2 = cellRenderer.GetCell (cell1, (NSTableCellView)null, tableView);
				// ISSUE: reference to a compiler-generated method
				headerWrapperView.AddSubview ((NSView)cell2);
				return (NSView)headerWrapperView;
			}


			public override string TitleForHeader (NSTableView tableView, nint section)
			{
				if (!this.list.IsGroupingEnabled)
					return (string)null;
				TemplatedItemsList<ItemsView<Cell>, Cell> sectionList = this.GetSectionList ((int)section);
				PropertyChangedEventHandler changedEventHandler1 = new PropertyChangedEventHandler (this.OnSectionPropertyChanged);
				sectionList.remove_PropertyChanged (changedEventHandler1);
				PropertyChangedEventHandler changedEventHandler2 = new PropertyChangedEventHandler (this.OnSectionPropertyChanged);
				sectionList.add_PropertyChanged (changedEventHandler2);
				return sectionList.Name;
			}
			*/

			private void OnSectionPropertyChanged (object sender, PropertyChangedEventArgs e)
			{
				//NSIndexPath pathForSelectedRow = this.uiTableView.IndexPathForSelectedRow;

				var templatedItemsList = (TemplatedItemsList<ItemsView<Cell>, Cell>)sender;
				int num = ((IList)this.list.TemplatedItems).IndexOf ((object)templatedItemsList);
				if (num == -1)
				{
					templatedItemsList.PropertyChanged -= OnSectionPropertyChanged;
				}
				else
				{
					// TODO: Um. Reload and select
					//this.uiTableView.ReloadSections (NSIndexSet.FromIndex ((nint)num), UITableViewRowAnimation.Automatic);
					//this.uiTableView.SelectRow (pathForSelectedRow, false, UITableViewScrollPosition.None);
				}
			}

			/*
			public override string[] SectionIndexTitles (NSTableView tableView)
			{
				if (this.list.TemplatedItems.get_ShortNames () == null)
					return (string[])null;
				return Enumerable.ToArray<string> ((IEnumerable<string>)this.list.TemplatedItems.get_ShortNames ());
			}
			*/

			public override bool ShouldSelectRow (NSTableView tableView, nint row)
			{
				throw new System.NotImplementedException ();
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				// TODO: Figure this out when I'm not soo tired
				/*
				NSTableView tableView = notification.Object as NSTableView;

				NSTableCellView cell1 = tableView.CellAt (indexPath);
				if (cell1 == null)
					return;
				Cell cell2 = (Cell)null;
				if (this.list.CachingStrategy == ListViewCachingStrategy.RecycleElement)
					cell2 = (Cell)((INativeElementView)cell1).Element;
				ListViewRenderer.ListViewDataSource.SetCellBackgroundColor (cell1, NSColor.Clear);
				this.selectionFromNative = true;
				// ISSUE: reference to a compiler-generated method
				UIView_UITextField.EndEditing ((NSView)tableView, true);
				this.list.NotifyRowTapped (indexPath.Section, indexPath.Row, cell2);
				*/
			}

			/*
			public override void RowDeselected (NSTableView tableView, NSIndexPath indexPath)
			{
				// ISSUE: reference to a compiler-generated method
				NSTableCellView cell = tableView.CellAt (indexPath);
				if (cell == null)
					return;
				ListViewRenderer.ListViewDataSource.SetCellBackgroundColor (cell, this.DefaultBackgroundColor);
			}
			*/

			public void OnItemSelected (object sender, SelectedItemChangedEventArgs eventArg)
			{
				if (this.selectionFromNative)
				{
					this.selectionFromNative = false;
				}
				else
				{
					Tuple<int, int> groupAndIndexOfItem = this.list.TemplatedItems.GetGroupAndIndexOfItem (eventArg.SelectedItem);
					if (groupAndIndexOfItem.Item1 == -1 || groupAndIndexOfItem.Item2 == -1)
					{
						//NSIndexPath pathForSelectedRow = this.uiTableView.IndexPathForSelectedRow;

						bool animated = true;
						if (uiTableView.SelectedRow >= 0)
						{
							var cell = uiTableView.GetCell (0, uiTableView.SelectedRow);
							var contextActionsCell = cell.ControlView as ContextActionsCell;
							if (contextActionsCell != null)
							{
								contextActionsCell.PrepareForDeselect ();
								//if (contextActionsCell.IsOpen)
								//	animated = false;
							}
						}
						else
							return;
						
						uiTableView.DeselectRow (uiTableView.SelectedRow);
					}
					else
					{
						this.uiTableView.SelectRow (groupAndIndexOfItem.Item2, false);
					}
				}
			}

			protected Cell GetCellFor (nint section, nint item)
			{
				var templatedItemsList = list.TemplatedItems;

				// TODO: Figure out Groups
				//if (this.list.IsGroupingEnabled)
				//	templatedItemsList = templatedItemsList [(int)section];
				
				return templatedItemsList [(int)item];
			}

			private TemplatedItemsList<ItemsView<Cell>, Cell> GetSectionList (int section)
			{
				return (TemplatedItemsList<ItemsView<Cell>, Cell>)((IList)this.list.TemplatedItems) [section];
			}

			private void OnShortNamesCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
			{
				// TODO: Is this important
				//this.uiTableView.ReloadSectionIndexTitles ();
			}

			private void UpdateShortNameListener ()
			{
				if (this.list.IsGroupingEnabled)
				{
					if (this.list.TemplatedItems.ShortNames == null)
						return;
					
					((INotifyCollectionChanged)this.list.TemplatedItems.ShortNames).CollectionChanged += OnShortNamesCollectionChanged;
				}
				else
				{
					if (this.list.TemplatedItems.ShortNames == null)
						return;
					((INotifyCollectionChanged)this.list.TemplatedItems.ShortNames).CollectionChanged -= OnShortNamesCollectionChanged;
				}
			}

			private static void SetCellBackgroundColor (NSTableCellView cell, NSColor color)
			{
				ContextActionsCell contextActionsCell = cell as ContextActionsCell;

				cell.SetBackgroundColor( color );
				if (contextActionsCell == null)
					return;
				contextActionsCell.ContentCell.SetBackgroundColor( color );
			}
		}
	}
}
