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
using Avalonia.Media;

namespace BitsOfNature.UI.Avalonia.Controls
{
    /// <summary>
    ///     Spinner control that displays rotating dots to indicate a 'busy' state
    /// </summary>
    public class BusyIndicator : Control
    {
        #region Constants

        private static IBrush s_backgroundBrush = new SolidColorBrush(0x00000000);

        #endregion

        #region Phase Property
        /// <summary>
        ///     Defines the <see cref="Phase"/> property.
        /// </summary>
        public static readonly StyledProperty<double> PhaseProperty =
            AvaloniaProperty.Register<BusyIndicator, double>(nameof(Phase), 0.0);

        /// <summary>
        ///     Gets or sets the phase of the spinner in [0..1]. Transitioning this value from 0 to 1 yields the spinning animation.
        /// </summary>
        public double Phase
        {
            get { return GetValue(PhaseProperty); }
            set { SetValue(PhaseProperty, value); }
        }
        #endregion

        #region Brush Property
        /// <summary>
        ///     Defines the <see cref="Foreground"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.Register<BusyIndicator, IBrush>(nameof(Foreground), null);

        /// <summary>
        ///     Gets or sets the foreground
        /// </summary>
        public IBrush Foreground
        {
            get { return GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion

        #region DotCount Property
        /// <summary>
        ///     Defines the <see cref="DotCount"/> property.
        /// </summary>
        public static readonly StyledProperty<int> DotCountProperty =
            AvaloniaProperty.Register<BusyIndicator, int>(nameof(DotCount), 5);

        /// <summary>
        ///     Gets or sets the number of dots to be rendered
        /// </summary>
        public int DotCount
        {
            get { return GetValue(DotCountProperty); }
            set { SetValue(DotCountProperty, value); }
        }
        #endregion

        #region IsActive Property
        /// <summary>
        ///     Defines the <see cref="IsActive"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsActive), false);

        /// <summary>
        ///     Gets or sets whether the spinner is currently active
        /// </summary>
        public bool IsActive
        {
            get { return GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        #endregion

        #region Static Construction
        /// <summary>
        ///     Static constructor
        /// </summary>
        static BusyIndicator()
        {
            AffectsRender<BusyIndicator>(PhaseProperty, ForegroundProperty, DotCountProperty, IsActiveProperty);
        }
        #endregion

        #region Control Overrides
        /// <summary>
        ///     <inheritdoc/>
        /// </summary>
        public override void Render(DrawingContext context)
        {
            if (!IsActive || DotCount == 0) { return; }

            double size = Math.Min(Bounds.Width, Bounds.Height);
            double dotRadius = 0.05 * size;
            double circleCenterX = 0.5 * Bounds.Width;
            double circleCenterY = 0.5 * Bounds.Height;
            double cirlceRedius = 0.5 * size - dotRadius - 1;

            Interval range = new Interval(Phase, Phase + 0.06 * (DotCount - 1));

            // fill with background first to invalidated the control area
            // context.FillRectangle(s_backgroundBrush, Bounds);

            double x;
            double y;
            foreach (double t in range.Discretize(DotCount))
            {
                (x, y) = Utils.GetPointForAngle(GetAngleAt(t), circleCenterX, circleCenterY, cirlceRedius);
                context.DrawEllipse(Foreground, null, new Point(x, y), dotRadius, dotRadius);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///     Gets the angle at which a dot should appear for the specified phase value
        /// </summary>
        /// <param name="phase">
        ///     The phase value
        /// </param>
        /// <returns>
        ///     The angle in [0..2π]
        /// </returns>
        private double GetAngleAt(double phase)
        {
            double t = Utils.Frac(phase);

            // The phase value smoothly varies the speed of the dot, so that v(t) = v0 + (4t - 4t²) * (v1 - v0)
            // with (v0, v1) = (min, max) speed
            double v0 = 1;
            double v1 = 3;

            // integrate v(t) from [0..x]
            double Integrate(double x) => v0 * x + 2 * x * x * (v1 - v0) - (4.0 / 3.0) * x * x * x * (v1 - v0);

            return 2 * Math.PI * (Integrate(t) / Integrate(1)) - 0.5 * Math.PI;
        }
        #endregion
    }

    public static class Utils
    {
        public static double Frac(double val)
        {
            return val - (int)val;
        }

        public static (double, double) GetPointForAngle(double angle, double circleCenterX, double circleCenterY, double circleRadius)
        {
            return (circleCenterX + circleRadius * Math.Cos(angle), circleCenterY + circleRadius * Math.Sin(angle));
        }
    }
}
