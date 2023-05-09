using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PsychoTestControlPanel
{
    /// <summary>
    /// Логика взаимодействия для ChoosePath.xaml
    /// </summary>
    public partial class ChoosePath : Page, INotifyPropertyChanged
    {
        private Visibility showBackButton = Visibility.Collapsed;
        public Visibility ShowBackButton { get => showBackButton; set { showBackButton = value; OnPropertyChanged("ShowBackButton"); } }
        public ChoosePath(bool showBackButton = false)
        {
            InitializeComponent();
            DataContext = this;
            if (showBackButton)
                ShowBackButton = Visibility.Visible;
            CoolLook.AllButtonsHover(this.Content);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.NavigationService.GoBack();
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.SelectedPath + "\\PsychoTestProject.exe";
                if (File.Exists(path))
                {
                    PathTB.Text = dialog.SelectedPath;
                }
                else
                    WpfMessageBox.Show("Указан неверный путь к программе. Отсутствует исполняемый файл.", WpfMessageBox.MessageBoxType.Warning);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string path = PathTB.Text + "\\PsychoTestProject.exe";
            if (File.Exists(path))
            {
                Properties.Settings.Default.PsychoTestPath = PathTB.Text;
                Properties.Settings.Default.Save();
                if (ShowBackButton == Visibility.Visible)
                    MainViewModel.MainFrame.NavigationService.GoBack();
                else
                {
                    MainViewModel.MainFrame.Source = null;
                    MainViewModel.MainFrame.Navigate(new Options());
                }
            }
            else
                WpfMessageBox.Show("Указан неверный путь к программе. Отсутствует исполняемый файл.", WpfMessageBox.MessageBoxType.Warning);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
