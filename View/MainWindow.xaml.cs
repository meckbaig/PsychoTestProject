﻿using PsychoTestProject.ViewModel;
using PsychoTestProject.Extensions;
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
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Windows.Navigation;

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
            System.IO.Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)).Count()>1)
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
            => e.Cancel = WpfMessageBox.Show("Вы точно хотите закрыть программу?", WpfMessageBox.MessageBoxType.ConfirmationWithYesNo) != MessageBoxResult.Yes;
    }
}
