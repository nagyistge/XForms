using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class TableViewRenderer : ViewRenderer<TableView, NSTableView>
	{
		//private KeyboardInsetTracker insetTracker;
		private CGRect previousFrame;

		// Not for Mac
		/*
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.insetTracker != null)
			{
				this.insetTracker.Dispose();
				this.insetTracker = null;
				Stack<NSView> uIViews = new Stack<NSView>(this.Subviews);
				while (uIViews.Count > 0)
				{
					NSView uIViews1 = uIViews.Pop();
					ViewCellRenderer.ViewTableCell viewTableCell = uIViews1 as ViewCellRenderer.ViewTableCell;
					if (viewTableCell == null)
					{
						NSView[] subviews = uIViews1.Subviews;
						for (int i = 0; i < (int)subviews.Length; i++)
						{
							uIViews.Push(subviews[i]);
						}
					}
					else
					{
						//viewTableCell.Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}
		*/

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return base.Control.GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
		}

		public override void Layout()
		{
			base.Layout();
			if (this.previousFrame != this.Frame)
			{
				this.previousFrame = this.Frame;

				// Not for Mac
				/*
				KeyboardInsetTracker keyboardInsetTracker = this.insetTracker;
				if (keyboardInsetTracker == null)
					return;
				keyboardInsetTracker.UpdateInsets();
				*/
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
		{
			if (e.NewElement != null)
			{
				if (e.NewElement.Intent != TableIntent.Data)
				{
					//uITableViewStyle = UITableViewStyle.Grouped;
				}

				if (base.Control == null)
				{
					NSTableView uITableView = new NSTableView(CGRect.Empty);

					//this.originalBackgroundView = uITableView.BackgroundView;

					base.SetNativeControl(uITableView);

					// TODO: WT.?
					/*
					this.insetTracker = new KeyboardInsetTracker(uITableView, () => base.Control.Window, (UIEdgeInsets insets) => {
						UITableView control = base.Control;
						UIEdgeInsets uIEdgeInset = insets;
						UIEdgeInsets uIEdgeInset1 = uIEdgeInset;
						base.Control.ScrollIndicatorInsets = uIEdgeInset;
						control.ContentInset = uIEdgeInset1;
					}, (CGPoint point) => {
						CGPoint contentOffset = base.Control.ContentOffset;
						contentOffset.Y = contentOffset.Y + point.Y;
						base.Control.SetContentOffset(contentOffset, true);
					});
					*/
				}
				this.SetSource();
				this.UpdateRowHeight();
			}
			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "RowHeight")
			{
				this.UpdateRowHeight();
				return;
			}
			if (e.PropertyName == "HasUnevenRows")
			{
				this.SetSource();
				return;
			}
			if (e.PropertyName == "BackgroundColor")
			{
				this.SetBackgroundColor (Element.BackgroundColor);
				return;
			}
		}

		private void SetSource()
		{
			NSTableViewSource unEvenTableViewModelRenderer;
			TableView element = base.Element;
			NSTableView control = base.Control;
			if (element.HasUnevenRows)
			{
				unEvenTableViewModelRenderer = new UnEvenTableViewModelRenderer(element);
			}
			else
			{
				unEvenTableViewModelRenderer = new TableViewModelRenderer(element);
			}
			control.Source = unEvenTableViewModelRenderer;
		}

		private void UpdateRowHeight()
		{
			int rowHeight = base.Element.RowHeight;
			if (rowHeight <= 0)
			{
				rowHeight = 44;
			}
			base.Control.RowHeight = rowHeight;
		}
	}
}