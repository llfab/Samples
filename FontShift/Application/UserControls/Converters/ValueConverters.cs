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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FontShift.Application.UserControls.Converters
{
    public class MinValueConverter : IMultiValueConverter
    {
        private static readonly Trace s_trace = Trace.ForType<MinValueConverter>();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count == 0)
            {
                return null;
            }

            return values.Min();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            s_trace.Fatal("{0} Function not supported. Make sure your Binding is OneWay", Trace.Site());
            throw new NotImplementedException();
        }
    }

    public class MaxValueConverter : IMultiValueConverter
    {
        private static readonly Trace s_trace = Trace.ForType<MaxValueConverter>();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count == 0)
            {
                return null;
            }

            return values.Max();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            s_trace.Fatal("{0} Function not supported. Make sure your Binding is OneWay", Trace.Site());
            throw new NotImplementedException();
        }
    }
}
