using PsychoTestCourseProject.View;
using PsychoTestCourseProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для Welcome.xaml
    /// </summary>
    public partial class Welcome : Page
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void LectionsButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Lections());

        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Tests());

        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Версия программы: {Assembly.GetExecutingAssembly().GetName().Version}\nПо вопросам обращаться на почту: meckbaig@yandex.ru", "Информация о программе PsychoTest", MessageBoxButton.OK);
        }
    }
}
