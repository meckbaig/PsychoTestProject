using Microsoft.VisualBasic;
using Microsoft.Win32;
using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PsychoTestProject.ViewModel
{
    internal class TestEditorViewModel : INotifyPropertyChanged
    {
        Frame EditFrame;
        EditorPage editorPage;
        public ObservableCollection<TestClass> TestList { get; set; }
        public static List<Image> ImageList { get; set; }
        private TestClass test;

        public TestClass Test
        {
            get => test;
            set
            {
                test = value;
                OnPropertyChanged("Test");
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public TestEditorViewModel(Frame frame)
        {
            EditFrame = frame;
            UpdateTestList();
            CreateNewTestBTCommand = new Command(o => CreateNewTest("CreateNewTestBT"));
            SaveTestBTCommand = new Command(o => SaveTest("SaveTestBTCommand"));
            DeleteTestBTCommand = new Command(o => DeleteTest("DeleteTestBTCommand"));
            GoToLectionsEditorCommand = new Command(o => GoToLectionsEditor("GoToLectionsEditorCommand"))
;        }

        private void UpdateTestList()
        {
            TestList = GetTestList();
            OnPropertyChanged("TestList");
        }

        private ObservableCollection<TestClass> GetTestList()
        {
            ObservableCollection<TestClass> testList = new ObservableCollection<TestClass>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Tests"), "*.xml"))
            {
                testList.Add(new TestClass(false) { Name = Path.GetFileNameWithoutExtension(file), Filename = file });
            }
            return testList;
        }

        public ICommand CreateNewTestBTCommand { get; set; }
        public ICommand SaveTestBTCommand { get; set; }
        public ICommand DeleteTestBTCommand { get; set; }
        public ICommand GoToLectionsEditorCommand { get; set; }

        public void CreateNewTest(object sender)
        {
            MainViewModel.CurrentTest = new TestClass(false);
            ObservableCollection<TestClass> testList = GetTestList();
            MainViewModel.CurrentTest.Name = "Новый тест";
            for (int i = 1; i < testList.Count; i++)
            {
                if (testList.FirstOrDefault(tl => tl.Name == MainViewModel.CurrentTest.Name) != null)
                    MainViewModel.CurrentTest.Name = $"Новый тест {i}";
                else break;
            }
            MainViewModel.CurrentTest.Questions.Add(new QuestionClass());
            editorPage = new EditorPage();
            EditFrame.Navigate(editorPage);
        }

        public void SaveTest(object sender)
        {
            XmlDocumentClass xmlDocument = new XmlDocumentClass();
            if (MainViewModel.CurrentTest != null)
            {
                MessageBoxResult dialogResult = WpfMessageBox.Show("Сохранить в директории по умолчанию? " +
                    "Нажмите \"нет\", чтобы выбрать директорию сохранения.", "Внимание!", MessageBoxButton.YesNoCancel);
                if (dialogResult == MessageBoxResult.Cancel)
                    return;

                editorPage.SaveQuestion();
                string savePath;
                if (MainViewModel.CurrentTest.Filename == null)
                    savePath = Path.Combine(Environment.CurrentDirectory, "Tests", MainViewModel.CurrentTest.Name + ".xml");
                else
                    savePath = Path.Combine(Path.GetDirectoryName(MainViewModel.CurrentTest.Filename), MainViewModel.CurrentTest.Name + ".xml");


                if (dialogResult == MessageBoxResult.No)
                {
                    SaveFileDialog fileDialog = new SaveFileDialog(){ Title = "Сохранение файлов", FileName = MainViewModel.CurrentTest.Name+".xml" };
                    if (fileDialog.ShowDialog() == true)
                        savePath = fileDialog.FileName;
                }
                xmlDocument = new XmlDocumentClass(MainViewModel.CurrentTest);
                xmlDocument.Save(savePath);
                MainViewModel.CurrentTest.Filename = savePath;
                if (editorPage.Image != null)
                {
                    savePath = Path.GetDirectoryName(savePath) + "\\" + Path.GetFileNameWithoutExtension(savePath) + editorPage.ImageExt;
                    File.WriteAllBytes(savePath, editorPage.Image);
                }
                WpfMessageBox.Show("Сохранено", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateTestList();
            }
            else if (Test == null)
                WpfMessageBox.Show("Выберите файл", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (Test?.Questions?[0] == null)
                WpfMessageBox.Show("Выберите другой файл или обратитесь к администратору", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                xmlDocument = new XmlDocumentClass(Test);
                xmlDocument.Save();
                if (editorPage.Image != null)
                    File.WriteAllBytes(Environment.CurrentDirectory + "\\Tests\\" + MainViewModel.CurrentTest.Name + editorPage.ImageExt, editorPage.Image);
                WpfMessageBox.Show("Сохранено", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateTestList();
            }

        }

        public void DeleteTest(object sender)
        {
            if (Test != null)
            {
                if (WpfMessageBox.Show($"Вы точно хотите удалить тест \"{Test.Name}\"?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    EditFrame.Content = null;
                    File.Delete(Test.Filename);
                    DeletePicture(Test.Filename);
                    UpdateTestList();

                }
            }
            else if (MainViewModel.CurrentTest != null)
            {
                if (WpfMessageBox.Show($"Вы точно хотите удалить тест \"{MainViewModel.CurrentTest.Name}\"?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    EditFrame.Content = null;
                    File.Delete(MainViewModel.CurrentTest.Filename);
                    DeletePicture(MainViewModel.CurrentTest.Filename);
                    UpdateTestList();
                }
            }
            else
                WpfMessageBox.Show("Выберите файл", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeletePicture(string fileName)
        {
            for (int i = 0; i < Enum.GetNames(typeof(ImageExtension)).Length; i++)
            {
                string path = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + $".{(ImageExtension)i}");
                if (File.Exists(path))
                {
                    File.Delete(path);
                    break;
                }
            }
        }

        public void GoToLectionsEditor (object sender)
        {
            MainViewModel.CurrentTest = null;
            MainViewModel.MainFrame.Navigate(new Lections(true));
        }

        Command openTestCommand;

        public Command OpenTestCommand
        {
            get
            {
                return openTestCommand ?? (openTestCommand = new Command(obj =>
                {
                    try
                    {
                        MainViewModel.CurrentTest = (obj as TestClass);

                        if (obj != null && MainViewModel.CurrentTest?.Questions?[0] == null)
                            WpfMessageBox.Show("Выберите другой файл или обратитесь к администратору", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                        {
                            if (obj == null)
                            {
                                OpenFileDialog fileDialog = new OpenFileDialog() { Title = "Выберите файл", Multiselect = false };

                                if (fileDialog.ShowDialog() == true)
                                {
                                    MainViewModel.CurrentTest = new TestClass(false) { Name = Path.GetFileNameWithoutExtension(fileDialog.FileName), Filename = fileDialog.FileName };
                                }
                                else return;
                            }
                            editorPage = new EditorPage();
                            EditFrame.Navigate(editorPage);
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
