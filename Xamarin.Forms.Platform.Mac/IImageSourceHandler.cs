using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public interface IImageSourceHandler : IRegisterable
  {
    Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = null, float scale = 1f);
  }
}
