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
using Avalonia.Controls.Templates;

namespace AvaloniaIosApp
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object data)
        {
            if (data is null)
                return null;

            string name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            Type type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
