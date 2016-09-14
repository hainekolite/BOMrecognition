using BomRainB.ViewModel.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BomRainB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closed += ShutDownApplication;
            DataContext = MainWindowVM.Instance;
        }

        private void ShutDownApplication(object sender, EventArgs e) => Application.Current.Shutdown();

        private void ItemsControlClicked(object sender, MouseButtonEventArgs e) => (MenuList.SelectedItem as SideBarItemVM).Command.Execute();
    }
}
