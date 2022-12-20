using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.View;
using PsychoTestCourseProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace PsychoTestCourseProject
{
    /// <summary>
    /// Логика взаимодействия для Lections.xaml
    /// </summary>
    public partial class Lections : Page
    {
        public Lections()
        {
            InitializeComponent();
            DataContext = new LectionsViewModel(Web);
            MainViewModel.MainWindow.Title = "Лекции";
            Web.EnsureCoreWebView2Async();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Back();
            Web.Dispose();
        }
    }
}
