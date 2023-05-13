using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using PsychoTestProject.View.TestKinds;
using System;
using System.Collections;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public static MainWindow MainWindow { get; set; }

        public static string UserDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PsychoTest";
        public static Frame? MainFrame { get; set; }
        public static object TestFrame { get; set; }
        public static bool TestStarted { get; set; }
        public static TestClass CurrentTest { get; set; }
        public static int CurrentQuestionNumber { get; set; }
        public static QuestionClass CurrentQuestion { get => CurrentTest.Questions[CurrentQuestionNumber-1]; set => CurrentTest.Questions[CurrentQuestionNumber-1] = value; }

        public static void Back()
        {
            CurrentTest = null;
            MainFrame.Content = null;
            MainFrame.Navigate(new Welcome());
        }

        public static List<T> GetVisualChilds<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> childs = new List<T>();
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(parent, i);
                if (v is T)
                    childs.Add(v as T);
                childs.AddRange(GetVisualChilds<T>(v));
            }
            return childs;
        }

        public static void MouseHover(Button button)
        {
            System.Windows.Media.Brush MouseLeave = button.Background;
            System.Windows.Media.Color bgcolor = (button.Background as System.Windows.Media.SolidColorBrush).Color;
            if (bgcolor.A < 10)
                bgcolor.A = 10;
            else
            {
                if (bgcolor.R < 50)
                    bgcolor.R = 50;
                if (bgcolor.G < 50)
                    bgcolor.G = 50;
                if (bgcolor.B < 50)
                    bgcolor.B = 50;
            }
            System.Windows.Media.Brush MouseEnter = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(255, (byte)(bgcolor.R - 50), (byte)(bgcolor.G - 50), (byte)(bgcolor.B - 50)));

            button.MouseEnter += (s, e) =>
            {
                if (button.IsEnabled)
                    button.Background = MouseEnter;
            };
            button.MouseLeave += (s, e) =>
            {
                if (button.IsEnabled)
                    button.Background = MouseLeave;
            };
            button.IsEnabledChanged += (s, e) =>
            {
                if (button.IsEnabled)
                    button.Background = MouseLeave;
                else
                    button.Background = MouseEnter;
            };
        }

        public static void AllButtonsHover(object pageContent)
        {
            foreach (Button button in GetVisualChilds<Button>(pageContent as DependencyObject))
            {
                if (button.Background.GetType() != new ImageBrush().GetType())
                {
                    MouseHover(button);
                }
            }
        }

        public static string ProperFileName(string fileName)
        {
            string error = "|/:*<>\\";

            foreach (char c in error)
            {
                if (fileName.Contains(c))
                {
                    fileName = fileName.Replace(c, '\0');
                }
            }
            return fileName;
        }

        public static BitmapImage GetBitmap(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            return bitmap;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
