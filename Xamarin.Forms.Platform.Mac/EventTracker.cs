using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class EventTracker : IDisposable
	{
		private double previousScale = 1.0;
		//private readonly Dictionary<IGestureRecognizer, UIGestureRecognizer> gestureRecognizers = new Dictionary<IGestureRecognizer, UIGestureRecognizer>();
		private bool disposed;
		private NSView handler;
		private readonly IVisualElementRenderer renderer;
		//private UITouchEventArgs shouldReceive;
		private readonly NotifyCollectionChangedEventHandler collectionChangedHandler;


		/*
		private ObservableCollection<IGestureRecognizer> ElementGestureRecognizers
		{
			get
			{
				IVisualElementRenderer visualElementRenderer = this.renderer;
				if ((visualElementRenderer != null ? visualElementRenderer.Element : (VisualElement)null) is View)
					return ((View)this.renderer.Element).get_GestureRecognizers () as ObservableCollection<IGestureRecognizer>;
				return (ObservableCollection<IGestureRecognizer>)null;
			}
		}
		*/

		public EventTracker (IVisualElementRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException ("renderer");
			//this.collectionChangedHandler =  new NotifyCollectionChangedEventHandler (this.ModelGestureRecognizersOnCollectionChanged);
			this.renderer = renderer;
			this.renderer.ElementChanged += new EventHandler<VisualElementChangedEventArgs> (this.OnElementChanged);
		}

		public void Dispose ()
		{
			if (this.disposed)
				return;
			this.disposed = true;
			/*
			foreach (KeyValuePair<IGestureRecognizer, UIGestureRecognizer> keyValuePair in this.gestureRecognizers)
			{
				this.handler.RemoveGestureRecognizer (keyValuePair.Value);
				keyValuePair.Value.Dispose ();
			}
			this.gestureRecognizers.Clear ();
			if (this.ElementGestureRecognizers != null)
				this.ElementGestureRecognizers.CollectionChanged -= this.collectionChangedHandler;
				*/
			this.handler = null;
		}

		public void LoadEvents (NSView handler)
		{
			if (this.disposed)
				throw new ObjectDisposedException ((string)null);
			//this.shouldReceive = (UITouchEventArgs)((r, t) => t.View is IVisualElementRenderer);
			this.handler = handler;
			this.OnElementChanged (this, new VisualElementChangedEventArgs ((VisualElement)null, this.renderer.Element));
		}

		/*
		protected virtual UIGestureRecognizer GetNativeRecognizer (IGestureRecognizer recognizer)
		{
			if (recognizer == null)
				return (UIGestureRecognizer)null;
			WeakReference weakRecognizer = new WeakReference (recognizer);
			WeakReference weakEventTracker = new WeakReference (this);
			TapGestureRecognizer gestureRecognizer1 = recognizer as TapGestureRecognizer;
			if (gestureRecognizer1 != null)
				return (UIGestureRecognizer)this.CreateTapRecognizer (1, gestureRecognizer1.NumberOfTapsRequired, (Action<UITapGestureRecognizer>)(r =>
				{
					TapGestureRecognizer gestureRecognizer2 = weakRecognizer.Target as TapGestureRecognizer;
					EventTracker eventTracker = weakEventTracker.Target as EventTracker;
					VisualElement visualElement;
					if (eventTracker == null)
					{
						visualElement = (VisualElement)null;
					}
					else
					{
						IVisualElementRenderer visualElementRenderer = eventTracker.renderer;
						visualElement = visualElementRenderer != null ? visualElementRenderer.Element : (VisualElement)null;
					}
					View sender = visualElement as View;
					if (gestureRecognizer2 == null || sender == null)
						return;
					gestureRecognizer2.SendTapped (sender);
				}));
			if (!(recognizer is PinchGestureRecognizer))
				return (UIGestureRecognizer)null;
			double startingScale = 1.0;
			return (UIGestureRecognizer)this.CreatePinchRecognizer ((Action<UIPinchGestureRecognizer>)(r =>
			{
				IPinchGestureController gestureController = weakRecognizer.Target as IPinchGestureController;
				EventTracker eventTracker = weakEventTracker.Target as EventTracker;
				VisualElement visualElement;
				if (eventTracker == null)
				{
					visualElement = (VisualElement)null;
				}
				else
				{
					IVisualElementRenderer visualElementRenderer = eventTracker.renderer;
					visualElement = visualElementRenderer != null ? visualElementRenderer.Element : (VisualElement)null;
				}
				View view = visualElement as View;
				if (gestureController == null || eventTracker == null || view == null)
					return;
				double num1 = eventTracker.previousScale;
				// ISSUE: reference to a compiler-generated method
				// ISSUE: reference to a compiler-generated method
				CGPoint cgPoint = NSApplication.SharedApplication.KeyWindow.ConvertPointToView (r.LocationInView ((NSView)null), eventTracker.renderer.NativeView);
				Point point = new Point ((double)cgPoint.X / view.Width, (double)cgPoint.Y / view.Height);
				long num2 = (long)(r.State - 1L);
				long num3 = 4;
				if ((ulong)num2 > (ulong)num3)
					return;
				switch ((uint)num2)
				{
				case 0:
					if (r.NumberOfTouches < (nint)2)
						break;
					gestureController.SendPinchStarted ((Element)view, point);
					startingScale = view.Scale;
					break;
				case 1:
					if (r.NumberOfTouches < (nint)2 && gestureController.IsPinching)
					{
						r.State = UIGestureRecognizerState.Ended;
						gestureController.SendPinchEnded ((Element)view);
						break;
					}
					double scale = 1.0;
					double num4 = Math.Abs ((double)r.Scale - num1) * startingScale;
					if (num1 < (double)r.Scale)
						scale = 1.0 + num4;
					if (num1 > (double)r.Scale)
						scale = 1.0 - num4;
					gestureController.SendPinch ((Element)view, scale, point);
					eventTracker.previousScale = (double)r.Scale;
					break;
				case 2:
					if (gestureController.IsPinching)
						gestureController.SendPinchEnded ((Element)view);
					eventTracker.previousScale = 1.0;
					break;
				case 3:
				case 4:
					if (!gestureController.IsPinching)
						break;
					gestureController.SendPinchCanceled ((Element)view);
					break;
				}
			}));
		}

		private void ModelGestureRecognizersOnCollectionChanged (object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			this.LoadRecognizers ();
		}
		*/

		private void OnElementChanged (object sender, VisualElementChangedEventArgs e)
		{
			/*
			if (e.OldElement != null)
			{
				View view = e.OldElement as View;
				if (view != null)
					((ObservableCollection<IGestureRecognizer>)view.get_GestureRecognizers ()).CollectionChanged -= this.collectionChangedHandler;
			}
			if (e.NewElement == null || this.ElementGestureRecognizers == null)
				return;
			this.ElementGestureRecognizers.CollectionChanged += this.collectionChangedHandler;
			this.LoadRecognizers ();
			*/
		}

		/*
		private void LoadRecognizers ()
		{
			if (this.ElementGestureRecognizers == null)
				return;
			foreach (IGestureRecognizer index in (Collection<IGestureRecognizer>) this.ElementGestureRecognizers)
			{
				if (!this.gestureRecognizers.ContainsKey (index))
				{
					UIGestureRecognizer nativeRecognizer = this.GetNativeRecognizer (index);
					if (nativeRecognizer != null)
					{
						nativeRecognizer.ShouldReceiveTouch = this.shouldReceive;
						// ISSUE: reference to a compiler-generated method
						this.handler.AddGestureRecognizer (nativeRecognizer);
						this.gestureRecognizers [index] = nativeRecognizer;
					}
				}
			}
			foreach (IGestureRecognizer key in Enumerable.ToArray<IGestureRecognizer>(Enumerable.Where<IGestureRecognizer>((IEnumerable<IGestureRecognizer>) this.gestureRecognizers.Keys, (Func<IGestureRecognizer, bool>) (key => !this.ElementGestureRecognizers.Contains(key)))))
			{
				UIGestureRecognizer gestureRecognizer = this.gestureRecognizers [key];
				this.gestureRecognizers.Remove (key);
				// ISSUE: reference to a compiler-generated method
				this.handler.RemoveGestureRecognizer (gestureRecognizer);
				gestureRecognizer.Dispose ();
			}
		}

		private UITapGestureRecognizer CreateTapRecognizer (int numFingers, int numTaps, Action<UITapGestureRecognizer> action)
		{
			return new UITapGestureRecognizer (action) {
				NumberOfTouchesRequired = (nuint)((uint)numFingers),
				NumberOfTapsRequired = (nuint)((uint)numTaps)
			};
		}

		private UIPinchGestureRecognizer CreatePinchRecognizer (Action<UIPinchGestureRecognizer> action)
		{
			return new UIPinchGestureRecognizer (action);
		}
		*/
	}
}
