// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2023 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace AvaloniaAnimationTest
{
    public partial class AnimationControl : UserControl
    {
        public AnimationControl()
        {
            InitializeComponent();
            //AttachAnimatedSolidVisual(this.FindControl<Control>("animatedBorder")!);
            AttachCustomVisual(this.FindControl<Control>("animatedBorderHost")!);
        }

        private CompositionSolidColorVisual _solidVisual;

        private void AttachAnimatedSolidVisual(Visual v)
        {
            void Update()
            {
                if (_solidVisual == null)
                    return;
                _solidVisual.Size = new Vector2((float)v.Bounds.Width, (float)v.Bounds.Height);
            }
            v.AttachedToVisualTree += delegate
            {
                var compositor = ElementComposition.GetElementVisual(v)?.Compositor;
                if (compositor == null || _solidVisual?.Compositor == compositor)
                    return;
                _solidVisual = compositor.CreateSolidColorVisual();
                ElementComposition.SetElementChildVisual(v, _solidVisual);
                _solidVisual.Color = Colors.Green;

                var scale = _solidVisual.Compositor.CreateVector3KeyFrameAnimation();
                scale.Duration = TimeSpan.FromSeconds(5);
                scale.IterationBehavior = AnimationIterationBehavior.Forever;
                scale.InsertKeyFrame(0, new Vector3(1, 0, 0));
                scale.InsertKeyFrame(1, new Vector3(1800, 0, 0));

                _solidVisual.StartAnimation("Offset", scale);

                Update();
            };
            v.PropertyChanged += (_, a) =>
            {
                if (a.Property == BoundsProperty)
                    Update();
            };
        }

        private CompositionCustomVisual _customVisual;

        private void AttachCustomVisual(Visual v)
        {
            void Update()
            {
                if (_customVisual == null)
                    return;
                _customVisual.Size = new Vector2((float)v.Bounds.Width, (float)v.Bounds.Height);
            }
            v.AttachedToVisualTree += delegate
            {
                var compositor = ElementComposition.GetElementVisual(v)?.Compositor;
                if (compositor == null || _customVisual?.Compositor == compositor)
                    return;
                _customVisual = compositor.CreateCustomVisual(new CustomVisualHandler());
                ElementComposition.SetElementChildVisual(v, _customVisual);
                _customVisual.SendHandlerMessage(CustomVisualHandler.StartMessage);
                Update();
            };

            v.PropertyChanged += (_, a) =>
            {
                if (a.Property == BoundsProperty)
                    Update();
            };
        }

        private class CustomVisualHandler : CompositionCustomVisualHandler
        {
            private double _animationElapsed;
            private double? _lastServerTime;
            private readonly double _animationLength = 1000;
            private bool _running;
            private readonly IImmutableBrush _brush;

            public static readonly object StopMessage = new(), StartMessage = new();

            public CustomVisualHandler()
                :base()
            {
                GradientStops gradientStops = new GradientStops();
                gradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 0, 0), 0));
                gradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 0, 0), 0.5));
                gradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 0, 0), 1));
                LinearGradientBrush gradientBrush = new LinearGradientBrush()
                {
                    GradientStops = gradientStops,
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative)
                };

                _brush = new ImmutableLinearGradientBrush(gradientBrush);

            }

            public override void OnRender(ImmediateDrawingContext drawingContext)
            {
                if (_running)
                {
                    if (_lastServerTime.HasValue)
                    {
                        _animationElapsed += (CompositionNow.TotalMilliseconds - _lastServerTime.Value);
                        _animationElapsed %= _animationLength;
                    }
                    
                    _lastServerTime = CompositionNow.TotalMilliseconds;
                }

                double borderWidth = EffectiveSize.X;
                double borderHeight = EffectiveSize.Y;
                double indicatorWidth = 256;
                double animationStage = _animationElapsed / _animationLength;

                double offsetX = (borderWidth + indicatorWidth) * animationStage - indicatorWidth;

                // Gradient brush only works using push transform
                // if used without push transform and Rect with
                // offset instead, the rendering is wrong. The gradient seems
                // to be stuck at the original position.
                using (drawingContext.PushPreTransform(Matrix.CreateTranslation(offsetX, 0)))
                {
                    drawingContext.DrawRectangle(_brush, null, new Rect(0, 0, indicatorWidth, borderHeight));
                }

                // uncomment below to get the wrong rendering
                //drawingContext.DrawRectangle(_brush, null, new Rect(offsetX, 0, indicatorWidth, borderHeight));
            }

            public override void OnMessage(object message)
            {
                if (message == StartMessage)
                {
                    _running = true;
                    _lastServerTime = null;
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


    }
}
