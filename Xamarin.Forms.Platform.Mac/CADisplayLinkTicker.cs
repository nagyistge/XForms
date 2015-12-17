using CoreAnimation;
using Foundation;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  internal class CADisplayLinkTicker : Ticker
  {
    private readonly BlockingCollection<Action> queue = new BlockingCollection<Action>();
    private CADisplayLink link;

    internal static CADisplayLinkTicker Default
    {
      get
      {
        return Ticker.Default as CADisplayLinkTicker;
      }
    }

    public CADisplayLinkTicker()
    {
      new Thread(new ThreadStart(this.StartThread)).Start();
    }

    public void Invoke(Action action)
    {
      this.queue.Add(action);
    }

    protected override void EnableTimer()
    {
      this.link = CADisplayLink.Create((Action) (() => this.SendSignals(-1)));
      // ISSUE: reference to a compiler-generated method
      this.link.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
    }

    protected override void DisableTimer()
    {
      if (this.link != null)
      {
        // ISSUE: reference to a compiler-generated method
        this.link.RemoveFromRunLoop(NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
        this.link.Dispose();
      }
      this.link = (CADisplayLink) null;
    }

    private void StartThread()
    {
      while (true)
      {
        Action action = this.queue.Take();
        bool flag = UIApplication.CheckForIllegalCrossThreadCalls;
        UIApplication.CheckForIllegalCrossThreadCalls = false;
        // ISSUE: reference to a compiler-generated method
        CATransaction.Begin();
        action();
        while (this.queue.TryTake(out action))
          action();
        // ISSUE: reference to a compiler-generated method
        CATransaction.Commit();
        UIApplication.CheckForIllegalCrossThreadCalls = flag;
      }
    }
  }
}
