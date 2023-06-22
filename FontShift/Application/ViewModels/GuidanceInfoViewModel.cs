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
    public class ValueVisualizationViewModel : ViewModel
    {
        public string Name { get; set; }

        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set
            {
                RaiseAndSetIfChanged(ref _displayText, value);
            }
        }

        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                RaiseAndSetIfChanged(ref _value, value);
                Update();
            }
        }

        private string _format;
        public string Format
        {
            get => _format;
            set
            {
                RaiseAndSetIfChanged(ref _format, value);
                Update();
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                RaiseAndSetIfChanged(ref _isValid, value);
                Update();
            }
        }

        private string _invalidText;
        public string InvalidText
        {
            get => _invalidText;
            set
            {
                RaiseAndSetIfChanged(ref _invalidText, value);
                Update();
            }
        }

        private void Update()
        {
            if (_isValid)
            {
                if (!string.IsNullOrEmpty(_format))
                {
                    DisplayText = string.Format(_format, _value);
                    return;
                }
            }

            DisplayText = _invalidText;
        }
    }

    public class GuidanceInfoViewModel : ViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        private readonly GuidanceInfoTemplatingViewModel _guidanceInfoTemplatingViewModel;
        public GuidanceInfoTemplatingViewModel GuidanceInfoTemplatingViewModel => _guidanceInfoTemplatingViewModel;

        private readonly GuidanceInfoPlatePreviewViewModel _guidanceInfoPlatePreviewViewModel;
        public GuidanceInfoPlatePreviewViewModel GuidanceInfoPlatePreviewViewModel => _guidanceInfoPlatePreviewViewModel;

        private readonly GuidanceInfoArticularTargetingViewModel _guidanceInfoArticularTargetingViewModel;
        public GuidanceInfoArticularTargetingViewModel GuidanceInfoArticularTargetingViewModel => _guidanceInfoArticularTargetingViewModel;

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

        public GuidanceInfoViewModel()
        {
            _trace = Trace.ForType<GuidanceInfoViewModel>();

            _guidanceInfoTemplatingViewModel = new GuidanceInfoTemplatingViewModel();
            _guidanceInfoPlatePreviewViewModel = new GuidanceInfoPlatePreviewViewModel();
            _guidanceInfoArticularTargetingViewModel = new GuidanceInfoArticularTargetingViewModel();

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
