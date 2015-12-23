using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public static class ImageExtensions
	{
		public static NSImageScale ToNSImageScale(this Aspect aspect)
		{
			// TODO: AspectFill
			// https://github.com/onekiloparsec/KPCScaleToFillNSImageView
			switch (aspect)
			{
			case Aspect.AspectFill:
				return NSImageScale.ProportionallyUpOrDown;
			case Aspect.Fill:
				return NSImageScale.None;
			default:
				return NSImageScale.ProportionallyUpOrDown;
			}
		}

		/*
		public static UIViewContentMode ToUIViewContentMode (this Aspect aspect)
		{
			switch (aspect)
			{
			case Aspect.AspectFill:
				return UIViewContentMode.ScaleAspectFill;
			case Aspect.Fill:
				return UIViewContentMode.ScaleToFill;
			default:
				return UIViewContentMode.ScaleAspectFit;
			}
		}
		*/

	}
}
