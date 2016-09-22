using BomRainB.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class LoginVM : ViewModelBase
    {

        public readonly RelayCommand _logInAccessCommand;
        public RelayCommand LogInAccessCommand => _logInAccessCommand;

        public bool canAccess;
        private MainWindowVM mainWindowData;

        public LoginVM(MainWindowVM mainWindowData)
        {
            _logInAccessCommand = new RelayCommand(GetLogInAcces);
            this.mainWindowData = mainWindowData;
        }

        private void GetLogInAcces()
        {
            mainWindowData.CanAccess = true;
        }

    }
}
