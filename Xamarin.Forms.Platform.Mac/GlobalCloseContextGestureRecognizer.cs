using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	/*
	 * I don't think we need this for mac
	internal class GlobalCloseContextGestureRecognizer : UIGestureRecognizer
	{
		private List<NSButton> buttons;
		private NSScrollView scrollView;

		public GlobalCloseContextGestureRecognizer (NSScrollView scrollView, List<NSButton> buttons, Action activated)
			: base (activated)
		{
			this.scrollView = scrollView;
			this.buttons = buttons;
			this.ShouldReceiveTouch = new UITouchEventArgs (this.OnShouldReceiveTouch);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			this.State = UIGestureRecognizerState.Began;
			// ISSUE: reference to a compiler-generated method
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			this.State = UIGestureRecognizerState.Ended;
			// ISSUE: reference to a compiler-generated method
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			this.State = UIGestureRecognizerState.Ended;
			// ISSUE: reference to a compiler-generated method
			base.TouchesEnded (touches, evt);
		}

		protected override void Dispose (bool disposing)
		{
			// ISSUE: reference to a compiler-generated method
			base.Dispose (disposing);
			if (!disposing)
				return;
			this.buttons = (List<NSButton>)null;
			this.scrollView = (NSScrollView)null;
		}

		private bool OnShouldReceiveTouch (UIGestureRecognizer r, UITouch t)
		{
			CGPoint cGPoint = t.LocationInView (this.scrollView);
			nfloat _nfloat = 0;
			nfloat _nfloat1 = 0;
			nfloat width = this.scrollView.ContentSize.Width;
			CGSize contentSize = this.scrollView.ContentSize;
			CGRect cGRect = new CGRect (_nfloat, _nfloat1, width, contentSize.Height);
			return !cGRect.Contains (cGPoint);
		}

	}
	*/
}
