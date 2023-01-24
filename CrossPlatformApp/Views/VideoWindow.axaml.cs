// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.ViewModels;
using CrossPlatformApp.Infrastructure;

namespace CrossPlatformApp.Views
{
    public partial class VideoWindow : Window
    {
        public VideoWindow()
        {
            //if (!Design.IsDesignMode)
            //{
            //    Owner = CrossPlatformAppSystem.Instance.Services.Get<MainWindow>();
            //}

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnOpened(object sender, EventArgs e)
        {
            if (!Design.IsDesignMode)
            {
                var vm = DataContext as VideoWindowViewModel;
                vm?.Play();
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!Design.IsDesignMode)
            {
                var vm = DataContext as VideoWindowViewModel;
                vm?.Stop();
            }
        }

    }
}
