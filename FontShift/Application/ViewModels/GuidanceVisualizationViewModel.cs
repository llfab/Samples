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
    public class GuidanceVisualizationViewModel : ViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        private readonly GuidanceVisualizationTemplatingViewModel _guidanceVisualizationTemplatingViewModel;
        public GuidanceVisualizationTemplatingViewModel GuidanceVisualizationTemplatingViewModel => _guidanceVisualizationTemplatingViewModel;

        private readonly GuidanceVisualizationPlatePreviewViewModel _guidanceVisualizationPlatePreviewViewModel;
        public GuidanceVisualizationPlatePreviewViewModel GuidanceVisualizationPlatePreviewViewModel => _guidanceVisualizationPlatePreviewViewModel;

        private readonly GuidanceVisualizationArticularTargetingViewModel _guidanceVisualizationArticularTargetingViewModel;
        public GuidanceVisualizationArticularTargetingViewModel GuidanceVisualizationArticularTargetingViewModel => _guidanceVisualizationArticularTargetingViewModel;

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

        public GuidanceVisualizationViewModel()
        {
            _trace = Trace.ForType<GuidanceVisualizationViewModel>();

            _guidanceVisualizationTemplatingViewModel = new GuidanceVisualizationTemplatingViewModel();
            _guidanceVisualizationPlatePreviewViewModel = new GuidanceVisualizationPlatePreviewViewModel();
            _guidanceVisualizationArticularTargetingViewModel = new GuidanceVisualizationArticularTargetingViewModel();

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
