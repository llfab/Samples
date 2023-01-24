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

namespace CrossPlatformApp.Models
{
    public class FluoroData
    {
        #region Events

        public event Action CurrentXrayImageChanged;
        public event Action IaeConnectionStateChanged;

        #endregion

        #region Fields

        private bool iaeConnected;
        public bool IaeConnected
        {
            get => iaeConnected;
            set
            {
                if (iaeConnected != value)
                {
                    iaeConnected = value;
                    IaeConnectionStateChanged?.Invoke();
                }
            }
        }

        private Bitmap currentXrayImage;
        public Bitmap CurrentXrayImage
        {
            get => currentXrayImage;
            set
            {
                if (currentXrayImage != value)
                {
                    currentXrayImage = value;
                    CurrentXrayImageChanged?.Invoke();
                }
            }
        }

        #endregion

        #region Construction

        public FluoroData()
        {
            currentXrayImage = null;
            iaeConnected = false;
        }

        #endregion
    }
}
