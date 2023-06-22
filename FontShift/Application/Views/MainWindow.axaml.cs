// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FontShift.Application.Infrastructure;
using FontShift.Application.ViewModels;
using System;

namespace FontShift.Application.Views
{
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly Trace _trace;

        #endregion

        #region Construction

        public MainWindow()
        {
            _trace = new Trace("MainWindow");

            try
            {
                InitializeComponent();

                if (!Design.IsDesignMode)
                {
#if DEBUG
                    this.AttachDevTools();
#endif
                }
            }
            catch (Exception ex)
            {
                _trace.Fatal("{0} Error.\n{1}", Trace.Site(), ex);
            }
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            FontShiftSystem.Instance.ShutdownRequested(out bool cancel);
            e.Cancel = cancel;
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            try
            {
                InitializeDragAndDrop();
            }
            catch (Exception ex)
            {
                _trace.Fatal("{0} Error.\n{1}", Trace.Site(), ex);
            }

            base.OnDataContextChanged(e);
        }

        #endregion

        #region Private Helpers

        private void InitializeDragAndDrop()
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;

            AddHandler(DragDrop.DropEvent, viewModel.Drop);
            AddHandler(DragDrop.DragOverEvent, viewModel.DragOver);
        }

        #endregion
    }
}
