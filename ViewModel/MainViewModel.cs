using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PsychoTestProject.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(Frame mainFrame, MainWindow thisMainWindow)
        {
            MainFrame = mainFrame;
            MainWindow = thisMainWindow;
            try
            {
                Directory.Delete(UserDataFolder, true);
                File.Delete($"{Path.Combine(Environment.CurrentDirectory, "Lections")}\\temp.html");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        public static MainWindow MainWindow;

        public static string UserDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PsychoTest";
        public static Frame? MainFrame { get; set; }
        public static object TestFrame { get; set; }

        public static bool TestStarted { get; set; }

        public static TestClass CurrentTest { get; set; }
        public static int CurrentQuestionId { get; set; }
        public static QuestionClass CurrentQuestion { get => CurrentTest.Questions[CurrentQuestionId-1]; set => CurrentTest.Questions[CurrentQuestionId-1] = value; }

        public static void Back()
        {
            CurrentTest = null;
            MainFrame.Navigate(new Welcome());
            MainWindow.Title = "PsychoTest";
        }

        
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
