using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public sealed class FileImageSourceHandler : IImageSourceHandler, IRegisterable
  {
    public Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = null, float scale = 1f)
    {
      UIImage result = (UIImage) null;
      FileImageSource fileImageSource = imagesource as FileImageSource;
      if (fileImageSource != null)
      {
        string file = fileImageSource.File;
        if (!string.IsNullOrEmpty(file))
        {
          // ISSUE: reference to a compiler-generated method
          result = File.Exists(file) ? new UIImage(file) : UIImage.FromBundle(file);
        }
      }
      return Task.FromResult<UIImage>(result);
    }
  }
}
