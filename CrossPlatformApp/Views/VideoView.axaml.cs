// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.ViewModels;

namespace CrossPlatformApp.Views
{
    public partial class VideoView : UserControl
    {
        public VideoView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
