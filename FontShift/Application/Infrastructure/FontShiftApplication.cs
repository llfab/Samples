// ===========================================================================
//                     B I T S   O F   N A T U R E 
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using FontShift.Application.Models;
using System;

namespace FontShift.Application.Infrastructure
{
    public class FontShiftApplication
    {
        #region Fields

        private readonly Trace _trace;

        private PresentationModel _presentationModel;

        #endregion

        #region Construction / Destruction

        public FontShiftApplication()
        {
            _trace = Trace.ForType<FontShiftApplication>();

            _trace.Info("{0} created.", Trace.Site());
        }

        public void Init()
        {
            try
            {
                // Init handlers / managers
                // Init models
                _presentationModel = new PresentationModel();
                FontShiftSystem.Instance.ServiceLocator.Register(_presentationModel);
                _presentationModel.Init();

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
                if (_presentationModel != null)
                {
                    _presentationModel.Exit();
                    _presentationModel = null;
                }

                _trace.Info("{0} successful.", Trace.Site());
            }
            catch (Exception ex)
            {
                _trace.Error("{0} failed.\n{1}", Trace.Site(), ex);
            }
        }

        #endregion

        #region Private Helpers

        #endregion
    }
}
