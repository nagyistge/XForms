using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public sealed class FileImageSourceHandler : IImageSourceHandler, IRegisterable
	{
		public Task<NSImage> LoadImageAsync (
			ImageSource imagesource, 
			CancellationToken cancelationToken)
		{
			NSImage result = null;

			FileImageSource fileImageSource = imagesource as FileImageSource;
			if (fileImageSource != null)
			{
				string file = fileImageSource.File;
				if (!string.IsNullOrEmpty (file))
				{
					result = new NSImage (file);
					//result = File.Exists (file) ? new NSImage (file) : NSImage.FromBundle (file);
				}
			}
			return Task.FromResult<NSImage> (result);
		}
	}
}
