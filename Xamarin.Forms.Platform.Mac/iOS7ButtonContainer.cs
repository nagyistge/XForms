using CoreGraphics;
using System;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	internal class iOS7ButtonContainer : NSView
	{
		private nfloat buttonWidth;

		public iOS7ButtonContainer (nfloat buttonWidth)
			: base (new CGRect ((nfloat)0, (nfloat)0, (nfloat)0, (nfloat)0))
		{
			this.buttonWidth = buttonWidth;
			this.ClipsToBounds = true;
		}

		public override void LayoutSubviews ()
		{
			nfloat width1 = this.Frame.Width;
			nfloat nfloat1 = (nfloat)0;
			for (int index = 0; index < this.Subviews.Length; ++index)
			{
				NSView uiView1 = this.Subviews [index];
				int num = this.Subviews.Length - index;
				nfloat nfloat2 = width1 - this.buttonWidth * (nfloat)num;
				NSView uiView2 = uiView1;
				nfloat x = nfloat2;
				nfloat y = (nfloat)0;
				nfloat width2 = uiView1.Frame.Width;
				CGRect frame = uiView1.Frame;
				nfloat height = frame.Height;
				CGRect cgRect = new CGRect (x, y, width2, height);
				uiView2.Frame = cgRect;
				nfloat nfloat3 = nfloat1;
				frame = uiView1.Frame;
				nfloat width3 = frame.Width;
				nfloat1 = nfloat3 + width3;
			}
		}
	}
}
