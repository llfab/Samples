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
    public class MainViewModel : ViewModel
    {
        #region Constants

        #endregion

        #region Fields

        private readonly Trace _trace;
        private readonly PresentationModel _presentationModel;

        private readonly MenuViewModel _menuViewModel;
        public MenuViewModel MenuViewModel => _menuViewModel;

        private readonly GuidanceViewModel _guidanceViewModel;
        public GuidanceViewModel GuidanceViewModel => _guidanceViewModel;

        #endregion

        #region Bindable Fields

        #endregion

        #region Construction

        public MainViewModel()
        {
            _trace = Trace.ForType<MainViewModel>();

            _menuViewModel = new MenuViewModel();
            _guidanceViewModel = new GuidanceViewModel();

            // read from resource
            ReadResources();

            if (Design.IsDesignMode)
            {
                #region Design Mode

                // nothing to do

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
            // nothing to do
        }

        #endregion

        #region Callbacks

        #endregion
    }
}
