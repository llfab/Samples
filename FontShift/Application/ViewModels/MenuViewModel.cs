// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FontShift.Application.Infrastructure;
using FontShift.Application.Models;
using FontShift.Application.Mvvm;
using System;

namespace FontShift.Application.ViewModels
{
    public class MenuViewModel : ViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;
        private readonly GuidanceModel _guidanceModel;

        #endregion

        #region Bindable Fields

        private bool _isSuperUserMode;
        public bool IsSuperUserMode
        {
            get => _isSuperUserMode;
            set
            {
                RaiseAndSetIfChanged(ref _isSuperUserMode, value);
            }
        }

        private bool _isDemoModeEnabled;
        public bool IsDemoModeEnabled
        {
            get => _isDemoModeEnabled;
            set
            {
                RaiseAndSetIfChanged(ref _isDemoModeEnabled, value);
            }
        }

        private GuidanceWorkflowStateType _currentGuidanceWorkflowState;
        public GuidanceWorkflowStateType CurrentGuidanceWorkflowState
        {
            get => _currentGuidanceWorkflowState;
            set
            {
                RaiseAndSetIfChanged(ref _currentGuidanceWorkflowState, value);
            }
        }

        private bool _isSystemMenuOpened;
        public bool IsSystemMenuOpened
        {
            get => _isSystemMenuOpened;
            set
            {
                RaiseAndSetIfChanged(ref _isSystemMenuOpened, value);
            }
        }

        private bool _isImageProcessingPaused;
        public bool IsImageProcessingPaused
        {
            get => _isImageProcessingPaused;
            set
            {
                RaiseAndSetIfChanged(ref _isImageProcessingPaused, value);
            }
        }

        #endregion

        #region Bindable Commands

        public SimpleCommand GotoGuidanceWorkflowStateTemplatingCommand { get; private set; }
        public SimpleCommand GotoGuidanceWorkflowStatePlatePreviewCommand { get; private set; }
        public SimpleCommand GotoGuidanceWorkflowStatePlatePositioningCommand { get; private set; }
        public SimpleCommand GotoGuidanceWorkflowStateArticularTargetingCommand { get; private set; }
        public SimpleCommand GotoGuidanceWorkflowStateShaftTargetingCommand { get; private set; }
        public SimpleCommand AcquireNextXrayCommand { get; private set; }
        public SimpleCommand ToggleImageProcessingPausedCommand { get; private set; }
        public SimpleCommand CreateScreenshotCommand { get; private set; }
        public SimpleCommand ShowSystemMenuCommand { get; private set; }
        public SimpleCommand ShowInfoCommand { get; private set; }
        public SimpleCommand ExitCommand { get; private set; }

        #endregion

        #region Construction

        public MenuViewModel()
        {
            _trace = Trace.ForType<MenuViewModel>();

            // read from resource
            ReadResources();

            GotoGuidanceWorkflowStateTemplatingCommand = SimpleCommand.Create(GotoGuidanceWorkflowStateTemplating);
            GotoGuidanceWorkflowStatePlatePreviewCommand = SimpleCommand.Create(GotoGuidanceWorkflowStatePlatePreview);
            GotoGuidanceWorkflowStatePlatePositioningCommand = SimpleCommand.Create(GotoGuidanceWorkflowStatePlatePositioning);
            GotoGuidanceWorkflowStateArticularTargetingCommand = SimpleCommand.Create(GotoGuidanceWorkflowStateArticularTargeting);
            GotoGuidanceWorkflowStateShaftTargetingCommand = SimpleCommand.Create(GotoGuidanceWorkflowStateShaftTargeting);
            AcquireNextXrayCommand = SimpleCommand.Create(AcquireNextXray);
            ToggleImageProcessingPausedCommand = SimpleCommand.Create(ToggleImageProcessingPaused);
            CreateScreenshotCommand = SimpleCommand.Create(CreateScreenshot);
            ShowSystemMenuCommand = SimpleCommand.Create(ShowSystemMenu);
            ShowInfoCommand = SimpleCommand.Create(ShowInfo);
            ExitCommand = SimpleCommand.Create(ShowExit);

            if (Design.IsDesignMode)
            {
                #region Design Mode

                IsSuperUserMode = true;
                IsDemoModeEnabled = true;
                CurrentGuidanceWorkflowState = GuidanceWorkflowStateType.Templating;

                #endregion
            }
            else
            {
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();
                _guidanceModel = _presentationModel.Guidance;

                // connect events
                _presentationModel.ActiveDialogChanged += (x, y) => ActionDispatcher.Execute(() => OnPresentationModelActiveDialogChanged(x, y));

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

        #region Command Handlers

        private void GotoGuidanceWorkflowStateTemplating()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _guidanceModel.SetWorkflowStateTemplating();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void GotoGuidanceWorkflowStatePlatePreview()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _guidanceModel.SetWorkflowStatePlatePreview();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void GotoGuidanceWorkflowStatePlatePositioning()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _guidanceModel.SetWorkflowStatePlatePositioning();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void GotoGuidanceWorkflowStateArticularTargeting()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _guidanceModel.SetWorkflowStateArticularTargeting();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void GotoGuidanceWorkflowStateShaftTargeting()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _guidanceModel.SetWorkflowStateShaftTargeting();
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void ToggleImageProcessingPaused()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void AcquireNextXray()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void CreateScreenshot()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void ShowSystemMenu()
        {
            try
            {
                _trace.Info("{0} called.", Trace.Site());

                _presentationModel.ShowSystemMenu();
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
            UpdateSuperUserMode();
            UpdateDemoMode();
            UpdateWorkflowState();
            UpdateSystemMenuState();
            UpdateImageProcessingPausedState();
        }

        private void UpdateSuperUserMode()
        {
            IsSuperUserMode = _presentationModel.SuperUserModeEnabled;
        }

        private void UpdateDemoMode()
        {
            IsDemoModeEnabled = _presentationModel.DemoModeEnabled;
        }

        private void UpdateWorkflowState()
        {
            GuidanceWorkflowStateType currentGuidanceWorkflowState = _guidanceModel.CurrentWorkflowState;

            CurrentGuidanceWorkflowState = currentGuidanceWorkflowState;
        }

        private void UpdateSystemMenuState()
        {
            bool isSystemMenuOpened = _presentationModel.ActiveDialog == ActiveDialogType.SystemMenu;

            IsSystemMenuOpened = isSystemMenuOpened;
        }

        private void UpdateImageProcessingPausedState()
        {

        }

        #endregion

        #region Callbacks

        private void OnGuidanceModelWorkflowStateChanged(GuidanceWorkflowStateType previousWorkflowState, GuidanceWorkflowStateType currentWorkflowState)
        {
            _trace.Debug("{0} Previous[{1}] Current[{2}]", Trace.Site(), previousWorkflowState, currentWorkflowState);

            UpdateWorkflowState();
        }

        private void OnPresentationModelActiveDialogChanged(ActiveDialogType previousActiveDialog, ActiveDialogType currentActiveDialog)
        {
            _trace.Debug("{0} Previous[{1}] Current[{2}]", Trace.Site(), previousActiveDialog, currentActiveDialog);

            UpdateSystemMenuState();
        }

        private void OnImageProcessingModelImageProcessingPausedChanged(bool isPaused)
        {
            _trace.Debug("{0} IsImageProcessingPaused[{1}]", Trace.Site(), isPaused);

            UpdateImageProcessingPausedState();
        }

        #endregion
    }
}
