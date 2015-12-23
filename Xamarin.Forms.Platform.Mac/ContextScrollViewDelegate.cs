using CoreGraphics;
using System;
using System.Collections.Generic;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	// Not for Mac
	/*
	internal class ContextScrollViewDelegate : NSScrollViewDelegate
	{
		private bool isDisposed;
		private bool isOpen;
		private GlobalCloseContextGestureRecognizer globalCloser;
		private UITapGestureRecognizer closer;
		private NSTableView table;
		private NSView backgroundView;
		private nfloat finalButtonSize;
		private nfloat buttonsWidth;
		private List<NSButton> buttons;
		private NSView container;

		public Action ClosedCallback { get; set; }

		public nfloat ButtonsWidth
		{
			get
			{
				return this.buttonsWidth;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
		}

		public ContextScrollViewDelegate (NSView container, List<NSButton> buttons, bool isOpen)
		{
			this.isOpen = isOpen;
			this.container = container;
			this.buttons = buttons;
			for (int index = 0; index < buttons.Count; ++index)
			{
				NSButton uiButton = buttons [index];
				uiButton.Hidden = !isOpen;
				nfloat nfloat = this.buttonsWidth;
				CGRect frame = uiButton.Frame;
				nfloat width = frame.Width;
				this.buttonsWidth = nfloat + width;
				frame = uiButton.Frame;
				this.finalButtonSize = frame.Width;
			}
		}

		public override void DraggingStarted (NSScrollView scrollView)
		{
			if (!this.isOpen)
				this.SetButtonsShowing (true);
			if (!this.GetContextCell (scrollView).Selected || this.isOpen)
				return;
			this.RemoveHighlight (scrollView);
		}

		public override void WillEndDragging (NSScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			nfloat x1 = this.buttonsWidth;
			nfloat x2 = targetContentOffset.X;
			nfloat nfloat1 = scrollView.Frame.Width * (nfloat)0.4f;
			nfloat nfloat2 = x1 * (nfloat)0.8f;
			if (x2 >= nfloat1 || x2 >= nfloat2)
			{
				this.isOpen = true;
				targetContentOffset = new CGPoint (x1, (nfloat)0);
				this.RemoveHighlight (scrollView);
				if (this.globalCloser != null)
					return;
				NSView uiView = (NSView)scrollView;
				while (uiView.Superview != null)
				{
					uiView = uiView.Superview;
					Action action = (Action)(() =>
					{
						if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
							this.RestoreHighlight (scrollView);
						this.isOpen = false;
						// ISSUE: reference to a compiler-generated method
						scrollView.SetContentOffset (new CGPoint ((nfloat)0, (nfloat)0), true);
						this.ClearCloserRecognizer (scrollView);
					});
					NSTableView uiTableView = uiView as NSTableView;
					if (uiTableView != null)
					{
						this.table = uiTableView;
						this.globalCloser = new GlobalCloseContextGestureRecognizer (scrollView, this.buttons, action);
						this.globalCloser.ShouldRecognizeSimultaneously = (UIGesturesProbe)((recognizer, r) => r == this.table.PanGestureRecognizer);
						// ISSUE: reference to a compiler-generated method
						uiTableView.AddGestureRecognizer ((UIGestureRecognizer)this.globalCloser);
						this.closer = new UITapGestureRecognizer (action);
						// ISSUE: reference to a compiler-generated method
						this.GetContextCell (scrollView).ContentCell.AddGestureRecognizer ((UIGestureRecognizer)this.closer);
					}
				}
			}
			else
			{
				this.ClearCloserRecognizer (scrollView);
				this.isOpen = false;
				targetContentOffset = new CGPoint ((nfloat)0, (nfloat)0);
				if (!UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
					return;
				this.RestoreHighlight (scrollView);
			}
		}

		public override void Scrolled (NSScrollView scrollView)
		{
			nfloat width1 = this.finalButtonSize;
			int count = this.buttons.Count;
			CGPoint contentOffset;
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
			{
				NSView uiView = this.container;
				nfloat width2 = scrollView.Frame.Width;
				nfloat y = (nfloat)0;
				contentOffset = scrollView.ContentOffset;
				nfloat x = contentOffset.X;
				nfloat height = scrollView.Frame.Height;
				CGRect cgRect = new CGRect (width2, y, x, height);
				uiView.Frame = cgRect;
			}
			else
			{
				nfloat nfloat = scrollView.ContentOffset.X / (nfloat)((float)count);
				if (nfloat > width1)
					width1 = nfloat + (nfloat)1;
				for (int index = count - 1; index >= 0; --index)
				{
					NSButton uiButton = this.buttons [index];
					CGRect frame = uiButton.Frame;
					CGRect cgRect = new CGRect (scrollView.Frame.Width + (nfloat)(count - (index + 1)) * nfloat, (nfloat)0, width1, frame.Height);
					uiButton.Frame = cgRect;
				}
			}
			contentOffset = scrollView.ContentOffset;
			if (!(contentOffset.X == (nfloat)0))
				return;
			this.isOpen = false;
			this.SetButtonsShowing (false);
			this.RestoreHighlight (scrollView);
			this.ClearCloserRecognizer (scrollView);
			if (this.ClosedCallback == null)
				return;
			this.ClosedCallback ();
		}

		public void Unhook (NSScrollView scrollView)
		{
			this.RestoreHighlight (scrollView);
			this.ClearCloserRecognizer (scrollView);
		}

		protected override void Dispose (bool disposing)
		{
			if (this.isDisposed)
				return;
			this.isDisposed = true;
			if (disposing)
			{
				this.ClosedCallback = (Action)null;
				this.table = (NSTableView)null;
				this.backgroundView = (NSView)null;
				this.container = (NSView)null;
				this.buttons = (List<NSButton>)null;
			}
			base.Dispose (disposing);
		}

		private void SetButtonsShowing (bool show)
		{
			for (int index = 0; index < this.buttons.Count; ++index)
				this.buttons [index].Hidden = !show;
		}

		private void ClearCloserRecognizer (NSScrollView scrollView)
		{
			if (this.globalCloser == null)
				return;
			// ISSUE: reference to a compiler-generated method
			this.GetContextCell (scrollView).ContentCell.RemoveGestureRecognizer ((UIGestureRecognizer)this.closer);
			this.closer.Dispose ();
			this.closer = (UITapGestureRecognizer)null;
			// ISSUE: reference to a compiler-generated method
			this.table.RemoveGestureRecognizer ((UIGestureRecognizer)this.globalCloser);
			this.table = (NSTableView)null;
			this.globalCloser.Dispose ();
			this.globalCloser = (GlobalCloseContextGestureRecognizer)null;
		}

		private void RestoreHighlight (NSScrollView scrollView)
		{
			if (this.backgroundView == null)
				return;
			ContextActionsCell contextCell = this.GetContextCell (scrollView);
			long num1 = 3;
			contextCell.SelectionStyle = (UITableViewCellSelectionStyle)num1;
			int num2 = 1;
			int num3 = 0;
			// ISSUE: reference to a compiler-generated method
			contextCell.SetSelected (num2 != 0, num3 != 0);
			// ISSUE: reference to a compiler-generated method
			scrollView.Superview.Superview.InsertSubview (this.backgroundView, (nint)0);
			this.backgroundView = (NSView)null;
		}

		private void RemoveHighlight (NSScrollView scrollView)
		{
			NSView[] subviews = scrollView.Superview.Superview.Subviews;
			int num = 0;
			for (int index = 0; index < subviews.Length; ++index)
			{
				if (subviews [index].Frame.Height > (nfloat)1)
					++num;
			}
			if (num <= 1)
				return;
			this.backgroundView = subviews [0];
			// ISSUE: reference to a compiler-generated method
			this.backgroundView.RemoveFromSuperview ();
			this.GetContextCell (scrollView).SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		private ContextActionsCell GetContextCell (NSScrollView scrollView)
		{
			NSView superview = scrollView.Superview.Superview;
			ContextActionsCell contextActionsCell = superview as ContextActionsCell;
			for (; superview.Superview != null; superview = superview.Superview)
			{
				contextActionsCell = superview as ContextActionsCell;
				if (contextActionsCell != null)
					break;
			}
			return contextActionsCell;
		}

		public void PrepareForDeselect (NSScrollView scrollView)
		{
			this.RestoreHighlight (scrollView);
		}
	}
	*/
}
