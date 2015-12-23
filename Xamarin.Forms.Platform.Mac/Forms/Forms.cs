using CoreFoundation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Xamarin.Forms.Platform.Mac;
using System.Net.Http;

namespace Xamarin.Forms
{
	public static class Forms
	{
		private static bool nevertrue;

		public static bool IsInitialized { get; private set; }

		public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

		static Forms ()
		{
			if (!Forms.nevertrue)
				return;
			Assembly.GetCallingAssembly ();
		}

		public static void Init ()
		{
			if (Forms.IsInitialized)
				return;
			Forms.IsInitialized = true;
			Color.Accent = Color.FromRgba (50, 79, 133, (int)byte.MaxValue);

			Log.Listeners.Add ( new DelegateLogListener ((c, m) =>
				{
					Console.WriteLine("{0}: {1}", c, m);
				}));

			Device.OS = TargetPlatform.Other;
			Device.PlatformServices = (IPlatformServices)new Forms.MacPlatformServices ();
			Device.Info = (DeviceInfo)new Forms.MacDeviceInfo ();

			// TODO: CADisplayLinkTicker
			//Ticker.Default = (Ticker)new CADisplayLinkTicker ();

			Registrar.RegisterAll (new Type[3] {
				typeof(ExportRendererAttribute),
				typeof(ExportCellAttribute),
				typeof(ExportImageSourceHandlerAttribute)
			});

			Device.Idiom = TargetIdiom.Desktop;
			ExpressionSearch.Default = (IExpressionSearch)new Forms.MacExpressionSearch ();
		}

		internal static void SendViewInitialized (this VisualElement self, NSView nativeView)
		{
			var eventHandler = Forms.ViewInitialized;
			if (eventHandler == null)
				return;
			
			eventHandler (self, new ViewInitializedEventArgs () 
			{
				View = self,
				NativeView = nativeView
			});
		}

		private class MacExpressionSearch : ExpressionVisitor, IExpressionSearch
		{
			private List<object> results;
			private Type targetType;

			public List<T> FindObjects<T> (Expression expression) where T : class
			{
				this.results = new List<object> ();
				this.targetType = typeof(T);
				this.Visit (expression);
				return Enumerable.ToList<T> (Enumerable.Select<object, T> ((IEnumerable<object>)this.results, (Func<object, T>)(o => o as T)));
			}

			protected override Expression VisitMember (MemberExpression node)
			{
				if (node.Expression is ConstantExpression && node.Member is FieldInfo)
				{
					object obj = ((ConstantExpression)node.Expression).Value;
					object o = ((FieldInfo)node.Member).GetValue (obj);
					if (this.targetType.IsInstanceOfType (o))
						this.results.Add (o);
				}
				return base.VisitMember (node);
			}
		}

		internal class MacDeviceInfo : DeviceInfo
		{
			private NSObject notification;
			private readonly Size pixelScreenSize;
			private readonly Size scaledScreenSize;
			private readonly double scalingFactor;

			public override Size PixelScreenSize
			{
				get	{return this.pixelScreenSize;}
			}

			public override Size ScaledScreenSize
			{
				get	{return this.scaledScreenSize;}
			}

			public override double ScalingFactor
			{
				get	{return this.scalingFactor;}
			}

			public MacDeviceInfo ()
			{
				//this.notification = UIDevice.Notifications.ObserveOrientationDidChange ((EventHandler<NSNotificationEventArgs>)((sender, args) => this.CurrentOrientation = Extensions.ToDeviceOrientation (UIDevice.CurrentDevice.Orientation)));

				this.scalingFactor = (double)NSScreen.MainScreen.BackingScaleFactor;
				CGRect bounds = NSScreen.MainScreen.Frame;

				double width = (double)bounds.Width;
				double height = (double)bounds.Height;

				scaledScreenSize = new Size (width, height);
				pixelScreenSize = new Size (scaledScreenSize.Width * scalingFactor, scaledScreenSize.Height * scalingFactor);
			}

			protected override void Dispose (bool disposing)
			{
				this.notification.Dispose ();
				base.Dispose (disposing);
			}
		}

		private class MacPlatformServices : IPlatformServices
		{
			private static MD5CryptoServiceProvider checksum = new MD5CryptoServiceProvider ();

			public bool IsInvokeRequired
			{
				get
				{
					return !NSThread.IsMain;
				}
			}

			public string GetMD5Hash (string input)
			{
				byte[] hash = Forms.MacPlatformServices.checksum.ComputeHash (Encoding.UTF8.GetBytes (input));
				char[] chArray = new char[32];
				for (int index = 0; index < 16; ++index)
				{
					chArray [index * 2] = (char)hex ((int)hash [index] >> 4);
					chArray [index * 2 + 1] = (char)hex ((int)hash [index] & 15);
				}
				return new string (chArray);
			}

			private static int hex (int v)
			{
				if (v < 10)
					return 48 + v;
				return 97 + v - 10;
			}

			public double GetNamedSize (NamedSize size, Type targetElementType, bool useOldSizes)
			{
				switch (size)
				{
				case NamedSize.Default:
					return typeof(Button).IsAssignableFrom (targetElementType) ? 15.0 : 17.0;
				case NamedSize.Micro:
					return 12.0;
				case NamedSize.Small:
					return 14.0;
				case NamedSize.Medium:
					return 17.0;
				case NamedSize.Large:
					return 22.0;
				default:
					throw new ArgumentOutOfRangeException ("size");
				}
			}

			public void OpenUriAction (Uri uri)
			{
				NSWorkspace.SharedWorkspace.OpenUrl (new NSUrl (uri.AbsoluteUri));
			}

			public void BeginInvokeOnMainThread (Action action)
			{
				NSRunLoop.Main.BeginInvokeOnMainThread (new Action (action.Invoke));
			}

			public void StartTimer (TimeSpan interval, Func<bool> callback)
			{
				NSRunLoop.Main.AddTimer (NSTimer.CreateRepeatingScheduledTimer (interval, (Action<NSTimer>)(t =>
				{
					if (callback ())
						return;
					// ISSUE: reference to a compiler-generated method
					t.Invalidate ();
				})), NSRunLoopMode.Common);
			}

			public async Task<Stream> GetStreamAsync (Uri uri, CancellationToken cancellationToken)
			{
				HttpClient client = GetHttpClient();
				Stream stream;
				try
				{
					using (HttpResponseMessage async = await client.GetAsync (uri, cancellationToken))
						stream = await async.Content.ReadAsStreamAsync ();
				}
				finally
				{
					if (client != null)
						client.Dispose ();
				}
				return stream;
			}

			private HttpClient GetHttpClient ()
			{
				HttpClientHandler httpClientHandler = new HttpClientHandler ();
				//TODO: Is proxy important for Mac?
				/*
				CFProxySettings systemProxySettings = CFNetwork.GetSystemProxySettings ();
				if (!string.IsNullOrEmpty (systemProxySettings.HTTPProxy))
				{
					httpClientHandler.Proxy = CFNetwork.GetDefaultProxy ();
					httpClientHandler.UseProxy = true;
				}
				*/
				return new HttpClient ((HttpMessageHandler)httpClientHandler);
			}

			public Assembly[] GetAssemblies ()
			{
				return AppDomain.CurrentDomain.GetAssemblies ();
			}

			public ITimer CreateTimer (Action<object> callback)
			{
				return (ITimer)new Forms.MacPlatformServices._Timer (new System.Threading.Timer ((TimerCallback)(o => callback (o))));
			}

			public ITimer CreateTimer (Action<object> callback, object state, int dueTime, int period)
			{
				return (ITimer)new Forms.MacPlatformServices._Timer (new System.Threading.Timer ((TimerCallback)(o => callback (o)), state, dueTime, period));
			}

			public ITimer CreateTimer (Action<object> callback, object state, long dueTime, long period)
			{
				return (ITimer)new Forms.MacPlatformServices._Timer (new System.Threading.Timer ((TimerCallback)(o => callback (o)), state, dueTime, period));
			}

			public ITimer CreateTimer (Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
			{
				return (ITimer)new Forms.MacPlatformServices._Timer (new System.Threading.Timer ((TimerCallback)(o => callback (o)), state, dueTime, period));
			}

			public ITimer CreateTimer (Action<object> callback, object state, uint dueTime, uint period)
			{
				return (ITimer)new Forms.MacPlatformServices._Timer (new System.Threading.Timer ((TimerCallback)(o => callback (o)), state, dueTime, period));
			}

			public IIsolatedStorageFile GetUserStoreForApplication ()
			{
				return (IIsolatedStorageFile)new Forms.MacPlatformServices._IsolatedStorageFile (IsolatedStorageFile.GetUserStoreForApplication ());
			}

			public class _Timer : ITimer
			{
				private readonly System.Threading.Timer timer;

				public _Timer (System.Threading.Timer timer)
				{
					this.timer = timer;
				}

				public void Change (int dueTime, int period)
				{
					this.timer.Change (dueTime, period);
				}

				public void Change (long dueTime, long period)
				{
					this.timer.Change (dueTime, period);
				}

				public void Change (TimeSpan dueTime, TimeSpan period)
				{
					this.timer.Change (dueTime, period);
				}

				public void Change (uint dueTime, uint period)
				{
					this.timer.Change (dueTime, period);
				}
			}

			public class _IsolatedStorageFile : IIsolatedStorageFile
			{
				private readonly IsolatedStorageFile isolatedStorageFile;

				public _IsolatedStorageFile (IsolatedStorageFile isolatedStorageFile)
				{
					this.isolatedStorageFile = isolatedStorageFile;
				}

				public Task<bool> GetDirectoryExistsAsync (string path)
				{
					return Task.FromResult<bool> (this.isolatedStorageFile.DirectoryExists (path));
				}

				public Task CreateDirectoryAsync (string path)
				{
					this.isolatedStorageFile.CreateDirectory (path);
					return (Task)Task.FromResult<bool> (true);
				}

				public Task<Stream> OpenFileAsync (string path, FileMode mode, FileAccess access)
				{
					return Task.FromResult<Stream> ((Stream)this.isolatedStorageFile.OpenFile (path, (System.IO.FileMode)mode, (System.IO.FileAccess)access));
				}

				public Task<Stream> OpenFileAsync (string path, FileMode mode, FileAccess access, FileShare share)
				{
					return Task.FromResult<Stream> ((Stream)this.isolatedStorageFile.OpenFile (path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share));
				}

				public Task<bool> GetFileExistsAsync (string path)
				{
					return Task.FromResult<bool> (this.isolatedStorageFile.FileExists (path));
				}

				public Task<DateTimeOffset> GetLastWriteTimeAsync (string path)
				{
					return Task.FromResult<DateTimeOffset> (this.isolatedStorageFile.GetLastWriteTime (path));
				}
			}
		}
	}
}