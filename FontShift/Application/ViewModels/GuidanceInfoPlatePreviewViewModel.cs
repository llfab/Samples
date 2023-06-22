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
using FontShift.Application.Utils;
using System;

namespace FontShift.Application.ViewModels
{
    public class GuidanceInfoPlatePreviewViewModel : GuidanceWorkflowStateViewModelBase
    {
        #region Constants

        //private const string InterMetatarsalAngleFormat = "{0:F1}°";
        //private const string Mt1LengthFormat = "{0:F1} mm";

        #endregion

        #region Fields

        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        #endregion

        #region Bindable Fields

        //private readonly ValueVisualizationViewModel _actualInterMetatarsalAngle;
        //public ValueVisualizationViewModel ActualInterMetatarsalAngle => _actualInterMetatarsalAngle;
        //private readonly ValueVisualizationViewModel _actualMt1Length;
        //public ValueVisualizationViewModel ActualMt1Length => _actualMt1Length;

        private string _notAvailableText;

        #endregion

        #region Construction

        public GuidanceInfoPlatePreviewViewModel()
            : base(GuidanceWorkflowStateType.PlatePreview)
        {
            // read from resource
            ReadResources();

            //_actualInterMetatarsalAngle = new ValueVisualizationViewModel() { Name = nameof(ActualInterMetatarsalAngle), Format = InterMetatarsalAngleFormat, InvalidText = _notAvailableText };
            //_actualMt1Length = new ValueVisualizationViewModel() { Name = nameof(ActualMt1Length), Format = Mt1LengthFormat, InvalidText = _notAvailableText };

            if (Design.IsDesignMode)
            {
                #region Design Mode

                #endregion
            }
            else
            {
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();
                _guidanceModel = _presentationModel.Guidance;

                // connect events
                UpdateAll();
            }

            _trace.Info("{0} created.", Trace.Site());
        }

        private void ReadResources()
        {
            _notAvailableText = AvaloniaUtils.GetResource<string>("GuidanceInfoPlatePreviewViewNotAvailable");
        }

        #endregion

        #region Public Access

        #endregion

        #region Private Helpers

        protected override void UpdateAll()
        {
            if (!IsSuspended)
            {
            }
            else
            {
                _trace.Verbose("{0} No update performed due to suspended state.", Trace.Site());
            }
        }

        #endregion

    }
}
