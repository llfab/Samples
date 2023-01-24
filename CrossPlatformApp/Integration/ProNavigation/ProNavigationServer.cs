// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using System;
using System.Globalization;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Imaging;
using BitsOfNature.Core.IO.Net.Http;
using BitsOfNature.Core.IO.Net.Rest;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.Mathematics;

namespace CrossPlatformApp.Integration.ProNavigation
{
    [RestResource()]
    public class ProNavigationServer
    {
        #region Fields

        private Trace trace = new Trace("NavigationServer");

        public delegate void TrackerConnectedHandler(ProNavigationCameraParameters parameters, Pose3D tooltipToCamera);
        public event TrackerConnectedHandler TrackerConnected;

        public delegate void TrackerDisconnectedHandler();
        public event TrackerDisconnectedHandler TrackerDisconnected;

        public delegate void TrackerSelectButtonClickedHandler();
        public event TrackerSelectButtonClickedHandler TrackerSelectButtonClicked;

        public delegate void TrackerNextButtonClickedHandler();
        public event TrackerNextButtonClickedHandler TrackerNextButtonClicked;

        public delegate void TrackerPreviousButtonClickedHandler();
        public event TrackerPreviousButtonClickedHandler TrackerPreviousButtonClicked;

        public delegate void TrackingDataChangedHandler(Pose3D pose);
        public event TrackingDataChangedHandler TrackingDataChanged;

        public delegate void VideoFrameReceivedHandler(ArgbImage image);
        public event VideoFrameReceivedHandler VideoFrameReceived;

        private object navigationAccess = new object();

        #endregion

        #region Construction / Destruction 

        public ProNavigationServer()
        {
            trace.Info("ProNavigationServer(): created.");
        }

        #endregion

        #region IProNavigation

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetTrackerToolConnected", RequestType = RestContentType.Json, ResponseType = RestContentType.None)]
        public void SetTrackerToolConnected(
            string id, string name, string deviceType, bool calibrated, 
            double sensorWidth, double sensorHeight, 
            double opticalCenterX, double opticalCenterY, 
            double pixelSizeX, double pixelSizeY, 
            double focalLength, 
            double distanceCoefficientRadialOne, double distanceCoefficientRadialTwo, double distanceCoefficientRadialThree, 
            double distanceCoefficientTangentialOne, double distanceCoefficientTangentialTwo, 
            string cameraToToolTrafo,
            string applicationTypeId)
        {
            trace.Verbose("SetTrackerToolConnected(): called.");

            lock (navigationAccess)
            {
                ProNavigationCameraParameters parameters = new ProNavigationCameraParameters()
                {
                    Id = id,
                    Name = name,
                    Type = deviceType,
                    Calibrated = calibrated,
                    SensorSize = new SizeD() { Width = sensorWidth, Height = sensorHeight, },
                    OpticalCenter = new PointD() { X = opticalCenterX, Y = opticalCenterY, },
                    PixelSize = new SizeD() { Width = pixelSizeX, Height = pixelSizeY, },
                    FocalLength = focalLength,
                    DistanceCoefficients = new DistanceCoefficientParameters()
                    {
                        Radial1 = distanceCoefficientRadialOne,
                        Radial2 = distanceCoefficientRadialTwo,
                        Radial3 = distanceCoefficientRadialThree,
                        Tangential1 = distanceCoefficientTangentialOne,
                        Tangential2 = distanceCoefficientTangentialTwo,
                    },
                    ApplicationTypeId = applicationTypeId,
                };

                string[] items = cameraToToolTrafo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length < 12)
                {
                    trace.Error("SetTrackerToolConnected(): Not enough matrix elements. Length[{0}]", items.Length);
                    return;
                }

                // Matrix is row-major
                Matrix rotation = new Matrix(3, 3);
                rotation[0, 0] = Parse(items[0]);
                rotation[0, 1] = Parse(items[1]);
                rotation[0, 2] = Parse(items[2]);

                rotation[1, 0] = Parse(items[3]);
                rotation[1, 1] = Parse(items[4]);
                rotation[1, 2] = Parse(items[5]);

                rotation[2, 0] = Parse(items[6]);
                rotation[2, 1] = Parse(items[7]);
                rotation[2, 2] = Parse(items[8]);

                Vector translation = new Vector(new double[] { Parse(items[9]), Parse(items[10]), Parse(items[11]) });
                Pose3D tooltipToCameraPose = Pose3D.Orthogonalize(rotation, translation).GetInverse();

                trace.Verbose("SetTrackerToolConnected(): Parameters[{0}] TooltipToCamera[{1}]", parameters, cameraToToolTrafo);

                TrackerConnected?.Invoke(parameters, tooltipToCameraPose);
            }
        }

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetTrackerToolDisconnected", RequestType = RestContentType.Json, ResponseType = RestContentType.None)]
        public void SetTrackerToolDisconnected()
        {
            trace.Info("SetTrackerToolDisconnected(): called.");

            lock (navigationAccess)
            {
                TrackerDisconnected?.Invoke();
            }
        }

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetTrackerToolSelectButtonClicked", RequestType = RestContentType.Json, ResponseType = RestContentType.None)]
        public void SetTrackerToolSelectButtonClicked()
        {
            trace.Info("SetTrackerToolSelectButtonClicked(): called.");

            lock (navigationAccess)
            {
                TrackerSelectButtonClicked?.Invoke();
            }
        }

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetTrackerToolNextButtonClicked", RequestType = RestContentType.Json, ResponseType = RestContentType.None)]
        public void SetTrackerToolNextButtonClicked()
        {
            trace.Info("SetTrackerToolNextButtonClicked(): called.");

            lock (navigationAccess)
            {
                TrackerNextButtonClicked?.Invoke();
            }
        }

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetTrackerToolPreviousButtonClicked", RequestType = RestContentType.Json, ResponseType = RestContentType.None)]
        public void SetTrackerToolPreviousButtonClicked()
        {
            trace.Info("SetTrackerToolPreviousButtonClicked(): called.");

            lock (navigationAccess)
            {
                TrackerPreviousButtonClicked?.Invoke();
            }
        }

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetTrackingData", RequestType = RestContentType.Json, ResponseType = RestContentType.None)]
        public void SetTrackingData(string toolToTracker, string trackerToBone)
        {
            lock (navigationAccess)
            {
                trace.Verbose("SetTrackingData(): called. TrackerToCameraPose[{0}]", toolToTracker);

                string[] items = toolToTracker.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length != 12)
                {
                    trace.Error("SetTrackingData(): Wrong number of matrix elements. Length[{0}]", items.Length);
                    return;
                }

                // Data in items is row-major
                Matrix rotation = new Matrix(3, 3);
                rotation[0, 0] = Parse(items[0]);
                rotation[0, 1] = Parse(items[1]);
                rotation[0, 2] = Parse(items[2]);

                rotation[1, 0] = Parse(items[3]);
                rotation[1, 1] = Parse(items[4]);
                rotation[1, 2] = Parse(items[5]);

                rotation[2, 0] = Parse(items[6]);
                rotation[2, 1] = Parse(items[7]);
                rotation[2, 2] = Parse(items[8]);

                Vector translation = new Vector(new double[] { Parse(items[9]), Parse(items[10]), Parse(items[11]) } );
                Pose3D orthogonalizedPose = Pose3D.Orthogonalize(rotation, translation);

                TrackingDataChanged?.Invoke(orthogonalizedPose);
                
                trace.Verbose("SetTrackingData(): OrthogonalizedPose[\n{0}]", orthogonalizedPose);
            }
        }

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetVideoFrame", RequestType = RestContentType.ImageJpeg, ResponseType = RestContentType.None, IsOneWay = true)]
        public void SetVideoFrame(ArgbImage image)
        {
            lock (navigationAccess)
            {
                VideoFrameReceived?.Invoke(image);
            }
        }

        private double Parse(string text)
        {
            return double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
