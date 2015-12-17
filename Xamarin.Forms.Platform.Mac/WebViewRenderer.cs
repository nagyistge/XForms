using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
  public class WebViewRenderer : UIWebView, IVisualElementRenderer, IDisposable, IRegisterable, IWebViewRenderer
  {
    private VisualElementTracker tracker;
    private VisualElementPackager packager;
    private EventTracker events;
    private bool ignoreSourceChanges;
    private WebNavigationEvent lastBackForwardEvent;

    public VisualElement Element { get; private set; }

    public UIView NativeView
    {
      get
      {
        return (UIView) this;
      }
    }

    public UIViewController ViewController
    {
      get
      {
        return (UIViewController) null;
      }
    }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    public WebViewRenderer()
      : base((CGRect) RectangleF.Empty)
    {
    }

    public void SetElement(VisualElement element)
    {
      VisualElement element1 = this.Element;
      this.Element = element;
      this.Element.add_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
      ((WebView) this.Element).add_EvalRequested(new EventHandler<EventArg<string>>(this.OnEvalRequested));
      ((WebView) this.Element).add_GoBackRequested(new EventHandler(this.OnGoBackRequested));
      ((WebView) this.Element).add_GoForwardRequested(new EventHandler(this.OnGoForwardRequested));
      this.Delegate = (IUIWebViewDelegate) new WebViewRenderer.CustomWebViewDelegate(this);
      this.BackgroundColor = UIColor.Clear;
      this.AutosizesSubviews = true;
      this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
      this.tracker = new VisualElementTracker((IVisualElementRenderer) this);
      this.packager = new VisualElementPackager((IVisualElementRenderer) this);
      this.packager.Load();
      this.events = new EventTracker((IVisualElementRenderer) this);
      this.events.LoadEvents((UIView) this);
      this.Load();
      this.OnElementChanged(new VisualElementChangedEventArgs(element1, element));
      if (element == null)
        return;
      Forms.SendViewInitialized(element, (UIView) this);
    }

    private void OnGoForwardRequested(object sender, EventArgs eventArgs)
    {
      if (this.CanGoForward)
      {
        this.lastBackForwardEvent = WebNavigationEvent.Forward;
        // ISSUE: reference to a compiler-generated method
        this.GoForward();
      }
      this.UpdateCanGoBackForward();
    }

    private void OnGoBackRequested(object sender, EventArgs eventArgs)
    {
      if (this.CanGoBack)
      {
        this.lastBackForwardEvent = WebNavigationEvent.Back;
        // ISSUE: reference to a compiler-generated method
        this.GoBack();
      }
      this.UpdateCanGoBackForward();
    }

    private void UpdateCanGoBackForward()
    {
      ((WebView) this.Element).CanGoBack = this.CanGoBack;
      ((WebView) this.Element).CanGoForward = this.CanGoForward;
    }

    private void OnEvalRequested(object sender, EventArg<string> eventArg)
    {
      // ISSUE: reference to a compiler-generated method
      this.EvaluateJavascript(eventArg.Data);
    }

    public override void LayoutSubviews()
    {
      // ISSUE: reference to a compiler-generated method
      base.LayoutSubviews();
      this.ScrollView.Frame = this.Bounds;
    }

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
      return UIViewExtensions.GetSizeRequest(this.NativeView, widthConstraint, heightConstraint, 44.0, 44.0);
    }

    public void SetElementSize(Xamarin.Forms.Size size)
    {
      this.Element.Layout(new Xamarin.Forms.Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
    }

    protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
    {
      // ISSUE: reference to a compiler-generated field
      EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
      if (eventHandler == null)
        return;
      eventHandler((object) this, e);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.IsLoading)
        {
          // ISSUE: reference to a compiler-generated method
          this.StopLoading();
        }
        this.Element.remove_PropertyChanged(new PropertyChangedEventHandler(this.HandlePropertyChanged));
        ((WebView) this.Element).remove_EvalRequested(new EventHandler<EventArg<string>>(this.OnEvalRequested));
        ((WebView) this.Element).remove_GoBackRequested(new EventHandler(this.OnGoBackRequested));
        ((WebView) this.Element).remove_GoForwardRequested(new EventHandler(this.OnGoForwardRequested));
      }
      // ISSUE: reference to a compiler-generated method
      base.Dispose(disposing);
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == WebView.SourceProperty.PropertyName))
        return;
      this.Load();
    }

    private void Load()
    {
      if (this.ignoreSourceChanges)
        return;
      if (((WebView) this.Element).Source != null)
        ((WebView) this.Element).Source.Load((IWebViewRenderer) this);
      this.UpdateCanGoBackForward();
    }

    public void LoadUrl(string url)
    {
      // ISSUE: reference to a compiler-generated method
      this.LoadRequest(new NSUrlRequest(new NSUrl(url)));
    }

    public void LoadHtml(string html, string baseUrl)
    {
      if (html == null)
        return;
      // ISSUE: reference to a compiler-generated method
      this.LoadHtmlString(html, baseUrl == null ? new NSUrl(NSBundle.MainBundle.BundlePath, true) : new NSUrl(baseUrl, true));
    }

    private class CustomWebViewDelegate : UIWebViewDelegate
    {
      private readonly WebViewRenderer renderer;
      private WebNavigationEvent lastEvent;

      private WebView WebView
      {
        get
        {
          return (WebView) this.renderer.Element;
        }
      }

      public CustomWebViewDelegate(WebViewRenderer renderer)
      {
        if (renderer == null)
          throw new ArgumentNullException("renderer");
        this.renderer = renderer;
      }

      private string GetCurrentUrl()
      {
        return this.renderer.Request.Url.AbsoluteUrl.ToString();
      }

      public override void LoadStarted(UIWebView webView)
      {
      }

      public override void LoadingFinished(UIWebView webView)
      {
        if (webView.IsLoading)
          return;
        this.renderer.ignoreSourceChanges = true;
        string currentUrl = this.GetCurrentUrl();
        this.WebView.SetValueFromRenderer(WebView.SourceProperty, (object) new UrlWebViewSource()
        {
          Url = currentUrl
        });
        this.renderer.ignoreSourceChanges = false;
        this.WebView.SendNavigated(new WebNavigatedEventArgs(this.lastEvent, this.WebView.Source, currentUrl, WebNavigationResult.Success));
        this.renderer.UpdateCanGoBackForward();
      }

      public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
      {
        WebNavigationEvent webNavigationEvent = WebNavigationEvent.NewPage;
        long num1 = (long) navigationType;
        long num2 = 5;
        if ((ulong) num1 <= (ulong) num2)
        {
          switch ((uint) num1)
          {
            case 0:
              webNavigationEvent = WebNavigationEvent.NewPage;
              break;
            case 1:
              webNavigationEvent = WebNavigationEvent.NewPage;
              break;
            case 2:
              webNavigationEvent = this.renderer.lastBackForwardEvent;
              break;
            case 3:
              webNavigationEvent = WebNavigationEvent.Refresh;
              break;
            case 4:
              webNavigationEvent = WebNavigationEvent.NewPage;
              break;
            case 5:
              webNavigationEvent = WebNavigationEvent.NewPage;
              break;
          }
        }
        this.lastEvent = webNavigationEvent;
        string str = request.Url.ToString();
        int num3 = (int) webNavigationEvent;
        UrlWebViewSource urlWebViewSource = new UrlWebViewSource();
        urlWebViewSource.Url = str;
        string url = str;
        WebNavigatingEventArgs args = new WebNavigatingEventArgs((WebNavigationEvent) num3, (WebViewSource) urlWebViewSource, url);
        this.WebView.SendNavigating(args);
        this.renderer.UpdateCanGoBackForward();
        return !args.Cancel;
      }

      public override void LoadFailed(UIWebView webView, NSError error)
      {
        string currentUrl = this.GetCurrentUrl();
        WebView webView1 = this.WebView;
        int num1 = (int) this.lastEvent;
        UrlWebViewSource urlWebViewSource = new UrlWebViewSource();
        urlWebViewSource.Url = currentUrl;
        string url = currentUrl;
        int num2 = 4;
        WebNavigatedEventArgs args = new WebNavigatedEventArgs((WebNavigationEvent) num1, (WebViewSource) urlWebViewSource, url, (WebNavigationResult) num2);
        webView1.SendNavigated(args);
        this.renderer.UpdateCanGoBackForward();
      }
    }
  }
}
