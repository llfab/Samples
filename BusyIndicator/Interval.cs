// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Collections;
using System.Collections.Generic;

namespace BitsOfNature.UI.Avalonia.Controls
{
    /// <summary>
    ///     A <see cref="Interval"/> with its minimal and maximal values defines an interval.
    /// </summary>
    public partial class Interval
    {
        #region Private Instance Fields
        /// <summary>
        ///     The minimal value as the lower bound of this <see cref="Interval"/>.
        /// </summary>
        private double _min;

        /// <summary>
        ///     The maximal value as the upper bound of this <see cref="Interval"/>.
        /// </summary>
        private double _max;
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets/Sets the minimal value as the lower bound of this <see cref="Interval"/>
        /// </summary>
        public double Min
        {
            get => _min;
            set => _min = value;
        }


        /// <summary>
        ///     Gets/Sets the maximal value as the upper bound of this <see cref="Interval"/>
        /// </summary>
        public double Max
        {
            get => _max;
            set => _max = value;
        }

        /// <summary>
        ///     Center of the range
        /// </summary>
        public double Center => 0.5 * (_min + _max);

        /// <summary>
        ///     Gets the extent of this <see cref="Interval"/>.
        /// </summary>
        /// <remarks>
        ///     <see cref="Interval"/>s with a negative range
        ///     are defined to be emtpy.
        /// </remarks>
        public double Extent => _max - _min;

        /// <summary>
        ///        Get a range from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        public static Interval Unit => new Interval(0, 1);

        /// <summary>
        ///     Get a range from <see cref="double.NegativeInfinity"/> to <see cref="double.PositiveInfinity"/>
        /// </summary>
        public static Interval WholeR => new Interval(double.NegativeInfinity, double.PositiveInfinity);

        /// <summary>
        ///     Gets whether the interval is bounded (i.e. neither bound is infinite)
        /// </summary>
        public bool IsBounded => !(double.IsInfinity(_min) || double.IsInfinity(_max));

        /// <summary>
        ///     Tests whether the current <see cref="Interval"/> is empty.
        /// </summary>
        public bool IsEmpty => _min > _max;

        /// <summary>
        ///     Gets the 2-element array containing the interval bounds { min, max }
        /// </summary>
        public double[] Bounds => new[] { _min, _max };
        #endregion

        #region Construction
        /// <summary>
        ///     Constructs a <see cref="Interval"/> [min, max].
        /// </summary>
        /// <param name="min">
        ///     The minimal value as the lower bound of this <see cref="Interval"/>.
        /// </param>
        /// <param name="max">
        ///     The maximal value as the lower bound of this <see cref="Interval"/>.
        /// </param>
        public Interval(double min, double max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        ///     Constructs a <see cref="Interval"/> [v;v] with <see cref="Extent"/> 0
        ///     as its minimal and maximal value are set to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to be set as minimal and maximal value 
        /// </param>
        public Interval(double value) : this(value, value) { }

        /// <summary>
        ///     Creates an empty <see cref="Interval"/>.
        /// </summary>
        /// <returns>
        ///     Returns the newly created empty <see cref="Interval"/>.
        /// </returns>
        public static Interval CreateEmpty() => new Interval(double.MaxValue, double.MinValue);

        /// <summary>
        ///     Creates an unconstrained <see cref="Interval"/>.
        /// </summary>
        /// <returns>
        ///     Returns the newly created unconstrained <see cref="Interval"/>.
        /// </returns>
        public static Interval CreateUnconstrained() => new Interval(double.MinValue, double.MaxValue);

        /// <summary>
        ///     Creates a range from a data source
        /// </summary>
        /// <param name="data">
        ///     An <see cref="IEnumerable"/> iterating over <see cref="double"/> values
        /// </param>
        /// <returns>
        ///     The range containing all elements of <paramref name="data"/>
        /// </returns>
        public static Interval CreateFromData(IEnumerable<double> data)
        {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            foreach (double d in data)
            {
                if (d < min) { min = d; }
                if (d > max) { max = d; }
            }

            return new Interval(min, max);
        }

        /// <summary>
        ///     Creates a range given its center and half extent (aka radius)
        /// </summary>
        /// <param name="center">
        ///     The center of the range
        /// </param>
        /// <param name="halfExtent">
        ///     The half extent (aka radius) of the range, must be a non-negative number
        /// </param>
        /// <returns>
        ///     The range
        /// </returns>
        public static Interval FromCenter(double center, double halfExtent)
        {
            return new Interval(center - halfExtent, center + halfExtent);
        }
        #endregion

        #region Public Members
        /// <summary>
        ///     Clones this <see cref="Interval"/>.
        /// </summary>
        /// <returns>
        ///     Returns the cloned copy of this <see cref="Interval"/>.
        /// </returns>
        public Interval Clone() => new Interval(_min, _max);

        /// <summary>
        ///     Tests whether a given value <paramref name="v"/> is in the
        ///     current range.
        /// </summary>
        /// <param name="v">
        ///     A value to test for.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c> if <paramref name="v"/> is in the
        ///     current range, <c>false</c> otherwise.
        /// </returns>
        public bool Contains(double v) => v >= _min && v <= _max;

        /// <summary>
        ///     Checks whether this range contains the range <paramref name="r"/>
        /// </summary>
        /// <param name="r">
        ///     A range
        /// </param>
        /// <returns>
        ///     <c>true</c>, if this range contains <paramref name="r"/>, <c>false</c> otherwise
        /// </returns>
        public bool Contains(Interval r) => r.Min >= _min && r.Max <= _max;

        /// <summary>
        ///     Clamps a given <paramref name="v"/> to this <see cref="Interval"/>.
        /// </summary>
        /// <param name="v">
        ///     A <see cref="double"/> value to be clamped.    
        /// </param>
        /// <returns>
        ///     Returns the clamped value.
        /// </returns>
        public double Clamp(double v) => Clamp(v, _min, _max);

        /// <summary>
        ///     Test, if this <see cref="Interval"/> intersects a given <paramref name="other"/> range.
        /// </summary>
        /// <param name="other">
        ///     The other <see cref="Interval"/> to be tested for intersection.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c>, if this <see cref="Interval"/> intersects the 
        ///     given <paramref name="other"/> <see cref="Interval"/>.
        /// </returns>
        public bool Intersects(Interval other) => !(other.Min > _max || other.Max < _min);

        /// <summary>
        ///     <see cref="object.ToString()"/>
        /// </summary>
        public override string ToString() => $"[{_min}; {_max}])";

        /// <summary>
        ///     Grows and modifies this range to include a given value <paramref name="v"/>.
        /// </summary>
        /// <param name="v">
        ///     The value to be inserted.
        /// </param>
        public void Grow(double v)
        {
            if (!IsEmpty)
            {
                _min = Math.Min(v, _min);
                _max = Math.Max(v, _max);
            }
            else { _min = _max = v; }
        }

        /// <summary>
        ///        Calculates the relative position of <paramref name="d"/> inside this range, 
        ///        so that 'd = min + result * extent'.
        /// </summary>
        /// <remarks>
        ///        If <paramref name="d"/> lies inside this range, the resulting value lies in [0..1] 
        ///        (0 = min, 1 = max).
        /// </remarks>
        /// <param name="d">
        ///        The data sample
        /// </param>
        /// <param name="clip">
        ///     If <c>true</c>, the position will be clipped to the interval [0..1]
        /// </param>
        /// <returns>
        ///        The relative position of <paramref name="d"/> inside this range
        /// </returns>
        public double GetRelativePosition(double d, bool clip = false)
        {
            double extent = _max - _min;

            if (extent == 0 && d == _min) { return 0; }
            double result = (d - _min) / extent;

            if (clip)
            {
                if (result < 0) { result = 0; }
                else if (result > 1) { result = 1; }
            }

            return result;
        }

        /// <summary>
        ///     Returns the range of <paramref name="absolute"/>, relative to this interval, e.g. if 
        ///     <paramref name="absolute"/> covers the first half of this interval, the result would be (0, 0.5)
        /// </summary>
        /// <param name="absolute">
        ///     The interval
        /// </param>
        /// <param name="clip">
        ///     <c>true</c>, if the specified interval should be clipped at <c>this</c> (i.e. the result will
        ///     be a subinterval of (0, 1)), <c>false</c> otherwise
        /// </param>
        /// <returns>
        ///     The relative interval
        /// </returns>
        public Interval GetRelative(Interval absolute, bool clip = false)
        {
            double rmin = GetRelativePosition(absolute.Min, clip);
            double rmax = GetRelativePosition(absolute.Max, clip);
            return new Interval(rmin, rmax);
        }

        /// <summary>
        ///     Calculates the absolute position of <paramref name="d"/> inside this range, so that
        ///     'result = min + d * extent'
        /// </summary>
        /// <remarks>
        ///     For <paramref name="d"/> in [0..1], the resulting position lies inside this range,
        ///     otherwise it lies outside
        /// </remarks>
        /// <param name="d">
        ///     Relative point inside this range
        /// </param>
        /// <returns>
        ///     The absolute position of <paramref name="d"/> inside this range
        /// </returns>
        public double GetAbsolutePosition(double d) => (1 - d) * _min + d * _max;

        /// <summary>
        ///     Returns the absolute range of <paramref name="relative"/>, e.g. if <paramref name="relative"/> 
        ///     is the interval (0, 0.5), the result would be the first half of <c><this/c> interval
        /// </summary>
        /// <param name="relative">
        ///     The interval
        /// </param>
        /// <param name="clip">
        ///     <c>true</c>, if the resulting interval should be clipped at <c>this</c> (i.e. the result will
        ///     be a subinterval of this interval), <c>false</c> otherwise
        /// </param>
        /// <returns>
        ///     The absolute interval
        /// </returns>
        public Interval GetAbsolute(Interval relative, bool clip = false)
        {
            if (clip) { relative = Interval.Intersect(this, relative); }
            double amin = GetAbsolutePosition(relative.Min);
            double amax = GetAbsolutePosition(relative.Max);
            return new Interval(amin, amax);
        }


        /// <summary>
        ///     Maps a value <paramref name="d"/> from this range to another <paramref name="targetRange"/>.
        /// </summary>
        /// <remarks>
        ///     For the result holds
        ///     (d  - this.Min) / this.Extent = (result - destRange.Min) / destRange.Extent;
        /// </remarks>
        /// <param name="d">
        ///     A value in the source range to be mapped within the target range.
        /// </param>
        /// <param name="targetRange">
        ///     The target range the value <paramref name="d"/> should be mapped to.
        /// </param>
        /// <returns>
        ///     Returns the mapped value <paramref name="d"/>.
        /// </returns>
        public double MapToRange(double d, Interval targetRange) => targetRange.GetAbsolutePosition(GetRelativePosition(d));

        /// <summary>
        ///     Grows and modifies this <see cref="Interval"/>
        ///     in a way to include the given <paramref name="other"/> <see cref="Interval"/>.
        /// </summary>
        /// <param name="other">
        ///     A <see cref="Interval"/> to be included.
        /// </param>
        public void Grow(Interval other)
        {
            if (other.IsEmpty) { return; }

            if (!IsEmpty)
            {
                _min = Math.Min(_min, other._min);
                _max = Math.Max(_max, other._max);
            }
            else
            {
                _min = other._min;
                _max = other._max;
            }
        }

        /// <summary>
        ///     Returns an inflated copy of this range (see <see cref="Inflate"/> for details)
        /// </summary>
        /// <param name="extent">
        ///     The extent by which the range should be inflated
        /// </param>
        /// <returns>
        ///     The inflated range
        /// </returns>
        public Interval GetInflated(double extent) => new Interval(_min - extent, _max + extent);

        /// <summary>
        ///     Returns the range (min + delta, max + delta)
        /// </summary>
        /// <param name="delta">
        ///     The translation delta
        /// </param>
        /// <returns>
        ///     The translated range
        /// </returns>
        public Interval GetTranslated(double delta)
        {
            return new Interval(_min + delta, _max + delta);
        }

        /// <summary>
        ///     Yields a discretization of the range in <paramref name="steps"/> steps
        /// </summary>
        /// <param name="steps">
        ///     Number of discretization steps
        /// </param>
        /// <returns>
        ///     The discrete values inside the range
        /// </returns>
        public IEnumerable<double> Discretize(int steps)
        {
            if (steps == 1) { yield return Center; yield break; }

            for (int i = 0; i < steps; i++)
            {
                yield return _min + i / (double)(steps - 1) * (_max - _min);
            }
        }

        /// <summary>
        ///     Yields a discretization of the range by the given step size. The values returned are of the form
        ///     'min + i * size' with 'i' in [0, 1, 2 ... max / size]
        /// </summary>
        /// <param name="size">
        ///     The step size
        /// </param>
        /// <returns>
        ///     The enumeration
        /// </returns>
        public IEnumerable<double> DiscretizeByStepSize(double size)
        {
            int count = (int)((_max - _min) / size);
            for (int i = 0; i <= count; i++) { yield return _min + i * size; }
        }

        /// <summary>
        ///     Returns a random sample from this range, drawn from a uniform distribution
        /// </summary>
        /// <param name="random">
        ///     The random number generator
        /// </param>
        /// <returns>
        ///     The sample
        /// </returns>
        public double Sample(Random random)
        {
            return GetAbsolutePosition(random.NextDouble());
        }
        #endregion

        #region Public Static Members
        /// <summary>
        ///        Tests whether the range (<paramref name="min"/>, <paramref name="max"/>) 
        ///        contains <paramref name="v"/>
        /// </summary>
        /// <param name="v">
        ///        A value to be tested.
        /// </param>
        /// <param name="min">
        ///     The minimal value as the range's lower bound.
        /// </param>
        /// <param name="max">
        ///     The maximal value as the range's upper bound.
        /// </param>
        /// <returns>
        ///        <c>true</c>, if <paramref name="v"/> lies inside the range, <c>false</c> otherwise
        /// </returns>
        public static bool Contains(double v, double min, double max) => (v >= min && v <= max);

        /// <summary>
        ///        Clamps a given <paramref name="v"/> to a given range specified by
        ///        <paramref name="min"/> and <paramref name="max"/> as the respective range bounds.
        /// </summary>
        /// <param name="v">
        ///     A <see cref="double"/> value to be clamped.    
        /// </param>
        /// <param name="min">
        ///     The minimal value as the range's lower bound.
        /// </param>
        /// <param name="max">
        ///     The maximal value as the range's lower bound.
        /// </param>
        /// <returns>
        ///     Returns the clamped value.
        /// </returns>
        public static double Clamp(double v, double min, double max) => Math.Max(min, Math.Min(v, max));

        /// <summary>
        ///     Clamps a given <paramref name="v"/> to a given range specified by
        ///     <paramref name="min"/> and <paramref name="max"/> as the respective range bounds.
        /// </summary>
        /// <param name="v">
        ///     A <see cref="float"/> value to be clamped.    
        /// </param>
        /// <param name="min">
        ///     The minimal value as the range's lower bound.
        /// </param>
        /// <param name="max">
        ///     The maximal value as the range's lower bound.
        /// </param>
        /// <returns>
        ///     Returns the clamped value.
        /// </returns>
        public static float Clamp(float v, float min, float max) => Math.Max(min, Math.Min(v, max));

        /// <summary>
        ///     Clamps a given <paramref name="v"/> to a given range specified by
        ///     <paramref name="min"/> and <paramref name="max"/> as the respective range bounds.
        /// </summary>
        /// <param name="v">
        ///     A <see cref="int"/> value to be clamped.    
        /// </param>
        /// <param name="min">
        ///     The minimal value as the range's lower bound.
        /// </param>
        /// <param name="max">
        ///     The maximal value as the range's lower bound.
        /// </param>
        /// <returns>
        ///     Returns the clamped value.
        /// </returns>
        public static int Clamp(int v, int min, int max) => Math.Max(min, Math.Min(v, max));

        /// <summary>
        ///     Calculates the intersection of two given <see cref="Interval"/>s, 
        ///     namely <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">
        ///     The first <see cref="Interval"/>.
        /// </param>
        /// <param name="b">
        ///     The second <see cref="Interval"/>.
        /// </param>
        /// <returns>
        ///     Returns the intersection of the given ranges.
        /// </returns>
        public static Interval Intersect(Interval a, Interval b) => new Interval(Math.Max(a.Min, b.Min), Math.Min(a.Max, b._max));

        /// <summary>
        ///     Gets the intersection of the input ranges
        /// </summary>
        /// <param name="ranges">
        ///     The ranges
        /// </param>
        /// <returns>
        ///     The intersection range, or <c>null</c> if the result is empty
        /// </returns>
        public static Interval Intersect(IEnumerable<Interval> ranges)
        {
            double min = double.NegativeInfinity;
            double max = double.PositiveInfinity;

            foreach (Interval r in ranges)
            {
                min = Math.Max(min, r.Min);
                max = Math.Min(max, r.Max);
            }

            return (min <= max) ? new Interval(min, max) : null;
        }

        /// <summary>
        ///     Calculates the union of two given <see cref="Interval"/>s, 
        ///     namely <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">
        ///     The first <see cref="Interval"/>.
        /// </param>
        /// <param name="b">
        ///     The second <see cref="Interval"/>.
        /// </param>
        /// <returns>
        ///     Returns the union of the given ranges.
        /// </returns>
        public static Interval Union(Interval a, Interval b)
        {
            Interval result = a.Clone();
            result.Grow(b);
            return result;
        }
        #endregion

        #region Equality Stuff
        /// <summary>
        ///     Equality operator
        /// </summary>
        public static bool operator ==(Interval r0, Interval r1)
        {
            bool null0 = object.ReferenceEquals(r0, null);
            bool null1 = object.ReferenceEquals(r1, null);
            return (null0 || null1) ? (null0 && null1) : (r0._min == r1._min && r0._max == r1._max);
        }

        /// <summary>
        ///     Inequality operator
        /// </summary>
        public static bool operator !=(Interval r0, Interval r1) => !(r0 == r1);

        /// <summary>
        ///     <see cref="object.Equals(object)"/>
        /// </summary>
        public override bool Equals(object obj) => (this == (obj as Interval));

        /// <summary>
        ///     <see cref="object.GetHashCode"/>
        /// </summary>
        public override int GetHashCode() => _min.GetHashCode() ^ _max.GetHashCode();
        #endregion

    }
}
