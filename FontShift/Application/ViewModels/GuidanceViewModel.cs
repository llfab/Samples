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
    public class GuidanceViewModel : ViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;

        private readonly GuidanceControlViewModel _guidanceControlViewModel;
        public GuidanceControlViewModel GuidanceControlViewModel => _guidanceControlViewModel;

        private readonly GuidanceVisualizationViewModel _guidanceVisualizationViewModel;
        public GuidanceVisualizationViewModel GuidanceVisualizationViewModel => _guidanceVisualizationViewModel;

        private readonly GuidanceInfoViewModel _guidanceInfoViewModel;
        public GuidanceInfoViewModel GuidanceInfoViewModel => _guidanceInfoViewModel;

        #endregion

        #region Bindable Fields

        private bool _dropAllowed;
        public bool DropAllowed
        {
            get => _dropAllowed;
            set { RaiseAndSetIfChanged(ref _dropAllowed, value); }
        }

        #endregion

        #region Construction

        public GuidanceViewModel()
        {
            _trace = Trace.ForType<GuidanceViewModel>();

            _guidanceControlViewModel = new GuidanceControlViewModel();
            _guidanceVisualizationViewModel = new GuidanceVisualizationViewModel();
            _guidanceInfoViewModel = new GuidanceInfoViewModel();

            // read from resource
            ReadResources();

            if (Design.IsDesignMode)
            {
                #region Design Mode

                DropAllowed = true;

                #endregion
            }
            else
            {
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();

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
            UpdateDropState();
        }

        private void UpdateDropState()
        {
            DropAllowed = _presentationModel.TestModeEnabled;
        }

        #endregion

        #region Callbacks

        #endregion
    }
}
