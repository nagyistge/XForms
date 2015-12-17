using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class FormsApplicationDelegate : UIApplicationDelegate
  {
    private UIWindow window;
    private Application application;
    private bool isSuspended;

    protected FormsApplicationDelegate()
    {
    }

    protected void LoadApplication(Application application)
    {
      if (application == null)
        throw new ArgumentNullException("application");
      Application.Current = application;
      this.application = application;
      application.add_PropertyChanged(new PropertyChangedEventHandler(this.ApplicationOnPropertyChanged));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.application != null)
        this.application.remove_PropertyChanged(new PropertyChangedEventHandler(this.ApplicationOnPropertyChanged));
      base.Dispose(disposing);
    }

    private void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (!(args.PropertyName == "MainPage"))
        return;
      this.UpdateMainPage();
    }

    public override bool WillFinishLaunching(UIApplication uiApplication, NSDictionary launchOptions)
    {
      return true;
    }

    public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
    {
      this.window = new UIWindow(UIScreen.MainScreen.Bounds);
      if (this.application == null)
        throw new InvalidOperationException("You MUST invoke LoadApplication () before calling base.FinishedLaunching ()");
      this.SetMainPage();
      this.application.SendStart();
      return true;
    }

    public override void OnActivated(UIApplication uiApplication)
    {
      if (this.application == null || !this.isSuspended)
        return;
      this.isSuspended = false;
      this.application.SendResume();
    }

    public override void OnResignActivation(UIApplication uiApplication)
    {
      if (this.application == null)
        return;
      this.isSuspended = true;
      this.application.SendSleepAsync().Wait();
    }

    public override void DidEnterBackground(UIApplication uiApplication)
    {
    }

    public override void WillEnterForeground(UIApplication uiApplication)
    {
    }

    public override void WillTerminate(UIApplication uiApplication)
    {
    }

    private void UpdateMainPage()
    {
      if (this.application.MainPage == null)
        return;
      PlatformRenderer platformRenderer = (PlatformRenderer) this.window.RootViewController;
      this.window.RootViewController = PageExtensions.CreateViewController(this.application.MainPage);
      if (platformRenderer == null)
        return;
      ((IDisposable) platformRenderer.Platform).Dispose();
    }

    private void SetMainPage()
    {
      this.UpdateMainPage();
      // ISSUE: reference to a compiler-generated method
      this.window.MakeKeyAndVisible();
    }
  }
}
