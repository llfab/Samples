using System;
using ARKit;
using Vision;
using UIKit;
using Foundation;

public partial class ViewController : UIViewController, IARSessionDelegate
{
    ARSCNView sceneView;
    VNRequest[] qrRequests;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Set up ARSCNView
        sceneView = new ARSCNView
        {
            Frame = View.Frame
        };
        View.AddSubview(sceneView);

        // Set the view's delegate
        sceneView.Session.Delegate = this;

        // Create a session configuration
        var configuration = new ARWorldTrackingConfiguration();

        // Run the view's session
        sceneView.Session.Run(configuration);

        // Set up Vision request
        SetupVision();
    }

    void SetupVision()
    {
        var qrRequest = new VNDetectBarcodesRequest(DetectedQRCode);
        qrRequests = new VNRequest[] { qrRequest };
    }

    public void DidUpdateFrame(ARSession session, ARFrame frame)
    {
        var pixelBuffer = frame.CapturedImage;
        var imageRequestHandler = new VNImageRequestHandler(pixelBuffer, new NSDictionary());

        try
        {
            imageRequestHandler.Perform(qrRequests, out NSError error);
            if (error != null)
            {
                Console.WriteLine(error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    void DetectedQRCode(VNRequest request, NSError error)
    {
        if (request.GetResults<VNBarcodeObservation>() is VNBarcodeObservation[] results)
        {
            foreach (var result in results)
            {
                if (result.PayloadStringValue != null)
                {
                    Console.WriteLine($"QR Code detected: {result.PayloadStringValue}");
                    // Handle the detected QR code payload
                }
            }
        }
    }
}