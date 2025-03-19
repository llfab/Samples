// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2024 Stryker
// ===========================================================================

using System;
using Avalonia.Controls;

namespace AvaloniaIosApp.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (DataContext is ViewModels.MainViewModel vm)
            {
                vm.SetSceneViewerControl(PART_SceneViewer);
            }
        }
    }
}
