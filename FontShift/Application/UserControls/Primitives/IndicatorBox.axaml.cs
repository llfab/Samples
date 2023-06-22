// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace FontShift.Application.UserControls.Primitives
{
    public enum IndicatorTypeOption
    {
        None,
        Info,
        Warning,
        Error
    }

    public class IndicatorBox : TemplatedControl
    {
        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<IndicatorBox, HorizontalAlignment>(nameof(HorizontalContentAlignmentProperty), HorizontalAlignment.Center);

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<IndicatorBox, VerticalAlignment>(nameof(VerticalContentAlignmentProperty), VerticalAlignment.Center);

        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> TextHorizontalAlignmentProperty =
            AvaloniaProperty.Register<IndicatorBox, HorizontalAlignment>(nameof(TextHorizontalAlignmentProperty), HorizontalAlignment.Center);

        public HorizontalAlignment TextHorizontalAlignment
        {
            get { return GetValue(TextHorizontalAlignmentProperty); }
            set { SetValue(TextHorizontalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> TextVerticalAlignmentProperty =
            AvaloniaProperty.Register<IndicatorBox, VerticalAlignment>(nameof(TextVerticalAlignmentProperty), VerticalAlignment.Center);

        public VerticalAlignment TextVerticalAlignment
        {
            get { return GetValue(TextVerticalAlignmentProperty); }
            set { SetValue(TextVerticalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<Thickness> TextMarginProperty =
            AvaloniaProperty.Register<IndicatorBox, Thickness>(nameof(TextMarginProperty), new Thickness(0));

        public Thickness TextMargin
        {
            get { return GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<IndicatorBox, string>(nameof(TextProperty), null);

        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> IconHorizontalAlignmentProperty =
            AvaloniaProperty.Register<IndicatorBox, HorizontalAlignment>(nameof(IconHorizontalAlignmentProperty), HorizontalAlignment.Center);

        public HorizontalAlignment IconHorizontalAlignment
        {
            get { return GetValue(IconHorizontalAlignmentProperty); }
            set { SetValue(IconHorizontalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> IconVerticalAlignmentProperty =
            AvaloniaProperty.Register<IndicatorBox, VerticalAlignment>(nameof(IconVerticalAlignmentProperty), VerticalAlignment.Center);

        public VerticalAlignment IconVerticalAlignment
        {
            get { return GetValue(IconVerticalAlignmentProperty); }
            set { SetValue(IconVerticalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<Thickness> IconMarginProperty =
            AvaloniaProperty.Register<IndicatorBox, Thickness>(nameof(IconMarginProperty), new Thickness(0));

        public Thickness IconMargin
        {
            get { return GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        public static readonly StyledProperty<IImage> IconProperty =
            AvaloniaProperty.Register<IndicatorBox, IImage>(nameof(IconProperty), null);

        public IImage Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly StyledProperty<double> IconScaleFactorProperty =
            AvaloniaProperty.Register<IndicatorBox, double>(nameof(IconScaleFactorProperty), 0.0);

        public double IconScaleFactor
        {
            get { return GetValue(IconScaleFactorProperty); }
            set { SetValue(IconScaleFactorProperty, value); }
        }

        public static readonly DirectProperty<IndicatorBox, double> IconWidthProperty =
            AvaloniaProperty.RegisterDirect<IndicatorBox, double>(nameof(IconWidthProperty), o => o.IconWidth);

        public double IconWidth
        {
            get
            {
                if (IconScaleFactor <= 0.0)
                {
                    return Icon == null ? 0.0 : Icon.Size.Width;
                }
                else
                {
                    return Icon == null ? 0.0 : IconScaleFactor * Icon.Size.Width;
                }
            }
        }

        public static readonly DirectProperty<IndicatorBox, double> IconHeightProperty =
            AvaloniaProperty.RegisterDirect<IndicatorBox, double>(nameof(IconHeightProperty), o => o.IconHeight);

        public double IconHeight
        {
            get
            {
                if (IconScaleFactor <= 0.0)
                {
                    return Icon == null ? 0.0 : Icon.Size.Height;
                }
                else
                {
                    return Icon == null ? 0.0 : IconScaleFactor * Icon.Size.Height;
                }
            }
        }

        public static readonly StyledProperty<IndicatorTypeOption> IndicatorTypeProperty =
            AvaloniaProperty.Register<IndicatorBox, IndicatorTypeOption>(nameof(IndicatorTypeProperty), IndicatorTypeOption.None);

        public IndicatorTypeOption IndicatorType
        {
            get { return GetValue(IndicatorTypeProperty); }
            set { SetValue(IndicatorTypeProperty, value); }
        }

    }
}
