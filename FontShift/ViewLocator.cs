// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace FontShift
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object data)
        {
            string name = data.GetType().FullName!.Replace("ViewModel", "View");
            Type type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is FontShift.Application.Mvvm.ViewModel;
        }
    }
}
