// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2024 Stryker
// ===========================================================================

using System;
using Avalonia;
using Avalonia.iOS;
using Foundation;

namespace AvaloniaIosApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public partial class AppDelegate : AvaloniaAppDelegate<App>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            try
            {
                return base.CustomizeAppBuilder(builder)
                .LogToTrace()
                .WithInterFont()
                .AfterSetup(_ =>
                {
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
