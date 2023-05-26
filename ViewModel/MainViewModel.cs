using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public static Frame? MainFrame { get; set; }

        public static string UserDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PsychoTest";
        public static object TestFrame { get; set; }
        public static bool TestStarted { get; set; }
        public static TestClass CurrentTest { get; set; }
        public static int CurrentQuestionNumber { get; set; }
        public static QuestionClass CurrentQuestion 
        { 
            get => CurrentTest.Questions[CurrentQuestionNumber-1]; 
            set => CurrentTest.Questions[CurrentQuestionNumber-1] = value;
        }

        DpiScale dpi = VisualTreeHelper.GetDpi(new Control());

        public double Scale
        {
            get
            {
                return Properties.Settings.Default.Scale;
            }
            set
            {
                double diff = value / Properties.Settings.Default.Scale;
                if (value < 0.5)
                {
                    value = 0.5;
                    diff = 1;
                }
                else if (MinHeightScale * diff > SystemParameters.FullPrimaryScreenHeight)
                {
                    value = Properties.Settings.Default.Scale;
                    diff = 1;
                }
                
                MainWindow.Width *= diff;
                MainWindow.Height *= diff;
                Properties.Settings.Default.Scale = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(Scale));
                OnPropertyChanged(nameof(MinWidthScale));
                OnPropertyChanged(nameof(MinHeightScale));
                ShowScale();
            }
        }

        public double ScalePercent { get => Math.Round(Scale * 100, 0); }
        public double MinWidthScale { get => Scale * 780; }
        public double MinHeightScale { get => Scale * 490; }
        public double StartupX { get => (SystemParameters.FullPrimaryScreenWidth - 800) / 2; }
        public double StartupY { get => (SystemParameters.FullPrimaryScreenHeight - 600) / 2; }


        public static void Back()
        {
            CurrentTest = null;
            MainFrame.Content = null;
            MainFrame.Navigate(new Welcome());
        }

        public void ShowScale()
        {
            OnPropertyChanged(nameof(ScalePercent));
            ColorAnimation animation = new ColorAnimation();
            animation.From = Color.FromArgb(0, 0, 0, 0);
            animation.To = Color.FromArgb(100, 0, 0, 0);
            animation.Duration = TimeSpan.FromMilliseconds(500);
            animation.AutoReverse = true;
            animation.DecelerationRatio = 1;
            MainWindow.ScaleTB.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
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
            string error = "\\|/:*<>";

            foreach (char c in error)
            {
                if (fileName.Contains(c))
                {
                    fileName = fileName.Replace(c, '\0');
                }
            }
            return fileName;
        }

        public static string FileNameNotNull(string fileName, string replaceString, string savePath)
        {
            if (fileName != string.Empty) 
                return fileName;

            List<string> list = new List<string>();
            string replaceName = Path.GetFileNameWithoutExtension(replaceString);
            string ext = Path.GetExtension(replaceString);
            foreach (string s in Directory.GetFiles(savePath, $"*{ext}"))
            {
                list.Add(Path.GetFileNameWithoutExtension(s));
            }
            fileName = replaceName;
            for (int i = 1; i < list.Count; i++)
            {
                if (list.FirstOrDefault(tl => tl == fileName) != null)
                    fileName = $"{replaceName} {i}";
                else break;
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
