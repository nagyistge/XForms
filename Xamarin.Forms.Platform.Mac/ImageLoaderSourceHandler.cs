using Foundation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public sealed class ImageLoaderSourceHandler : IImageSourceHandler, IRegisterable
	{
		public async Task<NSImage> LoadImageAsync (
			ImageSource imagesource, 
			CancellationToken cancelationToken)
		{
			NSImage image = null;
			UriImageSource uriImageSource = imagesource as UriImageSource;
			if (uriImageSource != null && uriImageSource.Uri != null)
			{
				using (Stream stream = await uriImageSource.GetStreamAsync (cancelationToken).ConfigureAwait (false))
				{
					if (stream != null)
						image = new NSImage (NSData.FromStream(stream));
				}
			}
			return image;
		}
	}
}
