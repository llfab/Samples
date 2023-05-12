// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;

namespace Animations
{
    /// <summary>
    ///     A cross fade animation the respects the <see cref="TransitioningContentControl.CurrentContent"/>
    ///     being non-null or null. In case of null, it skips the fade animation so null-content
    ///     will not be faded-out.
    ///     
    ///     In case of null-content we still run animations to be aligned with Opacity non-persistence
    ///     but we use a minimal <see cref="Animation.Duration"/> that will take maximum 1 render frame
    /// </summary>
    public class TransitioningContentControlCrossFade : IPageTransition
    {
        /// <summary>
        ///     The name of this animation to distinguish while debugging.
        /// </summary>
        private string _name;

        /// <summary>
        ///     The animation for normal fade out
        /// </summary>
        private readonly Animation _fadeOutAnimation;

        /// <summary>
        ///     The animation for normal fade in
        /// </summary>
        private readonly Animation _fadeInAnimation;

        /// <summary>
        ///     The animation for empty content fade out with
        ///     a minimal duration
        /// </summary>
        private readonly Animation _fadeOutNonAnimated;

        /// <summary>
        ///     The animation for empty content fade in with
        ///     a minimal duration
        /// </summary>
        private readonly Animation _fadeInNonAnimated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossFade"/> class.
        /// </summary>
        public TransitioningContentControlCrossFade()
            : this(TimeSpan.Zero)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossFade"/> class.
        /// </summary>
        /// <param name="duration">The duration of the animation.</param>
        public TransitioningContentControlCrossFade(TimeSpan duration)
        {
            _name = string.Empty;

            _fadeOutAnimation = new Animation
            {
                Children =
                {
                    new KeyFrame()
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Visual.OpacityProperty,
                                Value = 0d
                            }
                        },
                        Cue = new Cue(1d)
                    }

                }
            };
            _fadeOutNonAnimated = new Animation
            {
                Children =
                {
                    new KeyFrame()
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Visual.OpacityProperty,
                                Value = 0d
                            }
                        },
                        Cue = new Cue(1d)
                    }

                }
            };
            _fadeInAnimation = new Animation
            {
                Children =
                {
                    new KeyFrame()
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Visual.OpacityProperty,
                                Value = 1d
                            }
                        },
                        Cue = new Cue(1d)
                    }

                }
            };
            _fadeInNonAnimated = new Animation
            {
                Children =
                {
                    new KeyFrame()
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Visual.OpacityProperty,
                                Value = 1d
                            }
                        },
                        Cue = new Cue(1d)
                    }

                }
            };
            _fadeOutAnimation.Duration = _fadeInAnimation.Duration = duration;

            // duration must not be TimeSpan.Zero but shall use a minimum value
            _fadeOutNonAnimated.Duration = _fadeInNonAnimated.Duration = TimeSpan.FromMilliseconds(1);
        }

        private string InstanceId(TraceLevel _)
        {
            return _name;
        }

        /// <summary>
        ///     The name of this animation to distinguish while debugging.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        ///     Indicates whether tracing is enabled.
        /// </summary>
        public bool IsTracingEnabled
        {
            get => _isTracingEnabled;
            set => _isTracingEnabled = value;
        }

        /// <summary>
        ///     Gets the duration of the animation.
        /// </summary>
        public TimeSpan Duration
        {
            get => _fadeOutAnimation.Duration;
            set => _fadeOutAnimation.Duration = _fadeInAnimation.Duration = value;
        }

        /// <summary>
        ///     Gets or sets element entrance easing.
        /// </summary>
        public Easing FadeInEasing
        {
            get => _fadeInAnimation.Easing;
            set => _fadeInAnimation.Easing = value;
        }

        /// <summary>
        ///     Gets or sets element exit easing.
        /// </summary>
        public Easing FadeOutEasing
        {
            get => _fadeOutAnimation.Easing;
            set => _fadeOutAnimation.Easing = value;
        }

        /// <inheritdoc cref="Start(Visual, Visual, CancellationToken)" />
        public async Task Start(Visual from, Visual to, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            List<Task> tasks = new List<Task>();
            IDisposable obj = to != null ? to.SetValue(Visual.OpacityProperty, 0, BindingPriority.Animation)! : null;

            try
            {
                if (from != null)
                {
                    NotifyTransitioningOutStarted(from);

                    if (IsTransitioningContentControlWithNullContent(from))
                    {
                        // for null content we still need to run an animation but with zero duration
                        tasks.Add(_fadeOutNonAnimated.RunAsync(from, null, cancellationToken));
                    }
                    else
                    {
                        // for non-null content we just run the normal animation
                        tasks.Add(_fadeOutAnimation.RunAsync(from, null, cancellationToken));
                    }
                }

                if (to != null)
                {
                    NotifyTransitioningInStarted(to);

                    to.IsVisible = true;
                    if (IsTransitioningContentControlWithNullContent(to))
                    {
                        // for null content we still need to run an animation but with zero duration
                        tasks.Add(_fadeInNonAnimated.RunAsync(to, null, cancellationToken));
                    }
                    else
                    {
                        // for non-null content we just run the normal animation
                        tasks.Add(_fadeInAnimation.RunAsync(to, null, cancellationToken));
                    }
                }

                await Task.WhenAll(tasks);

                if (from != null) { NotifyTransitioningOutFinished(from); }
                if (to != null) { NotifyTransitioningInFinished(to); }

                if (from != null && !cancellationToken.IsCancellationRequested)
                {
                    from.IsVisible = false;
                }
            }
            finally
            {
                obj?.Dispose();
            }
        }

        /// <summary>
        ///     Starts the animation.
        /// </summary>
        /// <param name="from">
        ///     The control that is being transitioned away from. May be null.
        /// </param>
        /// <param name="to">
        ///     The control that is being transitioned to. May be null.
        /// </param>
        /// <param name="forward">
        ///     Unused for cross-fades.
        /// </param>
        /// <param name="cancellationToken">allowed cancel transition</param>
        /// <returns>
        ///     A <see cref="Task"/> that tracks the progress of the animation.
        /// </returns>
        Task IPageTransition.Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
        {
            return Start(from, to, cancellationToken);
        }

        /// <summary>
        ///     Helper function checking if the <see cref="Visual"/> is a
        ///     <see cref="TransitioningContentControl"/> with CurrentContent set
        ///     to null.
        /// </summary>
        private bool IsTransitioningContentControlWithNullContent(Visual visual)
        {
            if (visual is TransitioningContentControl transitioningContentControl)
            {
                if (transitioningContentControl.CurrentContent == null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Notifies transition started if the content is an <see cref="ITransitioningContentControlled"/> 
        /// </summary>
        private void NotifyTransitioningInStarted(Visual visual)
        {
            ITransitioningContentControlled controlled = TryGetTransitioningContentControlled(visual);
            if (controlled != null)
            {
                controlled.TransitioningInStarted();
            }
        }

        /// <summary>
        ///     Notifies transition finished if the content is an <see cref="ITransitioningContentControlled"/> 
        /// </summary>
        private void NotifyTransitioningInFinished(Visual visual)
        {
            ITransitioningContentControlled controlled = TryGetTransitioningContentControlled(visual);
            if (controlled != null)
            {
                controlled.TransitioningInFinished();
            }
        }

        /// <summary>
        ///     Notifies transition started if the content is an <see cref="ITransitioningContentControlled"/> 
        /// </summary>
        private void NotifyTransitioningOutStarted(Visual visual)
        {
            ITransitioningContentControlled controlled = TryGetTransitioningContentControlled(visual);
            if (controlled != null)
            {
                controlled.TransitioningOutStarted();
            }
        }

        /// <summary>
        ///     Notifies transition finished if the content is an <see cref="ITransitioningContentControlled"/> 
        /// </summary>
        private void NotifyTransitioningOutFinished(Visual visual)
        {
            ITransitioningContentControlled controlled = TryGetTransitioningContentControlled(visual);
            if (controlled != null)
            {
                controlled.TransitioningOutFinished();
            }
        }

        /// <summary>
        ///     Returns the <paramref name="visual"/> as <see cref="ITransitioningContentControlled"/> if possible
        ///     Otherwise: null
        /// </summary>
        private ITransitioningContentControlled TryGetTransitioningContentControlled(Visual visual)
        {
            if (visual is TransitioningContentControl transitioningContentControl)
            {
                if (transitioningContentControl.CurrentContent != null)
                {
                    return transitioningContentControl.CurrentContent as ITransitioningContentControlled;
                }
            }

            return null;
        }

        /// <summary>
        ///     Dumps the type of the visual
        /// </summary>
        private string Dump(Visual visual)
        {
            if (visual is TransitioningContentControl transitioningContentControl)
            {
                if (transitioningContentControl.CurrentContent == null)
                {
                    return "TCC<null>";
                }
                return transitioningContentControl.CurrentContent.ToString();
            }
            return visual == null ? "<null>" : visual.ToString();
        }
    }
}
