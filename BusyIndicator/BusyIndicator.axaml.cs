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
using BitsOfNature.Core.Geometry2D;
using BitsOfNature.Core.Mathematics;

namespace BitsOfNature.UI.Avalonia.Controls
{
    /// <summary>
    ///     Spinner control that displays rotating dots to indicate a 'busy' state
    /// </summary>
    public class BusyIndicator : Control
    {
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
        ///     Defines the <see cref="Brush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BrushProperty =
            AvaloniaProperty.Register<BusyIndicator, IBrush>(nameof(Brush), null);

        /// <summary>
        ///     Gets or sets the foreground brush
        /// </summary>
        public IBrush Brush
        {
            get { return GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
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
            AffectsRender<BusyIndicator>(PhaseProperty, BrushProperty, DotCountProperty, IsActiveProperty);
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
            Circle2D circle = new Circle2D(0.5 * Bounds.Width, 0.5 * Bounds.Height, 0.5 * size - dotRadius - 1);

            Interval range = new Interval(Phase, Phase + 0.06 * (DotCount - 1));

            foreach (double t in range.Discretize(DotCount))
            {
                Point2D p = circle.GetPointForAngle(GetAngleAt(t));
                context.DrawEllipse(Brush, null, new Point(p.X, p.Y), dotRadius, dotRadius);
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
            double t = MathUtils.Frac(phase);

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
}
