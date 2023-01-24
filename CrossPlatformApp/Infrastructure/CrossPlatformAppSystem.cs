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
using BitsOfNature.Core.Drawing.Interop;
using BitsOfNature.Core.Imaging;
using BitsOfNature.Core.IO;
using BitsOfNature.Core.IO.Logger;
using BitsOfNature.Core.IO.Serialization;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.Utils;
using BitsOfNature.Interop.Skia;
using BitsOfNature.UI.Avalonia.Localization;
using CrossPlatformApp.Configuration;

namespace CrossPlatformApp.Infrastructure
{
    public class CrossPlatformAppSystem
    {
        #region Fields

        private Trace trace = new Trace("PixrFemurLiveSystem");
        private AppConfigReader configReader;
        private ServiceLocator services;
        public ServiceLocator Services => services;
        private CrossPlatformAppApplication application;

        #endregion

        #region Construction / Destruction

        public static CrossPlatformAppSystem Instance
        {
            get;
            private set;
        }

        public CrossPlatformAppSystem()
        {
            configReader = new AppConfigReader(trace);
            services = new ServiceLocator();
            Instance = this;

            trace.Info("CrossPlatformAppSystem(): created.");
        }

        public void Init()
        {
            try
            {
                // Test trace performance
                //DateTime start = DateTime.Now;
                //for (int i = 0; i < 1000; i++)
                //{
                //    trace.Fatal("Fatal");
                //    trace.Error("Error");
                //    trace.Warning("Warning");
                //    trace.Info("Info");
                //    trace.Debug("Debug");
                //    trace.Verbose("Verbose");
                //}
                //DateTime end = DateTime.Now;
                //trace.Fatal("Duration[{0}]", end - start);

                // Test try serialization
                //FluoroReferenceBodyConfiguration conf = new FluoroReferenceBodyConfiguration();
                //conf.Name = "TestRefBody";
                //conf.Horizon = new FluoroReferenceBodyHorizonConfiguration(0, 1, 2, 3, 4, 5);
                //conf.Markers = new FluoroReferenceBodyMarkerConfiguration[3];
                //conf.Markers[0] = new FluoroReferenceBodyMarkerConfiguration(0, 1, 2, 3, 4);
                //conf.Markers[1] = new FluoroReferenceBodyMarkerConfiguration(1, 2, 3, 4, 5);
                //conf.Markers[2] = new FluoroReferenceBodyMarkerConfiguration(2, 4, 4, 5, 6);

                //IOUtils.Save("test.xml", conf, ContainerFormat.Xml);

                // Load applicaton configuration root and register in service locator
                //{
                //    ArgbImage image = ArgbImage.Load("D:/Test/08/Input.png");
                //    image.Save("D:/Test/08/OutputGdi.png");
                //}

                GraphicsService.Configure(new SkiaGraphicsService());

                //{
                //    ArgbImage image = ArgbImage.Load("D:/Test/08/Input.png");
                //    image.Save("D:/Test/08/OutputSkia.png");
                //}

                ScanSerializationAssemblies();
                CrossPlatformAppConfiguration configuration = LoadConfiguration();
                Instance.Services.Register(configuration);

                // Window State
                //System.Windows.WindowState mainWindowState = System.Windows.WindowState.Normal;

                //if (!Enum.TryParse<System.Windows.WindowState>(configuration.Targeting.WindowState, out mainWindowState))
                //{
                //    trace.Error("Init(): Unsupported WindowState[{0}]", configuration.Targeting.WindowState);
                //}

                //trace.Info("Init(): Applying WindowState[{0}]", mainWindowState);
                //Platform.Instance.MainWindow.WindowState = mainWindowState;

                // Logging
                //loggingManager = new LoggingManager();
                //loggingManager.Init();

                // Application
                application = new CrossPlatformAppApplication();
                application.Init();

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
                //if (loggingManager != null)
                //{
                //    loggingManager.Exit();
                //    loggingManager = null;
                //}

                if (application != null)
                {
                    application.Exit();
                    application = null;
                }

                trace.Info("Exit(): successful.");
            }
            catch (Exception ex)
            {
                trace.Error("Exit(): failed. {0}", ex);
            }
        }

        #endregion

        #region Private Helpers

        private void ScanSerializationAssemblies()
        {
            // To avoid exceptions from scanning non c# DLLs only selected DLLs are scanned

            // Scan main assembly
            SerializationTypeManager.Default.ScanBitsOfNatureCore();
            SerializationTypeManager.Default.ScanAssemblies(typeof(LocalizationData).Assembly);
            SerializationTypeManager.Default.ScanAssemblies(this.GetType().Assembly);

            // Scan this to register serialization / deserialization for CArmType enum
            //SerializationTypeManager.Default.ScanAssemblies(typeof(Stryker.Adapt.Interop.FluoroImageProcessing.CArmType).Assembly);
        }

        private CrossPlatformAppConfiguration LoadConfiguration()
        {
            CrossPlatformAppConfiguration configuration = new CrossPlatformAppConfiguration();

            string configurationPath = "";
            if (configReader.ReadString("CrossPlatformAppConfigurationPath", ref configurationPath))
            {
                configuration = IOUtils.Load<CrossPlatformAppConfiguration>(configurationPath, ContainerFormat.Xml);
                trace.Info("LoadConfiguration(): Data loaded successfully\n{0}", configuration);
            }
            else
            {
                trace.Error("LoadConfiguration(): Could not load data. No configuration path available.");
            }

            return configuration;
        }

        #endregion
    }
}
