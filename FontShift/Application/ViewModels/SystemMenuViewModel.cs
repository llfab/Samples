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
using System;

namespace FontShift.Application.ViewModels
{
    public class SystemMenuViewModel : ContentControlledViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;

        #endregion

        #region Bindable Fields

        #endregion

        #region Bindable Commands

        public SimpleCommand CloseMenuCommand { get; private set; }
        public SimpleCommand ShowInfoCommand { get; private set; }
        public SimpleCommand ExitCommand { get; private set; }

        #endregion

        #region Construction

        public SystemMenuViewModel()
        {
            _trace = Trace.ForType<SystemMenuViewModel>();

            // read from resource
            ReadResources();

            CloseMenuCommand = SimpleCommand.Create(CloseMenu);
            ShowInfoCommand = SimpleCommand.Create(ShowInfo);
            ExitCommand = SimpleCommand.Create(ShowExit);

            if (Design.IsDesignMode)
            {
                #region Design Mode

                // nothing to do

                #endregion
            }
            else
            {
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();

                // connect events
                
                UpdateAll();
            }

            _trace.Info("{0} created.", Trace.Site());
        }

        private void ReadResources()
        {
            // nothing to do
        }

        #endregion

        #region Command Handlers

        private void CloseMenu()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _presentationModel.HideSystemMenu();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void ShowInfo()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _presentationModel.ShowAboutDialog();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void ShowExit()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _presentationModel.ShowExitProgramDialog();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        #endregion

        #region Private Helpers

        private void UpdateAll()
        {
            // nothing to do
        }

        #endregion

        #region Callbacks

        #endregion
    }
}
