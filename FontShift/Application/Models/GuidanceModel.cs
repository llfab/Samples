// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2023 Stryker
// ===========================================================================

using FontShift.Application.Infrastructure;
using System;

namespace FontShift.Application.Models
{
    public enum GuidanceWorkflowStateType
    {
        None,
        Templating,
        PlatePreview,
        PlatePositioning,
        ArticularTargeting,
        ShaftTargeting
    }

    public class GuidanceModel
    {
        #region Events

        public delegate void GuidanceWorkflowStateChangedHandler(GuidanceWorkflowStateType previousWorkflowState, GuidanceWorkflowStateType currentWorkflowState);
        public event GuidanceWorkflowStateChangedHandler GuidanceWorkflowStateChanged;

        #endregion

        #region Fields

        private readonly Trace _trace;

        private GuidanceWorkflowStateType _currentWorkflowState;
        public GuidanceWorkflowStateType CurrentWorkflowState
        {
            get => _currentWorkflowState;
            private set
            {
                if (_currentWorkflowState != value)
                {
                    GuidanceWorkflowStateType previous = _currentWorkflowState;
                    _currentWorkflowState = value;
                    _trace.Info("{0} Previous[{1}] Current[{2}]", Trace.Site(), previous, _currentWorkflowState);
                    GuidanceWorkflowStateChanged?.Invoke(previous, _currentWorkflowState);
                }
            }
        }

        #endregion

        #region Construction

        public GuidanceModel()
        {
            _trace = Trace.ForType<GuidanceModel>();

            _currentWorkflowState = GuidanceWorkflowStateType.PlatePositioning;

            _trace.Info("{0} successful.", Trace.Site());
        }

        #endregion

        #region Init / Exit

        public void Init()
        {
            try
            {
                // processing model reference
                PresentationModel presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();

                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        public void Exit()
        {
            try
            {
                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        #endregion

        #region Public Access - Workflow State

        public void SetWorkflowStateNone()
        {
            CurrentWorkflowState = GuidanceWorkflowStateType.None;
        }

        public void SetWorkflowStateTemplating()
        {
            CurrentWorkflowState = GuidanceWorkflowStateType.Templating;
        }

        public void SetWorkflowStatePlatePreview()
        {
            CurrentWorkflowState = GuidanceWorkflowStateType.PlatePreview;
        }

        public void SetWorkflowStatePlatePositioning()
        {
            CurrentWorkflowState = GuidanceWorkflowStateType.PlatePositioning;
        }

        public void SetWorkflowStateArticularTargeting()
        {
            CurrentWorkflowState = GuidanceWorkflowStateType.ArticularTargeting;
        }

        public void SetWorkflowStateShaftTargeting()
        {
            CurrentWorkflowState = GuidanceWorkflowStateType.ShaftTargeting;
        }

        #endregion
    }
}
