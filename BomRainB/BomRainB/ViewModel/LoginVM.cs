using BomRainB.Business;
using BomRainB.Models;
using BomRainB.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BomRainB.ViewModel
{
    public class LoginVM : ViewModelBase
    {
        #region properties
        private const string INFO = "Currently logged as: ";
        private const string LOG_IN_FAILED = "User account or password are incorrect";

        private readonly ParameterCommand _logOutAccessCommand;
        public ParameterCommand LogOutAccessCommand => _logOutAccessCommand;

        private readonly ParameterCommand _logInAccessCommand;
        public ParameterCommand LogInAccessCommand => _logInAccessCommand;

        private Task logInTask;

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
                if (value.Length >= 1)
                {
                    if (regex.IsMatch(value.ElementAt(value.Length-1).ToString()))
                    {
                        _account = value;
                        OnPropertyChanged();
                    }
                }
                else if (value.Equals(string.Empty))
                {
                    _account = value;
                    OnPropertyChanged();
                }
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

        private Regex regex { get; set; }
        private UserBusiness userBusiness;
        private User user;

        #endregion properties

        #region constructor

        public LoginVM(MainWindowVM mainWindowData)
        {
            userBusiness = new UserBusiness();
            this.mainWindowData = mainWindowData;

            CanLogInAccess = !this.mainWindowData.CanAccess;
            CanLogOutAccess = this.mainWindowData.CanAccess;

            _logInAccessCommand = new ParameterCommand(LogInAction);
            _logOutAccessCommand = new ParameterCommand(LogOutAction);
            
            if (mainWindowData.CanAccess)
            {
                this.user = mainWindowData.user;
                PersonalInfo = string.Format("{0}{1} {2}", INFO, user.Name, user.LastName);
            }

            regex = new Regex(@"^[a-zA-Z0-9]?$");

            _account = string.Empty;
        }

        #endregion constructor

        #region LoginAciontsRelated
        private void LogInAction(object parameter)
        {
            var pass = parameter as PasswordBox;
            _password = pass.Password;
            if (logInTask == null || logInTask.IsCompleted)
            {
                logInTask = Task.Run(() => {
                    var usersQuery = userBusiness.GetUserByAccountPasswordByIQueryable(Account, _password).FirstOrDefault();
                    if (usersQuery != null)
                    {
                        this.mainWindowData.CanAccess = true;
                        CanLogInAccess = !this.mainWindowData.CanAccess;
                        CanLogOutAccess = this.mainWindowData.CanAccess;
                        this.mainWindowData.user = usersQuery;
                        user = this.mainWindowData.user;
                        PersonalInfo = string.Format("{0}{1} {2}", INFO, user.Name, user.LastName);
                        Account = string.Empty;
                        this.mainWindowData.updatePermissions();
                    }
                    else
                    {
                        this.mainWindowData.CanAccess = false;
                        PersonalInfo = string.Format("{0}", LOG_IN_FAILED);
                    }
                });
                pass.Password = string.Empty;
                PersonalInfo = "Working";
            }            
        }

        private void LogOutAction(object parameter)
        {
            var pass = parameter as PasswordBox;
            pass.Password = string.Empty;
            PersonalInfo = string.Empty;
            Account = string.Empty;
            _password = pass.Password;

            this.mainWindowData.CanAccess = false;
            this.mainWindowData.user.AccountType = 0;
            this.mainWindowData.updatePermissions();
            this.mainWindowData.user = null;

            CanLogInAccess = !this.mainWindowData.CanAccess;
            CanLogOutAccess = this.mainWindowData.CanAccess;

        }

        #endregion LoginAciontsRelated

    }
}
