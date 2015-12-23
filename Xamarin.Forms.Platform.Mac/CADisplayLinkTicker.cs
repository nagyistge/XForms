using CoreAnimation;
using Foundation;
using System;
using System.Collections.Concurrent;
using System.Threading;
using AppKit;
using Xamarin.Forms;
using CoreVideo;

namespace Xamarin.Forms.Platform.Mac
{
	internal class CADisplayLinkTicker : Ticker
	{
		private readonly BlockingCollection<Action> queue = new BlockingCollection<Action> ();
		private CVDisplayLink link;

		internal static CADisplayLinkTicker Default
		{
			get
			{
				return Ticker.Default as CADisplayLinkTicker;
			}
		}

		public CADisplayLinkTicker ()
		{
			new Thread (new ThreadStart (this.StartThread)).Start ();
		}

		public void Invoke (Action action)
		{
			this.queue.Add (action);
		}

		protected override void EnableTimer ()
		{
			this.link = Xamarin CoreAnimation.CADisplayLink.Create(() => this.SendSignals (-1));
			this.link.Start ();
			// TODO: Does this work?
			//this.link.AddToRunLoop (NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
		}

		protected override void DisableTimer ()
		{
			if (this.link != null)
			{
				this.link.Stop ();
				// TODO: Does this work?
				//this.link.RemoveFromRunLoop (NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
				this.link.Dispose ();
			}
			this.link = null;
		}

		private void StartThread ()
		{
			while (true)
			{
				Action action = this.queue.Take ();

				bool flag = NSApplication.CheckForIllegalCrossThreadCalls;
				NSApplication.CheckForIllegalCrossThreadCalls = false;

				CATransaction.Begin ();
				action ();
				while (this.queue.TryTake (out action))
					action ();
				CATransaction.Commit ();

				NSApplication.CheckForIllegalCrossThreadCalls = flag;
			}
		}
	}
}
