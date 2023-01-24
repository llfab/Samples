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
using BitsOfNature.Core.IO.FileFormats;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.Utils;
using CrossPlatformApp.Configuration;
using CrossPlatformApp.Integration.FluoroImageAcquisition;
using CrossPlatformApp.Integration.ProNavigation;
using CrossPlatformApp.Models;

namespace CrossPlatformApp.Infrastructure
{
    public class CaseInfo
    {
        public string CaseDirectory { get; set; }
        public string CaseLogFile { get; set; }
    }

    public class CrossPlatformAppApplication
    {
        #region Fields

        private Trace trace = new Trace("PixrFemurLiveApplication");
        //private string caseName;
        //private CaseInfo caseInfo;

        public CrossPlatformAppConfiguration configuration;
        private FluoroImageAcquisitionManager fluoroImageAcquisitionManager;
        private ProNavigationManager proNavigationManager;
        private PresentationData dataModel;
        //private MainViewModel mainViewModel;
        //private FluoroImageController fluoroImageController;
        //private NavigationController navigationController;

        #endregion

        #region Construction / Destruction

        public CrossPlatformAppApplication()
        {
            this.configuration = CrossPlatformAppSystem.Instance.Services.Get<CrossPlatformAppConfiguration>();
            dataModel = new PresentationData();
            CrossPlatformAppSystem.Instance.Services.Register(dataModel);

            trace.Info("CrossPlatformAppApplication(): created.");
        }

        public void Init()
        {
            try
            {
                // global data
                //this.caseName = string.Format("Case-{0}", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                //this.caseInfo = new CaseInfo()
                //{
                //    CaseDirectory = Path.Combine(this.configuration.CaseDirectory, caseName),
                //    CaseLogFile = this.configuration.CaseLogFile,
                //};

                fluoroImageAcquisitionManager = new FluoroImageAcquisitionManager(this.configuration.FluoroImageAcquisition);
                fluoroImageAcquisitionManager.Server.XrayImageReceived += OnFluoroImageAcquisitionMangerXrayImageReceived;
                fluoroImageAcquisitionManager.Client.ConnectionStateChanged += OnFluoroImageAcquisitionManagerConnectionStateChanged;
                fluoroImageAcquisitionManager.Client.IncomingImage += OnFluoroImageAcquisitionManagerIncomingImage;

                fluoroImageAcquisitionManager.Init();

                proNavigationManager = new ProNavigationManager(this.configuration.ProNavigation);
                proNavigationManager.Server.VideoFrameReceived += OnProNavigationManagerVideoFrameChanged;
                proNavigationManager.Server.TrackerConnected += OnProNavigationManagerTrackerConnected;
                proNavigationManager.Server.TrackerDisconnected += OnProNavigationManagerTrackerDisconnected;

                proNavigationManager.Init();

                this.navigationVideoFrameMerger = new TaskMerger<Action<BitsOfNature.Core.Imaging.ArgbImage>>("NavigationVideoFrameProcessingMerger", OnVideoFrameProcessingActionCalled, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMilliseconds(0), trace);

                //// Create central presentation model and register in service locator
                //this.dataModel = new PresentationData();
                //Platform.Instance.Services.Register(this.dataModel);

                //// Control / Model
                //this.fluoroImageController = new FluoroImageController();
                //Platform.Instance.Services.Register(this.fluoroImageController);

                //this.navigationController = new NavigationController();
                //Platform.Instance.Services.Register(this.navigationController);

                //this.fluoroImageController.Init();
                //this.navigationController.Init();

                //// Main View
                //this.mainViewModel = new MainViewModel();
                //MainView mainView = new MainView();
                //mainView.DataContext = mainViewModel;
                //IPerspectiveElement mainElement = PanelAdapterView.ForWpf(mainView);
                //Platform.Instance.Perspective.Layout.Add(mainElement);

                //// Error Window
                //ErrorView errorWindow = new ErrorView();
                //errorWindow = new ErrorView();
                //errorWindow.DataContext = new ErrorViewModel();
                //Platform.Instance.Services.Register(errorWindow);

                //// Carm Selection View
                //CarmSettingSelectionView carmSettingSelectionWindow = new CarmSettingSelectionView();
                //carmSettingSelectionWindow.DataContext = new CarmSettingSelectionViewModel();
                //Platform.Instance.Services.Register(carmSettingSelectionWindow);

                //// Reference Body Selection View
                //ReferenceBodySelectionView referenceBodySelectionWindow = new ReferenceBodySelectionView();
                //referenceBodySelectionWindow.DataContext = new ReferenceBodySelectionViewModel();
                //Platform.Instance.Services.Register(referenceBodySelectionWindow);

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
                //if (this.navigationController != null)
                //{
                //    this.navigationController.Exit();
                //    this.navigationController = null;
                //}

                //if (this.fluoroImageController != null)
                //{
                //    this.fluoroImageController.Exit();
                //    this.fluoroImageController = null;
                //}

                if (this.proNavigationManager != null)
                {
                    this.proNavigationManager.Exit();
                    this.proNavigationManager = null;
                }

                if (this.fluoroImageAcquisitionManager != null)
                {
                    this.fluoroImageAcquisitionManager.Exit();
                    this.fluoroImageAcquisitionManager = null;
                }

                trace.Info("Exit(): successful.");
            }
            catch (Exception ex)
            {
                trace.Error("Exit(): failed. {0}", ex);
            }
        }

        #endregion

        #region Callbacks

        private void OnFluoroImageAcquisitionManagerConnectionStateChanged(bool connected)
        {
            try
            {
                dataModel.Fluoro.IaeConnected = connected;
            }
            catch (Exception ex)
            {
                trace.Error("OnFluoroImageAcquisitionMangerXrayImageReceived(): Error\n{0}", ex);
            }
        }

        private void OnFluoroImageAcquisitionManagerIncomingImage(Stryker.Adapt.FluoroImageAcquisition.Data.Image image)
        {
            try
            {
                // TODO (FH): reestablish incoming image
                //dataModel.Fluoro.CurrentXrayImage = image.ImageData;
            }
            catch (Exception ex)
            {
                trace.Error("OnFluoroImageAcquisitionMangerXrayImageReceived(): Error\n{0}", ex);
            }
        }

        private void OnFluoroImageAcquisitionMangerXrayImageReceived(Avalonia.Media.Imaging.Bitmap image, BitsOfNature.Core.Imaging.Camera.ImageCalibration calibration)
        {
            try
            {
                dataModel.Fluoro.CurrentXrayImage = image;
            }
            catch (Exception ex)
            {
                trace.Error("OnFluoroImageAcquisitionMangerXrayImageReceived(): Error\n{0}", ex);
            }
        }

        private void OnProNavigationManagerTrackerConnected(ProNavigationCameraParameters parameters, BitsOfNature.Core.Geometry3D.Pose3D tooltipToCamera)
        {
            try
            {
                dataModel.Navigation.ToolConnected = true;
            }
            catch (Exception ex)
            {
                trace.Error("OnProNavigationManagerTrackerConnected(): Error\n{0}", ex);
            }
        }

        private void OnProNavigationManagerTrackerDisconnected()
        {
            try
            {
                dataModel.Navigation.ToolConnected = false;
            }
            catch (Exception ex)
            {
                trace.Error("OnProNavigationManagerTrackerDisconnected(): Error\n{0}", ex);
            }
        }

        private TaskMerger<Action<BitsOfNature.Core.Imaging.ArgbImage>> navigationVideoFrameMerger;

        private void OnProNavigationManagerVideoFrameChanged(BitsOfNature.Core.Imaging.ArgbImage image)
        {
            try
            {
                navigationVideoFrameMerger.Call(image);
            }
            catch (Exception ex)
            {
                trace.Error("OnProNavigationManagerVideoFrameChanged(): Error\n{0}", ex);
            }
        }

        private void OnVideoFrameProcessingActionCalled(BitsOfNature.Core.Imaging.ArgbImage image)
        {
            try
            {
                dataModel.Navigation.VideoFrame = image;
            }
            catch (Exception ex)
            {
                trace.Error("OnVideoFrameProcessingActionCalled(): error\n{0}", ex);
            }
        }



        #endregion
    }
}
