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
    public class GuidanceVisualizationTemplatingViewModel : GuidanceWorkflowStateViewModelBase
    {
        #region Constants

        #endregion

        #region Fields

        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        #endregion

        #region Bindable Fields

        #endregion

        #region Bindable Commands - Menu

        //public SimpleCommand GotoOsteotomyGuidanceWorkflowStatePlanningCommand { get; private set; }

        #endregion

        #region Construction

        public GuidanceVisualizationTemplatingViewModel()
            : base(GuidanceWorkflowStateType.Templating)
        {
            // commands
            //GotoOsteotomyGuidanceWorkflowStatePlanningCommand = SimpleCommand.Create(GotoOsteotomyGuidanceWorkflowStatePlanning, CanGotoOsteotomyGuidanceWorkflowStatePlanning);
            //AddCommand(GotoOsteotomyGuidanceWorkflowStatePlanningCommand);

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
                _guidanceModel = _presentationModel.Guidance;

                UpdateAll();
            }

            _trace.Info("{0} created.", Trace.Site());
        }

        private void ReadResources()
        {
            // nothing to do
        }

        #endregion

        #region Command Handler - Workflow

        //private bool CanGotoOsteotomyGuidanceWorkflowStatePlanning()
        //{
        //    return
        //        !_isMetatarsalModificationActive &&
        //        !_isOsteotomyPlaneModificationActive;
        //}

        //private void GotoOsteotomyGuidanceWorkflowStatePlanning()
        //{
        //    try
        //    {
        //        _trace.Info("{0} called.", Trace.Site());

        //        _guidanceTemplatingModel.SetWorkflowStatePlanning();
        //    }
        //    catch (Exception ex)
        //    {
        //        _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
        //    }
        //}

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
