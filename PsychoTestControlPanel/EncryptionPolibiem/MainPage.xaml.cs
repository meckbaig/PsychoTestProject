using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;

namespace PsychoTestControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainPage : Page, IMainView
    {
        public bool operationAllowed { get; set; } = true;
        public bool atLeastOneSuccessfull { get; set; } = false;
        public bool isEncodeChecked { get => Encode.IsChecked ?? false; }

        Encryptions Encryptions;

        public MainPage()
        {
            InitializeComponent();
            CoolLook.AllButtonsHover(Content);
            Encryptions = new Encryptions(this);
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            operationAllowed = true;
            atLeastOneSuccessfull = false;
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Title = "Выберите файл"
            };
            if (fileDialog.ShowDialog() == true)
            {
                Encryptions.FileCoder(fileDialog.FileName);
                if (operationAllowed)
                    CoolLook.ChangeColor(atLeastOneSuccessfull, BG);
                else
                    MessageBox.Show("Операция прервана");
            }
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            operationAllowed = true;
            atLeastOneSuccessfull = false;
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Title = "Выбор папки",
                FileName = $" "
            };
            if (fileDialog.ShowDialog() == true)
            {
                string path = Path.GetDirectoryName(fileDialog.FileName);
                Encryptions.Cycle(path);
                if (operationAllowed)
                    CoolLook.ChangeColor(atLeastOneSuccessfull, BG);
                else
                    MessageBox.Show("Операция прервана");
            }
        }

        private void WindowDrop(object sender, DragEventArgs e)
        {
            operationAllowed = true;
            atLeastOneSuccessfull = false;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool allGood = true;
                List<string> errorsWith = new List<string>();

                for (int i = 0; i < files.Length; i++)
                {
                    if (!operationAllowed)
                        break;
                    if (Directory.Exists(files[i]))
                        Encryptions.Cycle(files[i]);
                    else if (File.Exists(files[i]))
                        Encryptions.FileCoder(files[i]);
                    else
                    {
                        allGood = false;
                        errorsWith.Add(Path.GetFileName(files[i]));
                    }

                }
                if (allGood)
                {
                    if (operationAllowed)
                        CoolLook.ChangeColor(atLeastOneSuccessfull, BG);
                    else
                        MessageBox.Show("Операция прервана");
                }
                else
                {
                    string errorMessage = "Ошибка в файлах:";
                    foreach (var file in errorsWith)
                    {
                        errorMessage += "\n" + file;
                    }
                    if (!operationAllowed)
                        errorMessage += "\nОперация прервана";
                    MessageBox.Show(errorMessage);

                }
            }
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Settings());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.NavigationService.GoBack();
        }
    }
}
