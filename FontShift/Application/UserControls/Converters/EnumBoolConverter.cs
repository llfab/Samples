// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia.Data.Converters;
using FontShift.Application.Infrastructure;
using FontShift.Application.Models;
using System;
using System.Globalization;

namespace FontShift.Application.UserControls.Converters
{
    public class GuidanceWorkflowStateBoolConverter : EnumBoolConverter<GuidanceWorkflowStateType>
    { }

    public abstract class EnumBoolConverter<T> : IValueConverter
    {
        private static readonly Trace s_trace = new Trace(string.Format("EnumBoolConverter<{0}>", typeof(T).Name));

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
            s_trace.Fatal("{0} Function not supported. Make sure your Binding is OneWay", Trace.Site());
            throw new NotImplementedException(string.Format("{0} Function not supported. Make sure your Binding is OneWay", Trace.Site()));
        }
    }
}
