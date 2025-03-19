// ===========================================================================\r
//                          B I T S   O F   N A T U R E\r
// ===========================================================================\r
//  This document contains proprietary information. It is the exclusive\r
//  confidential property of Stryker Corporation and its affiliates.\r
// \r
//  Copyright (c) 2024 Stryker\r
// ===========================================================================
using System;
using System.Collections.Generic;

namespace AvaloniaIosApp.ViewModels
{
    public abstract class ViewModel :ViewModelBase, IDisposable
    {
        private readonly List<SimpleCommand> _commands = new List<SimpleCommand>();
        
        protected void AddCommand(SimpleCommand command)
        {
            _commands.Add(command);
        }
        
        protected void RemoveCommand(SimpleCommand command)
        {
            _commands.Remove(command);
        }
        
        protected void UpdateCommandStates()
        {
            foreach (SimpleCommand command in _commands)
            {
                command.TriggerCanExecuteChanged();
            }
        }

        public abstract void Dispose();
    }
}
