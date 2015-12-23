using Foundation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public sealed class StreamImagesourceHandler : IImageSourceHandler, IRegisterable
	{
		public async Task<NSImage> LoadImageAsync (
			ImageSource imagesource, 
			CancellationToken cancelationToken)
		{
			NSImage image = null;
			StreamImageSource streamImageSource = imagesource as StreamImageSource;
			if (streamImageSource != null && streamImageSource.Stream != null)
			{
				Stream stream = await streamImageSource.GetStreamAsync (cancelationToken).ConfigureAwait (false);
				if (stream != null)
					image = new NSImage (NSData.FromStream (stream));
			}
			return image;
		}
	}
}
