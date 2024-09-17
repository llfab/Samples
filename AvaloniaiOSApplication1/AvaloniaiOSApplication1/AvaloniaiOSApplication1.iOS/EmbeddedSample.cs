using System;
using Avalonia.Platform;
using CoreGraphics;
using Foundation;
using UIKit;
using Avalonia.iOS;
using AvaloniaiOSApplication1.Views;

namespace AvaloniaiOSApplication1.iOS;

public class EmbeddedSample : INativeDemoControl
{
    public IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
    {
#if !TVOS
            var webView = new WebKit.WKWebView(CGRect.Empty, new WebKit.WKWebViewConfiguration());
            webView.LoadRequest(new NSUrlRequest(new NSUrl("https://www.apple.com/")));

            return new UIViewControlHandle(webView);
#endif
    }
}