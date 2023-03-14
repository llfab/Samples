using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication10
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WorkflowStateType _currentWorkflowState;
        public WorkflowStateType CurrentWorkflowState
        {
            get => _currentWorkflowState;
            set
            {
                RaiseAndSetIfChanged(ref _currentWorkflowState, value);
            }
        }

        public MainWindowViewModel()
        {
            if (Design.IsDesignMode)
            {
                CurrentWorkflowState = WorkflowStateType.CartridgeSelection;
            }
            else
            {
                CurrentWorkflowState = WorkflowStateType.CartridgeSelection;
            }
        }
    }
}
