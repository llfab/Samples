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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animations
{
    public interface ITransitioningContentControlled
    {
        /// <summary>
        ///     Called when the transitioning in animation is started
        /// </summary>
        void TransitioningInStarted();

        /// <summary>
        ///     Called when the transitioning in animation is over
        /// </summary>
        void TransitioningInFinished();

        /// <summary>
        ///     Called when the transitioning out animation is started
        /// </summary>
        void TransitioningOutStarted();

        /// <summary>
        ///     Called when the transitioning out animation is over
        /// </summary>
        void TransitioningOutFinished();
    }
}
