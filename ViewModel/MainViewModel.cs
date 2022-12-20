using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PsychoTestCourseProject.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(Frame mainFrame, MainWindow thisMainWindow)
        {
            MainFrame = mainFrame;
            MainWindow = thisMainWindow;
        }

        public static MainWindow MainWindow;
        public static Frame? MainFrame { get; set; }
        public static object TestFrame { get; set; }

        public static bool TestStarted { get; set; }

        public static TestClass CurrentTest { get; set; }
        public static int CurrentQuestion { get; set; }

        public static void Back()
        {
            MainFrame.Navigate(new Welcome());
            MainWindow.Title = "Психо-тест";
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
