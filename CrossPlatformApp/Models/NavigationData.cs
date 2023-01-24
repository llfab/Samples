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
using Avalonia.Media.Imaging;
using BitsOfNature.Core.Imaging;

namespace CrossPlatformApp.Models
{
    public class NavigationData
    {
        #region Events

        public event Action VideoFrameChanged;
        public event Action ToolConnectionStateChanged;

        #endregion

        #region Fields

        private bool toolConnected;
        public bool ToolConnected
        {
            get => toolConnected;
            set
            {
                if (toolConnected != value)
                {
                    toolConnected = value;
                    ToolConnectionStateChanged?.Invoke();
                }
            }
        }

        private ArgbImage videoFrame;
        public ArgbImage VideoFrame
        {
            get => videoFrame;
            set
            {
                if (videoFrame != value)
                {
                    videoFrame = value;
                    VideoFrameChanged?.Invoke();
                }
            }
        }

        #endregion

        #region Construction

        public NavigationData()
        {
            videoFrame = null;
            toolConnected = false;
        }

        #endregion
    }
}
