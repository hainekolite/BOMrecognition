using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BomRainB.ViewModel.Commands
{
    public class RelayCommand : ViewModelBase, ICommand
    {
        private readonly Action action;
        private bool canAccess;
        public bool CanAccess
        {
            get
            {
                return (canAccess);
            }
            set
            {
                canAccess = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand(Action action, int relayIdentifier)
        {
            this.action = action;
            if (relayIdentifier.Equals(1))
                this.canAccess = true;            
            else
                this.canAccess = false;
            
        }

        public void updateAccess(int relayIndetifier)
        {
            if (relayIndetifier.Equals(1))
                CanAccess = true;
            else
                CanAccess = false;
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
