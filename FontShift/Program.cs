// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia;
using System;

namespace FontShift
{
    internal static class Program
    {
        /// <summary>
        ///     Initialization code. Don't use any Avalonia, third-party APIs or any
        ///     SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        ///     yet and stuff might break.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // Start the UI framework system
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);

#pragma warning disable S125 // Sections of code should not be commented out
                // Uncomment below line to test application shutdown better
                // Console.ReadLine();
#pragma warning restore S125 // Sections of code should not be commented out
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///     Avalonia configuration, don't remove; also used by visual designer.
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
        {
            OperatingSystem system = Environment.OSVersion;

            if (system.Platform == PlatformID.Win32NT)
            {
                return AppBuilder.Configure<App>()
                    .WithInterFont()
                    .UsePlatformDetect();
            }
            else
            {
                // TODO (fh): Startup for OS other than Windows currently identical.
                return AppBuilder.Configure<App>()
                    .WithInterFont()
                    .UsePlatformDetect();
            }
        }

    }
}
