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

namespace CrossPlatformApp.ViewModels
{
    public enum XrayType
    {
        None,
        Lateral,
        Inlet,
        Outlet,
    }

    public class XrayViewModel : CrossPlatformApp.Mvvm.ViewModelBase, IDisposable
    {
        private Trace trace;

        private XrayType type;

        private string text;
        public string Text
        {
            get { return text; }
            set { RaiseAndSetIfChanged(ref text, value); }
        }

        private bool iaeConnected;
        public bool IaeConnected
        {
            get { return iaeConnected; }
            set { RaiseAndSetIfChanged(ref iaeConnected, value); }
        }

        private bool xrayImageUpdateEnabled;
        public bool XrayImageUpdateEnabled
        {
            get { return xrayImageUpdateEnabled; }
            set { RaiseAndSetIfChanged(ref xrayImageUpdateEnabled, value); }
        }

        private Bitmap xrayImage;
        public Bitmap XrayImage
        {
            get { return xrayImage; }
            set { RaiseAndSetIfChanged(ref xrayImage, value); }
        }

        public XrayViewModel()
            : this(XrayType.None)
        { }

        public XrayViewModel(XrayType type = XrayType.None)
        {
            this.type = type;
            Text = "1234567890abcdefgh";

            XrayImageUpdateEnabled = true;

            if (!Design.IsDesignMode)
            {
                trace = new Trace("XrayViewModel");

                if (type == XrayType.None)
                {
                    PresentationData data = CrossPlatformApp.Infrastructure.CrossPlatformAppSystem.Instance.Services.Get<PresentationData>();
                    data.CurrentXrayImageChanged += OnDataCurrentXrayImageChanged;
                    data.IaeConnectionStateChanged += OnDataIaeConnectionStateChanged;
                }

                UpdateIaeConnectionState();
                UpdateXrayImage();
            }
            else
            {
                Text = "Image Acquisition NOT connected!";
                IaeConnected = false;
                IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                XrayImage = new Bitmap(assets.Open(new Uri("avares://CrossPlatformApp/Assets/Images/XrayImage.png")));
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

        private void OnDataIaeConnectionStateChanged()
        {
            UpdateIaeConnectionState();
        }

        private void OnDataCurrentXrayImageChanged()
        {
            UpdateXrayImage();
        }

        private void UpdateIaeConnectionState()
        {
            try
            {
                PresentationData data = CrossPlatformApp.Infrastructure.CrossPlatformAppSystem.Instance.Services.Get<PresentationData>();
                IaeConnected = data.Fluoro.IaeConnected;
                Text = data.Fluoro.IaeConnected ? "Image Acquisition connected!" : "Image Acquisition NOT connected!";
            }
            catch (Exception ex)
            {
                trace.Error("UpdateIaeConnectionState(): Error\n{0}", ex);
            }
        }

        private void UpdateXrayImage()
        {
            try
            {
                if (!xrayImageUpdateEnabled)
                {
                    return;
                }

                if (type == XrayType.None)
                {
                    PresentationData data = CrossPlatformApp.Infrastructure.CrossPlatformAppSystem.Instance.Services.Get<PresentationData>();
                    XrayImage = data.Fluoro.CurrentXrayImage;
                }
                else if (type == XrayType.Lateral)
                {
                    IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    XrayImage = new Bitmap(assets.Open(new Uri("avares://CrossPlatformApp/Assets/Images/Lateral.png")));
                }
                else if (type == XrayType.Inlet)
                {
                    IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    XrayImage = new Bitmap(assets.Open(new Uri("avares://CrossPlatformApp/Assets/Images/Inlet.png")));
                }
                else if (type == XrayType.Outlet)
                {
                    IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    XrayImage = new Bitmap(assets.Open(new Uri("avares://CrossPlatformApp/Assets/Images/Outlet.png")));
                }
            }
            catch (Exception ex)
            {
                trace.Error("UpdateXrayImage(): Error\n{0}", ex);
            }
        }
    }
}
