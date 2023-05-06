using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Page
    {
        public ChangePassword()
        {
            InitializeComponent();
            MainViewModel.AllButtonsHover(Content);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.NavigationService.GoBack();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Properties.Settings.Default.PsychoTestPath+"\\mgmt.cfg";
            File.WriteAllBytes(filePath, Algorythm.Encrypt(Encoding.UTF8.GetBytes(PasswordTB.Text)));
            MainViewModel.MainFrame.NavigationService.GoBack();
        }
    }
}
