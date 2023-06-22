// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using Avalonia.Threading;

namespace FontShift.Application.Mvvm
{
    public static class ActionDispatcher
    {
        public static void Execute(Action callback, DispatcherPriority? priority = null)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                callback.Invoke();
            }
            else
            {
                DispatcherPriority prio = priority ?? DispatcherPriority.Normal;
                Dispatcher.UIThread.Post(callback, prio);
            }
        }
    }
}
