using PsychoTestProject.Extensions;
using PsychoTestProject.Model;
using PsychoTestProject.View;
using PsychoTestProject.View.TestKinds;
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

namespace PsychoTestProject.ViewModel
{
    internal class TestsViewModel : INotifyPropertyChanged
    {
        Frame testFrame;
        Visibility visibility = Visibility.Hidden;

        public List<TestClass> TestList { get; set; }
        public String TestTitle { get; set; }
        private TestClass test;
        private TestType TestType { get; set; }
        public TestClass Test
        {
            get => test; 
            set
            {
                if (MainViewModel.TestStarted)
                {
                    if (WpfMessageBox.Show("Вы точно хотите покинуть тест?", "Выход из теста", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            TestTitle = "Тест " + value.Name;
            OnPropertyChanged("TestTitle");
            Visibility = Visibility.Visible;
            OnPropertyChanged("Visibility");
            MainViewModel.TestStarted = false;
            Question.qTimer?.Stop();
            testFrame.Content = null;
        }

        public Visibility TextVisibility { get; set; }  

        public Visibility Visibility 
        {
            set
            {
                visibility = TextVisibility = value;
                OnPropertyChanged("Visibility");
                OnPropertyChanged("TextVisibility");
            }
            get => visibility; 
        }

        public TestsViewModel(Frame frame, TestType testType)
        {
            MainViewModel.TestFrame = testFrame = frame;
            TestType = testType;
            TestList = new List<TestClass>();
            string testPath = "";
            switch (TestType)
            {
                case TestType.KnowlegeTest: testPath = "Tests\\Тестирование знаний"; break;
                case TestType.OrientationTest: testPath = "Tests\\Тестирование направленности личности"; break;
            }
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, testPath), "*.xml"))
            {
                TestList.Add(new TestClass(true) { Name = Path.GetFileNameWithoutExtension(file), Filename = file });
            }
            TextVisibility = Visibility.Visible;
            OnPropertyChanged("TextVisibility");
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
                            WpfMessageBox.Show("Выберите другой файл или обратитесь к администратору", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                        {
                            switch (TestType)
                            {
                                case TestType.KnowlegeTest: testFrame.Navigate(new Test((TestClass)obj)); break;
                                case TestType.OrientationTest: testFrame.Navigate(new MultiTest(TestType, (TestClass)obj)); break;
                            }
                            
                            Visibility = Visibility.Hidden;
                            MainViewModel.TestStarted = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        WpfMessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }));
            }
        }
    }
} 
