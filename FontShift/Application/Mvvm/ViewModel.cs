// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FontShift.Application.Mvvm
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
    }
}
