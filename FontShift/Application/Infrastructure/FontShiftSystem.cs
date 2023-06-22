// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using FontShift.Application.Models;
using FontShift.Application.Utils;
using System;

namespace FontShift.Application.Infrastructure
{
    public class FontShiftSystem
    {
        #region Constants

        public const string FontShiftConfiguration = "FontShiftConfiguration";
        public const string FontShiftImageProcessingConfiguration = "FontShiftImageProcessingConfiguration";

        #endregion

        #region Fields

        private readonly Trace _trace;

        private readonly ServiceLocator _serviceLocator;
        public ServiceLocator ServiceLocator => _serviceLocator;

        private readonly StartupParameters _startupParameters;

        private FontShiftApplication _application;

        private bool _isShuttingDown;

        #endregion

        #region Construction / Destruction

        public static FontShiftSystem Instance
        {
            get;
            private set;
        }

        public FontShiftSystem(StartupParameters startupParameters)
        {
            _trace = Trace.ForType<FontShiftSystem>();

            Instance = this;
            _startupParameters = startupParameters;
            _serviceLocator = new ServiceLocator();
            _isShuttingDown = false;

            _trace.Info("{0} created.", Trace.Site());
        }

        public void Init()
        {
            try
            {
                // Application
                _application = new FontShiftApplication();
                _application.Init();

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
                if (_application != null)
                {
                    _application.Exit();
                    _application = null;
                }

                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        public void Shutdown()
        {
            try
            {
                // indicate that we are shutting down
                _isShuttingDown = true;

                App.DoExit();

                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        public void ShutdownRequested(out bool cancel)
        {
            _trace.Info("{0} called.", Trace.Site());

            // if we are already shutting down, do not interrupt
            if (_isShuttingDown)
            {
                cancel = false;
                return;
            }

            PresentationModel model = ServiceLocator.Get<PresentationModel>();

            model.ShowExitProgramDialog();
            cancel = false;
        }

        #endregion
    }
}
