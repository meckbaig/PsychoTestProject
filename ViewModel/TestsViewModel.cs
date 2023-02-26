using PsychoTestProject.Extensions;
using PsychoTestProject.Model;
using PsychoTestProject.View;
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

        public TestClass Test
        {
            get => test; 
            set
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

        public TestsViewModel(Frame frame)
        {
            MainViewModel.TestFrame = testFrame = frame;
            TestList = new List<TestClass>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Tests"), "*.xml"))
            {
                TestList.Add(new TestClass(true) { Name = Path.GetFileNameWithoutExtension(file), Filename = file });
            }
            TestTitle = "Тесты содержат вопросы трёх типов:\n" +
                        "с одиночным выбором, с множественным выбором " +
                        "и открытые вопросы со свободным выбором\n\n" +
                        "На открытые вопросы с перечислением требуется " +
                        "давать ответы с пробелом через запятую, например:\n" +
                        "Ответ1, Ответ2, Ответ3\n\n" +
                        "Критерии оценивания:\n" +
                        "Оценка 5 - 86-100% правильных ответов\n" +
                        "Оценка 4 - 76-85% правильных ответов\n" +
                        "Оценка 3 - 61-75% правильных ответов\n" +
                        "Оценка 2 - менее 60% правильных ответов";
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
