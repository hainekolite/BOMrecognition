using BomRainB.Business;
using BomRainB.Models;
using BomRainB.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class LoginVM : ViewModelBase
    {

        private const string INFO = "Currently logged as: ";

        public readonly RelayCommand _logInAccessCommand;
        public RelayCommand LogInAccessCommand => _logInAccessCommand;

        private readonly RelayCommand _logOutAccessCommand;
        public RelayCommand LogOutAccessCommand => _logOutAccessCommand;

        private bool _canLogOutAccess;
        public bool CanLogOutAccess
        {
            get
            {
                return (_canLogOutAccess);
            }
            set
            {
                _canLogOutAccess = value;
                OnPropertyChanged();
            }

        }

        private bool _canLogInAccess;
        public bool CanLogInAccess
        {
            get
            {
                return (_canLogInAccess);
            }
            set
            {
                _canLogInAccess = value;
                OnPropertyChanged();
            }
        }

        private MainWindowVM mainWindowData;

        private string _account;
        public string Account
        {
            get
            {
                return (_account);
            }
            set
            {
                _account = value;
                OnPropertyChanged();
            }
        }

        private string _password { get; set; }

        private string _personalInfo;
        public string PersonalInfo
        {
            get
            {
                return (_personalInfo);
            }
            set
            {
                _personalInfo = value;
                OnPropertyChanged();
            }
        }

        private Regex regex;
        private UserBusiness userBusiness;
        private User user;

        public LoginVM(MainWindowVM mainWindowData)
        {
            userBusiness = new UserBusiness();
            this.mainWindowData = mainWindowData;

            CanLogInAccess = !this.mainWindowData.CanAccess;
            CanLogOutAccess = this.mainWindowData.CanAccess;

            _logInAccessCommand = new RelayCommand(LogInAction);
            _logOutAccessCommand = new RelayCommand(LogOutAction);
            
            if (mainWindowData.CanAccess)
            {
                this.user = mainWindowData.user;
                PersonalInfo = string.Format("{0}{1} {2}", INFO, user.Name, user.LastName);
            }

            _account = string.Empty;
        }

        private void LogInAction()
        {
            var usersQuery = userBusiness.GetAll().Where(u => u.AccountName == Account).FirstOrDefault();
            if (usersQuery != null)
            {
                this.mainWindowData.CanAccess = true;
                CanLogInAccess = !this.mainWindowData.CanAccess;
                CanLogOutAccess = this.mainWindowData.CanAccess;
                this.mainWindowData.user = usersQuery;
                user = this.mainWindowData.user;
                PersonalInfo = string.Format("{0}{1} {2}", INFO, user.Name, user.LastName);
                Account = string.Empty;
            }   
        }

        private void LogOutAction()
        {
            this.mainWindowData.CanAccess = false;
            CanLogInAccess = !this.mainWindowData.CanAccess;
            CanLogOutAccess = this.mainWindowData.CanAccess;
            this.mainWindowData.user = null;
            PersonalInfo = string.Empty;
            Account = string.Empty;
        }

    }
}
