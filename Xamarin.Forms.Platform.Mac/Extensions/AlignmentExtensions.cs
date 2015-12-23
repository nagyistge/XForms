using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	internal static class AlignmentExtensions
	{
		internal static NSTextAlignment ToNativeTextAlignment (this TextAlignment alignment)
		{
			if (alignment == TextAlignment.Center)
				return NSTextAlignment.Center;
			return alignment == TextAlignment.End ? NSTextAlignment.Right : NSTextAlignment.Left;
		}
	}
}
