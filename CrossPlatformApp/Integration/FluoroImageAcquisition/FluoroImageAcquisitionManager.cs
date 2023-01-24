// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using System;
using System.IO;
using System.Net;
using BitsOfNature.Core;
using BitsOfNature.Core.Imaging;
using BitsOfNature.Core.IO.Net.Rest;
using BitsOfNature.Core.IO.Serialization;
using BitsOfNature.Core.IO.Tracing;
using CrossPlatformApp.Configuration;
using Avalonia.Media.Imaging;
using Stryker.Adapt.FluoroImageAcquisition;
using BitsOfNature.UI.Avalonia.Serialization;

namespace CrossPlatformApp.Integration.FluoroImageAcquisition
{
    public class FluoroImageAcquisitionManager
    {
        #region Fields

        private Trace trace = new Trace("FluoroImageAcquisitionManager");
        private FluoroImageAcquisitionConfiguration configuration;
        private ImageAcquisitionClient imageAcquisitionClient;
        public ImageAcquisitionClient Client { get { return imageAcquisitionClient; } }

        private FluoroImageAcquisitionServer imageAcquisitionServer;
        public FluoroImageAcquisitionServer Server { get { return imageAcquisitionServer; } }

        private RestServer restServer;

        #endregion

        #region Construction / Destruction

        public FluoroImageAcquisitionManager(FluoroImageAcquisitionConfiguration configuration)
        {
            this.configuration = configuration;

            this.imageAcquisitionClient = new ImageAcquisitionClient("PixrFemurLive", 0, IPAddress.Loopback, configuration.EngineServerPort, IPAddress.Parse(configuration.EngineServerIpAddress), 5000);
            this.imageAcquisitionServer = new FluoroImageAcquisitionServer();

            trace.Info("FluoroImageAcquisitionManager(): created.");
        }

        public void Init()
        {
            try
            {
                if (!configuration.Enabled)
                {
                    return;
                }

                if (string.IsNullOrEmpty(configuration.EngineServerIpAddress) || IPAddress.Parse(configuration.EngineServerIpAddress).Equals(IPAddress.Any))
                {
                    trace.Warning("Init(): No connection to IAE established du to IpAddress[{0}] settings.", configuration.EngineServerIpAddress);
                }
                else
                {
                    imageAcquisitionClient.Connect();
                }

                SerializationTypeManager defaultSerializationManager = SerializationTypeManager.Default;
                SerializationTypeManager restServerSerializationManager = new SerializationTypeManager(defaultSerializationManager);
                restServerSerializationManager.RegisterType("Bitmap", typeof(Bitmap), new BitmapBase64SerializationProxy(), true);
                restServerSerializationManager.RegisterType("ArgbImage", typeof(ArgbImage), new ArgbImageBase64SerializationProxy(), true);
                restServerSerializationManager.RegisterType("Image8Bit", typeof(Image8Bit), new Image8BitBase64SerializationProxy(), true);

                restServer = new RestServer(this.configuration.ImageAcquisitionServerUri, imageAcquisitionServer, restServerSerializationManager);
                restServer.Start();

                trace.Info("Init(): successful.");
            }
            catch (Exception ex)
            {
                trace.Error("Init(): failed. {0}", ex);
            }
        }

        public void Exit()
        {
            try
            {
                if (restServer != null)
                {
                    restServer.Stop();
                    restServer.Dispose();
                    restServer = null;
                }

                if (imageAcquisitionClient != null)
                {
                    imageAcquisitionClient.Disconnect();
                    imageAcquisitionClient.Dispose();
                    imageAcquisitionClient = null;
                }

                trace.Info("Exit(): successful.");
            }
            catch (Exception ex)
            {
                trace.Error("Exit(): failed. {0}", ex);
            }
        }

        #endregion
    }
}
