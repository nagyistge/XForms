using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public static class NSViewExtensions
	{
		public static IEnumerable<NSView> Descendants (this NSView self)
		{
			if (self.Subviews == null)
				return Enumerable.Empty<NSView> ();
			return Enumerable.Concat<NSView> ((IEnumerable<NSView>)self.Subviews, Enumerable.SelectMany<NSView, NSView> ((IEnumerable<NSView>)self.Subviews, (Func<NSView, IEnumerable<NSView>>)(s => NSViewExtensions.Descendants (s))));
		}

		public static SizeRequest GetSizeRequest (
			this NSControl self, 
			double widthConstraint, 
			double heightConstraint, 
			double minimumWidth = -1.0, 
			double minimumHeight = -1.0)
		{
			if (self == null)
				return new SizeRequest (
					new Size (widthConstraint, heightConstraint), 
					new Size (minimumWidth, minimumHeight));

			CGSize cgSize = self.SizeThatFits ((CGSize)new SizeF ((float)widthConstraint, (float)heightConstraint));
			Xamarin.Forms.Size request = new Xamarin.Forms.Size (cgSize.Width == (nfloat)float.PositiveInfinity ? double.PositiveInfinity : (double)cgSize.Width, cgSize.Height == (nfloat)float.PositiveInfinity ? double.PositiveInfinity : (double)cgSize.Height);
			Xamarin.Forms.Size minimum = new Xamarin.Forms.Size (minimumWidth < 0.0 ? request.Width : minimumWidth, minimumHeight < 0.0 ? request.Height : minimumHeight);
			return new SizeRequest (request, minimum);
		}

		internal static T FindDescendantView<T> (this NSView view) where T : NSView
		{
			Queue<NSView> queue = new Queue<NSView> ();
			queue.Enqueue (view);
			while (queue.Count > 0)
			{
				NSView uiView = queue.Dequeue ();
				T obj = uiView as T;
				if ((object)obj != null)
					return obj;
				for (int index = 0; index < uiView.Subviews.Length; ++index)
					queue.Enqueue (uiView.Subviews [index]);
			}
			return default (T);
		}

		internal static NSView FindFirstResponder (this NSView view)
		{
			//if (view.IsFirstResponder)
			if (view.Window.FirstResponder == view)
				return view;
			foreach (NSView view1 in view.Subviews)
			{
				NSView firstResponder = NSViewExtensions.FindFirstResponder (view1);
				if (firstResponder != null)
					return firstResponder;
			}
			return (NSView)null;
		}

		internal static bool IsFirstResponder(this NSView self)
		{
			if (self == null)
				return false;
			return self.Window.FirstResponder == self;
		}
	}
}
