using Foundation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public sealed class ImageLoaderSourceHandler : IImageSourceHandler, IRegisterable
  {
    public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = null, float scale = 1f)
    {
      UIImage image = (UIImage) null;
      UriImageSource uriImageSource = imagesource as UriImageSource;
      if (uriImageSource != null && uriImageSource.get_Uri() != (Uri) null)
      {
        using (Stream stream = await uriImageSource.GetStreamAsync(cancelationToken).ConfigureAwait(false))
        {
          if (stream != null)
          {
            // ISSUE: reference to a compiler-generated method
            image = UIImage.LoadFromData(NSData.FromStream(stream), (nfloat) scale);
          }
        }
      }
      return image;
    }
  }
}
