using PsychoTestCourseProject.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PsychoTestCourseProject.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public MainViewModel(Frame mainFrame)
        {
            MainFrame = mainFrame;
        }

        public static Frame? MainFrame { get; set; }
        public static object TestFrame { get; set; }

        public static bool TestStarted { get; set; }

        public static TestClass CurrentTest { get; 
            set; }
        public static int CurrentQuestion { get; set; }

        public static void Back()
        {
            MainFrame.Navigate(new Welcome());
        }
    }
}
