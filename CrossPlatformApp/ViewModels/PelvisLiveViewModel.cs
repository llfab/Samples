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
    public class PelvisLiveViewModel : CrossPlatformApp.Mvvm.ViewModelBase, IDisposable
    {
        private Trace trace;

        private bool isAll;
        public bool IsAll
        {
            get { return isAll; }
            set
            {
                if (isAll != value)
                {
                    RaiseAndSetIfChanged(ref isAll, value);
                    if (value)
                    {
                        IsLateral = false;
                        IsInletOutlet = false;

                        TopRowHeight = GridLength.Star;
                        BorderRowHeight = new GridLength(2);
                        BottomRowHeight = GridLength.Star;

                        LeftColumnWidth = GridLength.Star;
                        BorderColumnWidth = new GridLength(2);
                        RightColumnWidth = GridLength.Star;
                    }
                }
            }
        }

        private bool isLateral;
        public bool IsLateral
        {
            get { return isLateral; }
            set
            {
                if (isLateral != value)
                {
                    RaiseAndSetIfChanged(ref isLateral, value);
                    if (value)
                    {
                        IsAll = false;
                        IsInletOutlet = false;

                        TopRowHeight = GridLength.Star;
                        BorderRowHeight = new GridLength(0);
                        BottomRowHeight = new GridLength(0);

                        LeftColumnWidth = GridLength.Star;
                        BorderColumnWidth = new GridLength(2);
                        RightColumnWidth = GridLength.Star;
                    }
                }
            }
        }

        private bool isInletOutlet;
        public bool IsInletOutlet
        {
            get { return isInletOutlet; }
            set
            {
                if (isInletOutlet != value)
                {
                    RaiseAndSetIfChanged(ref isInletOutlet, value);
                    if (value)
                    {
                        IsAll = false;
                        IsLateral = false;

                        TopRowHeight = new GridLength(0);
                        BorderRowHeight = new GridLength(0);
                        BottomRowHeight = GridLength.Star;

                        LeftColumnWidth = GridLength.Star;
                        BorderColumnWidth = new GridLength(2);
                        RightColumnWidth = GridLength.Star;
                    }
                }
            }
        }

        private GridLength topRowHeight;
        public GridLength TopRowHeight
        {
            get { return topRowHeight; }
            set { RaiseAndSetIfChanged(ref topRowHeight, value);  }
        }

        private GridLength borderRowHeight;
        public GridLength BorderRowHeight
        {
            get { return borderRowHeight; }
            set { RaiseAndSetIfChanged(ref borderRowHeight, value); }
        }

        private GridLength bottomRowHeight;
        public GridLength BottomRowHeight
        {
            get { return bottomRowHeight; }
            set { RaiseAndSetIfChanged(ref bottomRowHeight, value); }
        }

        private GridLength leftColumnWidth;
        public GridLength LeftColumnWidth
        {
            get { return leftColumnWidth; }
            set { RaiseAndSetIfChanged(ref leftColumnWidth, value); }
        }

        private GridLength borderColumnWidth;
        public GridLength BorderColumnWidth
        {
            get { return borderColumnWidth; }
            set { RaiseAndSetIfChanged(ref borderColumnWidth, value); }
        }

        private GridLength rightColumnWidth;
        public GridLength RightColumnWidth
        {
            get { return rightColumnWidth; }
            set { RaiseAndSetIfChanged(ref rightColumnWidth, value); }
        }

        private XrayViewModel xrayModel1;
        public XrayViewModel XrayModel1
        {
            get { return xrayModel1; }
        }

        private VideoViewModel videoModel2;
        public VideoViewModel VideoModel2
        {
            get { return videoModel2; }
        }

        private XrayViewModel xrayModel3;
        public XrayViewModel XrayModel3
        {
            get { return xrayModel3; }
        }

        private XrayViewModel xrayModel4;
        public XrayViewModel XrayModel4
        {
            get { return xrayModel4; }
        }

        public PelvisLiveViewModel()
        {
            TopRowHeight = GridLength.Star;
            BorderRowHeight = new GridLength(2);
            BottomRowHeight = GridLength.Star;

            LeftColumnWidth = GridLength.Star;
            BorderColumnWidth = new GridLength(2);
            RightColumnWidth = GridLength.Star;

            xrayModel1 = new XrayViewModel(XrayType.Lateral);
            videoModel2 = new VideoViewModel();
            xrayModel3 = new XrayViewModel(XrayType.Inlet);
            xrayModel4 = new XrayViewModel(XrayType.Outlet);

            if (!Design.IsDesignMode)
            {
                trace = new Trace("XrayViewModel");
                IsAll = true;
            }
            else
            {
                IsAll = true;
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

    }
}
