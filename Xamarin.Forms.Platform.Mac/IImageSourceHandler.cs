using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public interface IImageSourceHandler : IRegisterable
  {
    Task<NSImage> LoadImageAsync(
			ImageSource imagesource, 
			CancellationToken cancelationToken);
  }
}
