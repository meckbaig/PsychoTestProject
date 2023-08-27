using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (Process.GetProcesses().Where(p => p.ProcessName ==
            System.IO.Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)).Count() > 1)
            {
                if (WpfMessageBox.Show("Копия программы уже открыта!", WpfMessageBox.MessageBoxType.Error) == MessageBoxResult.OK)
                    this.Close();
                else
                    this.Close();
            }
            InitializeComponent();
            DataContext = new MainViewModel(MainFrame);
            if (!File.Exists(Properties.Settings.Default.PsychoTestPath + "\\PsychoTestProject.exe"))
            {
                Properties.Settings.Default.PsychoTestPath = String.Empty;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.PsychoTestPath != String.Empty)
                MainFrame.Navigate(new Options());
            else
                MainFrame.Navigate(new ChoosePath());
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
