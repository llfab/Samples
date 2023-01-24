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
using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using CrossPlatformApp.Models;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.UI.Avalonia;
using BitsOfNature.Core.Imaging;
using BitsOfNature.Core.IO.FileFormats;
using BitsOfNature.Core.Drawing.Interop;
using BitsOfNature.UI.Avalonia.Utils;

namespace CrossPlatformApp.ViewModels
{
    public class VideoViewModel : CrossPlatformApp.Mvvm.ViewModelBase, IDisposable
    {
        private Trace trace;

        private bool videoFrameUpdateEnabled;
        public bool VideoFrameUpdateEnabled
        {
            get { return videoFrameUpdateEnabled; }
            set { RaiseAndSetIfChanged(ref videoFrameUpdateEnabled, value); }
        }

        private ArgbImage videoFrame;
        public ArgbImage VideoFrame
        {
            get { return videoFrame; }
            set { RaiseAndSetIfChanged(ref videoFrame, value); }
        }

        private PresentationData data;

        public VideoViewModel()
        {
            if (!Design.IsDesignMode)
            {
                trace = new Trace("XrayViewModel");

                VideoFrameUpdateEnabled = false;

                data = CrossPlatformApp.Infrastructure.CrossPlatformAppSystem.Instance.Services.Get<PresentationData>();
                data.VideoFrameChanged += OnDataVideoFrameChanged;
                data.ToolConnectionStateChanged += OnDataToolConnectionStateChanged;

                UpdateToolConnectionState();
                UpdateVideoFrame();
            }
            else
            {
                VideoFrameUpdateEnabled = true;
                IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                Stream stream = assets.Open(new Uri("avares://CrossPlatformApp/Assets/Images/Video.png"));
                VideoFrame = DesignerUtils.LoadImage<ArgbImage>(stream);
            }
        }

        public void Dispose()
        {
            try
            {
            }
            catch (Exception ex)
            {
                trace.Error("Dispose(): Error.\n{0}", ex);
            }
        }

        private void OnDataToolConnectionStateChanged()
        {
            UpdateToolConnectionState();
        }

        private void OnDataVideoFrameChanged()
        {
            UpdateVideoFrame();
        }

        private void UpdateToolConnectionState()
        {
            try
            {
                //PresentationData data = CrossPlatformApp.Infrastructure.CrossPlatformAppSystem.Instance.Services.Get<PresentationData>();
                //Tool = data.Fluoro.IaeConnected;
                //Text = data.Fluoro.IaeConnected ? "Image Acquisition connected!" : "Image Acquisition NOT connected!";
            }
            catch (Exception ex)
            {
                trace.Error("UpdateToolConnectionState(): Error\n{0}", ex);
            }
        }

        private void UpdateVideoFrame()
        {
            try
            {
                if (!videoFrameUpdateEnabled)
                {
                    return;
                }

                VideoFrame = data.Navigation.VideoFrame;
            }
            catch (Exception ex)
            {
                trace.Error("UpdateVideoFrame(): Error\n{0}", ex);
            }
        }

    }
}
