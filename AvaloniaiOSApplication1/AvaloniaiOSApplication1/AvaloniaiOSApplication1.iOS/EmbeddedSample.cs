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
        // Create an ARSCNView
        ARSCNView arView = new ARSCNView()
        {
            // Set the frame
            Frame = new CGRect(0, 0, 200, 200),
            // Enable automatic lighting
            AutoenablesDefaultLighting = true,
            // Enable statistics
            ShowsStatistics = true
        };

        // Ensure the session is initialized
        if (arView.Session == null)
        {
            arView.Session = new ARSession();
        }
            
        // Create a simple scene
        SCNScene scene = new SCNScene();
        arView.Scene = scene;

        // Add a simple object to the scene
        SCNNode sphere = SCNNode.FromGeometry(SCNSphere.Create(0.1f));
        sphere.Position = new SCNVector3(0, 0, -0.5f); // Position the sphere 0.5 meters in front of the camera
        scene.RootNode.AddChildNode(sphere);

        // Start the AR session
        ARWorldTrackingConfiguration configuration = new ARWorldTrackingConfiguration
        {
            PlaneDetection = ARPlaneDetection.Horizontal
        };
        arView.Session.Run(configuration, ARSessionRunOptions.RemoveExistingAnchors);

        return new UIViewControlHandle(arView);
    }
}