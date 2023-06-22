// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using FontShift.Application.Infrastructure;
using FontShift.Application.Utils;

namespace FontShift.Application.Models
{
    #region Definitions

    public enum ActiveDialogType
    {
        None,
        SystemMenu,
        About,
        ExitProgram,
    }

    public enum ActiveTopMostDialogType
    {
        None,
        ConfirmationMessage,
        ErrorConfirmationMessage,
        ImageProcessingBusyMessage,
    }

    #endregion

    public class PresentationModel
    {
        #region Constants

        private readonly string _dateTimeFormat;

        #endregion

        #region Events

        public delegate void ActiveDialogChangedHandler(ActiveDialogType previousActiveDialog, ActiveDialogType currentActiveDialog);
        public event ActiveDialogChangedHandler ActiveDialogChanged;

        public delegate void ActiveTopMostDialogChangedHandler(ActiveTopMostDialogType previousActiveTopMostDialog, ActiveTopMostDialogType currentActiveTopMostDialog);
        public event ActiveTopMostDialogChangedHandler ActiveTopMostDialogChanged;

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly object _activeDialogAccess;

        private readonly GuidanceModel _guidance;
        public GuidanceModel Guidance => _guidance;

        private bool _superUserModeEnabled;
        public bool SuperUserModeEnabled => _superUserModeEnabled;

        private bool _testModeEnabled;
        public bool TestModeEnabled => _testModeEnabled;

        private bool _demoModeEnabled;
        public bool DemoModeEnabled => _demoModeEnabled;

        private ActiveDialogType _activeDialog;
        public ActiveDialogType ActiveDialog
        {
            get => _activeDialog;
            private set
            {
                lock (_activeDialogAccess)
                {
                    if (_activeDialog != value)
                    {
                        ActiveDialogType previous = _activeDialog;
                        _activeDialog = value;
                        _trace.Info("{0} Previous[{1}] Current[{2}]", Trace.Site(), previous, _activeDialog);
                        ActiveDialogChanged?.Invoke(previous, _activeDialog);
                    }
                }
            }
        }

        private ActiveTopMostDialogType _activeTopMostDialog;
        public ActiveTopMostDialogType ActiveTopMostDialog
        {
            get => _activeTopMostDialog;
            private set
            {
                lock (_activeDialogAccess)
                {
                    if (_activeTopMostDialog != value)
                    {
                        ActiveTopMostDialogType previous = _activeTopMostDialog;
                        _activeTopMostDialog = value;
                        _trace.Info("{0} Previous[{1}] Current[{2}]", Trace.Site(), previous, _activeTopMostDialog);
                        ActiveTopMostDialogChanged?.Invoke(previous, _activeTopMostDialog);
                    }
                }
            }
        }

        private readonly string _locale;
        public string Locale
        {
            get => _locale;
        }
        #endregion

        #region Construction

        public PresentationModel()
        {
            _trace = Trace.ForType<PresentationModel>();
            _locale = "en-US";
            _dateTimeFormat = FontShiftConstants.GlobalBackendDateTimeDefaultFormat;
            _activeDialogAccess = new object();

            // Default settings
            _superUserModeEnabled = false;
            _testModeEnabled = false;
            _demoModeEnabled = false;
            _activeDialog = ActiveDialogType.None;
            _activeTopMostDialog = ActiveTopMostDialogType.None;

            _guidance = new GuidanceModel();

            _trace.Info("{0} created.", Trace.Site());
        }

        #endregion

        #region Init / Exit

        public void Init()
        {
            try
            {
                _guidance.Init();

                // override from configuration for testing purposes
                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        public void Exit()
        {
            try
            {
                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        #endregion

        #region Public Access - Info

        public Version GetFontShiftVersion()
        {
            return Assembly.GetAssembly(typeof(PresentationModel)).GetName().Version;
        }

        #endregion

        #region Public Access - Active Dialog

        public void HideAnyDialog()
        {
            lock (_activeDialogAccess)
            {
                ActiveDialog = ActiveDialogType.None;
            }
        }

        #endregion

        #region Public Access - Active Top Most Dialog

        public void HideAnyTopMostDialog()
        {
            lock (_activeDialogAccess)
            {
                ActiveTopMostDialog = ActiveTopMostDialogType.None;
            }
        }

        #endregion

        #region Public Access - Dialogs / Menus

        public void ShowSystemMenu()
        {
            lock (_activeDialogAccess)
            {
                if (ActiveDialog == ActiveDialogType.None)
                {
                    ActiveDialog = ActiveDialogType.SystemMenu;
                }
            }
        }

        public void HideSystemMenu()
        {
            lock (_activeDialogAccess)
            {
                if (ActiveDialog == ActiveDialogType.SystemMenu)
                {
                    ActiveDialog = ActiveDialogType.None;
                }
            }
        }

        public void ShowAboutDialog()
        {
            lock (_activeDialogAccess)
            {
                if (ActiveDialog == ActiveDialogType.None || ActiveDialog == ActiveDialogType.SystemMenu)
                {
                    ActiveDialog = ActiveDialogType.About;
                }
            }
        }

        #endregion

        #region Public Access - Exit Application

        public void ShowExitProgramDialog()
        {
            lock (_activeDialogAccess)
            {
                if (ActiveDialog == ActiveDialogType.None || ActiveDialog == ActiveDialogType.SystemMenu)
                {
                    HideAnyTopMostDialog();
                    ActiveDialog = ActiveDialogType.ExitProgram;
                }
            }
        }

        #endregion
    }
}
