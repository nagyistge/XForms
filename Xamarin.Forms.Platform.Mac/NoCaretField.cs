using CoreGraphics;
using AppKit;

namespace Xamarin.Forms.Platform.Mac
{
	internal class NoCaretField : NSTextView
	{
		public NoCaretField ()
			: base (new CGRect ())
		{
			InsertionPointColor = NSColor.Clear;
		}

	}
}
