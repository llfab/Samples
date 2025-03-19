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

namespace AvaloniaIosApp.Desktop
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                        .LogToTrace()
                        .UsePlatformDetect()
                        .WithInterFont();
        }
    }
}
