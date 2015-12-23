using Foundation;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class ImageCellRenderer : TextCellRenderer
  {
		public override NSTableCellView GetCell(Cell item, NSTableCellView reusableCell, NSTableView tv)
		{
			CellTableViewCell cell = (CellTableViewCell)base.GetCell(item, reusableCell, tv);
			this.SetImage((ImageCell)item, cell);
			return cell;
		}

		protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			CellTableViewCell cellTableViewCell = (CellTableViewCell)sender;
			ImageCell cell = (ImageCell)cellTableViewCell.Cell;
			base.HandlePropertyChanged(sender, args);
			if (args.PropertyName == ImageCell.ImageSourceProperty.PropertyName)
			{
				SetImage(cell, cellTableViewCell);
			}
		}

		private void SetImage(ImageCell cell, CellTableViewCell target)
		{
			// TODO: WTF ?
			/*
			ImageCellRenderer.<SetImage>d__2 stateMachine;
			stateMachine.cell = cell;
			stateMachine.target = target;
			stateMachine.<>t__builder = AsyncVoidMethodBuilder.Create();
			stateMachine.<>1__state = -1;
			stateMachine.<>t__builder.Start<ImageCellRenderer.<SetImage>d__2>(ref stateMachine);
			*/
		}

		/*
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			public CellTableViewCell target;

			public <>c__DisplayClass2_0()
			{
				base.\u002Ector();
			}
		}

		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_1
		{
			public UIImage uiimage;
			public ImageCellRenderer.<>c__DisplayClass2_0 CS\u0024<>8__locals1;

			public <>c__DisplayClass2_1()
			{
				base.\u002Ector();
			}

			internal void <SetImage>b__0()
			{
				this.CS\u0024<>8__locals1.target.ImageView.Image = this.uiimage;
				this.CS\u0024<>8__locals1.target.SetNeedsLayout();
			}
		}

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <SetImage>d__2 : IAsyncStateMachine
		{
			public int <>1__state;
			public AsyncVoidMethodBuilder <>t__builder;
			public CellTableViewCell target;
			public ImageCell cell;
			private ImageCellRenderer.<>c__DisplayClass2_1 <>8__1;
			private ImageCellRenderer.<>c__DisplayClass2_0 <>8__2;
			private ImageCellRenderer.<>c__DisplayClass2_1 <>7__wrap1;
			private ConfiguredTaskAwaitable<UIImage>.ConfiguredTaskAwaiter <>u__1;

			void IAsyncStateMachine.MoveNext()
			{
				int num1 = this.<>1__state;
				try
				{
					ImageSource imageSource;
					IImageSourceHandler imageSourceHandler;
					if (num1 != 0)
					{
						this.<>8__2 = new ImageCellRenderer.<>c__DisplayClass2_0();
						this.<>8__2.target = this.target;
						imageSource = this.cell.ImageSource;
						this.<>8__2.target.ImageView.Image = (UIImage) null;
						if (imageSource != null && (M0) (imageSourceHandler = (IImageSourceHandler) Registrar.Registered.GetHandler<IImageSourceHandler>(( imageSource).GetType())) != null)
						{
							this.<>8__1 = new ImageCellRenderer.<>c__DisplayClass2_1();
							this.<>8__1.CS\u0024<>8__locals1 = this.<>8__2;
						}
						else
						{
							this.<>8__2.target.ImageView.Image = (UIImage) null;
							goto label_14;
						}
					}
					try
					{
						ConfiguredTaskAwaitable<UIImage>.ConfiguredTaskAwaiter awaiter;
						int num2;
						if (num1 != 0)
						{
							this.<>7__wrap1 = this.<>8__1;
							UIImage uiImage = this.<>7__wrap1.uiimage;
							awaiter = imageSourceHandler.LoadImageAsync(imageSource, new CancellationToken(), 1f).ConfigureAwait(false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								this.<>1__state = num2 = 0;
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaitable<UIImage>.ConfiguredTaskAwaiter, ImageCellRenderer.<SetImage>d__2>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = this.<>u__1;
							this.<>u__1 = new ConfiguredTaskAwaitable<UIImage>.ConfiguredTaskAwaiter();
							this.<>1__state = num2 = -1;
						}
						UIImage result = awaiter.GetResult();
						awaiter = new ConfiguredTaskAwaitable<UIImage>.ConfiguredTaskAwaiter();
						this.<>7__wrap1.uiimage = result;
						this.<>7__wrap1 = (ImageCellRenderer.<>c__DisplayClass2_1) null;
					}
					catch (TaskCanceledException ex)
					{
						this.<>8__1.uiimage = (UIImage) null;
					}
					// ISSUE: method pointer
					NSRunLoop.Main.BeginInvokeOnMainThread(new Action( this.<>8__1, __methodptr(<SetImage>b__0)));
					this.<>8__1 = (ImageCellRenderer.<>c__DisplayClass2_1) null;
				}
				catch (Exception ex)
				{
					this.<>1__state = -2;
					this.<>t__builder.SetException(ex);
					return;
				}
				label_14:
				this.<>1__state = -2;
				this.<>t__builder.SetResult();
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				this.<>t__builder.SetStateMachine(stateMachine);
			}
		}
		*/

  }
}
