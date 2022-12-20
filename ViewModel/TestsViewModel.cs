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
                if (MainViewModel.TestStarted)
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
            MainViewModel.TestStarted = false;
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
            MainViewModel.TestFrame = testFrame = frame;
            TestList = new List<TestClass>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Tests"), "*.xml"))
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
                    try
                    {
                        MainViewModel.CurrentTest = (obj as TestClass);
                        if (MainViewModel.CurrentTest?.Questions?[0] == null)
                            MessageBox.Show("Выберите другой файл или обратитесь к администратору", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                        {
                            testFrame.Navigate(new Test((TestClass)obj));
                            Visibility = Visibility.Hidden;
                            MainViewModel.TestStarted = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }));
            }
        }
    }
} 
