// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using System;
using BitsOfNature.Core.Imaging;
using BitsOfNature.Core.IO.Net.Rest;
using BitsOfNature.Core.IO.Serialization;
using BitsOfNature.Core.IO.Tracing;
using CrossPlatformApp.Configuration;
using Avalonia.Media.Imaging;
using BitsOfNature.UI.Avalonia.Serialization;

namespace CrossPlatformApp.Integration.ProNavigation
{
    public class ProNavigationManager
    {
        #region Fields

        private Trace trace = new Trace("NavigationManager");
        private ProNavigationConfiguration configuration;
        private ProNavigationServer navigationServer;
        public ProNavigationServer Server { get { return navigationServer; } }

        private RestServer restServer;

        #endregion

        #region Construction / Destruction

        public ProNavigationManager(ProNavigationConfiguration configuration)
        {
            this.configuration = configuration; ;

            this.navigationServer = new ProNavigationServer();

            trace.Info("ProNavigationManager(): created.");
        }

        public void Init()
        {
            try
            {
                if (!configuration.Enabled)
                {
                    return;
                }

                SerializationTypeManager defaultSerializationManager = SerializationTypeManager.Default;
                SerializationTypeManager restServerSerializationManager = new SerializationTypeManager(defaultSerializationManager);
                restServerSerializationManager.RegisterType("Bitmap", typeof(Bitmap), new BitmapBase64SerializationProxy(), true);
                restServerSerializationManager.RegisterType("ArgbImage", typeof(ArgbImage), new ArgbImageBase64SerializationProxy(), true);
                restServerSerializationManager.RegisterType("RgbImage", typeof(RgbImage), new RgbImageBase64SerializationProxy(), true);
                restServerSerializationManager.RegisterType("Image8Bit", typeof(Image8Bit), new Image8BitBase64SerializationProxy(), true);

                restServer = new RestServer(this.configuration.NavigationServerUri, navigationServer, restServerSerializationManager);
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
