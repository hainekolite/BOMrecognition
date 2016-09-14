using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BomRainB.ViewModel.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action action;
        private readonly bool canAccess;
        public bool CanAccess => canAccess;

        public RelayCommand(Action action, int relayIdentifier)
        {
            this.canAccess = true; //check for security singleton ?
            if (this.canAccess)
                this.action = action;
            else
                this.action = () => Console.WriteLine("Illegal command!");
        }

        public RelayCommand(Action action)
        {
            this.action = action;
            this.canAccess = true;
        }

        public bool CanExecute(object parameter = null) => canAccess;

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter = null) => action();

    }
}
