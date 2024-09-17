using System;
using Avalonia.Platform;
using CoreGraphics;
using Foundation;
using UIKit;
using Avalonia.iOS;
using AvaloniaiOSApplication1.Views;
using ARKit;
using SceneKit;

namespace AvaloniaiOSApplication1.iOS;

public class EmbeddedSample : INativeDemoControl
{
    public IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
    {
#if !TVOS
            //var webView = new WebKit.WKWebView(CGRect.Empty, new WebKit.WKWebViewConfiguration());
            //webView.LoadRequest(new NSUrlRequest(new NSUrl("https://www.apple.com/")));

            //return new UIViewControlHandle(webView);

        // Create an ARSCNView
        var arView = new ARSCNView()
        {
            // Set the frame
            Frame = CGRect.Empty,
            // Enable automatic lighting
            AutoenablesDefaultLighting = true,
            // Enable statistics
            ShowsStatistics = true
        };

        // Create a simple scene
        var scene = new SCNScene();
        arView.Scene = scene;

        // Add a simple object to the scene
        var sphere = SCNNode.FromGeometry(SCNSphere.Create(0.1f));
        sphere.Position = new SCNVector3(0, 0, -0.5f); // Position the sphere 0.5 meters in front of the camera
        scene.RootNode.AddChildNode(sphere);

        // Start the AR session
        var configuration = new ARWorldTrackingConfiguration
        {
            PlaneDetection = ARPlaneDetection.Horizontal
        };
        arView.Session.Run(configuration, ARSessionRunOptions.RemoveExistingAnchors);

        return new UIViewControlHandle(arView);
#endif
    }
}