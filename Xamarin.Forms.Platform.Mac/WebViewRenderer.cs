using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using System.Drawing;
using AppKit;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	public class WebViewRenderer : WebKit.WebView, IVisualElementRenderer, IDisposable, IRegisterable, IWebViewRenderer
	{
		private VisualElementTracker tracker;
		private VisualElementPackager packager;
		private EventTracker events;
		private bool ignoreSourceChanges;
		private WebNavigationEvent lastBackForwardEvent;

		public VisualElement Element { get; set; }

		public NSView NativeView
		{
			get { return this; }
		}

		public NSViewController ViewController
		{
			get	{ return null; }
		}

		public WebViewRenderer()
		{
			this.UIDelegate = new CustomWebViewDelegate (this);
		}

		/*
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.IsLoading)
				{
					this.StopLoading();
				}

				this.Element.PropertyChanged -= new PropertyChangedEventHandler(this.HandlePropertyChanged);
				((WebView)this.Element).EvalRequested -= new EventHandler<EventArg<string>>(this.OnEvalRequested);
				((WebView)this.Element).GoBackRequested -= new EventHandler(this.OnGoBackRequested);
				((WebView)this.Element).GoForwardRequested -= new EventHandler(this.OnGoForwardRequested);
			}
			base.Dispose(disposing);
		}
		*/

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return (NativeView as NSControl).GetSizeRequest(widthConstraint, heightConstraint, 44, 44);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == WebView.SourceProperty.PropertyName)
			{
				this.Load();
			}
		}

		/*
		public override void Layout()
		{
			base.Layout();
			this.ScrollView.Frame = this.Bounds;
		}
		*/

		private void Load()
		{
			if (this.ignoreSourceChanges)
			{
				return;
			}
			if (((WebView)this.Element).Source != null)
			{
				((WebView)this.Element).Source.Load(this);
			}
			this.UpdateCanGoBackForward();
		}

		public void LoadHtml(string html, string baseUrl)
		{
			if (html == null)
				return;

			base.MainFrame.LoadHtmlString (html, new NSUrl(baseUrl));
		}

		public void LoadUrl(string url)
		{
			base.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url)));
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			EventHandler<VisualElementChangedEventArgs> eventHandler = this.ElementChanged;
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		private void OnEvalRequested(object sender, EventArg<string> eventArg)
		{
			base.StringByEvaluatingJavaScriptFromString(eventArg.Data);
		}

		private void OnGoBackRequested(object sender, EventArgs eventArgs)
		{
			if (CanGoBack())
			{
				lastBackForwardEvent = WebNavigationEvent.Back;
				GoBack();
			}
			this.UpdateCanGoBackForward();
		}

		private void OnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			if (CanGoForward())
			{
				lastBackForwardEvent = WebNavigationEvent.Forward;
				GoForward();
			}
			this.UpdateCanGoBackForward();
		}

		public void SetElement(VisualElement element)
		{
			VisualElement visualElement = this.Element;
			this.Element = element;
			this.Element.PropertyChanged += new PropertyChangedEventHandler(this.HandlePropertyChanged);
			((WebView)this.Element).EvalRequested += new EventHandler<EventArg<string>>(this.OnEvalRequested);
			((WebView)this.Element).GoBackRequested += new EventHandler(this.OnGoBackRequested);
			((WebView)this.Element).GoForwardRequested += new EventHandler(this.OnGoForwardRequested);

			base.UIDelegate = new WebViewRenderer.CustomWebViewDelegate(this);

			this.SetBackgroundColor ( NSColor.Clear );
			this.AutoresizesSubviews = true;
			// TODO: I dunno?
			//this.AutoresizingMask = NSViewResizingMask. = UIViewAutoresizing.FlexibleDimensions;
			this.tracker = new VisualElementTracker(this);
			this.packager = new VisualElementPackager(this);
			this.packager.Load();
			this.events = new EventTracker(this);
			this.events.LoadEvents(this);
			this.Load();
			this.OnElementChanged(new VisualElementChangedEventArgs(visualElement, element));
			if (element != null)
			{
				element.SendViewInitialized(this);
			}
		}

		public void SetElementSize(Size size)
		{
			this.Element.Layout(new Rectangle(this.Element.X, this.Element.Y, size.Width, size.Height));
		}

		private void UpdateCanGoBackForward()
		{
			((WebView)this.Element).CanGoBack = base.CanGoBack();
			((WebView)this.Element).CanGoForward = this.CanGoForward();
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		private class CustomWebViewDelegate : WebKit.WebUIDelegate
		{
			private readonly WebViewRenderer renderer;
			private WebNavigationEvent lastEvent;

			private WebView WebView
			{
				get	{ return (WebView)this.renderer.Element; }
			}

			public CustomWebViewDelegate(WebViewRenderer renderer)
			{
				if (renderer == null)
					throw new ArgumentNullException("renderer");

				this.renderer = renderer;

				renderer.FailedLoadWithError += (sender, e) => Fail();
				renderer.OnFailedLoading += (sender, e) => Fail();
				renderer.FinishedLoad += (sender, e) => Finished();
				renderer.OnFinishedLoading += (sender, e) => Finished();
			}

			private void Fail()
			{
				string currentUrl = this.GetCurrentUrl();
				this.WebView.SendNavigated(new WebNavigatedEventArgs(this.lastEvent, new UrlWebViewSource()
				{
					Url = currentUrl
				}, currentUrl, WebNavigationResult.Failure));
				this.renderer.UpdateCanGoBackForward();
			}

			private void Finished()
			{
				if (renderer.IsLoading)
					return;

				this.renderer.ignoreSourceChanges = true;
				string currentUrl = this.GetCurrentUrl();
				((IElementController)this.WebView).SetValueFromRenderer(WebView.SourceProperty, new UrlWebViewSource()
				{
					Url = currentUrl
				});
				this.renderer.ignoreSourceChanges = false;
				WebNavigatedEventArgs webNavigatedEventArg = new WebNavigatedEventArgs(this.lastEvent, this.WebView.Source, currentUrl, WebNavigationResult.Success);
				this.WebView.SendNavigated(webNavigatedEventArg);
				this.renderer.UpdateCanGoBackForward();
			}


			private string GetCurrentUrl()
			{
				return renderer.MainFrameUrl;
			}

			/*
			public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
			{
				WebNavigationEvent webNavigationEvent = WebNavigationEvent.NewPage;
				UIWebViewNavigationType uIWebViewNavigationType = navigationType;
				if (uIWebViewNavigationType <= UIWebViewNavigationType.Other)
				{
					switch ((uint)uIWebViewNavigationType)
					{
					case 0:
						{
							webNavigationEvent = WebNavigationEvent.NewPage;
							break;
						}
					case 1:
						{
							webNavigationEvent = WebNavigationEvent.NewPage;
							break;
						}
					case 2:
						{
							webNavigationEvent = this.renderer.lastBackForwardEvent;
							break;
						}
					case 3:
						{
							webNavigationEvent = WebNavigationEvent.Refresh;
							break;
						}
					case 4:
						{
							webNavigationEvent = WebNavigationEvent.NewPage;
							break;
						}
					case 5:
						{
							webNavigationEvent = WebNavigationEvent.NewPage;
							break;
						}
					}
				}
				else
				{
				}
				this.lastEvent = webNavigationEvent;
				string str = request.Url.ToString();
				WebNavigatingEventArgs webNavigatingEventArg = new WebNavigatingEventArgs(webNavigationEvent, new UrlWebViewSource()
				{
					Url = str
				}, str);
				this.WebView.SendNavigating(webNavigatingEventArg);
				this.renderer.UpdateCanGoBackForward();
				return !webNavigatingEventArg.Cancel;
			}
			*/
		}
	}
}
