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
using UIKit;

namespace AvaloniaIosApp.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        public static void Main(string[] args)
        {
            try
            {
                // if you want to use a different Application Delegate class from "AppDelegate"
                // you can specify it here.
                UIApplication.Main(args, null, typeof(AppDelegate));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
