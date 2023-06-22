// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace FontShift.Application.UserControls.Primitives
{
    public class MenuActionItem : Button
    {
        #region Styled Properties

        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<MenuActionItem, Orientation>(nameof(OrientationProperty), Orientation.Horizontal);

        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> TextHorizontalAlignmentProperty =
            AvaloniaProperty.Register<MenuActionItem, HorizontalAlignment>(nameof(TextHorizontalAlignmentProperty), HorizontalAlignment.Center);

        public HorizontalAlignment TextHorizontalAlignment
        {
            get { return GetValue(TextHorizontalAlignmentProperty); }
            set { SetValue(TextHorizontalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> TextVerticalAlignmentProperty =
            AvaloniaProperty.Register<MenuActionItem, VerticalAlignment>(nameof(TextVerticalAlignmentProperty), VerticalAlignment.Center);

        public VerticalAlignment TextVerticalAlignment
        {
            get { return GetValue(TextVerticalAlignmentProperty); }
            set { SetValue(TextVerticalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<Thickness> TextMarginProperty =
            AvaloniaProperty.Register<MenuActionItem, Thickness>(nameof(TextMarginProperty), new Thickness(0));

        public Thickness TextMargin
        {
            get { return GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<MenuActionItem, string>(nameof(TextProperty), null);

        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> IconHorizontalAlignmentProperty =
            AvaloniaProperty.Register<MenuActionItem, HorizontalAlignment>(nameof(IconHorizontalAlignmentProperty), HorizontalAlignment.Center);

        public HorizontalAlignment IconHorizontalAlignment
        {
            get { return GetValue(IconHorizontalAlignmentProperty); }
            set { SetValue(IconHorizontalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> IconVerticalAlignmentProperty =
            AvaloniaProperty.Register<MenuActionItem, VerticalAlignment>(nameof(IconVerticalAlignmentProperty), VerticalAlignment.Center);

        public VerticalAlignment IconVerticalAlignment
        {
            get { return GetValue(IconVerticalAlignmentProperty); }
            set { SetValue(IconVerticalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<Thickness> IconMarginProperty =
            AvaloniaProperty.Register<MenuActionItem, Thickness>(nameof(IconMarginProperty), new Thickness(0));

        public Thickness IconMargin
        {
            get { return GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        public static readonly StyledProperty<IImage> IconProperty =
            AvaloniaProperty.Register<MenuActionItem, IImage>(nameof(IconProperty), null);

        public IImage Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly StyledProperty<double> IconScaleFactorProperty =
            AvaloniaProperty.Register<MenuActionItem, double>(nameof(IconScaleFactorProperty), 0.0);

        public double IconScaleFactor
        {
            get { return GetValue(IconScaleFactorProperty); }
            set { SetValue(IconScaleFactorProperty, value); }
        }

        public static readonly DirectProperty<MenuActionItem, double> IconWidthProperty =
            AvaloniaProperty.RegisterDirect<MenuActionItem, double>(nameof(IconWidthProperty), o => o.IconWidth);

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

        public static readonly DirectProperty<MenuActionItem, double> IconHeightProperty =
            AvaloniaProperty.RegisterDirect<MenuActionItem, double>(nameof(IconHeightProperty), o => o.IconHeight);

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

        public static readonly StyledProperty<HorizontalAlignment> PathIconHorizontalAlignmentProperty =
            AvaloniaProperty.Register<MenuActionItem, HorizontalAlignment>(nameof(PathIconHorizontalAlignmentProperty), HorizontalAlignment.Center);

        public HorizontalAlignment PathIconHorizontalAlignment
        {
            get { return GetValue(PathIconHorizontalAlignmentProperty); }
            set { SetValue(PathIconHorizontalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> PathIconVerticalAlignmentProperty =
            AvaloniaProperty.Register<MenuActionItem, VerticalAlignment>(nameof(PathIconVerticalAlignmentProperty), VerticalAlignment.Center);

        public VerticalAlignment PathIconVerticalAlignment
        {
            get { return GetValue(PathIconVerticalAlignmentProperty); }
            set { SetValue(PathIconVerticalAlignmentProperty, value); }
        }

        public static readonly StyledProperty<Thickness> PathIconMarginProperty =
            AvaloniaProperty.Register<MenuActionItem, Thickness>(nameof(PathIconMarginProperty), new Thickness(0));

        public Thickness PathIconMargin
        {
            get { return GetValue(PathIconMarginProperty); }
            set { SetValue(PathIconMarginProperty, value); }
        }

        public static readonly StyledProperty<Geometry> PathIconDataProperty =
            AvaloniaProperty.Register<MenuActionItem, Geometry>(nameof(PathIconDataProperty), null);

        public Geometry PathIconData
        {
            get { return GetValue(PathIconDataProperty); }
            set { SetValue(PathIconDataProperty, value); }
        }

        public static readonly StyledProperty<double> PathIconWidthProperty =
            AvaloniaProperty.Register<MenuActionItem, double>(nameof(PathIconWidthProperty), 32);

        public double PathIconWidth
        {
            get { return GetValue(PathIconWidthProperty); }
            set { SetValue(PathIconWidthProperty, value); }
        }

        public static readonly StyledProperty<double> PathIconHeightProperty =
            AvaloniaProperty.Register<MenuActionItem, double>(nameof(PathIconHeightProperty), 32);

        public double PathIconHeight
        {
            get { return GetValue(PathIconHeightProperty); }
            set { SetValue(PathIconHeightProperty, value); }
        }

        public static readonly StyledProperty<TimeSpan> PressedAnimationStateDurationProperty =
            AvaloniaProperty.Register<MenuActionItem, TimeSpan>(nameof(PressedAnimationStateDurationProperty), TimeSpan.Zero);

        public TimeSpan PressedAnimationStateDuration
        {
            get { return GetValue(PressedAnimationStateDurationProperty); }
            set 
            {
                SetValue(PressedAnimationStateDurationProperty, value);

                UpdatePressedAnimationTimer(value);
            }
        }

        #endregion

        #region Fields

        private const string PressedAnimationState = ":pressedAnimation";
        private readonly DispatcherTimer _pressedAnimationStateTimer;

        #endregion

        #region Construction

        public MenuActionItem()
        {
            _pressedAnimationStateTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = PressedAnimationStateDuration,
            };
            _pressedAnimationStateTimer.Tick += OnPressedAnimationStateTimerTick;
        }

        #endregion

        #region Helpers - Pressed Animation

        private void UpdatePressedAnimationTimer(TimeSpan duration)
        {
            if (duration > TimeSpan.Zero)
            {
                if (_pressedAnimationStateTimer != null)
                {
                    _pressedAnimationStateTimer.Stop();
                    _pressedAnimationStateTimer.Interval = duration;
                }
            }
        }

        private void StartPressedAnimationTimer()
        {
            if (_pressedAnimationStateTimer != null)
            {
                _pressedAnimationStateTimer.Start();
            }
        }

        private void StopPressedAnimationTimer()
        {
            if (_pressedAnimationStateTimer != null)
            {
                _pressedAnimationStateTimer.Stop();
            }
        }
        
        private void AddPressedAnimationState()
        {
            RemovePressedAnimationState();
            PseudoClasses.Add(PressedAnimationState);
        }

        private void RemovePressedAnimationState()
        {
            if (PseudoClasses.Contains(PressedAnimationState))
            {
                PseudoClasses.Remove(PressedAnimationState);
            }
        }

        private void OnPressedAnimationStateTimerTick(object sender, EventArgs e)
        {
            StopPressedAnimationTimer();
            RemovePressedAnimationState();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (PressedAnimationStateDuration > TimeSpan.Zero)
            {
                AddPressedAnimationState();
                StartPressedAnimationTimer();
            }

            base.OnPointerPressed(e);
        }

        #endregion
    }
}
