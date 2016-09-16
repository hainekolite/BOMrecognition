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

        private MainWindowVM()
        {
            SideBarItems = new ObservableCollection<SideBarItemVM>();

            SideBarItems.Add(new SideBarItemVM("Bill of Materials Check",
                PackIconKind.History, 
                new RelayCommand(() => UpdateUI(new BOMCheck() { DataContext = new BOMCheckVM() }))));
            SideBarItems[0].Command.Execute();
        }

        public void UpdateUI(ContentControl view) => CurrentView = view;

    }
}
