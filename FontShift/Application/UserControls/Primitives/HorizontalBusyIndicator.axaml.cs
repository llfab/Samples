// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2023 Stryker
// ===========================================================================

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using FontShift.Application.Infrastructure;
using System;
using System.Numerics;

namespace FontShift.Application.UserControls.Primitives
{
    public partial class HorizontalBusyIndicator : UserControl
    {
        #region Fields

        private readonly Trace _trace;
        private CompositionCustomVisual _customVisual;

        #endregion

        #region Construction

        public HorizontalBusyIndicator()
        {
            _trace = Trace.ForType<HorizontalBusyIndicator>();

            try
            {
                AvaloniaXamlLoader.Load(this);
                AttachCustomVisual(this.FindControl<Border>("animationHost"));
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error\n{1}", Trace.Site(), ex);
            }
        }

        #endregion

        #region Bindable Properties

        public static readonly StyledProperty<double> IndicatorWidthProperty =
            AvaloniaProperty.Register<HorizontalBusyIndicator, double>(nameof(IndicatorWidthProperty), 0);

        public double IndicatorWidth
        {
            get { return GetValue(IndicatorWidthProperty); }
            set { SetValue(IndicatorWidthProperty, value); }
        }

        public static readonly StyledProperty<IBrush> IndicatorBrushProperty =
            AvaloniaProperty.Register<HorizontalBusyIndicator, IBrush>(nameof(IndicatorBrushProperty), new SolidColorBrush());

        public IBrush IndicatorBrush
        {
            get { return GetValue(IndicatorBrushProperty); }
            set { SetValue(IndicatorBrushProperty, value); }
        }

        public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
            AvaloniaProperty.Register<HorizontalBusyIndicator, TimeSpan>(nameof(AnimationDurationProperty), TimeSpan.Zero);

        public TimeSpan AnimationDuration
        {
            get { return GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly StyledProperty<bool> IsBusyProperty =
            AvaloniaProperty.Register<HorizontalBusyIndicator, bool>(nameof(IsBusyProperty), false);

        public bool IsBusy
        {
            get { return GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        #endregion

        #region Private Helpers

        private void AttachCustomVisual(Visual v)
        {
            void UpdateSize()
            {
                if (_customVisual == null)
                {
                    return;
                }

                _customVisual.Size = new Vector2((float)v.Bounds.Width, (float)v.Bounds.Height);
            }

            void UpdateAnimationState()
            {
                if (IsBusy)
                {
                    _customVisual?.SendHandlerMessage(CustomVisualHandler.StartMessage);
                }
                else
                {
                    _customVisual?.SendHandlerMessage(CustomVisualHandler.StopMessage);
                }
            }

            v.AttachedToVisualTree += delegate
            {
                var compositor = ElementComposition.GetElementVisual(v)?.Compositor;
                if (compositor == null || _customVisual?.Compositor == compositor)
                    return;
                _customVisual = compositor.CreateCustomVisual(new CustomVisualHandler(IndicatorWidth, IndicatorBrush, AnimationDuration));
                ElementComposition.SetElementChildVisual(v, _customVisual);
                if (IsBusy)
                {
                    _customVisual.SendHandlerMessage(CustomVisualHandler.StartMessage);
                }
                UpdateSize();
            };

            v.PropertyChanged += (_, a) =>
            {
                if (a.Property == BoundsProperty)
                {
                    UpdateSize();
                }
                else if (a.Property == IsBusyProperty)
                {
                    UpdateAnimationState();
                }
            };
        }

        private class CustomVisualHandler : CompositionCustomVisualHandler
        {
            private double _animationElapsedMsec;
            private double? _lastServerTimeMsec;
            private readonly double _animationDurationMsec;
            private readonly double _indicatorWidth;
            private readonly IImmutableBrush _brush;

            private bool _running;

            public static readonly object StopMessage = new(), StartMessage = new();

            public CustomVisualHandler(double indicatorWidth, IBrush indicatorBrush, TimeSpan animationDuration)
                : base()
            {
                _indicatorWidth = indicatorWidth;
                _animationDurationMsec = animationDuration.TotalMilliseconds;

                if (indicatorBrush is LinearGradientBrush linearGradientBrush)
                {
                    _brush = new ImmutableLinearGradientBrush(linearGradientBrush);
                }
                else if (indicatorBrush is SolidColorBrush solidColorBrush)
                {
                    _brush = new ImmutableSolidColorBrush(solidColorBrush);
                }
                else
                {
                }
            }

            public override void OnRender(ImmediateDrawingContext drawingContext)
            {
                if (!_running) { return; }

                if (_lastServerTimeMsec.HasValue)
                {
                    _animationElapsedMsec += (CompositionNow.TotalMilliseconds - _lastServerTimeMsec.Value);
                    _animationElapsedMsec %= _animationDurationMsec;
                }

                _lastServerTimeMsec = CompositionNow.TotalMilliseconds;

                double borderWidth = EffectiveSize.X;
                double borderHeight = EffectiveSize.Y;
                double animationStage = _animationElapsedMsec / _animationDurationMsec;

                double offsetX = (borderWidth + _indicatorWidth) * animationStage - _indicatorWidth;

                using (drawingContext.PushPreTransform(Matrix.CreateTranslation(offsetX, 0)))
                {
                    drawingContext.DrawRectangle(_brush, null, new Rect(0, 0, _indicatorWidth, borderHeight));
                }
            }

            public override void OnMessage(object message)
            {
                if (message == StartMessage)
                {
                    _running = true;
                    _lastServerTimeMsec = null;
                    RegisterForNextAnimationFrameUpdate();
                }
                else if (message == StopMessage)
                    _running = false;
            }

            public override void OnAnimationFrameUpdate()
            {
                if (_running)
                {
                    Invalidate();
                    RegisterForNextAnimationFrameUpdate();
                }
            }
        }
        #endregion
    }
}
