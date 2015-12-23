using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class VisualElementChangedEventArgs : ElementChangedEventArgs<VisualElement>
	{
		public VisualElementChangedEventArgs (VisualElement oldElement, VisualElement newElement)
			: base (oldElement, newElement)
		{
		}
	}
}
