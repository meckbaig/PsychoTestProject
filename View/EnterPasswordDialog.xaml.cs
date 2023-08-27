using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
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
using System.Windows.Shapes;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для EnterPasswordDialog.xaml
    /// </summary>
    public partial class EnterPasswordDialog : Window
    {
        public EnterPasswordDialog()
        {
            InitializeComponent();
            if (File.Exists(Environment.CurrentDirectory + "\\mgmt.cfg"))
                _password = Encoding.UTF8.GetString(CryptoMethod.Decrypt(Environment.CurrentDirectory + "\\mgmt.cfg"));
            else
            {
                WpfMessageBox.Show("Для доступа к редактированию требуется установить пароль администратора!" +
                    "\nПуть к программе скопирован в буфер обмена", WpfMessageBox.MessageBoxType.Warning);
                Clipboard.SetData(DataFormats.Text, (Object)Environment.CurrentDirectory);
                Close();
            }
            Password.Focus();   
            MainViewModel.AllButtonsHover(this.Content);
            DataContext = this;
        }
        static EnterPasswordDialog _dialog;
        static private bool _result = false;
        static private string _password;

        public double Scale { get => Properties.Settings.Default.Scale; }
        public double MaxWidthScale { get => Scale * 500; }
        public double MaxHeightScale { get => Scale * 500; }
        public double MinWidthScale { get => Scale * 300; }
        public double MinHeightScale { get => Scale * 160; }

        public static bool Show()
        {
            try
            {
                _dialog = new EnterPasswordDialog();
                _dialog.ShowDialog();
                bool result = _result;
                _result = false;
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void Continue()
        {
            if (Password.Password == _password)
            {
                try
                {
                    File.WriteAllText(Environment.CurrentDirectory + "/tmp.txt", "");
                    File.Delete(Environment.CurrentDirectory + "/tmp.txt");
                }
                catch (UnauthorizedAccessException)
                {
                    MainViewModel.FilesEditPermission = false;
                    if (WpfMessageBox.Show("Программа запущена не от имени администратора, доступ к редактированию файлов запрещён\n" +
                        "Хотите перезапустить программу от имени администратора?", "Внимание!",
                        MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                    {
                        MainViewModel.RunAsAdmin();
                    };
                }
                _result = true;
                _dialog = null;
                Close();
            }
            else
                WpfMessageBox.Show("Введён неверный пароль. Для восстановления пароля обратитесь к администратору.", WpfMessageBox.MessageBoxType.Warning);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog = null;
            Close();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Continue();
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Continue();
        }
    }
}
