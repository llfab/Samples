// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using FontShift.Application.Infrastructure;
using FontShift.Application.ViewModels;
using FontShift.Application.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FontShift
{
    public partial class App : Avalonia.Application
    {
        private const string HideSplashScreen = "HideSplashScreen";

        private Trace _trace;

        private FontShiftSystem _pixrBunionSystem;

        private SplashScreenWindow _splashScreenWindow;
        private MainWindow _mainWindow;
        private MainWindowViewModel _mainWindowViewModel;

        public override void Initialize()
        {
            _trace = Trace.ForType<App>();

            #region Design Mode

            if (Design.IsDesignMode)
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
            }

            #endregion

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            _trace.Debug("{0} called.", Trace.Site());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                bool hideSplashScreen = true;

                if (hideSplashScreen)
                {
                    CompleteApplicationStart();
                }
                else
                {
                    // Open splash screen as early as possible
                    _splashScreenWindow = new SplashScreenWindow();
                    desktopLifetime.MainWindow = _splashScreenWindow;

                    // delegate actual application start to when UI thread has time again
                    Dispatcher.UIThread.Post(() => CompleteApplicationStart(), DispatcherPriority.Background);
                }
            }
            base.OnFrameworkInitializationCompleted();
        }

        public void CompleteApplicationStart()
        {
            _trace.Debug("{0} called.", Trace.Site());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                if (Design.IsDesignMode)
                {
                    // nothing to do (yet)
                }
                else
                {
                    StartupParameters startupParameters = new StartupParameters()
                    {
                        Args = Array.Empty<string>()
                    };

                    // Start the system
                    _pixrBunionSystem = new FontShiftSystem(startupParameters);
                    _pixrBunionSystem.Init();

                    CultureInfo.CurrentCulture = new CultureInfo("en-US");
                }

                // create the window with its data context
                _mainWindowViewModel = new MainWindowViewModel();
                _mainWindow = new MainWindow()
                {
                    DataContext = _mainWindowViewModel,
                };

                desktopLifetime.MainWindow = _mainWindow;
                desktopLifetime.Exit += OnDesktopLifetimeExit;
                desktopLifetime.ShutdownRequested += OnDesktopLifetimeShutdownRequested;

                if (_splashScreenWindow != null)
                {
                    // Show main window to avoid framework shutdown when closing splash screen
                    _mainWindow.Show();

                    // Finally, close the splash screen
                    _splashScreenWindow.Close();
                }
            }

            _trace.Debug("{0} done.", Trace.Site());
        }

        //private void ApplyWindowProperties(StartupConfiguration startup)
        //{
        //    // check if we have configured a correct window state
        //    WindowState windowState = WindowState.Normal;
        //    if (!string.IsNullOrEmpty(startup.WindowState))
        //    {
        //        if (Enum.TryParse<WindowState>(startup.WindowState, true, out WindowState state))
        //        {
        //            windowState = state;
        //        }
        //        else
        //        {
        //            _trace.Fatal("{0} WindowState[{1}] invalid.", Trace.Site(), startup.WindowState);
        //        }
        //    }

        //    _mainWindow.WindowState = windowState;

        //    // calculate the x/y screen location addend dependent on the 
        //    // configured screen number (screen number starts with 1!!!)
        //    Int2 screenLocationAddend = Int2.Zeros;
        //    if (startup.ScreenNumber > 0)
        //    {
        //        List<Screen> screens = _mainWindow.Screens.All.OrderBy(x => x.Bounds.X).ThenBy(y => y.Bounds.Y).ToList();
        //        for (int i = 0; i < screens.Count; i++)
        //        {
        //            Screen screen = screens[i];
        //            _trace.Debug("{0} Primary[{1}] PixelDensity[{2}] Bounds[{3}] WorkingArea[{4}]", Trace.Site(), screen.IsPrimary, screen.Scaling, screen.Bounds, screen.WorkingArea);
        //        }

        //        if (startup.ScreenNumber > screens.Count)
        //        {
        //            _trace.Fatal("{0} SelectedScreenNumber[{1}] invalid. AvailableScreens[{2}]", Trace.Site(), startup.ScreenNumber, screens.Count);
        //        }
        //        else
        //        {
        //            Screen selectedScreen = screens[startup.ScreenNumber - 1];
        //            screenLocationAddend.X = selectedScreen.Bounds.X;
        //            screenLocationAddend.Y = selectedScreen.Bounds.Y;
        //        }
        //    }

        //    // remove system decorations (e.g. title bar) when not in full screen mode
        //    if (windowState != WindowState.FullScreen && startup.HideSystemDecorations)
        //    {
        //        _mainWindow.SystemDecorations = SystemDecorations.None;
        //    }

        //    // adjust screen position (if configured to do so)
        //    if (startup.WindowLocation.X >= 0 && startup.WindowLocation.Y >= 0)
        //    {
        //        if (windowState == WindowState.Normal)
        //        {
        //            // for normal window state just add the screen addend to the desired relative position
        //            _mainWindow.Position = new PixelPoint(startup.WindowLocation.X + screenLocationAddend.X, startup.WindowLocation.Y + screenLocationAddend.Y);
        //        }
        //        else if (windowState == WindowState.FullScreen)
        //        {
        //            // for full screen state make sure that zeros are used for the relative position 
        //            // such that the final position will be the upper left corner of the selected screen
        //            _mainWindow.Position = new PixelPoint(0 + screenLocationAddend.X, 0 + screenLocationAddend.Y);
        //        }
        //    }

        //    // only set the windows size if we have select normal window state
        //    if (windowState == WindowState.Normal && startup.WindowSize.Width > 0 && startup.WindowSize.Height > 0)
        //    {
        //        _mainWindow.Width = startup.WindowSize.Width;
        //        _mainWindow.Height = startup.WindowSize.Height;
        //    }
        //}

        private void OnDesktopLifetimeShutdownRequested(object sender, ShutdownRequestedEventArgs e)
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                // nothing to do
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void OnDesktopLifetimeExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _pixrBunionSystem.Exit();

                _trace.Info("{0} done.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        public static void DoExit()
        {
            try
            {
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    desktopLifetime.TryShutdown();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
