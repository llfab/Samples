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
    public class GuidanceControlViewModel : ViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        private readonly GuidanceControlTemplatingViewModel _guidanceControlTemplatingViewModel;
        public GuidanceControlTemplatingViewModel GuidanceControlTemplatingViewModel => _guidanceControlTemplatingViewModel;

        private readonly GuidanceControlPlatePreviewViewModel _guidanceControlPlatePreviewViewModel;
        public GuidanceControlPlatePreviewViewModel GuidanceControlPlatePreviewViewModel => _guidanceControlPlatePreviewViewModel;

        private readonly GuidanceControlArticularTargetingViewModel _guidanceControlArticularTargetingViewModel;
        public GuidanceControlArticularTargetingViewModel GuidanceControlArticularTargetingViewModel => _guidanceControlArticularTargetingViewModel;

        #endregion

        #region Bindable Fields

        private GuidanceWorkflowStateType _currentGuidanceWorkflowState;
        public GuidanceWorkflowStateType CurrentGuidanceWorkflowState
        {
            get => _currentGuidanceWorkflowState;
            set
            {
                RaiseAndSetIfChanged(ref _currentGuidanceWorkflowState, value);
            }
        }

        #endregion

        #region Construction

        public GuidanceControlViewModel()
        {
            _trace = Trace.ForType<GuidanceControlViewModel>();

            _guidanceControlTemplatingViewModel = new GuidanceControlTemplatingViewModel();
            _guidanceControlPlatePreviewViewModel = new GuidanceControlPlatePreviewViewModel();
            _guidanceControlArticularTargetingViewModel = new GuidanceControlArticularTargetingViewModel();

            // read from resource
            ReadResources();

            if (Design.IsDesignMode)
            {
                #region Design Mode

                CurrentGuidanceWorkflowState = GuidanceWorkflowStateType.PlatePreview;

                #endregion
            }
            else
            {
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();
                _guidanceModel = _presentationModel.Guidance;

                // connect events
                _guidanceModel.GuidanceWorkflowStateChanged += (x, y) => ActionDispatcher.Execute(() => OnGuidanceModelWorkflowStateChanged(x, y));
                
                UpdateAll();
            }

            _trace.Info("{0} created.", Trace.Site());
        }

        private void ReadResources()
        {
            // nothing to do
        }

        #endregion

        #region Public Access

        #endregion

        #region Private Helpers

        private void UpdateAll()
        {
            UpdateWorkflowState();
        }

        private void UpdateWorkflowState()
        {
            GuidanceWorkflowStateType currentGuidanceWorkflowState = _guidanceModel.CurrentWorkflowState;

            CurrentGuidanceWorkflowState = currentGuidanceWorkflowState;
        }

        #endregion

        #region Callbacks

        private void OnGuidanceModelWorkflowStateChanged(GuidanceWorkflowStateType previousWorkflowState, GuidanceWorkflowStateType currentWorkflowState)
        {
            _trace.Debug("{0} Previous[{1}] Current[{2}]", Trace.Site(), previousWorkflowState, currentWorkflowState);

            UpdateWorkflowState();
        }

        #endregion
    }
}
