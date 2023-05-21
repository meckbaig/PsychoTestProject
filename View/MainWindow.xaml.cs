using PsychoTestProject.ViewModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (Process.GetProcesses().Where(p => p.ProcessName ==
            System.IO.Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)).Count() > 1)
            {
                if (WpfMessageBox.Show("Копия программы уже открыта!", "Внимание!",
                    MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                    Close();
                else
                    Close();
            }
            InitializeComponent();
            DataContext = new MainViewModel(MainFrame, this);
            MainFrame.Navigate(new Welcome());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        {
            if (WpfMessageBox.Show("Вы точно хотите закрыть программу?", WpfMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.Yes)
            {
                Process.GetCurrentProcess().Kill();
            }
            else
                e.Cancel = true;
        }
    }
}
