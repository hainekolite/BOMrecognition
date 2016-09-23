using BomRainB.Models;
using BomRainB.ViewModel;
using BomRainB.ViewModel.Commands;
using BomRainB.ViewModel.Controls;
using BomRainB.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BomRainB
{
    public class MainWindowVM : ViewModelBase
    {
        private static MainWindowVM _instance = new MainWindowVM();
        public static MainWindowVM Instance => _instance;

        public ObservableCollection<SideBarItemVM> SideBarItems { get; private set; }

        private ContentControl _currentView;
        public ContentControl CurrentView
        {
            get
            {
                return _currentView;
            }
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private bool _canAccess;
        public bool CanAccess
        {
            get
            {
                return (_canAccess);
            }
            set
            {
                _canAccess = value;
                OnPropertyChanged();
            }    
        }

        public User user { get; set; }
        private Revision revision { get; set; }

        private MainWindowVM()
        {
            _canAccess = false;
            SideBarItems = new ObservableCollection<SideBarItemVM>();

            SideBarItems.Add(new SideBarItemVM("Login",
                PackIconKind.Account,
                new RelayCommand(() => UpdateUI(new Login() { DataContext = new LoginVM(this) }))));

            SideBarItems.Add(new SideBarItemVM("Bill of Materials Check",
                PackIconKind.BookOpen, 
                new RelayCommand(() => UpdateUI(new BOMCheck() { DataContext = new BOMCheckVM(this.user) }), (this.user == null? 0 : this.user.AccountType))));
            SideBarItems[0].Command.Execute();
        }

        public void UpdateUI(ContentControl view) => CurrentView = view;

        public void updatePermissions()
        {
            if (SideBarItems.Count > 1)
            {
                for (int i = 1; i < SideBarItems.Count; i++)
                    SideBarItems.ElementAt(i).Command.updateAccess(user.AccountType);
            }
            
        }

    }
}
