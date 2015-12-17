using Foundation;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ImageCellRenderer : TextCellRenderer
  {
    public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
    {
      CellTableViewCell target = (CellTableViewCell) base.GetCell(item, reusableCell, tv);
      this.SetImage((ImageCell) item, target);
      return (UITableViewCell) target;
    }

    protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      CellTableViewCell target = (CellTableViewCell) sender;
      ImageCell cell = (ImageCell) target.Cell;
      base.HandlePropertyChanged(sender, args);
      if (!(args.PropertyName == ImageCell.ImageSourceProperty.PropertyName))
        return;
      this.SetImage(cell, target);
    }

    private async void SetImage(ImageCell cell, CellTableViewCell target)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ImageCellRenderer.\u003C\u003Ec__DisplayClass2_0 cDisplayClass20 = new ImageCellRenderer.\u003C\u003Ec__DisplayClass2_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass20.target = target;
      ImageSource imageSource = cell.ImageSource;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass20.target.ImageView.Image = (UIImage) null;
      IImageSourceHandler imageSourceHandler;
      if (imageSource != null && (M0) (imageSourceHandler = (IImageSourceHandler) Registrar.Registered.GetHandler<IImageSourceHandler>(((object) imageSource).GetType())) != null)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        ImageCellRenderer.\u003C\u003Ec__DisplayClass2_1 cDisplayClass21_1 = new ImageCellRenderer.\u003C\u003Ec__DisplayClass2_1();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass21_1.CS\u0024\u003C\u003E8__locals1 = cDisplayClass20;
        try
        {
          // ISSUE: variable of a compiler-generated type
          ImageCellRenderer.\u003C\u003Ec__DisplayClass2_1 cDisplayClass21_2 = cDisplayClass21_1;
          // ISSUE: reference to a compiler-generated field
          UIImage uiImage1 = cDisplayClass21_2.uiimage;
          UIImage uiImage2 = await imageSourceHandler.LoadImageAsync(imageSource, new CancellationToken(), 1f).ConfigureAwait(false);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass21_2.uiimage = uiImage2;
          cDisplayClass21_2 = (ImageCellRenderer.\u003C\u003Ec__DisplayClass2_1) null;
        }
        catch (TaskCanceledException ex)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass21_1.uiimage = (UIImage) null;
        }
        // ISSUE: reference to a compiler-generated method
        NSRunLoop.Main.BeginInvokeOnMainThread(new Action(cDisplayClass21_1.\u003CSetImage\u003Eb__0));
        cDisplayClass21_1 = (ImageCellRenderer.\u003C\u003Ec__DisplayClass2_1) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        cDisplayClass20.target.ImageView.Image = (UIImage) null;
      }
    }
  }
}
