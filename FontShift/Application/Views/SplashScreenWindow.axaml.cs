// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2023 Stryker
// ===========================================================================

using Avalonia.Controls;

namespace FontShift.Application.Views
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();

            if (_image.Source != null)
            {
                Width = _image.Source.Size.Width;
                Height = _image.Source.Size.Height;
            }
        }
    }
}
