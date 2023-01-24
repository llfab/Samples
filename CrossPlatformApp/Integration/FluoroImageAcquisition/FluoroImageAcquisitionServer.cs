// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Imaging.Camera;
using BitsOfNature.Core.IO.Net.Http;
using BitsOfNature.Core.IO.Net.Rest;
using BitsOfNature.Core.IO.Tracing;

using Avalonia.Media.Imaging;

namespace CrossPlatformApp.Integration.FluoroImageAcquisition
{
    [RestResource()]
    public class FluoroImageAcquisitionServer
    {
        #region Fields

        private Trace trace = new Trace("FluoroImageAcquisitionServer");

        public delegate void XrayImageReceivedHandler(Bitmap image, ImageCalibration calibration);
        public event XrayImageReceivedHandler XrayImageReceived;

        private object fluoroImageAcquisition = new object();

        #endregion

        #region Construction / Destruction 

        public FluoroImageAcquisitionServer()
        {
            trace.Info("FluoroImageAcquisitionServer(): created.");
        }

        #endregion

        #region IFluoroImageAcquisition

        [RestRoute(HttpMethod = HttpMethodType.Any, SubPath = "SetXrayImage", RequestType = RestContentType.Json, ResponseType = RestContentType.None, IsOneWay = true)]
        public void SetXrayImage(Bitmap image, ImageCalibration calibration)
        {
            lock (fluoroImageAcquisition)
            {
                trace.Verbose("SetXrayImage(): called. Size[{0}x{1}]", image.Size.Width, image.Size.Height);
                XrayImageReceived?.Invoke(image, calibration);
            }
        }

        #endregion
    }
}
