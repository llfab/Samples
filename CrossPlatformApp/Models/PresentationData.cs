// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using BitsOfNature.Core.IO.Tracing;

namespace CrossPlatformApp.Models
{
    public class PresentationData
    {
        #region Events

        public event Action CurrentXrayImageChanged;
        public event Action IaeConnectionStateChanged;

        public event Action VideoFrameChanged;
        public event Action ToolConnectionStateChanged;

        #endregion

        #region Fields

        private Trace trace;

        private FluoroData fluoro;
        public FluoroData Fluoro => fluoro;

        private NavigationData navigation;
        public NavigationData Navigation => navigation;

        #endregion

        #region Contstruction

        public PresentationData()
        {
            trace = new Trace("PresentationData");
            fluoro = new FluoroData();
            navigation = new NavigationData();

            fluoro.CurrentXrayImageChanged += OnFluoroCurrentXrayImageChanged;
            fluoro.IaeConnectionStateChanged += OnFluoroIaeConnectionStateChanged;

            navigation.ToolConnectionStateChanged += OnNavigationToolConnectionStateChanged;
            navigation.VideoFrameChanged += OnNavigationVideoFrameChanged;
        }

        private void OnFluoroIaeConnectionStateChanged()
        {
            try
            {
                if (IaeConnectionStateChanged != null)
                {
                    Dispatcher.UIThread.Post(IaeConnectionStateChanged);
                }
            }
            catch (Exception ex)
            {
                trace.Error("OnFluoroIaeConnectionStateChanged(): Error\n{0}", ex);
            }
        }

        private void OnFluoroCurrentXrayImageChanged()
        {
            try
            {
                if (CurrentXrayImageChanged != null)
                {
                    Dispatcher.UIThread.Post(CurrentXrayImageChanged);
                }
            }
            catch (Exception ex)
            {
                trace.Error("OnFluoroCurrentXrayImageChanged(): Error\n{0}", ex);
            }
        }

        private void OnNavigationVideoFrameChanged()
        {
            try
            {
                if (VideoFrameChanged != null)
                {
                    Dispatcher.UIThread.Post(VideoFrameChanged);
                }
            }
            catch (Exception ex)
            {
                trace.Error("OnNavigationVideoFrameChanged(): Error\n{0}", ex);
            }
        }

        private void OnNavigationToolConnectionStateChanged()
        {
            try
            {
                if (ToolConnectionStateChanged != null)
                {
                    Dispatcher.UIThread.Post(ToolConnectionStateChanged);
                }
            }
            catch (Exception ex)
            {
                trace.Error("OnNavigationToolConnectionStateChanged(): Error\n{0}", ex);
            }
        }

        #endregion

        #region Callbacks

        #endregion
    }
}
