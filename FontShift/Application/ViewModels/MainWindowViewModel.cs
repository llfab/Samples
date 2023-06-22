// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia.Controls;
using Avalonia.Input;
using FontShift.Application.Infrastructure;
using FontShift.Application.Models;
using FontShift.Application.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FontShift.Application.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Constants

        private string _errorHeaderImageProcessingFailed;
        private string _errorMessageImageProcessingFailed;

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;

        private readonly MainViewModel _mainViewModel;
        public MainViewModel MainViewModel => _mainViewModel;

        private readonly SystemMenuViewModel _systemMenuViewModel;

        #endregion

        #region Bindable Fields

        private bool _hasVisibleDialog;
        public bool HasVisibleDialog
        {
            get => _hasVisibleDialog;
            set
            {
                RaiseAndSetIfChanged(ref _hasVisibleDialog, value);
            }
        }

        private bool _hasVisibleTopMostDialog;
        public bool HasVisibleTopMostDialog
        {
            get => _hasVisibleTopMostDialog;
            set
            {
                RaiseAndSetIfChanged(ref _hasVisibleTopMostDialog, value);
            }
        }

        private ContentControlledViewModel _currentDialog;
        public ContentControlledViewModel CurrentDialog
        {
            get => _currentDialog;
            set
            {
                RaiseAndSetIfChanged(ref _currentDialog, value);
                HasVisibleDialog = (_currentDialog != null);
            }
        }

        private ContentControlledViewModel _currentTopMostDialog;
        public ContentControlledViewModel CurrentTopMostDialog
        {
            get => _currentTopMostDialog;
            set
            {
                RaiseAndSetIfChanged(ref _currentTopMostDialog, value);
                HasVisibleTopMostDialog = (_currentTopMostDialog != null);
            }
        }

        #endregion

        #region Bindable Commands


        #endregion

        #region Construction

        public MainWindowViewModel()
        {
            _trace = Trace.ForType<MainWindowViewModel>();
            
            _mainViewModel = new MainViewModel();

            _systemMenuViewModel = new SystemMenuViewModel();

            // read from resource
            ReadResources();

            if (Design.IsDesignMode)
            {
                #region Design Mode

                CurrentDialog = null;
                CurrentTopMostDialog = null;

                #endregion
            }
            else
            {
                
                _presentationModel = FontShiftSystem.Instance.ServiceLocator.Get<PresentationModel>();

                // connect events
                _presentationModel.ActiveDialogChanged += (x, y) => ActionDispatcher.Execute(() => OnPresentationModelActiveDialogChanged(x, y));
                _presentationModel.ActiveTopMostDialogChanged += (x, y) => ActionDispatcher.Execute(() => OnPresentationModelActiveTopMostDialogChanged(x, y));

                UpdateAll();
            }

            _trace.Info("{0} created.", Trace.Site());
        }

        private void ReadResources()
        {
        }

        #endregion

        #region Command Handlers


        #endregion

        #region Public Access

        public void DragOver(object sender, DragEventArgs e)
        {
            // Only allow if the dragged data a single filename with allowed format
            if (IsAllowedDataObject(e.Data))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        public void Drop(object sender, DragEventArgs e)
        {
            e.DragEffects &= (DragDropEffects.Copy);

            if (IsAllowedDataObject(e.Data))
            {
                HandleDroppedFiles(GetFileNamesFromDataObject(e.Data));
            }
        }

        #endregion

        #region Private Helpers - Update

        private void UpdateAll()
        {
            UpdateActiveDialog();
        }

        private void UpdateActiveDialog()
        {
            ActiveDialogType currentDialogType = _presentationModel.ActiveDialog;

            switch (currentDialogType)
            {
                case ActiveDialogType.None:
                    ShowCurrentDialogNone();
                    break;
                case ActiveDialogType.SystemMenu:
                    ShowCurrentDialogSystemMenu();
                    break;
                case ActiveDialogType.About:
                    break;
                case ActiveDialogType.ExitProgram:
                    break;
            }
        }

        private void UpdateActiveTopMostDialog()
        {
            ActiveTopMostDialogType currentTopMostDialogType = _presentationModel.ActiveTopMostDialog;

            switch (currentTopMostDialogType)
            {
                case ActiveTopMostDialogType.None:
                    ShowCurrentTopMostDialogNone();
                    break;
                case ActiveTopMostDialogType.ConfirmationMessage:
                    break;
                case ActiveTopMostDialogType.ErrorConfirmationMessage:
                    break;
                case ActiveTopMostDialogType.ImageProcessingBusyMessage:
                    break;
            }
        }

        private void ShowCurrentDialogNone()
        {
            try
            {
                CurrentDialog = null;
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void ShowCurrentDialogSystemMenu()
        {
            try
            {
                CurrentDialog = _systemMenuViewModel;
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        private void ShowCurrentTopMostDialogNone()
        {
            try
            {
                CurrentTopMostDialog = null;
            }
            catch (Exception ex)
            {
                _trace.Error("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        #endregion

        #region Private Helpers - Drag&Drop

        private bool IsAllowedDataObject(IDataObject dataObject)
        {
            return 
                false;
        }

        private IEnumerable<string> GetFileNamesFromDataObject(IDataObject dataObject)
        {
            return dataObject.GetFiles()?.Select(x => x.Path.LocalPath);
        }

        private void HandleDroppedFiles(IEnumerable<string> filenames)
        {
        }

        #endregion

        #region Callbacks

        private void OnPresentationModelActiveDialogChanged(ActiveDialogType previousActiveDialog, ActiveDialogType currentActiveDialog)
        {
            _trace.Debug("{0} Previous[{1}] Current[{2}]", Trace.Site(), previousActiveDialog, currentActiveDialog);

            UpdateActiveDialog();
        }

        private void OnPresentationModelActiveTopMostDialogChanged(ActiveTopMostDialogType previousTopMostActiveDialog, ActiveTopMostDialogType currentActiveTopMostDialog)
        {
            _trace.Debug("{0} Previous[{1}] Current[{2}]", Trace.Site(), previousTopMostActiveDialog, currentActiveTopMostDialog);

            UpdateActiveTopMostDialog();
        }

        #endregion
    }
}
