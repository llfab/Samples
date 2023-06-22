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
using System;

namespace FontShift.Application.ViewModels
{
    public class GuidanceControlArticularTargetingViewModel : GuidanceWorkflowStateViewModelBase
    {
        #region Constants

        #endregion

        #region Fields

        private readonly PresentationModel _presentationModel;

        #endregion

        #region Bindable Fields

        #endregion

        #region Bindable Commands - Menu

        #endregion

        #region Construction

        public GuidanceControlArticularTargetingViewModel()
            : base(GuidanceWorkflowStateType.ArticularTargeting)
        {
            // commands
            //ShowCalculatedMetatarsalDataCommand = SimpleCommand.Create(ShowCalculatedMetatarsalData, CanShowCalculatedMetatarsalData);
            //AddCommand(ShowCalculatedMetatarsalDataCommand);

            // read from resource
            ReadResources();

            if (Design.IsDesignMode)
            {
                #region Design Mode

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

        #region Command Handler - XY

        #endregion

        #region Public Access

        #endregion

        #region Private Helpers

        protected override void UpdateAll()
        {
            if (!IsSuspended)
            {
                UpdateCommandStates();
            }
            else
            {
                _trace.Verbose("{0} No update performed due to suspended state.", Trace.Site());
            }
        }

        #endregion

    }
}
