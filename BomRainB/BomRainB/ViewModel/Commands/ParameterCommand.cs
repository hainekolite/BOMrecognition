using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BomRainB.ViewModel.Commands
{
    public class ParameterCommand : ICommand
    {
        private readonly Action<object> action;
        private readonly bool canAccess;
        public bool CanAccess => canAccess;

        public ParameterCommand(Action<object> action)
        {
            this.canAccess = true;
            this.action = action;
        }

        public bool CanExecute(object parameter) => canAccess;

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}
