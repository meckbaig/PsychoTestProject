using PsychoTestProject.Extensions;
using PsychoTestProject.View.TestKinds;
using PsychoTestProject.ViewModel;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для Transition.xaml
    /// </summary>
    public partial class Transition : Page, INotifyPropertyChanged
    {
        public string Description { get; set; }
        public double ContentHight { get; set; }
        TestType testType;

        public Transition(TestType type)
        {
            InitializeComponent();
            DataContext = this;
            MainViewModel.AllButtonsHover(Content);
            this.testType = type;
            switch (type)
            {
                case TestType.KnowlegeTest:
                    Title = "Тестирование знаний";                    
                    break;
                case TestType.OrientationTest:  
                    Title = "Тестирование направленности личности";                     
                    break;
                case TestType.ProTest:          
                    Title = "Тестирование профессионализма";                     
                    break;
                case TestType.LeongardTest:     
                    Title = "Тест «Акцентуации характера К. Леонгард»"; 
                    break;
                case TestType.AizenkTest:       
                    Title = "Тест айзенка";
                    break;
                case TestType.PerseveranceTest: 
                    Title = "Тест на усидчивость"; 
                    break;
                default: break;
            }
            MainViewModel.MainWindow.Title = Title;
            string transPath = Path.Combine(Environment.CurrentDirectory, "Tests", Title, "Transition.text");
            if (File.Exists(transPath))
                Description = Encoding.UTF8.GetString(CryptoMethod.Decrypt(transPath));
            else
            {
                WpfMessageBox.Show("Файлы данного теста отстутствуют или повреждены. Для импорта файлов обратитесь к администратору.");

                this.Loaded += (s,e) => MainViewModel.MainFrame.GoBack();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ContentHight = ActualHeight-50-Row0.ActualHeight-Row2.ActualHeight;
            OnPropertyChanged(nameof(ContentHight));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Back();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object page = NewPage(testType);
                MainViewModel.MainFrame.Content = null;
                MainViewModel.MainFrame.Navigate(page);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is IndexOutOfRangeException)
            {
                WpfMessageBox.Show("Файлы данного теста отстутствуют или повреждены. Для импорта файлов обратитесь к администратору.",
                    WpfMessageBox.MessageBoxType.Error);
                MainViewModel.Back();
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.Message, WpfMessageBox.MessageBoxType.Error);
                MainViewModel.Back();
            }
        }

        private object NewPage(TestType type)
        {
            switch (type)
            {
                case TestType.KnowlegeTest: return new Tests();
                case TestType.OrientationTest: return new Tests(type);
                case TestType.ProTest: return new MultiTest(type);
                case TestType.LeongardTest: return new MultiTest(type);
                case TestType.AizenkTest: return new MultiTest(type);
                case TestType.PerseveranceTest: return new PerseveranceTest();
                default: return null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
