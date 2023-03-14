// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaApplication10
{
    public enum WorkflowStateType
    {
        None,
        Osteotomy,
        FragmentDisplacement,
        CartridgeSelection,
        ScrewPlacement
    }

    public class WorkflowStateBoolConverter : EnumBoolConverter<WorkflowStateType>
    { }

    public abstract class EnumBoolConverter<T> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not T enumValue)
            {
                return value;
            }

            if (targetType != typeof(bool) && targetType != typeof(System.Boolean) && Nullable.GetUnderlyingType(targetType) != typeof(bool) && Nullable.GetUnderlyingType(targetType) != typeof(System.Boolean))
            {
                return value;
            }

            if (parameter is not string stringParameter)
            {
                return value;
            }

            return enumValue.ToString().Equals(stringParameter, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Function not supported. Make sure your Binding is OneWay");
        }
    }
}
