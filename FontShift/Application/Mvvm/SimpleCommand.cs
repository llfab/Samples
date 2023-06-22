using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FontShift.Application.Mvvm
{
    public sealed class SimpleCommand<T> : SimpleCommand, System.Windows.Input.ICommand
    {
        #region Fields

        private readonly Action<T> _executeCallback;
        private readonly Func<T, bool> _canExecuteCallback;
        private readonly Func<T, Task> _asyncExecuteCallback;
        private bool _isExecuting;

        #endregion

        #region Construction

        public SimpleCommand(Action<T> excuteCallback)
        {
            _executeCallback = excuteCallback;
            _canExecuteCallback = null;
        }

        public SimpleCommand(Action<T> excuteCallback, Func<T, bool> canExecuteCallback)
        {
            _executeCallback = excuteCallback;
            _canExecuteCallback = canExecuteCallback;
        }

        public SimpleCommand(Func<T, Task> callback)
        {
            _asyncExecuteCallback = callback;
            _canExecuteCallback = null;
        }

        public SimpleCommand(Func<T, Task> callback, Func<T, bool> canExecuteCallback)
        {
            _asyncExecuteCallback = callback;
            _canExecuteCallback = canExecuteCallback;
        }

        #endregion

        #region Bindable Fields

        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                RaiseAndSetIfChanged(ref _isExecuting, value);
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region ICommand

        public override void TriggerCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public override event EventHandler CanExecuteChanged;

        public override bool CanExecute(object parameter)
        {
            if (_isExecuting)
            {
                return false;
            }

            if (_canExecuteCallback == null)
            {
                return true;
            }

            return _canExecuteCallback.Invoke((T)parameter);
        }

        public override async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                IsExecuting = true;
                if (_executeCallback != null)
                    _executeCallback((T)parameter);
                else
                    await _asyncExecuteCallback((T)parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        #endregion
    }

    public abstract class SimpleCommand : System.Windows.Input.ICommand, INotifyPropertyChanged
    {
        #region Static Construction

        public static SimpleCommand Create(Action executeCallback, Func<bool> canExecuteCallback = null) 
            => canExecuteCallback == null ? new SimpleCommand<object>(_ => executeCallback()) : new SimpleCommand<object>(_ => executeCallback(), _ => canExecuteCallback());
        
        public static SimpleCommand Create<TArg>(Action<TArg> executeCallback, Func<TArg, bool> canExecuteCallback = null) 
            => canExecuteCallback == null ? new SimpleCommand<TArg>(executeCallback) : new SimpleCommand<TArg>(executeCallback, canExecuteCallback);
        
        public static SimpleCommand CreateFromTask(Func<Task> asyncExecuteCallback, Func<bool> canExecuteCallback = null) 
            => canExecuteCallback == null ? new SimpleCommand<object>(_ => asyncExecuteCallback()) : new SimpleCommand<object>(_ => asyncExecuteCallback(), _ => canExecuteCallback());

        public static SimpleCommand CreateFromTask<TArg>(Func<TArg, Task> asyncExecuteCallback, Func<TArg, bool> canExecuteCallback = null)
            => canExecuteCallback == null ? new SimpleCommand<TArg>(asyncExecuteCallback) : new SimpleCommand<TArg>(asyncExecuteCallback, canExecuteCallback);

        #endregion

        #region Abstract Members

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public abstract event EventHandler CanExecuteChanged;
        public abstract void TriggerCanExecuteChanged();

        #endregion

        #region INotifyPropertyChanged

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

        #endregion
    }
}
