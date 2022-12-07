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

namespace PsychoTestCourseProject.View
{
    /// <summary>
    /// Логика взаимодействия для Tests.xaml
    /// </summary>
    public partial class Tests : Page
    {
        public Tests()
        {
            InitializeComponent();
            DataContext = new TestsViewModel(TestFrame);
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
