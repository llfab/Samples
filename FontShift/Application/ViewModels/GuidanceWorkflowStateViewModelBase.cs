// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia.Controls;
using FontShift.Application.Infrastructure;
using FontShift.Application.Models;
using FontShift.Application.Mvvm;

namespace FontShift.Application.ViewModels
{
    public abstract class GuidanceWorkflowStateViewModelBase : ViewModel
    {
        #region Fields

        protected readonly Trace _trace;
        private readonly GuidanceWorkflowStateType _workflowState;

        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        #endregion

        #region Bindable Fields

        private bool _isSuspended;
        public bool IsSuspended
        {
            get => _isSuspended;
            set
            {
                if (value !=  _isSuspended)
                {
                    _trace.Info("{0} PreviousIsSuspended[{1}] => CurrentIsSuspended[{2}]", Trace.Site(), _isSuspended, value);
                }
                RaiseAndSetIfChanged(ref _isSuspended, value);
            }
        }

        #endregion

        #region Construction

        protected GuidanceWorkflowStateViewModelBase(GuidanceWorkflowStateType workflowState)
        {
            _trace = new Trace(this.GetType().Name);
            _workflowState = workflowState;
            _isSuspended = false;

            if (Design.IsDesignMode)
            {
                #region Design Mode

                // nothing to do

                #endregion
            }
            else
            {
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();
                _guidanceModel = _presentationModel.Guidance;

                // connect events
                _guidanceModel.GuidanceWorkflowStateChanged += (previous, current) => ActionDispatcher.Execute(() => OnGuidanceModelGuidanceWorkflowStateChanged(previous, current));

                UpdateSuspendedState(false);
            }

            _trace.Info("{0} Base created. IsSuspended[{1}]", Trace.Site(), _isSuspended);
        }

        #endregion

        #region Private Helpers

        protected void UpdateSuspendedState(bool triggerUpdate)
        {
            IsSuspended = (_guidanceModel.CurrentWorkflowState != _workflowState);

            if (triggerUpdate)
            {
                UpdateAll();
            }
        }

        protected abstract void UpdateAll();

        #endregion

        #region Callbacks - Guidance Model

        private void OnGuidanceModelGuidanceWorkflowStateChanged(GuidanceWorkflowStateType previousWorkflowState, GuidanceWorkflowStateType currentWorkflowState)
        {
            _trace.Debug("{0} Base called. Previous[{1}] Current[{2}]", Trace.Site(), previousWorkflowState, currentWorkflowState);

            UpdateSuspendedState(true);
        }

        #endregion
    }
}
