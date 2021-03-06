using CoreGraphics;
using System;
using System.ComponentModel;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class ViewCellRenderer : CellRenderer
	{
		public override NSTableCellView GetCell (Cell item, NSTableCellView reusableCell, NSTableView tv)
		{
			return null;

			// TODO: Figure out cells
			/*
			var viewCell = (ViewCell)item;
			var cell = reusableCell as NSTableCellView;
			if (cell == null)
			{
				cell = GetCell (item, null, tv);
				//cell = new NSTableCellView (tv, viewCell, null, 0);
			}
			else
				cell.ViewCell.PropertyChanged -= ViewCellPropertyChanged;
			viewCell.PropertyChanged += ViewCellPropertyChanged;

			cell.ViewCell = viewCell;
			UpdateBackground ((NSTableCellView)cell, item);
			ViewCellRenderer.UpdateIsEnabled (cell, viewCell);

			return cell;
			*/
		}

		private void ViewCellPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			/*
			ViewCell viewCell = (ViewCell)sender;
			ViewCellRenderer.ViewTableCell cell = (ViewCellRenderer.ViewTableCell)CellRenderer.GetRealCell ((BindableObject)viewCell);
			if (!(e.PropertyName == Cell.IsEnabledProperty.PropertyName))
				return;
			ViewCellRenderer.UpdateIsEnabled (cell, viewCell);
			*/
		}

		private static void UpdateIsEnabled (NSTableCellView cell, ViewCell viewCell)
		{
			//cell.UserInteractionEnabled = viewCell.IsEnabled;
			cell.TextField.Enabled = viewCell.IsEnabled;
		}

		/*
		internal class ViewTableCell : NSTableCellView, INativeElementView
		{
			private ViewCell viewCell;
			private WeakReference<IVisualElementRenderer> rendererRef;

			Element INativeElementView.Element
			{
				get
				{
					return (Element)this.ViewCell;
				}
			}

			public ViewCell ViewCell
			{
				get
				{
					return this.viewCell;
				}
				set
				{
					if (this.viewCell == value)
						return;
					this.UpdateCell (value);
				}
			}

			public ViewTableCell (string key)
				: base (UITableViewCellStyle.Default, key)
			{
			}

			private void UpdateCell (ViewCell cell)
			{
				if (this.viewCell != null)
					Device.BeginInvokeOnMainThread (new Action (((Cell)this.viewCell).SendDisappearing));
				this.viewCell = cell;
				Device.BeginInvokeOnMainThread (new Action (((Cell)this.viewCell).SendAppearing));
				IVisualElementRenderer target;
				if (this.rendererRef == null || !this.rendererRef.TryGetTarget (out target))
				{
					target = this.GetNewRenderer ();
				}
				else
				{
					if (target.Element != null && target == Platform.GetRenderer (target.Element))
						target.Element.ClearValue (Platform.RendererProperty);
					Type handlerType = Registrar.Registered.GetHandlerType ((this.viewCell.View).GetType ());
					if (target.GetType () == handlerType || target is Platform.DefaultRenderer && handlerType == (Type)null)
					{
						target.SetElement ((VisualElement)this.viewCell.View);
					}
					else
					{
						(target.Element.Platform as Platform).DisposeRendererAndChildren (target);
						target = this.GetNewRenderer ();
					}
				}
				Platform.SetRenderer ((VisualElement)this.viewCell.View, target);
			}

			private IVisualElementRenderer GetNewRenderer ()
			{
				IVisualElementRenderer renderer = Platform.CreateRenderer ((VisualElement)this.viewCell.View);
				this.rendererRef = new WeakReference<IVisualElementRenderer> (renderer);
				// ISSUE: reference to a compiler-generated method
				this.ContentView.AddSubview (renderer.NativeView);
				return renderer;
			}

			public override CGSize SizeThatFits (CGSize size)
			{
				IVisualElementRenderer target;
				if (!this.rendererRef.TryGetTarget (out target))
				{
					// ISSUE: reference to a compiler-generated method
					return base.SizeThatFits (size);
				}
				double widthConstraint = (double)size.Width;
				double heightConstraint = size.Height > (nfloat)0 ? (double)size.Height : double.PositiveInfinity;
				SizeRequest sizeRequest = target.Element.GetSizeRequest (widthConstraint, heightConstraint);
				return new CGSize (size.Width, (nfloat)((float)sizeRequest.Request.Height) + (nfloat)1f / UIScreen.MainScreen.Scale);
			}

			public override void LayoutSubviews ()
			{
				// ISSUE: reference to a compiler-generated method
				base.LayoutSubviews ();
				CGRect frame = this.ContentView.Frame;
				this.ViewCell.View.Layout (RectangleExtensions.ToRectangle (frame));
				IVisualElementRenderer target;
				if (this.rendererRef == null || !this.rendererRef.TryGetTarget (out target))
					return;
				target.NativeView.Frame = frame;
			}

			protected override void Dispose (bool disposing)
			{
				IVisualElementRenderer target;
				if (disposing && this.rendererRef != null && (this.rendererRef.TryGetTarget (out target) && target.Element != null))
				{
					Platform platform = target.Element.Platform as Platform;
					if (platform != null)
						platform.DisposeModelAndChildrenRenderers ((Element)target.Element);
					this.rendererRef = (WeakReference<IVisualElementRenderer>)null;
				}
				// ISSUE: reference to a compiler-generated method
				base.Dispose (disposing);
			}
		}
		*/
	}
}
