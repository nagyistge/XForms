using Foundation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public sealed class StreamImagesourceHandler : IImageSourceHandler, IRegisterable
  {
    public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = null, float scale = 1f)
    {
      UIImage image = (UIImage) null;
      StreamImageSource streamImageSource = imagesource as StreamImageSource;
      if (streamImageSource != null && streamImageSource.get_Stream() != null)
      {
        Stream stream = await streamImageSource.GetStreamAsync(cancelationToken).ConfigureAwait(false);
        if (stream != null)
        {
          // ISSUE: reference to a compiler-generated method
          image = UIImage.LoadFromData(NSData.FromStream(stream), (nfloat) scale);
        }
      }
      return image;
    }
  }
}
