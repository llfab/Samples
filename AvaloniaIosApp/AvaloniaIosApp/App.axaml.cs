// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2024 Stryker
// ===========================================================================

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using AvaloniaIosApp.ViewModels;
using AvaloniaIosApp.Views;

namespace AvaloniaIosApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            AvaloniaIosAppSystem system = new AvaloniaIosAppSystem();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                MainViewModel vm = AvaloniaIosAppSystem.Instance.MainViewModel;

                vm.Init3D();
                desktop.Exit += (sender, args) => vm.Exit3D();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                MainViewModel vm = AvaloniaIosAppSystem.Instance.MainViewModel;

                vm.Init3D();
                //singleViewPlatform.Exit += (sender, args) => vm.Exit3D();
                
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = vm
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static IStorageProvider GetStorageProvider()
        {
            TopLevel result = Application.Current?.ApplicationLifetime switch
            {
                IClassicDesktopStyleApplicationLifetime d => d.MainWindow,
                ISingleViewApplicationLifetime s => TopLevel.GetTopLevel(s.MainView),
                _ => null
            };
            return result?.StorageProvider;
        }


    }
}
