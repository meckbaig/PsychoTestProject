using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.Model;
using PsychoTestCourseProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Formats.Asn1.AsnWriter;

namespace PsychoTestCourseProject.ViewModel
{
    internal class TestsViewModel : INotifyPropertyChanged
    {
        Frame testFrame;
        Visibility visibility = Visibility.Hidden;

        public List<TestClass> TestList { get; set; }
        private TestClass test;

        public TestClass Test
        {
            get => test; set
            {
                if (Supporting.testStarted)
                {
                    if (MessageBox.Show("Вы точно хотите покинуть тест?", "Выход из теста", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        ShowTestPreviewMethod(value);
                }
                else
                    ShowTestPreviewMethod(value);
            }
        }

        private void ShowTestPreviewMethod(TestClass value)
        {
            test = value;
            OnPropertyChanged("Test");
            Visibility = Visibility.Visible;
            OnPropertyChanged("Visibility");
            Supporting.testStarted = false;
            testFrame.Content = null;
        }

        public Visibility Visibility 
        {
            set
            {
                visibility = value;
                OnPropertyChanged("Visibility");
            }
            get => visibility; 
        }

        public TestsViewModel(Frame frame)
        {
            Supporting.testFrame = testFrame = frame;
            TestList = new List<TestClass>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Tests")))
            {
                TestList.Add(new TestClass() { Name = Path.GetFileNameWithoutExtension(file), Filename = file });
            }

        }

        Command openTestCommand;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public Command OpenTestCommand
        {
            get
            {
                return openTestCommand ?? (openTestCommand = new Command(obj =>
                {
                    testFrame.Navigate(new Test((TestClass)obj));
                    Visibility = Visibility.Hidden;
                    Supporting.testStarted = true;
                }));
            }
        }
    }
} 
