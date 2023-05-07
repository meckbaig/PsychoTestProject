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

namespace PsychoTestControlPanel
{
    /// <summary>
    /// Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        public Options()
        {
            InitializeComponent();
            CoolLook.AllButtonsHover(this.Content);
        }

        private void ChangePathButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new ChoosePath(true));
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new ChangePassword());
        }

        private void PolibiemButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new MainPage());
        }
    }
}
