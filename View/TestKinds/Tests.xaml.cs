using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace PsychoTestProject.View
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
            MainViewModel.MainWindow.Title = "Тесты";
            MainViewModel.AllButtonsHover(this.Content);
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.TestStarted)
            {
                if (MessageBox.Show("Вы точно хотите покинуть тест?", "Выход из теста", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Exit();
                }
            }
            else
                Exit();
        }

        private void Exit()
        {
            MainViewModel.Back();
            Question.qTimer?.Stop();
            MainViewModel.TestStarted = false;
        }
    }
}
