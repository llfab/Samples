// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia;
using Avalonia.Data.Converters;
using FontShift.Application.Infrastructure;
using System;
using System.Globalization;

namespace FontShift.Application.UserControls.Converters
{
    public class DoubleCornerRadiusConverter : IValueConverter
    {
        private static readonly Trace s_trace = Trace.ForType<DoubleCornerRadiusConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double doubleValue)
            {
                return value;
            }

            if (targetType != typeof(CornerRadius))
            {
                return value;
            }

            return new CornerRadius(doubleValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            s_trace.Fatal("{0} Function not supported. Make sure your Binding is OneWay", Trace.Site());
            throw new NotImplementedException();
        }
    }

    public class BoundsCornerRadiusConverter : IValueConverter
    {
        private static readonly Trace s_trace = Trace.ForType<BoundsCornerRadiusConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Rect rectValue)
            {
                return new CornerRadius(0);
            }

            if (targetType != typeof(CornerRadius))
            {
                return new CornerRadius(0);
            }

            double relativeShortEdgeValue = 0.5;
            if (parameter != null)
            {
                relativeShortEdgeValue = System.Convert.ToDouble(parameter);
            }

            double shortEdge = Math.Min(rectValue.Width, rectValue.Height);
            double cornerRadius = shortEdge * relativeShortEdgeValue;

            return new CornerRadius(cornerRadius);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            s_trace.Fatal("{0} Function not supported. Make sure your Binding is OneWay", Trace.Site());
            throw new NotImplementedException();
        }
    }
}
