using PsychoTestProject.ViewModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
                WpfMessageBox.Show("Копия программы уже открыта!", "Внимание!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            InitializeComponent();
            DataContext = new MainViewModel(MainFrame, this);
            MainFrame.Navigate(new Welcome());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WpfMessageBox.Show("Вы точно хотите закрыть программу?", WpfMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.Yes)
                Process.GetCurrentProcess().Kill();
            else
                e.Cancel = true;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && MainFrame.Content is Welcome)
            {
                if (e.Delta > 0)
                    (DataContext as MainViewModel).Scale += 0.1;
                else
                    (DataContext as MainViewModel).Scale -= 0.1;
            }
        }
    }
}
